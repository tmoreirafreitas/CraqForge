using CraqForge.DocuCraft.Abstractions.FileManagement;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Numerics;

namespace CraqForge.DocuCraft.Extractions.Ocr.Images
{
    internal sealed class ImageProcessingForTesseract(ILogger<ImageProcessingForTesseract> logger) : IImageProcessingForTesseract
    {
        private enum ImageType { Printed, Handwritten, UnevenLighting }

        public async Task<byte[]> PreprocessImage(byte[] imageBytes)
        {
            try
            {
                using var image = Image.Load<Rgba32>(imageBytes);

                if (ShouldResize(image))
                {
                    const int targetSize = 1800;
                    var factor = Math.Max(1, targetSize / Math.Max(image.Width, image.Height));
                    logger.LogInformation("Redimensionando imagem de {Width}x{Height}", image.Width, image.Height);
                    image.Mutate(x => x.Resize(image.Width * factor, image.Height * factor));
                }

                // Grayscale primeiro para pipelines
                image.Mutate(x => x.Grayscale());

                var type = ClassifyImage(image);
                logger.LogInformation("Tipo de imagem classificado como: {Type}", type);

                switch (type)
                {
                    case ImageType.Printed:
                        ApplyPrintedPipeline(image);
                        break;
                    case ImageType.Handwritten:
                        ApplyHandwrittenPipeline(image);
                        break;
                    case ImageType.UnevenLighting:
                        ApplyPhotoPipeline(image);
                        break;
                }

                ApplyDeskewWithFft(image, logger);

                await using var msOut = new MemoryStream();
                await image.SaveAsync(msOut, new PngEncoder());
                return msOut.ToArray();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro durante o pré-processamento da imagem.");
                throw;
            }
        }

        private static void ApplyPrintedPipeline(Image<Rgba32> image)
        {
            image.Mutate(x =>
            {
                x.GaussianBlur(1.5f)
                 .BinaryThreshold(0.5f);
            });
        }

        private static void ApplyHandwrittenPipeline(Image<Rgba32> image)
        {
            image.Mutate(x =>
            {
                x.Contrast(1.2f)
                 .GaussianBlur(0.5f)
                 .BinaryThreshold(0.4f);
            });
        }

        private static void ApplyPhotoPipeline(Image<Rgba32> image)
        {
            image.Mutate(x =>
            {
                x.GaussianBlur(2f)
                .BinaryThreshold(0.45f);
                //.Normalize()
                //.BinaryThreshold(0.45f);
            });
        }

        private static ImageType ClassifyImage(Image<Rgba32> image)
        {
            double mean = 0, variance = 0;
            int totalPixels = image.Width * image.Height;
            Span<Rgba32> pixelSpan = new();

            image.CopyPixelDataTo(pixelSpan);
            foreach (var pixel in pixelSpan)
            {
                double intensity = (pixel.R + pixel.G + pixel.B) / 3.0;
                mean += intensity;
            }
            mean /= totalPixels;
            foreach (var pixel in pixelSpan)
            {
                double intensity = (pixel.R + pixel.G + pixel.B) / 3.0;
                variance += Math.Pow(intensity - mean, 2);
            }
            variance /= totalPixels;

            if (variance < 500) return ImageType.Printed;
            if (variance > 2000) return ImageType.Handwritten;
            return ImageType.UnevenLighting;
        }

        private static bool ShouldResize(Image<Rgba32> image)
        {
            return image.Width < 1000 || image.Height < 1000;
        }

        // ===========================
        // DESKEW (FFT + refinamento)
        // ===========================
        private static void ApplyDeskewWithFft(Image<Rgba32> image, ILogger logger)
        {
            try
            {
                // binariza leve pra FFT
                using var bin = image.Clone(ctx => ctx.BinaryThreshold(0.5f));

                // Coarse por FFT 2D
                double coarseDeg = EstimateSkewAngleFft(bin, logger);

                // Refina em ±1°
                double refined = RefineAngleByProjection(bin, coarseDeg, sweep: 1.0, step: 0.2);

                if (Math.Abs(refined) > 0.01)
                {
                    logger.LogDebug("Deskew: coarse={Coarse:F2}°, refined={Refined:F2}°", coarseDeg, refined);
                    image.Mutate(x => x.Rotate((float)-refined));
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "FFT falhou, usando fallback de varredura.");
                // Fallback simples (antigo)
                double angle = EstimateSkewAngleBySweep(image);
                if (Math.Abs(angle) > 0.01)
                {
                    logger.LogDebug("Deskew (fallback): {Angle:F2}°", angle);
                    image.Mutate(x => x.Rotate((float)-angle));
                }
            }
        }

        // ---------- Coarse com FFT 2D ----------
        private static double EstimateSkewAngleFft(Image<Rgba32> bin, ILogger logger)
        {
            // Downscale para limitar custo (max 1024)
            const int maxDim = 1024;
            using var work = DownscaleIfNeeded(bin, maxDim);

            int w = work.Width;
            int h = work.Height;

            // Cria matriz real [0..1] e aplica janela de Hann 2D para reduzir efeitos de borda
            int W = NextPow2(w);
            int H = NextPow2(h);
            var data = new Complex[H, W];
            var hannX = Hann(W);
            var hannY = Hann(H);

            work.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < H; y++)
                {
                    var row = y < accessor.Height ? accessor.GetRowSpan(y) : default;
                    for (int x = 0; x < W; x++)
                    {
                        float val = 0f;
                        if (y < accessor.Height && x < row.Length)
                        {
                            // binário: 1 (branco) / 0 (preto) — ajuste se preferir invertido
                            val = row[x].R > 128 ? 1f : 0f;
                        }
                        double window = hannX[x] * hannY[y];
                        data[y, x] = new Complex(val * window, 0);
                    }
                }
            });

            // FFT 2D
            FFT2D(data, forward: true);

            // Histograma de orientação [0..180)
            // Ignora região baixa frequência (raio < rMin) para evitar DC
            const double rMin = 2.0;
            const int bins = 180;
            var hist = new double[bins];

            int cx = W / 2;
            int cy = H / 2;

            // Como nosso FFT não aplica shift, precisamos mapear índices para frequências centradas
            for (int y = 0; y < H; y++)
            {
                int fy = y <= cy ? y : y - H;
                for (int x = 0; x < W; x++)
                {
                    if (x == 0 && y == 0) continue;

                    int fx = x <= cx ? x : x - W;

                    double r = Math.Sqrt(fx * fx + fy * fy);
                    if (r < rMin) continue;

                    double angleRad = Math.Atan2(fy, fx); // [-π, π]
                    double angleDeg = angleRad * 180.0 / Math.PI; // [-180, 180]
                    if (angleDeg < 0) angleDeg += 180.0;          // [0, 180)

                    double power = data[y, x].Magnitude;
                    // Peso opcional pelo raio para priorizar frequências médias
                    double weight = power; // * r;

                    int binRound = (int)Math.Round(angleDeg);
                    if (binRound == bins) binRound = 0;
                    hist[binRound] += weight;
                }
            }

            // Pico do histograma → orientação dominante φ* (em frequências)
            int maxIdx = 0;
            double maxVal = double.NegativeInfinity;
            for (int i = 0; i < bins; i++)
            {
                if (hist[i] > maxVal)
                {
                    maxVal = hist[i];
                    maxIdx = i;
                }
            }

            double phi = maxIdx; // em graus [0,180)
            // Para linhas horizontais no domínio espacial, energia fica ao longo de φ≈90° no espectro.
            // Skew θ ≈ φ - 90 (ajustado para [-90,90])
            double theta = phi - 90.0;
            if (theta > 90) theta -= 180;
            if (theta < -90) theta += 180;

            // Limita a máx 15° por segurança (caso a página esteja muito distorcida)
            theta = Math.Clamp(theta, -15.0, 15.0);

            logger.LogDebug("FFT: φ*={Phi:F2}°, θ≈{Theta:F2}°", phi, theta);

            return theta;
        }

        // ---------- Refino local por projeção ----------
        private static double RefineAngleByProjection(Image<Rgba32> bin, double coarseDeg, double sweep, double step)
        {
            double bestAngle = coarseDeg;
            double bestScore = double.NegativeInfinity;

            for (double a = coarseDeg - sweep; a <= coarseDeg + sweep + 1e-6; a += step)
            {
                using var rotated = bin.Clone(ctx => ctx.Rotate((float)a));
                double score = CalculateHorizontalVariance(rotated);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestAngle = a;
                }
            }

            return bestAngle;
        }

        // ---------- Fallback simples ----------
        private static double EstimateSkewAngleBySweep(Image<Rgba32> image)
        {
            double bestAngle = 0;
            double bestVariance = double.NegativeInfinity;

            for (double angle = -5; angle <= 5; angle += 0.5)
            {
                using var rotated = image.Clone(ctx => ctx.Rotate((float)angle));
                double variance = CalculateHorizontalVariance(rotated);
                if (variance > bestVariance)
                {
                    bestVariance = variance;
                    bestAngle = angle;
                }
            }
            return bestAngle;
        }

        // ---------- Métrica de projeção ----------
        private static double CalculateHorizontalVariance(Image<Rgba32> image)
        {
            double[] lineSums = new double[image.Height];

            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    double sum = 0;
                    for (int x = 0; x < row.Length; x++)
                    {
                        // Considera 'branco' como 1, 'preto' 0 (ajuste se da sua pipeline)
                        sum += row[x].R > 128 ? 1 : 0;
                    }
                    lineSums[y] = sum;
                }
            });

            double mean = lineSums.Average();
            double variance = 0;
            for (int i = 0; i < lineSums.Length; i++)
            {
                double d = lineSums[i] - mean;
                variance += d * d;
            }
            variance /= lineSums.Length;
            return variance;
        }

        // ---------- Utils de imagem ----------
        private static Image<Rgba32> DownscaleIfNeeded(Image<Rgba32> src, int maxDim)
        {
            int w = src.Width;
            int h = src.Height;
            int maxSrc = Math.Max(w, h);
            if (maxSrc <= maxDim)
                return src.Clone(); // mantém

            double scale = (double)maxDim / maxSrc;
            int nw = Math.Max(8, (int)Math.Round(w * scale));
            int nh = Math.Max(8, (int)Math.Round(h * scale));
            return src.Clone(ctx => ctx.Resize(nw, nh));
        }

        // ---------- FFT helpers ----------
        private static int NextPow2(int v)
        {
            int p = 1;
            while (p < v) p <<= 1;
            return p;
        }

        private static double[] Hann(int n)
        {
            var w = new double[n];
            for (int i = 0; i < n; i++)
                w[i] = 0.5 * (1 - Math.Cos(2 * Math.PI * i / (n - 1)));
            return w;
        }

        private static void FFT2D(Complex[,] data, bool forward)
        {
            int H = data.GetLength(0);
            int W = data.GetLength(1);

            // linhas
            var row = new Complex[W];
            for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < W; x++)
                    row[x] = data[y, x];

                FFT1D(row, forward);

                for (int x = 0; x < W; x++)
                    data[y, x] = row[x];
            }

            // colunas
            var col = new Complex[H];
            for (int x = 0; x < W; x++)
            {
                for (int y = 0; y < H; y++)
                    col[y] = data[y, x];

                FFT1D(col, forward);

                for (int y = 0; y < H; y++)
                    data[y, x] = col[y];
            }

            // normalização no inverso (não usamos, mas deixo pronto)
            if (!forward)
            {
                double scale = 1.0 / (W * H);
                for (int y = 0; y < H; y++)
                    for (int x = 0; x < W; x++)
                        data[y, x] *= scale;
            }
        }

        // Radix-2 iterativo
        private static void FFT1D(Complex[] buffer, bool forward)
        {
            int n = buffer.Length;
            int bits = (int)Math.Log2(n);

            // bit-reversal
            for (int i = 0, j = 0; i < n; i++)
            {
                if (i < j)
                {
                    (buffer[i], buffer[j]) = (buffer[j], buffer[i]);
                }
                int mask = n >> 1;
                while ((j & mask) != 0)
                {
                    j &= ~mask;
                    mask >>= 1;
                }
                j |= mask;
            }

            double dir = forward ? -1.0 : 1.0;

            for (int len = 2; len <= n; len <<= 1)
            {
                int half = len >> 1;
                double ang = dir * 2.0 * Math.PI / len;
                Complex wLen = new Complex(Math.Cos(ang), Math.Sin(ang));

                for (int i = 0; i < n; i += len)
                {
                    Complex w = Complex.One;
                    for (int j = 0; j < half; j++)
                    {
                        Complex u = buffer[i + j];
                        Complex v = buffer[i + j + half] * w;
                        buffer[i + j] = u + v;
                        buffer[i + j + half] = u - v;
                        w *= wLen;
                    }
                }
            }
        }
    }
}

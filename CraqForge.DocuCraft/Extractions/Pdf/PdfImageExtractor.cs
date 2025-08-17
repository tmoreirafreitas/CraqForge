using iText.IO.Image;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace CraqForge.DocuCraft.Extractions.Pdf
{
    public sealed class PdfImageExtractor : IEventListener
    {
        public List<byte[]> Images { get; } = [];

        public void EventOccurred(IEventData data, EventType type)
        {
            if (type == EventType.RENDER_IMAGE)
            {
                var renderInfo = (ImageRenderInfo)data;
                var image = renderInfo.GetImage();
                if (image != null && image.IdentifyImageType() != ImageType.NONE)
                {
                    Images.Add(image.GetImageBytes());
                }
            }
        }

        public ICollection<EventType> GetSupportedEvents() =>
            new HashSet<EventType> { EventType.RENDER_IMAGE };
    }
}

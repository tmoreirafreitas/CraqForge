using System.Runtime.Serialization;

namespace CraqForge.DocuCraft.Abstractions.Ocr
{
    public enum OcrAccuracy
    {
        [EnumMember(Value = "Standard")] Standard,
        [EnumMember(Value = "Fast")] Fast,
        [EnumMember(Value = "Best")] Best
    }
}

using System.Reflection;
using System.Runtime.Serialization;

namespace CraqForge.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string? GetEnumMemberValue(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<EnumMemberAttribute>();
            return attribute != null ? attribute.Value : value.ToString();
        }
    }
}
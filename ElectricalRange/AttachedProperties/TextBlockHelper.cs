using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ProjectsNow.AttachedProperties
{
    public static class TextBlockHelper
    {
        public static IEnumerable<string> GetLines(this TextBlock source)
        {
            string value = null;
            var text = source.Text;
            int offset = 0;
            TextPointer lineStart = source.ContentStart.GetPositionAtOffset(1, LogicalDirection.Forward);
            do
            {
                TextPointer lineEnd = lineStart != null ? lineStart.GetLineStartPosition(1) : null;
                int length = lineEnd != null ? lineStart.GetOffsetToPosition(lineEnd) : text.Length - offset;
                value = text.Substring(offset, length);
                yield return value.Replace("\n", "").Replace("\r", "");
                offset += length;
                lineStart = lineEnd;
            }
            while (lineStart != null);
        }

        public static int GetLinesCount(this TextBlock tb)
        {
            var propertyInfo = GetPrivatePropertyInfo(typeof(TextBlock), "LineCount");
            var result = (int)propertyInfo.GetValue(tb);
            return result;
        }

        private static PropertyInfo GetPrivatePropertyInfo(Type type, string propertyName)
        {
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic);
            return props.FirstOrDefault(propInfo => propInfo.Name == propertyName);
        }
    }
}

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.Xaml.Interactions.Core
{
    internal static class TypeConverterHelper
    {
        public static object Convert(string value, string destinationTypeFullName)
        {
            if (string.IsNullOrEmpty(destinationTypeFullName))
            {
                throw new ArgumentNullException("destinationTypeFullName");
            }

            var scope = TypeConverterHelper.GetScope(destinationTypeFullName);
            if (string.Equals(scope, "System", StringComparison.Ordinal))
            {
                if (string.Equals(destinationTypeFullName, typeof(string).FullName, StringComparison.Ordinal))
                {
                    return value;
                }
                if (string.Equals(destinationTypeFullName, typeof(bool).FullName, StringComparison.Ordinal))
                {
                    return Boolean.Parse(value);
                }
                if (string.Equals(destinationTypeFullName, typeof(int).FullName, StringComparison.Ordinal))
                {
                    return Int32.Parse(value, CultureInfo.InvariantCulture);
                }
                if (string.Equals(destinationTypeFullName, typeof(double).FullName, StringComparison.Ordinal))
                {
                    return Double.Parse(value, CultureInfo.CurrentCulture);
                }
            }
            var type = TypeConverterHelper.GetType(destinationTypeFullName);
            var invariantCulture = CultureInfo.InvariantCulture;
            var objArray = new Object[] { scope, type, value };
            var str = string.Format(invariantCulture, "<ContentControl xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:c='using:{0}'><c:{1}>{2}</c:{1}></ContentControl>", objArray);
            
            var contentControl = XamlReader.Load(str) as ContentControl;
            return contentControl == null ? null : contentControl.Content;
        }

        private static string GetScope(string name)
        {
            var num = name.LastIndexOf('.');
            return num == name.Length - 1 ? name : name.Substring(0, num);
        }

        private static string GetType(string name)
        {
            var num = name.LastIndexOf('.');
            if (num == name.Length - 1)
            {
                return name;
            }
            var num1 = num + 1;
            return name.Substring(num1);
        }
    }
}

using System.Globalization;
using System.Linq;

namespace ModernRonin.FluentArgumentParser
{
    public static class StringExtensions
    {
        public static string CamelCased(this string self)
        {
            if (self.Length == 0) return self;
            return char.ToLowerInvariant(self[0]) + self.Substring(1);
        }

        public static string KebabCased(this string self)
        {
            if (self.Length == 0) return self;

            var start = toLower(self[0]);
            return self[1..].Aggregate(start, (r, c) => r + convert(c));

            string convert(char c) => char.IsLower(c) ? c.ToString() : "-" + toLower(c);
            string toLower(char c) => char.ToLower(c, CultureInfo.CurrentCulture).ToString();
        }

        public static string After(this string self, string marker)
        {
            var i = self?.IndexOf(marker);
            if (!i.HasValue || i < 0) return self;
            return self.Substring(i.Value + marker.Length);
        }

        public static string Before(this string self, string marker)
        {
            var i = self?.IndexOf(marker);
            if (!i.HasValue || i < 0) return self;
            return self.Substring(0, i.Value);
        }
    }
}
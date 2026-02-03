using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLyBanHang.Them
{
    public static class Them1
    {
        public static string GenerateSlug(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            string normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            string result = sb.ToString().Normalize(NormalizationForm.FormC);

            result = Regex.Replace(result, @"[^a-zA-Z0-9\s-]", "");
            result = Regex.Replace(result, @"\s+", "-");

            return result.ToLower().Trim('-');
        }
    }
}

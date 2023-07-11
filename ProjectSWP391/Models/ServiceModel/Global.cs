using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.RegularExpressions;

namespace ProjectSWP391.Models.ServiceModel
{
    public static class Global
    {
        public static Account? CurrentUser { get; set; } = null;

        public static List<DateTime> GetSchedule()
        {
            List<DateTime> nextFiveDays = new List<DateTime>();
            DateTime today = DateTime.Today;
            nextFiveDays.Add(today);
            for (int i = 0; i < 6; i++)
            {
                DateTime nextDay = today.AddDays(i + 1);
                nextFiveDays.Add(nextDay);
            }
            return nextFiveDays;
        }

        public static string ExtractFirstImage(string content)
        {
            // Find the first occurrence of an image tag using regex
            string pattern = @"<img\s.*?src\s*=\s*['""](.*?)['""].*?>";
            Match match = Regex.Match(content, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                // Extract the image URL from the matched tag
                string imageUrl = match.Groups[1].Value;
                return imageUrl;
            }

            return String.Empty;
        }
    }
}

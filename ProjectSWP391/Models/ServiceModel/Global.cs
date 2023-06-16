using Microsoft.CodeAnalysis.CSharp.Syntax;

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
    }
}

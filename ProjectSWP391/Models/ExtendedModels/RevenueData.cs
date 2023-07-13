namespace ProjectSWP391.Models.ExtendedModels
{
    public class RevenueData
    {
        public int Month { get; set; }
        public decimal Revenue { get; set; }

        public RevenueData(int month, decimal revenue)
        {
            Month = month;
            Revenue = revenue;
        }

    }
}

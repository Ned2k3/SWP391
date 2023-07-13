namespace ProjectSWP391.Models.ExtendedModels
{
    public class MyViewModel
    {

        public List<RevenueData> ProductRevenueByMonth { get; set; }
        public List<RevenueData> ServiceRevenueByMonth { get; set; }
        public List<ProductUserData>? ProductUserDatas { get; set; }
        public List<ServiceUserData>? ServiceUserDatas { get; set; }
    }
}

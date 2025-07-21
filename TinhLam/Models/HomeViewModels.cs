using TinhLam.Data;

namespace TinhLam.Models
{
    public class HomeViewModels
    {
        public List<Product> LatestProducts { get; set; } = new();
        public List<Product> BestSellingProducts { get; set; } = new();
    }
}

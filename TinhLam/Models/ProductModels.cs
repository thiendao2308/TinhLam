using TinhLam.Data;
using System.ComponentModel.DataAnnotations;


namespace TinhLam.Models
{
    public class ProductModels
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public int StockQuantity { get; set; }

        // Nếu có quan hệ với Category
        public int CategoryId { get; set; }
        public CategoryModels CategoryName { get; set; }
    }
}

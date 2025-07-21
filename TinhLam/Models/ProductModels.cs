using TinhLam.Data;
using System.ComponentModel.DataAnnotations;


namespace TinhLam.Models
{
    public class ProductModels
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public int StockQuantity { get; set; }

        // Nếu có quan hệ với Category
        public int CategoryId { get; set; }
        public CategoryModels CategoryName { get; set; } = new();
    }
}

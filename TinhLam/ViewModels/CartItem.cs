namespace TinhLam.ViewModels
{
    public class CartItem
    {
        public int MaProduct { get; set; }
        public string Hinh { get; set; } = string.Empty;
        public string TenProduct { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int SoLuong { get; set; }
        public int? ProductSizeId { get; set; }
        public string Size { get; set; } = string.Empty;
        public decimal ThanhTien => SoLuong * Price;
    }
}

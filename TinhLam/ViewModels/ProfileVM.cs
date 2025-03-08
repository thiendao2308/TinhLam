using System.Collections.Generic;
using TinhLam.Data;

namespace TinhLam.ViewModels
{
    public class ProfileVM
    {
        public int UserId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string DiaChi { get; set; }
        public List<Order> Orders { get; set; }
    }
}

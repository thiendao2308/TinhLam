using Microsoft.AspNetCore.Mvc;
using TinhLam.Data;
using TinhLam.ViewModels;

namespace TinhLam.ViewComponents
{
    public class MenuCategoryViewComponent : ViewComponent
    {
        public readonly TlinhContext db;
        public MenuCategoryViewComponent(TlinhContext context) => db = context;

        public IViewComponentResult Invoke()
        {
            var data = db.Categories.Select(cate => new MenuCategoryVM
            {  
                CategoryId = cate.CategoryId,
                CategoryName = cate.CategoryName,
                SoLuong = cate.Products.Count
            });

            return View(data);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using TinhLam.Data;
using TinhLam.ViewModels;

namespace TinhLam.Components
{
    public class AdminSidebarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShopWeb.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // 1. Xóa chìa khóa (Token)
            Response.Cookies.Delete("AuthToken");
            
            // 2. Xóa tên hiển thị
            Response.Cookies.Delete("UserName");

            // 3. Xóa quyền người dùng
            Response.Cookies.Delete("UserRole");

            // 4. (Tùy chọn) Gửi kèm một thông báo ngọt ngào
            TempData["SuccessMsg"] = "Bạn đã đăng xuất thành công. Hẹn gặp lại!";

            // 5. Lập tức chuyển hướng về trang chủ
            return RedirectToPage("/Index");
        }
    }
}

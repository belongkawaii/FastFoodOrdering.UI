using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace ShopWeb.Pages
{
    public class AdminPageModel : PageModel
    {
        private readonly HttpClient _httpClient;

        // Danh sách sản phẩm để hiển thị trên trang Admin
        public List<Product> Products { get; set; } = new();

        public AdminPageModel(IHttpClientFactory factory)
        {
            // Sử dụng IHttpClientFactory là cách chuẩn trong .NET
            _httpClient = factory.CreateClient();
        }

        public async Task<IActionResult> OnGetAsync() 
        {
            // 1. Kiểm tra Token trong Cookie
            var token = Request.Cookies["AuthToken"];

            // Nếu không có token -> Chưa đăng nhập -> Về trang Signin
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMsg"] = "Vui lòng đăng nhập với quyền Admin.";
                return RedirectToPage("/Signin");
            }

            try 
            {
                // 2. Gắn token vào header để API biết bạn là ai và có quyền gì
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                // 3. Gọi API lấy danh sách sản phẩm
                // Lưu ý: Nếu API của bạn có phân quyền, nó sẽ trả về 403 Forbidden nếu bạn ko phải admin
                var response = await _httpClient.GetAsync("http://localhost:5014/api/products");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<List<Product>>();
                    
                    // Xử lý logic ảnh giống bên Index nếu cần
                    Products = data?.Select(p =>
                    {
                        if (!string.IsNullOrEmpty(p.imageUrl))
                        {
                            p.imageUrl = p.imageUrl.Replace("http://", "https://");
                        }
                        return p;
                    }).ToList() ?? new List<Product>();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden || 
                         response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Nếu Token sai hoặc Token của User thường (Role 1) ko được vào Admin
                    TempData["ErrorMsg"] = "Bạn không có quyền truy cập vào khu vực quản trị!";
                    return RedirectToPage("/Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = "Lỗi kết nối hệ thống: " + ex.Message;
            }

            return Page();
        }
    }
}
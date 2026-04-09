using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace ShopWeb.Pages
{
    public class AdminPageModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<Product> Products { get; set; } = new();

        public AdminPageModel(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        public async Task<IActionResult> OnGetAsync() 
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMsg"] = "Vui lòng đăng nhập với quyền Admin.";
                return RedirectToPage("/Signin");
            }

            try 
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync("http://localhost:5014/api/products");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<List<Product>>();
                    Products = data?.Select(p => {
                        if (!string.IsNullOrEmpty(p.imageUrl)) 
                            p.imageUrl = p.imageUrl.Replace("http://", "https://");
                        return p;
                    }).ToList() ?? new List<Product>();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden || 
                         response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    TempData["ErrorMsg"] = "Bạn không có quyền truy cập!";
                    return RedirectToPage("/Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = "Lỗi kết nối hệ thống: " + ex.Message;
            }

            return Page();
        }

        // 🔥 HÀM XỬ LÝ XÓA SẢN PHẨM
        public async Task<IActionResult> OnPostDeleteProductAsync(int id)
            {
                var token = Request.Cookies["AuthToken"];
                if (string.IsNullOrEmpty(token)) return new JsonResult(new { success = false, message = "Unauthorized" });

                try
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    
                    // Gọi tới API backend của bạn
                    var response = await _httpClient.DeleteAsync($"http://localhost:5014/api/admin/products/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        // Trả về kết quả thành công cho AJAX
                        return new JsonResult(new { success = true, message = "Xóa sản phẩm thành công!" });
                    }
                    
                    return new JsonResult(new { success = false, message = "API trả về lỗi: " + response.StatusCode });
                }
                catch (Exception ex)
                {
                    return new JsonResult(new { success = false, message = ex.Message });
                }
            }
    }
}
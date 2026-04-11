using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace ShopWeb.Pages
{
    public class OrderHistoryModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<OrderDto> Orders { get; set; } = new();

        public OrderHistoryModel(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        private void AttachToken()
        {
            var token = Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // 🔥 HIỂN THỊ DANH SÁCH ĐƠN HÀNG
        public async Task<IActionResult> OnGetAsync()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Signin"); // Bắt buộc đăng nhập

            AttachToken();
            try
            {
                var response = await _httpClient.GetAsync("https://fastfoodorderingsystem-gaeka7bbhncrhnhp.southeastasia-01.azurewebsites.net/api/orders/history");
                if (response.IsSuccessStatusCode)
                {
                    Orders = await response.Content.ReadFromJsonAsync<List<OrderDto>>() ?? new List<OrderDto>();
                }
            }
            catch { Orders = new List<OrderDto>(); }

            return Page();
        }

        // 🔥 XỬ LÝ NÚT HỦY ĐƠN HÀNG (AJAX GỌI VÀO ĐÂY)
        public async Task<JsonResult> OnPostCancelAsync(int orderId)
        {
            AttachToken();
            var response = await _httpClient.PutAsync($"https://fastfoodorderingsystem-gaeka7bbhncrhnhp.southeastasia-01.azurewebsites.net/api/orders/{orderId}/cancel", null);

            if (response.IsSuccessStatusCode)
            {
                return new JsonResult(new { success = true, message = "Đã hủy đơn hàng thành công!" });
            }
            else
            {
                // Đọc lỗi từ Backend (ví dụ: "Đơn hàng đang giao không thể hủy")
                var errorStr = await response.Content.ReadAsStringAsync();
                return new JsonResult(new { success = false, message = "Không thể hủy: " + errorStr });
            }
        }
    }

    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; } // 0: Pending, 1: Confirmed, 2: Paid, 3: Cancelled
    }
}
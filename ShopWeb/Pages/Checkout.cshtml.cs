using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace ShopWeb.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CheckoutModel(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        public CartDTO Cart { get; set; }

        [BindProperty]
        public OrderRequestDTO Order { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        // 🔥 LOAD GIỎ HÀNG
        public async Task OnGetAsync()
        {
            var response = await _httpClient.GetAsync("https://localhost:7214/api/cart");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Cart = JsonSerializer.Deserialize<CartDTO>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }

        // 🔥 ĐẶT HÀNG
        public async Task<IActionResult> OnPostAsync()
        {
            // AC2: VALIDATE
            if (string.IsNullOrEmpty(Order.FullName) ||
                string.IsNullOrEmpty(Order.Phone) ||
                string.IsNullOrEmpty(Order.Address))
            {
                ErrorMessage = "❌ Vui lòng nhập đầy đủ thông tin";
                await OnGetAsync();
                return Page();
            }

            // check giỏ hàng
            var cartResponse = await _httpClient.GetAsync("https://localhost:7214/api/cart");
            var jsonCart = await cartResponse.Content.ReadAsStringAsync();

            Cart = JsonSerializer.Deserialize<CartDTO>(jsonCart, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (Cart == null || Cart.items == null || !Cart.items.Any())
            {
                ErrorMessage = "❌ Giỏ hàng trống";
                return Page();
            }

            // 🔥 GỌI API TẠO ORDER
            var content = new StringContent(JsonSerializer.Serialize(Order), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7214/api/order", content);

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "✅ Đặt hàng thành công!";

                // 🔥 XÓA GIỎ HÀNG
                await _httpClient.DeleteAsync("https://localhost:7214/api/cart/clear");

                Cart = null;
            }
            else
            {
                ErrorMessage = "❌ Đặt hàng thất bại";
            }

            return Page();
        }
    }
}

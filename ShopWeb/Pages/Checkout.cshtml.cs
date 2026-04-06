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

        // ✅ Fix warning null
        public CartDTO? Cart { get; set; }

        [BindProperty]
        public OrderRequestDTO Order { get; set; } = new();

        public string ErrorMessage { get; set; } = "";
        public string SuccessMessage { get; set; } = "";

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
            // ✅ VALIDATE
            if (string.IsNullOrEmpty(Order.FullName) ||
                string.IsNullOrEmpty(Order.Phone) ||
                string.IsNullOrEmpty(Order.Address))
            {
                ErrorMessage = "❌ Vui lòng nhập đầy đủ thông tin";
                await OnGetAsync();
                return Page();
            }

            // 🔥 LOAD GIỎ HÀNG
            var cartResponse = await _httpClient.GetAsync("https://localhost:7214/api/cart");
            var jsonCart = await cartResponse.Content.ReadAsStringAsync();

            Cart = JsonSerializer.Deserialize<CartDTO>(jsonCart, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // ✅ CHECK CART
            if (Cart == null || Cart.Items == null || !Cart.Items.Any())
            {
                ErrorMessage = "❌ Giỏ hàng trống";
                return Page();
            }

            // 🔥 GỌI API ORDER
            var content = new StringContent(
                JsonSerializer.Serialize(Order),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("https://localhost:7214/api/orders", content);

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "✅ Đặt hàng thành công!";

                // 🔥 CLEAR CART
                await _httpClient.DeleteAsync("https://localhost:7214/api/cart/clear");

                Cart = null;
                Order = new OrderRequestDTO(); // reset form
            }
            else
            {
                ErrorMessage = "❌ Đặt hàng thất bại";
            }

            return Page();
        }
    }

    // ===== DTO =====

    public class CartDTO
    {
        public int Id { get; set; }
        public List<CartItemDTO> Items { get; set; } = new();
    }

    public class CartItemDTO
    {
        public string Name { get; set; } = "";
        public int Quantity { get; set; }
        public double Price { get; set; }
    }

    public class OrderRequestDTO
    {
        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
    }
}
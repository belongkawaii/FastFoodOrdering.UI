using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace ShopWeb.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly HttpClient _httpClient;
       
        private readonly string _apiBaseUrl = "https://fastfoodorderingsystem-gaeka7bbhncrhnhp.southeastasia-01.azurewebsites.net/api";

        public CheckoutModel(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        public CartDTO Cart { get; set; }

        [BindProperty]
        public OrderRequestDTO Order { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        // Hàm hỗ trợ gắn Token vào HttpClient
        private void AttachToken()
        {
            // Giả sử bạn lưu token đăng nhập trong Cookie mang tên "AuthToken"
            var token = Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // 🔥 LOAD GIỎ HÀNG
        public async Task<IActionResult> OnGetAsync()
        {
            AttachToken();
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Cart");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                // Nếu giỏ hàng trống, API trả về JSON chứa { message: "..." }
                // Ta cần check xem nó có chứa Items không
                if (json.Contains("\"items\":") || json.Contains("\"Items\":"))
                {
                    Cart = JsonSerializer.Deserialize<CartDTO>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            return Page();
        }

        // 🔥 ĐẶT HÀNG
        public async Task<IActionResult> OnPostAsync()
        {
            AttachToken();
            Order.Phone = NormalizePhoneNumber(Order.Phone);

            // AC2: VALIDATE CƠ BẢN
            if (string.IsNullOrEmpty(Order.FullName) ||
                string.IsNullOrEmpty(Order.Phone) ||
                string.IsNullOrEmpty(Order.Address))
            {
                ErrorMessage = "❌ Vui lòng nhập đầy đủ thông tin giao hàng!";
                await OnGetAsync(); // Load lại giỏ hàng để hiển thị
                return Page();
            }

            if (!Regex.IsMatch(Order.Phone, @"^\d{10,11}$"))
            {
                ErrorMessage = "❌ Số điện thoại phải gồm 10 đến 11 chữ số hợp lệ.";
                await OnGetAsync();
                return Page();
            }

            // 🔥 GỌI API TẠO ORDER
            var content = new StringContent(JsonSerializer.Serialize(Order), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Orders/checkout", content);

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "✅ Đặt hàng thành công! Mã đơn hàng đã được ghi nhận.";
                Cart = null; // Ẩn giỏ hàng đi vì Backend đã dọn sạch rồi
            }
            else
            {
                // Đọc thông báo lỗi từ Backend (VD: "Giỏ hàng trống")
                var errorResponse = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"❌ Đặt hàng thất bại: {errorResponse}";
                await OnGetAsync();
            }

            return Page();
        }

        private static string NormalizePhoneNumber(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return string.Empty;
            }

            return Regex.Replace(phone, @"\D", string.Empty);
        }
    }

    // Các class DTO hứng dữ liệu
    public class CartDTO
    {
        public List<CartItemDTO> Items { get; set; }
        
        public decimal TotalAmount => Items?.Sum(i => i.Quantity * i.Product.Price) ?? 0;
    }

    public class CartItemDTO
    {
        public int Quantity { get; set; }
        public ProductDTO Product { get; set; }
    }

    public class ProductDTO
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderRequestDTO
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Note { get; set; }

        public int PaymentMethod { get; set; } = 0; // 0 = COD, 1 = Transfer  
    }
}

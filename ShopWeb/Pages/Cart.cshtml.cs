using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ShopWeb.Pages
{
    public class CartModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<Product> Products { get; set; } = new();
        public Cart Carts { get; set; } = new();
        public CartModel()
        {
            _httpClient = new HttpClient();
        }
        public async Task OnGetAsync()
        {
            var token = Request.Cookies["JWToken"];
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    // Gắn token vào header để API xác thực được bạn là ai
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    // Gọi API lấy giỏ hàng
                    var cartResponse = await _httpClient.GetAsync("https://localhost:7214/api/cart");

                    if (cartResponse.IsSuccessStatusCode)
                    {
                        // Đổ dữ liệu vào biến UserCart đã khai báo ở ngoài
                        Carts = await cartResponse.Content.ReadFromJsonAsync<Cart>() ?? new Cart();
                    }
                }
                catch
                {
                    // Nếu lỗi API giỏ hàng, khởi tạo giỏ trống để không lỗi trang
                    Carts = new Cart();
                }
            }
        }
    }
}

public class Product
{
    public int id { get; set; }
    public string name { get; set; } = "";
    public decimal price { get; set; }
    public string imageUrl { get; set; } = "";
}

public class CartItem
{
    public int id { get; set; }
    public int productId { get; set; }
    public int quantity { get; set; }
    public int cartId { get; set; }
    public Product product { get; set; }
}

public class Cart
{
    public int id { get; set; }
    public int userId { get; set; }
    public List<CartItem> items { get; set; } = new();

    // Tính tổng tiền: Dùng thuộc tính này để hiển thị trên giao diện
    public decimal TotalAmount => items?.Sum(i => i.quantity * i.product.price) ?? 0;
    public int TotalQuantity => items?.Sum(i => i.quantity) ?? 0;
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ShopWeb.Pages   // 🔥 nhớ đúng namespace project của bạn
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<Product> Products { get; set; } = new();
        public Cart Carts { get; set; } = new();
        public IndexModel()
        {
            _httpClient = new HttpClient();
        }

        /*hàm gọi API lấy dữ liệu sản phẩm*/
        public async Task OnGet() 
        {
            var response = await _httpClient.GetFromJsonAsync<List<Product>>("http://localhost:5014/api/products");
            Products = response?.Select(p =>
            {
                p.imageUrl = p.imageUrl.Replace("http://", "https://");
                return p;
            }).ToList() ?? new List<Product>();

            var token = Request.Cookies["JWToken"];
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    // Gắn token vào header để API xác thực được bạn là ai
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    // Gọi API lấy giỏ hàng
                    var cartResponse = await _httpClient.GetAsync("http://localhost:5014/api/cart");

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

        

        public async Task<IActionResult> OnPostAddItemAsync(int productId)
        {
            // Lấy token từ Cookie
            var token = Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMsg"] = "Vui lòng đăng nhập để đặt hàng!";
                return RedirectToPage();
            }

            // Tạo DTO khớp với yêu cầu của CartController
            var addToCartDto = new AddToCartApiResponse { ProductId = productId, Quantity = 1 };

            try
            {
                // Gắn Token vào Header Authorization
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PostAsJsonAsync("http://localhost:5014/api/cart/item", addToCartDto);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMsg"] = "Đã thêm món ăn vào giỏ hàng thành công!";
                }
                else
                {
                    TempData["ErrorMsg"] = "Không thể thêm vào giỏ. Vui lòng thử lại.";
                }
            }
            catch
            {
                TempData["ErrorMsg"] = "Lỗi hệ thống khi đặt hàng.";
            }


            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetCartAsync()
        {
            // 1. Lấy Token từ Cookie
            var token = Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Index"); // Chưa login thì về trang chủ
            }

            try
            {
                // 2. Gắn Token vào Header
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                // 3. Gọi API lấy giỏ hàng
                var response = await _httpClient.GetAsync("http://localhost:5014/api/cart");

                if (response.IsSuccessStatusCode)
                {
                    // Đọc dữ liệu từ API
                    Carts = await response.Content.ReadFromJsonAsync<Cart>() ?? new Cart();

                    // Xử lý lại đường dẫn ảnh (giống trang Index)
                    foreach (var item in Carts.items)
                    {
                        if (item.product != null && !item.product.imageUrl.StartsWith("http"))
                        {
                            item.product.imageUrl = "http://localhost:5014" + item.product.imageUrl;
                        }
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToPage("/Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = "Không thể tải giỏ hàng: " + ex.Message;
            }

            return Page();
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
    public class AddToCartApiResponse
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}


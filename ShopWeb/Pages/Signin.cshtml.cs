using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShopWeb.Pages
{
    public class SigninModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<Product> Products { get; set; } = new();
        public Cart Carts { get; set; } = new();
        public SigninModel()
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

        
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public async Task<IActionResult> OnPostLoginAsync()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                TempData["ErrorMessageLogin"] = "Vui lòng nhập đầy đủ thông tin.";
                return RedirectToPage();
            }
            try
            {
                // Gọi tới AuthController đã có của bạn
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7214/api/auth/login", new
                {
                    email = Email,
                    password = Password
                });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (result != null && !string.IsNullOrEmpty(result.Token))
                    {
                        // Lưu Token vào Cookie bảo mật
                        Response.Cookies.Append("JWToken", result.Token, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            Expires = DateTimeOffset.UtcNow.AddHours(3)
                        });
                        Response.Cookies.Append("UserName", result.fullName);
                        TempData["SuccessMsg"] = "Chào mừng bạn đã quay trở lại!";
                        return RedirectToPage("/Index");
                    }
                }
                TempData["ErrorMsg"] = "Đăng nhập thất bại. Kiểm tra lại email/mật khẩu.";
                return Page();
            }
            catch
            {
                TempData["ErrorMsg"] = "Lỗi kết nối server xác thực.";
            }
            return Page();
        }
    }
}

public class LoginResponse
{
    public string Token { get; set; } = "";
    public string Message { get; set; } = "";
    public string fullName { get; set; } = "";
}
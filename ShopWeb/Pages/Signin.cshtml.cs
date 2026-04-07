using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShopWeb.Pages
{
    public class SigninModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public Cart Carts { get; set; } = new();

        
        public SigninModel(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public async Task OnGetAsync()
        {
            var token = Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var cartResponse = await _httpClient.GetAsync("http://localhost:5014/api/cart");

                    if (cartResponse.IsSuccessStatusCode)
                    {
                        Carts = await cartResponse.Content.ReadFromJsonAsync<Cart>() ?? new Cart();
                    }
                }
                catch
                {
                    Carts = new Cart();
                }
            }
        }

        public async Task<IActionResult> OnPostLoginAsync()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                TempData["ErrorMessageLogin"] = "Vui lòng nhập đầy đủ thông tin.";
   
                await OnGetAsync();
                return Page();
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("http://localhost:5014/api/auth/login", new
                {
                    email = Email,
                    password = Password
                });

                if (response.IsSuccessStatusCode)
{
    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result != null && !string.IsNullOrEmpty(result.Token))
            {
                // 1. Lưu Token và UserName vào Cookie (như cũ)
                Response.Cookies.Append("AuthToken", result.Token, new CookieOptions {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddHours(3)
                });
                Response.Cookies.Append("UserName", result.fullName);

                // 2. Kiểm tra Role để điều hướng
                if (result.Role == 0) 
                {
                    // Nếu Role là 0 -> Chuyển đến trang Admin
                    TempData["SuccessMsg"] = "Chào mừng Admin quay trở lại!";
                    return RedirectToPage("/AdminPage"); // Đảm bảo bạn có file AdminPage.cshtml
                }
                else 
                {
                    // Nếu Role là 1 (hoặc các giá trị khác) -> Về Index
                    TempData["SuccessMsg"] = "Đăng nhập thành công!";
                    return RedirectToPage("/Index");
                }
            }
        }

                TempData["ErrorMsg"] = "Đăng nhập thất bại. Kiểm tra lại email/mật khẩu.";
                await OnGetAsync(); // Load lại navbar
                return Page();
            }
            catch
            {
                TempData["ErrorMsg"] = "Lỗi kết nối server xác thực.";
                await OnGetAsync(); // Load lại navbar
                return Page();
            }
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; } = "";
        public string Message { get; set; } = "";
        public string fullName { get; set; } = "";

        public int Role { get; set; }
    }

    
}
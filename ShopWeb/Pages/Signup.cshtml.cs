using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;


[IgnoreAntiforgeryToken]
public class SignupModel : PageModel
{
    
    private readonly string _apiUrl = "https://fastfoodorderingsystem-gaeka7bbhncrhnhp.southeastasia-01.azurewebsites.net/api/Auth";

    public void OnGet()
    {
        
    }

    // ==========================================================
    // TRẠM 1: Nhận yêu cầu Gửi OTP từ JS -> Kiểm tra định dạng -> Chuyển tiếp tới API
    // ==========================================================
    public async Task<JsonResult> OnPostSendOtpAsync([FromBody] SendOtpReq req)
    {
        // 1. KIỂM TRA ĐỊNH DẠNG EMAIL
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (string.IsNullOrWhiteSpace(req.email) || !emailRegex.IsMatch(req.email))
        {
            return new JsonResult(new { success = false, message = "Email không hợp lệ! Vui lòng kiểm tra lại." });
        }

        var phoneRegex = new Regex(@"^0\d{9}$");
        if (string.IsNullOrWhiteSpace(req.phone) || !phoneRegex.IsMatch(req.phone))
        {
            return new JsonResult(new
            {
                success = false,
                message = "Số điện thoại không hợp lệ! Vui lòng nhập đủ 10 số và bắt đầu bằng số 0."
            });
        }
        // 2. KIỂM TRA ĐỊNH DẠNG MẬT KHẨU
        // Ít nhất 8 ký tự, 1 chữ hoa, 1 chữ thường, 1 số
        var passRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$");
        if (string.IsNullOrWhiteSpace(req.password) || !passRegex.IsMatch(req.password))
        {
            return new JsonResult(new
            {
                success = false,
                message = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường và số."
            });
        }

        

        // 3. ĐÓNG GÓI GỬI API (Chỉ gửi email đi lấy OTP để tránh Backend báo lỗi dư dữ liệu)
        using var client = new HttpClient();
        var apiPayload = new { email = req.email };
        var content = new StringContent(JsonSerializer.Serialize(apiPayload), Encoding.UTF8, "application/json");

        // Gọi sang API 
        var response = await client.PostAsync($"{_apiUrl}/send-register-otp", content);

        if (response.IsSuccessStatusCode)
        {
            return new JsonResult(new { success = true });
        }

        return new JsonResult(new { success = false, message = "Email đã tồn tại hoặc lỗi hệ thống Backend." });
    }

    // ==========================================================
    // TRẠM 2: Nhận yêu cầu Đăng ký từ JS -> Chuyển tiếp tới API
    // ==========================================================
    public async Task<JsonResult> OnPostRegisterAsync([FromBody] RegisterReq req)
    {
        using var client = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"{_apiUrl}/register", content);

        if (response.IsSuccessStatusCode)
        {
            return new JsonResult(new { success = true });
        }

        return new JsonResult(new { success = false, message = "Mã OTP không đúng hoặc đã hết hạn!" });
    }
}

// --- CÁC CLASS PHỤ TRỢ (Để hứng dữ liệu JSON từ JavaScript gửi lên) ---
public class SendOtpReq
{
    public string email { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
    public string phone { get; set; } = string.Empty;
}

public class RegisterReq
{
    public string fullName { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string phone { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
    public string confirmPassword { get; set; } = string.Empty;
    public string otpCode { get; set; } = string.Empty;
}
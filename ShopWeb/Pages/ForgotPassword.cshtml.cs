using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

[IgnoreAntiforgeryToken]
public class ForgotPasswordModel : PageModel
{
    private readonly string _apiUrl = "https://fastfoodorderingsystem-gaeka7bbhncrhnhp.southeastasia-01.azurewebsites.net/api/Auth";

    public void OnGet()
    {
    }

    // ==========================================================
    // TRẠM 1: Nhận yêu cầu Gửi OTP
    // ==========================================================
    public async Task<JsonResult> OnPostSendForgotOtpAsync([FromBody] SendForgotOtpReq req)
    {
        using var client = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json");

        // Gọi sang API quên mật khẩu vừa viết
        var response = await client.PostAsync($"{_apiUrl}/send-forgot-password-otp", content);

        if (response.IsSuccessStatusCode)
        {
            return new JsonResult(new { success = true });
        }

        return new JsonResult(new { success = false, message = "Email này không tồn tại trong hệ thống." });
    }

    // ==========================================================
    // TRẠM 2: Nhận yêu cầu Đổi mật khẩu
    // ==========================================================
    public async Task<JsonResult> OnPostResetPasswordAsync([FromBody] ResetPasswordReq req)
    {
        // 1. KIỂM TRA MẬT KHẨU CÓ KHỚP NHAU KHÔNG
        if (req.newPassword != req.confirmPassword)
        {
            return new JsonResult(new { success = false, message = "Mật khẩu xác nhận không khớp!" });
        }

        // 2. KIỂM TRA ĐỊNH DẠNG MẬT KHẨU (Giống trang Đăng ký)
        var passRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$");
        if (string.IsNullOrWhiteSpace(req.newPassword) || !passRegex.IsMatch(req.newPassword))
        {
            return new JsonResult(new
            {
                success = false,
                message = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường và số."
            });
        }

        // 3. NẾU MỌI THỨ ĐÃ CHUẨN MỰC, TIẾN HÀNH GỌI API
        using var client = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"{_apiUrl}/reset-password", content);

        if (response.IsSuccessStatusCode)
        {
            return new JsonResult(new { success = true });
        }

        
        return new JsonResult(new { success = false, message = "Mã OTP không đúng hoặc đã hết hạn!" });
    }
}

// --- CLASS PHỤ TRỢ HỨNG DỮ LIỆU ---
public class SendForgotOtpReq
{
    public string email { get; set; } = string.Empty;
}

public class ResetPasswordReq
{
    public string email { get; set; } = string.Empty;
    public string otpCode { get; set; } = string.Empty;
    public string newPassword { get; set; } = string.Empty;
    public string confirmPassword { get; set; } = string.Empty;
}
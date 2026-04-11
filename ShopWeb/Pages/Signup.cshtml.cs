using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

// Bỏ qua kiểm tra token nội bộ để JavaScript có thể dễ dàng gọi vào hàm này
[IgnoreAntiforgeryToken]
public class SignupModel : PageModel
{
    // Địa chỉ API gốc đang chạy trên cổng 5014
    private readonly string _apiUrl = "https://fastfoodorderingsystem-gaeka7bbhncrhnhp.southeastasia-01.azurewebsites.net/api/Auth";

    public void OnGet()
    {
        // Vẫn để trống để load giao diện HTML
    }

    // ==========================================================
    // TRẠM 1: Nhận yêu cầu Gửi OTP từ JS -> Chuyển tiếp tới API
    // ==========================================================
    public async Task<JsonResult> OnPostSendOtpAsync([FromBody] SendOtpReq req)
    {
        using var client = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json");

        // Gọi sang API 5014
        var response = await client.PostAsync($"{_apiUrl}/send-register-otp", content);

        if (response.IsSuccessStatusCode)
        {
            return new JsonResult(new { success = true });
        }

        return new JsonResult(new { success = false, message = "Email đã tồn tại hoặc lỗi hệ thống." });
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
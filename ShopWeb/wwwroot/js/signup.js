document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("form-register");
    const errorMessage = document.getElementById("error-message");
    const submitBtn = document.getElementById("submitBtn");
    const otpInput = document.getElementById("otpCode");
    const emailInput = document.getElementById("email");

    if (!form) return;

    let isOtpSent = false; // Biến cờ: False = Chưa gửi mail, True = Đã gửi mail

    form.addEventListener("submit", async (e) => {
        // Chặn load lại trang
        e.preventDefault();
        errorMessage.innerHTML = "";
        errorMessage.classList.add("hidden");

        // 1. Lấy tất cả dữ liệu và làm sạch khoảng trắng (trim)
        const fullName = document.getElementById("fullName")?.value.trim();
        const email = emailInput?.value.trim();
        const phone = document.getElementById("phone")?.value.trim();
        const password = document.getElementById("password")?.value;
        const confirmPassword = document.getElementById("confirmPassword")?.value;

        // ==========================================
        // 2. BỘ KIỂM TRA DỮ LIỆU (VALIDATION CƠ BẢN TẠI JS)
        // ==========================================

        // Kiểm tra rỗng tất cả các trường
        if (!fullName || !email || !phone || !password || !confirmPassword) {
            errorMessage.textContent = "Vui lòng điền đầy đủ tất cả thông tin!";
            errorMessage.classList.remove("hidden");
            return; // Chặn không cho gọi API
        }

        // Kiểm tra mật khẩu khớp nhau
        if (password !== confirmPassword) {
            errorMessage.textContent = "Mật khẩu xác nhận không khớp!";
            errorMessage.classList.remove("hidden");
            return;
        }

        // Kiểm tra độ dài OTP nếu đang ở bước 2
        if (isOtpSent) {
            const currentOtp = otpInput?.value.trim();
            if (!currentOtp || currentOtp.length < 6) {
                errorMessage.textContent = "Vui lòng nhập đúng 6 số OTP!";
                errorMessage.classList.remove("hidden");
                return;
            }
        }

        // ==========================================
        // BƯỚC 1: NẾU CHƯA GỬI MAIL -> GỌI API SEND OTP (CÓ KÈM PASS VÀ PHONE ĐỂ C# KIỂM TRA)
        // ==========================================
        if (!isOtpSent) {
            submitBtn.textContent = "Đang kiểm tra và gửi email...";
            submitBtn.disabled = true;

            try {
                const res = await fetch("?handler=SendOtp", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    // 🔥 SỬA TẠI ĐÂY: Gửi cả 3 biến email, password, phone lên cho class SendOtpReq của C#
                    body: JSON.stringify({
                        email: email,
                        password: password,
                        phone: phone
                    })
                });

                const result = await res.json();

                if (result.success) {
                    isOtpSent = true;

                    // Hiệu ứng giao diện
                    otpInput.classList.remove("hidden");
                    emailInput.readOnly = true;

                    // Khóa các ô khác lại để người dùng không sửa sau khi đã gửi OTP
                    document.getElementById("phone").readOnly = true;
                    document.getElementById("password").readOnly = true;
                    document.getElementById("confirmPassword").readOnly = true;

                    submitBtn.textContent = "Hoàn tất Đăng ký";
                    submitBtn.style.backgroundColor = "#28a745";

                    alert("Mã OTP đã được gửi! Vui lòng kiểm tra email.");
                } else {
                    // Nếu C# kiểm tra Regex thấy lỗi (vd: nhập "1"), nó sẽ trả về message lỗi ở đây
                    errorMessage.textContent = result.message || "Email này đã tồn tại hoặc có lỗi.";
                    errorMessage.classList.remove("hidden");
                }
            } catch (err) {
                errorMessage.textContent = "Lỗi kết nối server API";
                errorMessage.classList.remove("hidden");
            } finally {
                if (!isOtpSent) submitBtn.textContent = "Gửi mã OTP";
                submitBtn.disabled = false;
            }
        }
        // ==========================================
        // BƯỚC 2: NẾU ĐÃ GỬI MAIL -> GỌI API REGISTER
        // ==========================================
        else {
            submitBtn.textContent = "Đang xử lý...";
            submitBtn.disabled = true;

            const data = {
                fullName: fullName,
                email: email,
                phone: phone,
                password: password,
                confirmPassword: confirmPassword,
                otpCode: otpInput?.value.trim()
            };

            try {
                const res = await fetch("?handler=Register", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(data)
                });

                const result = await res.json();

                if (result.success) {
                    alert("Đăng ký thành công!");
                    window.location.href = "/Signin"; // Đổi link này nếu trang đăng nhập của bạn tên khác
                } else {
                    errorMessage.textContent = result.message || "Mã OTP sai hoặc đã hết hạn!";
                    errorMessage.classList.remove("hidden");
                }
            } catch (err) {
                errorMessage.textContent = "Lỗi kết nối server API";
                errorMessage.classList.remove("hidden");
            } finally {
                submitBtn.textContent = "Hoàn tất Đăng ký";
                submitBtn.disabled = false;
            }
        }
    });
});
function togglePassword(inputId, icon) {
    const input = document.getElementById(inputId);
    if (input.type === "password") {
        input.type = "text"; // Biến thành text để nhìn thấy chữ
        icon.textContent = "🙈"; // Đổi icon thành nhắm mắt
    } else {
        input.type = "password"; // Trả lại thành dấu chấm
        icon.textContent = "👁️"; // Đổi icon thành mở mắt
    }
}
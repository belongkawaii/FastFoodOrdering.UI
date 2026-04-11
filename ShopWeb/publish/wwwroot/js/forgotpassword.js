document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("form-forgot");
    const errorMessage = document.getElementById("error-message");
    const submitBtn = document.getElementById("submitBtn");
    
    // Các input
    const emailInput = document.getElementById("email");
    const otpInput = document.getElementById("otpCode");
    const newPasswordInput = document.getElementById("newPassword");
    const confirmPasswordInput = document.getElementById("confirmPassword");
    
    // Các thẻ div chứa UI bước 1 và 2
    const step1Div = document.getElementById("step-1-email");
    const step2Div = document.getElementById("step-2-reset");

    if (!form) return;

    let isOtpSent = false; 

    form.addEventListener("submit", async (e) => {
        e.preventDefault();
        errorMessage.innerHTML = "";
        errorMessage.classList.add("hidden");

        const email = emailInput?.value.trim();

        // VALIDATION CHUNG
        if (!email) {
            errorMessage.textContent = "Vui lòng nhập email!";
            errorMessage.classList.remove("hidden");
            return;
        }

        // ==========================================
        // BƯỚC 1: CHƯA GỬI MAIL -> GỌI API SEND OTP
        // ==========================================
        if (!isOtpSent) {
            submitBtn.textContent = "Đang kiểm tra...";
            submitBtn.disabled = true;

            try {
                const res = await fetch("?handler=SendForgotOtp", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ email: email })
                });

                const result = await res.json();

                if (result.success) {
                    isOtpSent = true;

                    // Hiệu ứng chuyển giao diện sang Bước 2
                    step1Div.classList.add("hidden"); // Ẩn chỗ nhập email đi
                    step2Div.classList.remove("hidden"); // Hiện chỗ nhập OTP và Pass
                    
                    submitBtn.textContent = "Xác nhận đổi mật khẩu";
                    submitBtn.style.backgroundColor = "#28a745";

                    alert("Mã OTP đã được gửi! Vui lòng kiểm tra email.");
                } else {
                    errorMessage.textContent = result.message;
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
        // BƯỚC 2: ĐÃ GỬI MAIL -> GỌI API RESET PASSWORD
        // ==========================================
        else {
            const otpCode = otpInput?.value.trim();
            const newPassword = newPasswordInput?.value;
            const confirmPassword = confirmPasswordInput?.value;

            // Validation riêng cho bước 2
            if (!otpCode || !newPassword || !confirmPassword) {
                errorMessage.textContent = "Vui lòng điền đầy đủ OTP và Mật khẩu mới!";
                errorMessage.classList.remove("hidden");
                return;
            }

            if (newPassword !== confirmPassword) {
                errorMessage.textContent = "Xác nhận mật khẩu không khớp!";
                errorMessage.classList.remove("hidden");
                return;
            }

            submitBtn.textContent = "Đang xử lý...";
            submitBtn.disabled = true;

            const data = {
                email: email, // Gửi kèm email cũ đi để BE đối chiếu
                otpCode: otpCode,
                newPassword: newPassword,
                confirmPassword: confirmPassword
            };

            try {
                const res = await fetch("?handler=ResetPassword", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(data)
                });

                const result = await res.json();

                if (result.success) {
                    alert("Đặt lại mật khẩu thành công! Vui lòng đăng nhập bằng mật khẩu mới.");
                    window.location.href = "/Signin"; // Đá về trang đăng nhập
                } else {
                    errorMessage.textContent = result.message;
                    errorMessage.classList.remove("hidden");
                }
            } catch (err) {
                errorMessage.textContent = "Lỗi kết nối server API";
                errorMessage.classList.remove("hidden");
            } finally {
                submitBtn.textContent = "Xác nhận đổi mật khẩu";
                submitBtn.disabled = false;
            }
        }
    });
});
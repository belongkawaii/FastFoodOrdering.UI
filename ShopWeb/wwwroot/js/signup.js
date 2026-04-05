
const email = document.getElementById("email");
const password = document.getElementById("password");
const confirmPassword = document.getElementById("confirmPassword");
const phoneNumber = document.getElementById("phone");
const fullName = document.getElementById("fullName");
const form = document.getElementById("form-register");
const errorMessage = document.getElementById("error-message");

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
        // 2. BỘ KIỂM TRA DỮ LIỆU (VALIDATION CHẶT CHẼ)
        // ==========================================

        // Kiểm tra rỗng tất cả các trường
        if (!fullName || !email || !phone || !password || !confirmPassword) {
            errorMessage.textContent = "Vui lòng điền đầy đủ tất cả thông tin!";
            errorMessage.classList.remove("hidden");
            return; // Chặn không cho gọi API
        }

        // Kiểm tra mật khẩu
        if (password !== confirmPassword) {
            errorMessage.textContent = "Mật khẩu không khớp!";
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
        // BƯỚC 1: NẾU CHƯA GỬI MAIL -> GỌI API SEND OTP
        // ==========================================
        if (!isOtpSent) {
            submitBtn.textContent = "Đang gửi email...";
            submitBtn.disabled = true;

            try {
                const res = await fetch("?handler=SendOtp", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ email: email })
                });

                const result = await res.json();

                // DÙNG result.success ĐỂ KIỂM TRA, KHÔNG DÙNG res.ok NỮA
                if (result.success) {
                    isOtpSent = true;

                    // Hiệu ứng giao diện
                    otpInput.classList.remove("hidden");
                    emailInput.readOnly = true;
                    submitBtn.textContent = "Hoàn tất Đăng ký";
                    submitBtn.style.backgroundColor = "#28a745";

                    alert("Mã OTP đã được gửi! Vui lòng kiểm tra email.");
                } else {
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

                // DÙNG result.success ĐỂ KIỂM TRA
                if (result.success) {
                    alert("Đăng ký thành công!");
                    window.location.href = "/Index";
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
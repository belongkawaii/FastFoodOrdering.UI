document.addEventListener("DOMContentLoaded", function () {
    const checkoutForm = document.querySelector("form");
    const submitBtn = document.querySelector(".order-btn");

    if (checkoutForm && submitBtn) {
        checkoutForm.addEventListener("submit", function () {
            // Khi form bắt đầu submit (đã vượt qua required của HTML5)
            // Đổi text và vô hiệu hóa nút bấm
            submitBtn.innerHTML = "⏳ Đang xử lý đơn hàng...";
            submitBtn.style.backgroundColor = "#888";
            
            // Dùng setTimeout để đảm bảo form vẫn kịp gửi đi trước khi nút bị disabled
            setTimeout(() => {
                submitBtn.disabled = true;
            }, 10);
        });
    }
});
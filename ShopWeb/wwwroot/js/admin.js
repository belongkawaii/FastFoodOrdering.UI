function showSection(sectionId, element) {

    // 🔥 đổi section
    const sections = document.querySelectorAll('.section');
    sections.forEach(sec => sec.classList.remove('active'));

    document.getElementById(sectionId).classList.add('active');

    // 🔥 đổi menu active
    const menuItems = document.querySelectorAll('.menu-item');
    menuItems.forEach(item => item.classList.remove('active'));

    element.classList.add('active');

    function showSection(sectionId) {
        const sections = document.querySelectorAll('.section');
        sections.forEach(sec => sec.classList.remove('active'));

        document.getElementById(sectionId).classList.add('active');
    }
}

async function deleteProduct(id) {
    // 1. Xác nhận với người dùng
    if (!confirm("Bạn có chắc chắn muốn xóa sản phẩm này không?")) {
        return;
    }

    try {
        // 2. Gọi API xóa
        const res = await fetch(`http://localhost:5014//api/admin/products/${id}`, {
            method: "DELETE",
            headers: {
                "Content-Type": "application/json"
                // Nếu có Token bảo mật, hãy thêm vào đây:
                // "Authorization": `Bearer ${yourToken}`
            }
        });

        // 3. Xử lý kết quả
        if (res.ok) {
            alert("Xóa sản phẩm thành công!");
            // Tải lại trang hoặc xóa dòng đó khỏi table để cập nhật giao diện
            location.reload(); 
        } else {
            const errorData = await res.json();
            alert("Lỗi: " + (errorData.message || "Không thể xóa sản phẩm."));
        }
    } catch (error) {
        console.error("Error deleting product:", error);
        alert("Đã xảy ra lỗi khi kết nối đến máy chủ.");
    }
}
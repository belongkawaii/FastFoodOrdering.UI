
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

    if (!form) {
        console.error("Không tìm thấy form");
        return;
    }

    form.addEventListener("submit", async (e) => {
        e.preventDefault();

        const data = {
            fullName: document.getElementById("fullName")?.value,
            email: document.getElementById("email")?.value,
            phone: document.getElementById("phone")?.value,
            password: document.getElementById("password")?.value,
            confirmPassword: document.getElementById("confirmPassword")?.value
        };

        try {
            const res = await fetch("https://localhost:7214/api/auth/register", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(data)
            });

            const result = await res.json();

            if (res.ok) {
                alert("Đăng ký thành công!");
                errorMessage.classList.add("hidden");
            } else {
                errorMessage.innerHTML = "";

                if (result.errors) {
                    for (const key in result.errors) {
                        result.errors[key].forEach(msg => {
                            const p = document.createElement("p");
                            p.textContent = msg;
                            errorMessage.appendChild(p);
                        });
                    }
                }

                errorMessage.classList.remove("hidden");
            }

        } catch (err) {
            console.error(err);
            errorMessage.textContent = "Lỗi kết nối server";
            errorMessage.classList.remove("hidden");
        }
    });
});
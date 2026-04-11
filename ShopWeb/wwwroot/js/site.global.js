const btnClose = document.querySelector(".btn-x");
const loginContainer = document.querySelector(".login-page");
const btnOpen = document.querySelector(".btn-open");
const overlay = document.querySelector(".overlay");

if (btnOpen && loginContainer && overlay) {
    btnOpen.addEventListener("click", (e) => {
        e.preventDefault();
        loginContainer.classList.remove("hidden");
        overlay.classList.remove("hidden");
    });
}

function closePageLogin() {
    if (!loginContainer || !overlay) {
        return;
    }

    loginContainer.classList.add("hidden");
    overlay.classList.add("hidden");
}

if (overlay) {
    overlay.addEventListener("click", closePageLogin);
}

if (btnClose) {
    btnClose.addEventListener("click", closePageLogin);
}

let count = 0;
const btnClose = document.querySelector(".btn-x")
const loginContainer = document.querySelector('.login-page')
const btnOpen = document.querySelector('.btn-open')
const overlay = document.querySelector('.overlay')
const form = document.querySelector('form')
const emailInput = document.getElementById('email')
const passwordInput = document.getElementById('password')
const errorMessage = document.getElementById('error-msg')

function addToCart() {
    count++;
    document.getElementById("cart-count").innerText = count;
}

// Banner auto slide
const banners = [
    "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=1200",
    "https://images.unsplash.com/photo-1490645935967-10de6ba17061?w=1200",
];

let index = 0;

setInterval(() => {
    index = (index + 1) % banners.length;
    document.getElementById("banner-img").src = banners[index];
}, 3000);

document.querySelectorAll('.product-name').forEach(function(el) {
    const span = el.querySelector('span');
    const containerWidth = el.offsetWidth;

    // chỉ chạy nếu text dài hơn container
    if (span.scrollWidth > containerWidth) {
        el.addEventListener('mouseenter', () => {
            const distance = span.scrollWidth - containerWidth;
            span.style.transition = `transform ${distance/20}s linear`;
            span.style.transform = `translateX(-${distance}px)`;
        });

        el.addEventListener('mouseleave', () => {
            span.style.transition = 'transform 0.3s ease';
            span.style.transform = 'translateX(0)';
        });
    }
});

btnOpen.addEventListener('click', (e) => {
    e.preventDefault()
    loginContainer.classList.remove('hidden')
    overlay.classList.remove('hidden')
})
function closePageLogin() {
    loginContainer.classList.add('hidden')
    overlay.classList.add('hidden')
}

overlay.addEventListener('click', closePageLogin)

btnClose.addEventListener('click', closePageLogin)

//form.addEventListener('submit', async (e) => {
//    e.preventDefault()

//    const email = emailInput.value
//    const password = passwordInput.value

//    try {
//        const res = await fetch("https://localhost:7214/api/auth/login", {
//            method: "POST",
//            headers: {
//                "Content-Type": "application/json"
//            },
//            body: JSON.stringify({ email, password })
//        })

//        const data = await res.json()

//        if (!res.ok) throw new Error(data.message || "Sai tài khoản hoặc mật khẩu")


//        errorMessage.classList.add('hidden')

//        alert("Đăng nhập thành công!")

//        closePageLogin()
//    }
//    catch (err) {
//        errorMessage.textContent = err.message
//        errorMessage.classList.remove("hidden")
//    }
//})
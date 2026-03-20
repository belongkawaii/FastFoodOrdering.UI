let count = 0;
const btnClose = document.querySelector(".btn-x")
const loginContainer = document.querySelector('.login-page')
const btnOpen = document.querySelector('.btn-open')
const overlay = document.querySelector('.overlay')
function addToCart() {
    count++;
    document.getElementById("cart-count").innerText = count;
}

// Banner auto slide
const banners = [
    "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=1200",
    "https://images.unsplash.com/photo-1490645935967-10de6ba17061?w=1200",
    "https://images.unsplash.com/photo-1506089676908-3592f7389d4d?w=1200"
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
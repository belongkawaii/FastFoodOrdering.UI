const bannerImg = document.getElementById("banner-img");
const navToggle = document.querySelector(".nav-toggle");
const homeNavbar = document.querySelector(".home-navbar");

const banners = [
    "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=1200",
    "https://images.unsplash.com/photo-1490645935967-10de6ba17061?w=1200",
];

if (bannerImg) {
    let currentBannerIndex = 0;

    setInterval(() => {
        currentBannerIndex = (currentBannerIndex + 1) % banners.length;
        bannerImg.src = banners[currentBannerIndex];
    }, 3000);
}

if (navToggle && homeNavbar) {
    navToggle.addEventListener("click", () => {
        const isOpen = homeNavbar.classList.toggle("is-open");
        navToggle.setAttribute("aria-expanded", isOpen ? "true" : "false");
    });
}

document.addEventListener("DOMContentLoaded", function () {
    const checkoutForm = document.querySelector(".checkout-container form");
    const phoneInput = document.getElementById("Order_Phone");

    function formatPhoneNumber(value) {
        const digits = value.replace(/\D/g, "").slice(0, 11);

        if (digits.length <= 4) {
            return digits;
        }

        if (digits.length <= 7) {
            return `${digits.slice(0, 4)} ${digits.slice(4)}`;
        }

        return `${digits.slice(0, 4)} ${digits.slice(4, 7)} ${digits.slice(7)}`;
    }

    if (phoneInput) {
        phoneInput.addEventListener("input", function () {
            phoneInput.value = formatPhoneNumber(phoneInput.value);
        });
    }

    if (checkoutForm) {
        checkoutForm.addEventListener("submit", function () {
            if (phoneInput) {
                phoneInput.value = phoneInput.value.replace(/\D/g, "");
            }
        });
    }
});

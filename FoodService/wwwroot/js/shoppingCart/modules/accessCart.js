import initAlertMessage from "./alertMessage.js";

export default function initAccessCart() {
    const cartContainer = document.querySelector(".modal-cart");
    const cartButton = document.querySelector(".cart-button");

    if (!cartButton) {
        console.error("Cart button not found");
        return;
    }

    if (!cartContainer) {
        console.error("Cart container not found");
        return;
    }

    function openOrCloseCart() {
        cartContainer.classList.toggle("cart-open");
        initAlertMessage(true);
    }

    cartButton.addEventListener("click", openOrCloseCart);
}
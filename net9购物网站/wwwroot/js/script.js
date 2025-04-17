function searchProducts() {
    const query = document.getElementById("searchInput").value.toLowerCase();
    const products = document.querySelectorAll(".product-card");

    products.forEach(product => {
        const title = product.querySelector(".product-title").textContent.toLowerCase();
        if (title.includes(query)) {
            product.style.display = "block";
        } else {
            product.style.display = "none";
        }
    });
}

function openCart() {
    window.location.href = "/Cart";
}

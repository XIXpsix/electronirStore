document.addEventListener("DOMContentLoaded", function () {
    const nameSearch = document.getElementById("nameSearch");
    const minPrice = document.getElementById("minPrice");
    const maxPrice = document.getElementById("maxPrice");
    const sortType = document.getElementById("sortType");

    function debounce(func, timeout = 500) {
        let timer;
        return (...args) => {
            clearTimeout(timer);
            timer = setTimeout(() => { func.apply(this, args); }, timeout);
        };
    }

    const debouncedFilter = debounce(() => applyFilters());

    if (nameSearch) nameSearch.addEventListener("input", debouncedFilter);
    if (minPrice) minPrice.addEventListener("input", debouncedFilter);
    if (maxPrice) maxPrice.addEventListener("input", debouncedFilter);
    if (sortType) sortType.addEventListener("change", applyFilters);
});

function selectCategory(id, element) {
    document.getElementById("categoryId").value = id;
    const buttons = document.querySelectorAll("#categoryList button");
    buttons.forEach(btn => {
        btn.classList.remove("active");
        btn.classList.remove("bg-warning");
        btn.classList.add("bg-dark");
        btn.classList.add("text-white");
    });
    element.classList.remove("bg-dark");
    element.classList.remove("text-white");
    element.classList.add("active");
    applyFilters();
}

async function applyFilters() {
    const categoryIdVal = document.getElementById("categoryId").value;
    const minPriceEl = document.getElementById("minPrice");
    const maxPriceEl = document.getElementById("maxPrice");
    const nameSearchEl = document.getElementById("nameSearch");
    const sortTypeEl = document.getElementById("sortType");

    const data = {
        categoryId: parseInt(categoryIdVal) || 0,
        minPrice: parseFloat(minPriceEl?.value) || 0,
        maxPrice: parseFloat(maxPriceEl?.value) || 0,
        name: nameSearchEl?.value || "",
        sortType: sortTypeEl?.value || "default"
    };

    try {
        const response = await fetch('/Product/Filter', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (!response.ok) throw new Error('Ошибка сети');

        const result = await response.json();
        const container = document.getElementById("productsContainer");

        if (!container) return;

        container.innerHTML = "";

        if (result.data && result.data.length > 0) {
            result.data.forEach(product => {
                // ИСПРАВЛЕНО: Убрана заглушка /img/placeholder.jpg
                const productHtml = `
                    <div class="col-12 col-sm-6 col-md-4 col-lg-3 mb-4 fade-in">
                        <div class="card h-100 border-secondary" style="background-color: #1e1e1e;">
                            <div class="position-relative bg-white overflow-hidden rounded-top" style="height: 200px; display: flex; align-items: center; justify-content: center;">
                                <img src="${product.imagePath || ''}" alt="${product.name}" style="max-height: 90%; max-width: 90%; object-fit: contain;">
                            </div>
                            <div class="card-body d-flex flex-column p-3">
                                <h6 class="card-title text-warning text-truncate mb-1" title="${product.name}">
                                    ${product.name}
                                </h6>
                                <p class="small text-muted mb-2">${product.category ? product.category.name : ''}</p>
                                <div class="mt-auto">
                                    <h5 class="text-white mb-2">${(product.price || 0).toLocaleString()} ₽</h5>
                                    <a href="/Product/GetProduct/${product.id}" class="btn btn-warning w-100 fw-bold">Подробнее</a>
                                </div>
                            </div>
                        </div>
                    </div>
                `;
                container.insertAdjacentHTML('beforeend', productHtml);
            });
        } else {
            container.innerHTML = `
                <div class="col-12 text-center py-5">
                    <h3 class="text-muted fw-bold">Товаров нет</h3>
                    <p class="text-secondary">В этой категории пока пусто.</p>
                </div>
            `;
        }

    } catch (error) {
        console.error('Ошибка:', error);
    }
}
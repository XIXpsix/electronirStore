document.addEventListener("DOMContentLoaded", function () {

    // --- ЛОГИКА СЛАЙДЕРА ЦЕНЫ ---
    const rangeInput = document.querySelectorAll(".range-input input"),
        priceInput = document.querySelectorAll(".price-input input"),
        range = document.querySelector(".slider .progress");

    let priceGap = 1000;

    function updateProgress() {
        let minVal = parseInt(rangeInput[0].value),
            maxVal = parseInt(rangeInput[1].value);
        let maxRange = parseInt(rangeInput[0].max);

        range.style.left = ((minVal / maxRange) * 100) + "%";
        range.style.right = (100 - (maxVal / maxRange) * 100) + "%";
    }

    priceInput.forEach(input => {
        input.addEventListener("input", e => {
            let minPrice = parseInt(priceInput[0].value),
                maxPrice = parseInt(priceInput[1].value);
            let maxRange = parseInt(rangeInput[0].max);

            if ((maxPrice - minPrice >= priceGap) && maxPrice <= maxRange) {
                if (e.target.className.includes("input-min")) {
                    rangeInput[0].value = minPrice;
                    range.style.left = ((minPrice / maxRange) * 100) + "%";
                } else {
                    rangeInput[1].value = maxPrice;
                    range.style.right = (100 - (maxPrice / maxRange) * 100) + "%";
                }
            }
        });
    });

    rangeInput.forEach(input => {
        input.addEventListener("input", e => {
            let minVal = parseInt(rangeInput[0].value),
                maxVal = parseInt(rangeInput[1].value);

            if ((maxVal - minVal) < priceGap) {
                if (e.target.className.includes("range-min")) {
                    rangeInput[0].value = maxVal - priceGap;
                } else {
                    rangeInput[1].value = minVal + priceGap;
                }
            } else {
                priceInput[0].value = minVal;
                priceInput[1].value = maxVal;
                updateProgress();
            }
        });
    });

    updateProgress();

    // --- ЛОГИКА ОТПРАВКИ ДАННЫХ (AJAX) ---

    async function fetchProducts() {
        const minPrice = parseInt(priceInput[0].value);
        const maxPrice = parseInt(priceInput[1].value);
        const sortType = document.getElementById('sortSelect').value;

        // Получаем ID категории из скрытого поля в List.cshtml
        const categoryId = document.getElementById('categoryId')?.value || 0;

        const filterData = {
            CategoryId: parseInt(categoryId),
            MinPrice: minPrice,
            MaxPrice: maxPrice,
            SortType: sortType
        };

        try {
            const response = await fetch('/Product/Filter', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(filterData)
            });

            if (response.ok) {
                const result = await response.json();
                renderProducts(result.data);
            } else {
                console.error("Ошибка при получении данных");
            }
        } catch (error) {
            console.error("Ошибка сети:", error);
        }
    }

    // Функция перерисовки товаров
    function renderProducts(products) {
        const container = document.getElementById('productsContainer');
        container.innerHTML = '';

        if (!products || products.length === 0) {
            container.innerHTML = '<div class="col-12"><div class="alert alert-info">Товары не найдены</div></div>';
            return;
        }

        products.forEach(product => {
            // Генерируем HTML для одной карточки
            const productHtml = `
                <div class="col product-card">
                    <div class="card h-100">
                        <div class="card-body">
                            <h5 class="card-title">${product.name}</h5>
                            <p class="card-text text-truncate">${product.description}</p>
                            <h6 class="text-primary">${product.price} ₽</h6>
                        </div>
                        <div class="card-footer bg-transparent border-top-0">
                            <a href="/Product/GetProduct?id=${product.id}" class="btn btn-primary w-100">Подробнее</a>
                        </div>
                    </div>
                </div>
            `;
            container.insertAdjacentHTML('beforeend', productHtml);
        });
    }

    // Привязываем события
    const applyBtn = document.getElementById('applyFiltersBtn');
    if (applyBtn) {
        applyBtn.addEventListener('click', fetchProducts);
    }

    const sortSelect = document.getElementById('sortSelect');
    if (sortSelect) {
        sortSelect.addEventListener('change', fetchProducts);
    }
});
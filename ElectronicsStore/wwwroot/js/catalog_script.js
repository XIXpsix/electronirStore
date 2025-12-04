document.addEventListener("DOMContentLoaded", function () {

    // --- НАСТРОЙКИ СЛАЙДЕРА ---
    const rangeInput = document.querySelectorAll(".range-input input"),
        priceInput = document.querySelectorAll(".price-input input"),
        range = document.querySelector(".slider .progress");

    let priceGap = 1000;

    // Обновление полоски прогресса
    function updateProgress() {
        let minVal = parseInt(rangeInput[0].value),
            maxVal = parseInt(rangeInput[1].value);
        let maxRange = parseInt(rangeInput[0].max); // Берем макс из атрибута

        if (maxRange === 0) return; // Защита

        range.style.left = ((minVal / maxRange) * 100) + "%";
        range.style.right = (100 - (maxVal / maxRange) * 100) + "%";
    }

    // Ввод вручную в поля (Input numbers)
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

    // Движение ползунков (Range sliders)
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

    // Инициализация при загрузке
    updateProgress();


    // --- ЛОГИКА AJAX (Сортировка и Фильтрация) ---

    const productsContainer = document.getElementById('productsContainer');
    const categoryId = document.getElementById('categoryId')?.value || 0;

    async function fetchProducts() {
        // Собираем данные
        const minPrice = parseInt(priceInput[0].value) || 0;
        const maxPrice = parseInt(priceInput[1].value) || 300000;
        const sortType = document.getElementById('sortSelect').value;

        // Показываем загрузку (опционально)
        productsContainer.style.opacity = '0.5';

        const filterData = {
            CategoryId: parseInt(categoryId),
            MinPrice: minPrice,
            MaxPrice: maxPrice,
            SortType: sortType // "price_asc", "name_asc", etc.
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
                const products = result.data ? result.data : result;
                renderProducts(products);
            } else {
                console.error("Ошибка сервера: " + response.status);
            }
        } catch (error) {
            console.error("Ошибка сети:", error);
        } finally {
            productsContainer.style.opacity = '1';
        }
    }

    function renderProducts(products) {
        productsContainer.innerHTML = '';

        if (!products || products.length === 0) {
            productsContainer.innerHTML = '<div class="col-12"><div class="alert alert-dark text-center">Товары не найдены</div></div>';
            return;
        }

        products.forEach(product => {
            const imagePath = product.imagePath ? product.imagePath : "https://dummyimage.com/300x300/dee2e6/6c757d.jpg&text=No+Image";
            const formattedPrice = new Intl.NumberFormat('ru-RU').format(product.price);

            const html = `
                <div class="col product-card">
                    <div class="card h-100 shadow border-0 bg-dark overflow-hidden">
                        <div class="d-flex align-items-center justify-content-center bg-white p-2" style="height: 160px;">
                            <img src="${imagePath}" class="img-fluid" alt="${product.name}" style="max-height: 100%; max-width: 100%; object-fit: contain;">
                        </div>

                        <div class="card-body d-flex flex-column p-3">
                            <h5 class="card-title font-weight-bold mb-1" style="color: #ff9900; font-size: 1.1rem;">${product.name}</h5>
                            <p class="card-text text-white small mb-3" style="opacity: 0.8;">${product.description}</p>
                            
                            <div class="mt-auto">
                                <div class="d-flex justify-content-between align-items-center">
                                    <span class="font-weight-bold text-white fs-5">${formattedPrice} ₽</span>
                                    <button class="btn btn-sm btn-warning fw-bold">В корзину</button>
                                </div>
                            </div>
                        </div>
                        <div class="card-footer bg-transparent border-top-0 p-2">
                            <a href="/Product/GetProduct?id=${product.id}" class="btn btn-outline-light w-100 btn-sm">Подробнее</a>
                        </div>
                    </div>
                </div>
            `;
            productsContainer.insertAdjacentHTML('beforeend', html);
        });
    }

    // --- ПРИВЯЗКА СОБЫТИЙ ---

    // 1. Кнопка "Применить" (фильтр цены)
    const applyBtn = document.getElementById('applyFiltersBtn');
    if (applyBtn) {
        applyBtn.addEventListener('click', fetchProducts);
    }

    // 2. Выпадающий список сортировки (СРАЗУ ПРИ ИЗМЕНЕНИИ)
    const sortSelect = document.getElementById('sortSelect');
    if (sortSelect) {
        sortSelect.addEventListener('change', fetchProducts);
    }
});
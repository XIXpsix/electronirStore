document.addEventListener("DOMContentLoaded", function () {
    // Находим элементы фильтров
    const nameSearch = document.getElementById("nameSearch");
    const categoryId = document.getElementById("categoryId");
    const minPrice = document.getElementById("minPrice");
    const maxPrice = document.getElementById("maxPrice");
    const sortType = document.getElementById("sortType");

    // Функция "debounce" (задержка), чтобы не спамить запросами при каждом нажатии клавиши
    // Поиск сработает через 500мс после того, как вы перестали печатать
    function debounce(func, timeout = 500) {
        let timer;
        return (...args) => {
            clearTimeout(timer);
            timer = setTimeout(() => { func.apply(this, args); }, timeout);
        };
    }

    // Обернутая функция фильтрации с задержкой
    const debouncedFilter = debounce(() => applyFilters());

    // --- ПРИВЯЗКА СОБЫТИЙ (Живой поиск) ---

    // Для текстового поля и цен реагируем на ввод (input)
    if (nameSearch) nameSearch.addEventListener("input", debouncedFilter);
    if (minPrice) minPrice.addEventListener("input", debouncedFilter);
    if (maxPrice) maxPrice.addEventListener("input", debouncedFilter);

    // Для выпадающих списков реагируем на изменение (change) сразу
    if (categoryId) categoryId.addEventListener("change", applyFilters);
    if (sortType) sortType.addEventListener("change", applyFilters);
});

// Основная функция фильтрации
async function applyFilters() {
    const categoryIdEl = document.getElementById("categoryId");
    const minPriceEl = document.getElementById("minPrice");
    const maxPriceEl = document.getElementById("maxPrice");
    const nameSearchEl = document.getElementById("nameSearch");
    const sortTypeEl = document.getElementById("sortType");

    const data = {
        categoryId: parseInt(categoryIdEl?.value) || 0,
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

        // --- ЛОГИКА ОТОБРАЖЕНИЯ ---
        if (result.data && result.data.length > 0) {
            // Если товары ЕСТЬ, рисуем их
            result.data.forEach(product => {
                const productHtml = `
                    <div class="col-12 col-sm-6 col-md-4 col-lg-3 mb-4 fade-in">
                        <div class="card h-100 border-secondary" style="background-color: #1e1e1e;">
                            <div class="position-relative bg-white overflow-hidden rounded-top" style="height: 200px; display: flex; align-items: center; justify-content: center;">
                                <img src="${product.imagePath || '/img/placeholder.jpg'}" alt="${product.name}" style="max-height: 90%; max-width: 90%; object-fit: contain;">
                            </div>
                            <div class="card-body d-flex flex-column p-3">
                                <h6 class="card-title text-warning text-truncate mb-1" title="${product.name}">
                                    ${product.name}
                                </h6>
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
            // Если товаров НЕТ (ваш текст)
            container.innerHTML = `
                <div class="col-12 text-center py-5">
                    <h3 class="text-muted fw-bold">Товаров нет</h3>
                    <p class="text-secondary">По вашему запросу ничего не найдено.</p>
                </div>
            `;
        }

    } catch (error) {
        console.error('Ошибка:', error);
    }
}
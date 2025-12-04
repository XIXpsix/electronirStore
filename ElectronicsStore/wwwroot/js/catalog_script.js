document.addEventListener("DOMContentLoaded", function () {

    // --- ЛОГИКА СЛАЙДЕРА ЦЕНЫ ---

    const rangeInput = document.querySelectorAll(".range-input input"),
        priceInput = document.querySelectorAll(".price-input input"),
        range = document.querySelector(".slider .progress");

    let priceGap = 1000; // Минимальная разница между ползунками

    // Функция обновления полосы прогресса (синей линии между ползунками)
    function updateProgress() {
        let minVal = parseInt(rangeInput[0].value),
            maxVal = parseInt(rangeInput[1].value);

        // Расчет процентов для позиционирования
        let maxRange = parseInt(rangeInput[0].max); // Берем максимум из атрибута max

        range.style.left = ((minVal / maxRange) * 100) + "%";
        range.style.right = (100 - (maxVal / maxRange) * 100) + "%";
    }

    // Обработка ввода цифр в инпуты (От/До)
    priceInput.forEach(input => {
        input.addEventListener("input", e => {
            let minPrice = parseInt(priceInput[0].value),
                maxPrice = parseInt(priceInput[1].value);

            // Валидация: макс. цена не меньше мин., и в пределах допустимого
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

    // Обработка движения ползунков
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

    // Инициализация прогресс-бара при загрузке
    updateProgress();


    // --- ЛОГИКА СОРТИРОВКИ ---

    const sortSelect = document.getElementById('sortSelect');
    const productsContainer = document.getElementById('productsContainer');

    if (sortSelect && productsContainer) {
        sortSelect.addEventListener('change', function () {
            const sortValue = this.value;
            const products = Array.from(productsContainer.getElementsByClassName('product-card'));

            products.sort((a, b) => {
                const priceA = parseFloat(a.dataset.price);
                const priceB = parseFloat(b.dataset.price);
                const nameA = a.dataset.name.toLowerCase();
                const nameB = b.dataset.name.toLowerCase();

                if (sortValue === 'price_asc') {
                    return priceA - priceB;
                } else if (sortValue === 'price_desc') {
                    return priceB - priceA;
                } else if (sortValue === 'name_asc') {
                    if (nameA < nameB) return -1;
                    if (nameA > nameB) return 1;
                    return 0;
                } else {
                    // default - можно вернуть исходный порядок, если бы мы хранили индексы, 
                    // но пока оставим как есть или добавим data-id для сортировки по id
                    return 0;
                }
            });

            // Очищаем контейнер и добавляем отсортированные элементы
            productsContainer.innerHTML = '';
            products.forEach(product => productsContainer.appendChild(product));
        });
    }


    // --- ЛОГИКА ФИЛЬТРАЦИИ (Кнопка "Применить") ---

    const applyBtn = document.getElementById('applyFiltersBtn');

    if (applyBtn) {
        applyBtn.addEventListener('click', function () {
            let minPrice = parseInt(priceInput[0].value);
            let maxPrice = parseInt(priceInput[1].value);

            const products = productsContainer.getElementsByClassName('product-card');

            Array.from(products).forEach(product => {
                const productPrice = parseFloat(product.dataset.price);

                if (productPrice >= minPrice && productPrice <= maxPrice) {
                    product.style.display = 'block'; // Показываем
                } else {
                    product.style.display = 'none'; // Скрываем
                }
            });
        });
    }
});
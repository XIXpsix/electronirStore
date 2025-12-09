async function applyFilters() {
    const data = {
        categoryId: parseInt(document.getElementById("categoryId").value) || 0,
        minPrice: parseFloat(document.getElementById("minPrice").value) || 0,
        maxPrice: parseFloat(document.getElementById("maxPrice").value) || 0,
        name: document.getElementById("nameSearch").value,
        sortType: document.getElementById("sortType").value
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
        const countLabel = document.getElementById("productsCount");

        container.innerHTML = "";

        if (result.data && result.data.length > 0) {
            countLabel.textContent = `${result.data.length} шт.`;

            result.data.forEach(product => {
                // ВАЖНО: Этот HTML полностью совпадает с новым дизайном в List.cshtml
                const productHtml = `
                    <div class="col-12 mb-3">
                        <div class="card h-100 overflow-hidden">
                            <div class="row g-0">
                                <div class="col-md-3 bg-white d-flex align-items-center justify-content-center p-3">
                                    <img src="${product.imagePath}" class="img-fluid rounded" alt="${product.name}" 
                                         style="max-height: 160px; width: auto; object-fit: contain;">
                                </div>
                                <div class="col-md-9">
                                    <div class="card-body d-flex flex-column h-100">
                                        <div class="d-flex justify-content-between align-items-start">
                                            <h4 class="card-title mb-1 text-white">${product.name}</h4>
                                            <h3 class="text-warning mb-0 fw-bold">${product.price.toLocaleString()} ₽</h3>
                                        </div>
                                        <p class="card-text text-muted mt-2 mb-3 small flex-grow-1">
                                            ${product.description}
                                        </p>
                                        <div class="mt-auto text-end">
                                            <a href="/Product/GetProduct/${product.id}" class="btn btn-primary stretched-link">
                                                Смотреть товар
                                            </a>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                `;
                container.insertAdjacentHTML('beforeend', productHtml);
            });
        } else {
            countLabel.textContent = "0 шт.";
            container.innerHTML = `
                <div class="col-12 text-center py-5">
                    <h4 class="text-muted">Ничего не найдено :(</h4>
                    <p class="text-muted">Попробуйте изменить запрос</p>
                </div>
            `;
        }

    } catch (error) {
        console.error('Ошибка:', error);
    }
}

// Поиск по Enter
document.getElementById("nameSearch").addEventListener("keypress", function (event) {
    if (event.key === "Enter") {
        event.preventDefault();
        applyFilters();
    }
});
document.addEventListener('DOMContentLoaded', function () {

    // Ждем, пока весь HTML будет загружен
    window.addEventListener('scroll', function () {

        var header = document.getElementById('header-top');
        if (!header) return; // Выходим, если элемента нет

        var scrollTop = window.scrollY;
        var maxScroll = 150; // Дистанция, на которой эффект полностью применится

        // Расчет прозрачности на основе прокрутки
        // Math.min(..., 1) гарантирует, что прозрачность не будет > 1
        var opacity = Math.min(scrollTop / maxScroll, 0.8); // 0.8 = 80% прозрачности

        // Применяем фон. `rgba(255, 255, 255, 0.8)`
        // Это белый фон (255, 255, 255) с 80% прозрачности
        header.style.backgroundColor = `rgba(255, 255, 255, ${0.8 + opacity * 0.2})`; // от 0.8 до 1.0

        // Добавляем/убираем тень
        if (scrollTop > 10) {
            header.style.boxShadow = '0 4px 6px -1px rgba(0, 0, 0, 0.1)';
        } else {
            header.style.boxShadow = 'none';
        }
    });
});
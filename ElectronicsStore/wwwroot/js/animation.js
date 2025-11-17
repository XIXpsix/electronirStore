// Ждем, пока загрузится вся страница
document.addEventListener("DOMContentLoaded", () => {

    // Создаем "наблюдателя"
    // Он будет следить за элементами, которые мы ему укажем
    const observer = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
            // Если элемент (entry) появился в зоне видимости (isIntersecting)
            if (entry.isIntersecting) {
                // Добавляем ему класс .visible, который запустит CSS-анимацию
                entry.target.classList.add("visible");
                // Перестаем за ним следить, т.к. он уже показан
                observer.unobserve(entry.target);
            }
        });
    }, {
        // Анимация сработает, когда 10% элемента будет видно
        threshold: 0.1
    });

    // Находим ВСЕ элементы с классом .hidden-on-scroll
    const hiddenElements = document.querySelectorAll(".hidden-on-scroll");

    // "Приказываем" наблюдателю следить за каждым из этих элементов
    hiddenElements.forEach((el) => observer.observe(el));
});
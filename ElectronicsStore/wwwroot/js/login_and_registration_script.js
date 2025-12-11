/**
 * Глобальная функция для переключения между формами входа и регистрации
 * внутри модального окна (#authModal).
 * * В HTML: <button onclick="switchMode('register')">
 */
window.switchMode = function (mode) {
    const slider = document.getElementById('authSlider');

    if (!slider) {
        console.error("Auth modal slider element (id='authSlider') not found.");
        return;
    }

    // В модальном окне два блока 'auth-view' (login и register).
    // Они расположены рядом и управляются контейнером 'authSlider'.
    // Для переключения мы используем CSS transform.

    if (mode === 'register') {
        // Передвигаем слайдер, чтобы показать форму регистрации.
        // Используется 50% для центрирования, если в модальном окне 2/3 колонки
        // (как предполагает ваш HTML-шаблон).
        slider.style.transform = 'translateX(-50%)';
    } else if (mode === 'login') {
        // Сбрасываем позицию, чтобы показать форму входа.
        slider.style.transform = 'translateX(0)';
    }

    // Очищаем ошибки валидации при переключении
    const forms = document.querySelectorAll('#loginView form, #registerView form');
    forms.forEach(form => {
        // Сбрасываем все сообщения об ошибках
        const validationSpans = form.querySelectorAll('.text-danger');
        validationSpans.forEach(span => span.textContent = '');

        // Сброс валидации (если используется jQuery Validation)
        if (typeof $ !== 'undefined' && $.fn.validate) {
            $(form).data('validator').resetForm();
        }
    });
};

// Ваш существующий код для открытия/закрытия модального окна (если он нужен 
// для элементов, не управляемых Bootstrap'ом)
document.addEventListener('DOMContentLoaded', function () {
    // ВАШ КОД ДЛЯ ОТПРАВКИ/ВАЛИДАЦИИ ФОРМЫ ВХОДА И РЕГИСТРАЦИИ ТУТ...
    // Старая логика hiddenOpen_Closeclick и .signin-btn/ .signup-btn удалена, 
    // так как она несовместима с новой структурой модального окна.
});
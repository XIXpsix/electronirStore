/**
 * Глобальная функция для переключения между формами входа и регистрации
 * внутри модального окна (#authModal).
 */
window.switchMode = function (mode) {
    const slider = document.getElementById('authSlider');

    if (!slider) {
        console.error("Auth modal slider element (id='authSlider') not found.");
        return;
    }

    if (mode === 'register') {
        // Передвигаем слайдер, чтобы показать форму регистрации.
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

        // Удаление ошибок, добавленных AJAX-обработчиком
        $(form).find('.ajax-error-message').remove();

        // ✅ Исправление предыдущей ошибки "Cannot read properties of undefined"
        if (typeof $ !== 'undefined' && $.fn.validate) {
            $(form).data('validator')?.resetForm();
        }
    });
};

// =======================================================
// АЯКС ОБРАБОТЧИК ДЛЯ ОТПРАВКИ ФОРМ ВХОДА/РЕГИСТРАЦИИ
// =======================================================
$(document).ready(function () {

    // Функция для обработки AJAX отправки формы
    function handleAjaxFormSubmission(e) {
        e.preventDefault(); // 🚩 ГЛАВНАЯ СТРОКА: ОСТАНОВИТЬ стандартную отправку формы

        var form = $(this);
        var url = form.attr('action');
        var method = form.attr('method') || 'POST';
        var data = form.serialize();

        // Поиск элементов для отображения ошибок
        var validationSummary = form.find('[data-valmsg-summary="ModelOnly"]');
        var isModalForm = form.closest('#authModal').length > 0;

        // 1. Очистка предыдущих ошибок
        form.find('.ajax-error-message').remove();
        if (validationSummary.length > 0) {
            validationSummary.empty();
        }

        // Выполняем AJAX-запрос
        $.ajax({
            url: url,
            type: method,
            data: data,
            dataType: 'json',
            success: function (response) {
                if (response.isValid) {
                    // УСПЕХ: Перенаправление, используя URL из ответа C#
                    window.location.href = response.redirectUrl;
                } else {
                    // НЕУДАЧА: Отобразить ошибку
                    var errorMessage = response.description || 'Неизвестная ошибка сервера.';

                    if (isModalForm) {
                        // Для модального окна
                        var errorDiv = $('<div class="text-danger mb-3 ajax-error-message"></div>');
                        errorDiv.text(errorMessage);
                        form.prepend(errorDiv);
                    } else if (validationSummary.length > 0) {
                        // Для автономных страниц
                        var errorList = $('<ul>').append($('<li>').text(errorMessage));
                        validationSummary.html(errorList);
                    } else {
                        alert(errorMessage);
                    }
                }
            },
            error: function (xhr, status, error) {
                // Обработка общей AJAX-ошибки
                var errorMsg = 'Произошла критическая ошибка при обращении к серверу.';

                var errorDiv = $('<div class="text-danger mb-3 ajax-error-message"></div>');
                errorDiv.text(errorMsg);
                form.prepend(errorDiv);
            }
        });
    }

    // Привязываем функцию к отправке всех форм
    $('form[asp-action="Login"], form[asp-action="Register"], #authModal form').on('submit', handleAjaxFormSubmission);
});
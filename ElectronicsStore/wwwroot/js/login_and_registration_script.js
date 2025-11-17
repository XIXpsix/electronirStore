// Ждем, пока вся HTML-страница (DOM) загрузится
document.addEventListener("DOMContentLoaded", function () {

    // --- Находим все нужные элементы на странице ---

    // 1. Кнопки в шапке (header)
    const loginButton = document.getElementById("click-to-hide-login");
    const registerButton = document.getElementById("click-to-hide-register");

    // 2. Элементы модального окна
    const modalOverlay = document.getElementById("modal-overlay");
    const modalCloseButton = document.getElementById("modal-close");

    // Находим ГЛАВНОЕ ОКНО (белый блок)
    const modalContentBox = document.getElementById("modal-content-box");

    // 3. Контейнеры форм (внутри модального окна)
    const loginFormContainer = document.getElementById("login-form-container");
    const registerFormContainer = document.getElementById("register-form-container");

    // 4. Ссылки для переключения (внутри модального окна)

    // ✅ ✅ ✅ ВОТ ПРАВИЛЬНЫЙ КОД ✅ ✅ ✅
    const showRegisterLink = document.getElementById("show-register-link");
    const showLoginLink = document.getElementById("show-login-link");
    // ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅


    // --- Функции для управления окном ---

    // Функция: Показать окно
    function showModal() {
        if (modalOverlay) {
            modalOverlay.classList.add("active");
        }
    }

    // Функция: Скрыть окно
    function hideModal() {
        if (modalOverlay) {
            modalOverlay.classList.remove("active");
        }
    }

    // Функция: Показать форму Входа
    function showLoginForm() {
        if (modalContentBox) {
            // Убираем класс, CSS вернет слайдер в исходное положение
            modalContentBox.classList.remove("show-register");
        }
        showModal(); // Показываем окно, если оно было скрыто
    }

    // Функция: Показать форму Регистрации
    function showRegisterForm() {
        if (modalContentBox) {
            // Добавляем класс, CSS сдвинет слайдер
            modalContentBox.classList.add("show-register");
        }
        showModal(); // Показываем окно, если оно было скрыто
    }


    // --- Назначаем "слушателей" событий ---

    // 1. Клик на кнопку "Войти" в шапке
    if (loginButton) {
        loginButton.addEventListener("click", function (e) {
            e.preventDefault();
            showLoginForm();
        });
    }

    // 2. Клик на кнопку "Регистрация" в шапке
    if (registerButton) {
        registerButton.addEventListener("click", function (e) {
            e.preventDefault();
            showRegisterForm();
        });
    }

    // 3. Клик на "крестик" (закрыть)
    if (modalCloseButton) {
        modalCloseButton.addEventListener("click", function () {
            hideModal();
        });
    }

    // 4. Клик на ссылку "Регистрация" (внутри окна)
    if (showRegisterLink) {
        showRegisterLink.addEventListener("click", function (e) {
            e.preventDefault();
            showRegisterForm();
        });
    }

    // 5. Клик на ссылку "Войти" (внутри окна)
    if (showLoginLink) {
        showLoginLink.addEventListener("click", function (e) {
            e.preventDefault();
            showLoginForm();
        });
    }

    // 6. Клик на фон (оверлей) - чтобы закрыть окно
    if (modalOverlay) {
        modalOverlay.addEventListener("click", function (e) {
            if (e.target === modalOverlay) {
                hideModal();
            }
        });
    }

    // 7. Закрытие окна по нажатию Esc
    document.addEventListener("keydown", function (e) {
        if (e.key === "Escape" && modalOverlay.classList.contains("active")) {
            hideModal();
        }
    });

});
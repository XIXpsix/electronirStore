/* --- 1. УПРАВЛЕНИЕ МОДАЛЬНЫМ ОКНОМ И АНИМАЦИЕЙ --- */

// Открытие модального окна
function openAuthModal(mode) {
    var authModalEl = document.getElementById('authModal');
    var authModal = new bootstrap.Modal(authModalEl);

    // Сначала переключаем на нужный вид без анимации
    switchMode(mode, false);

    // Показываем окно
    authModal.show();
}

// Переключение между Входом и Регистрацией
function switchMode(mode, animate = true) {
    var slider = document.getElementById('authSlider');
    var loginView = document.getElementById('loginView');
    var registerView = document.getElementById('registerView');

    if (!slider || !loginView || !registerView) return;

    // Убираем активные классы прозрачности
    loginView.classList.remove('active');
    registerView.classList.remove('active');

    if (mode === 'register') {
        // Сдвигаем влево (показываем регистрацию)
        slider.classList.remove('mode-login');
        slider.classList.add('mode-register');

        // Плавно показываем контент регистрации
        if (animate) {
            setTimeout(() => registerView.classList.add('active'), 200);
        } else {
            registerView.classList.add('active');
        }

    } else {
        // Сдвигаем вправо (показываем вход)
        slider.classList.remove('mode-register');
        slider.classList.add('mode-login');

        // Плавно показываем контент входа
        if (animate) {
            setTimeout(() => loginView.classList.add('active'), 200);
        } else {
            loginView.classList.add('active');
        }
    }
}

// Инициализация при загрузке страницы
document.addEventListener("DOMContentLoaded", function () {
    var loginView = document.getElementById('loginView');
    if (loginView) loginView.classList.add('active');

    // Навешиваем обработчики на формы
    attachFormHandlers();
});


/* --- 2. ЛОГИКА ОТПРАВКИ ФОРМ (AJAX FETCH) --- */

function attachFormHandlers() {
    // Находим формы внутри модального окна
    const loginForm = document.querySelector('#loginView form');
    const registerForm = document.querySelector('#registerView form');

    if (loginForm) {
        loginForm.addEventListener('submit', function (e) {
            e.preventDefault(); // Останавливаем обычную отправку (чтобы не было белого экрана)
            submitForm(this, '/Account/Login');
        });
    }

    if (registerForm) {
        registerForm.addEventListener('submit', function (e) {
            e.preventDefault();
            submitForm(this, '/Account/Register');
        });
    }
}

// Универсальная функция отправки
async function submitForm(form, url) {
    const formData = new FormData(form);
    const errorContainer = form.querySelector('.error-message') || createErrorContainer(form);

    // Очищаем старые ошибки
    errorContainer.textContent = '';
    errorContainer.style.display = 'none';

    // Превращаем FormData в объект для отправки (если контроллер ждет JSON) или отправляем как есть
    // В вашем случае контроллер ждет Form/ViewModel, так что FormData подойдет, 
    // но лучше отправить как x-www-form-urlencoded или JSON, если контроллер ждет [FromBody].
    // Однако стандартный контроллер MVC ждет form-data.

    try {
        const response = await fetch(url, {
            method: 'POST',
            body: formData
        });

        // Если сервер вернул JSON (например, ошибку или успех)
        if (response.headers.get("content-type")?.includes("application/json")) {
            const data = await response.json();

            if (response.ok) {
                // УСПЕХ: Перезагружаем страницу
                window.location.reload();
            } else {
                // ОШИБКА СЕРВЕРА (например "Неверный пароль")
                showError(errorContainer, data.error || data.description || "Произошла ошибка");
            }
        } else {
            // Если сервер вернул HTML (например, редирект), просто перезагружаем
            if (response.ok) window.location.reload();
        }
    } catch (error) {
        console.error('Ошибка:', error);
        showError(errorContainer, "Ошибка соединения с сервером");
    }
}

// Вспомогательная функция для создания блока ошибок, если его нет
function createErrorContainer(form) {
    let div = document.createElement('div');
    div.className = 'error-message text-danger text-center mt-3 fw-bold';
    form.appendChild(div);
    return div;
}

function showError(container, message) {
    container.textContent = message;
    container.style.display = 'block';

    // Эффект тряски для формы (визуализация ошибки)
    const form = container.closest('form');
    if (form) {
        form.classList.add('shake-animation');
        setTimeout(() => form.classList.remove('shake-animation'), 500);
    }
}
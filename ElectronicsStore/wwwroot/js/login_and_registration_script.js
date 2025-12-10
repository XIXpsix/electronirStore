/* --- 1. УПРАВЛЕНИЕ МОДАЛЬНЫМ ОКНОМ --- */

function openAuthModal(mode) {
    var authModalEl = document.getElementById('authModal');
    if (!authModalEl) return;

    var authModal = new bootstrap.Modal(authModalEl);

    // Сначала переключаем режим
    switchMode(mode, false);

    authModal.show();
}

/* --- 2. ПЕРЕКЛЮЧЕНИЕ РЕЖИМОВ (СЛАЙДЕР) --- */

function switchMode(mode, animate = true) {
    var slider = document.getElementById('authSlider');
    var loginView = document.getElementById('loginView');
    var registerView = document.getElementById('registerView');

    if (!slider || !loginView || !registerView) return;

    // Сбрасываем активные классы (прозрачность)
    loginView.classList.remove('active');
    registerView.classList.remove('active');

    if (mode === 'register') {
        // Сдвигаем влево
        slider.classList.remove('mode-login');
        slider.classList.add('mode-register');

        // Показываем контент регистрации
        if (animate) {
            setTimeout(() => registerView.classList.add('active'), 200);
        } else {
            registerView.classList.add('active');
        }
    } else {
        // Сдвигаем вправо
        slider.classList.remove('mode-register');
        slider.classList.add('mode-login');

        // Показываем контент входа
        if (animate) {
            setTimeout(() => loginView.classList.add('active'), 200);
        } else {
            loginView.classList.add('active');
        }
    }
}

/* --- 3. ЛОГИКА ОТПРАВКИ ФОРМ (AJAX) --- */

document.addEventListener("DOMContentLoaded", function () {
    // Устанавливаем начальное состояние
    var loginView = document.getElementById('loginView');
    if (loginView) loginView.classList.add('active');

    // Подключаем обработчики форм
    const loginForm = document.querySelector('#loginView form');
    const registerForm = document.querySelector('#registerView form');

    if (loginForm) {
        loginForm.addEventListener('submit', function (e) {
            e.preventDefault(); // ОТМЕНЯЕМ стандартную отправку
            submitForm(this, '/Account/Login');
        });
    }

    if (registerForm) {
        registerForm.addEventListener('submit', function (e) {
            e.preventDefault(); // ОТМЕНЯЕМ стандартную отправку
            submitForm(this, '/Account/Register');
        });
    }
});

async function submitForm(form, url) {
    const formData = new FormData(form);

    // Ищем или создаем контейнер для ошибок внутри формы
    let errorContainer = form.querySelector('.error-message');
    if (!errorContainer) {
        errorContainer = document.createElement('div');
        errorContainer.className = 'error-message text-danger text-center mt-3 fw-bold';
        // Вставляем ошибку перед кнопкой отправки
        const btn = form.querySelector('button[type="submit"]');
        if (btn) btn.before(errorContainer);
        else form.appendChild(errorContainer);
    }

    errorContainer.textContent = '';
    errorContainer.style.display = 'none';

    try {
        const response = await fetch(url, {
            method: 'POST',
            body: formData
        });

        // Проверяем, вернул ли сервер JSON
        const contentType = response.headers.get("content-type");
        if (contentType && contentType.indexOf("application/json") !== -1) {
            const data = await response.json();

            if (response.ok) {
                // УСПЕХ: Перезагружаем страницу
                window.location.reload();
            } else {
                // ОШИБКА: Показываем её в форме
                errorContainer.textContent = data.error || data.description || "Ошибка сервера";
                errorContainer.style.display = 'block';

                // Эффект тряски
                form.classList.add('shake');
                setTimeout(() => form.classList.remove('shake'), 500);
            }
        } else {
            // Если сервер вернул HTML (например, редирект), просто перезагружаем
            if (response.ok) window.location.reload();
        }
    } catch (error) {
        console.error('Ошибка:', error);
        errorContainer.textContent = "Ошибка соединения";
        errorContainer.style.display = 'block';
    }
}
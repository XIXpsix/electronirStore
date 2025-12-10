/* --- 1. УПРАВЛЕНИЕ МОДАЛЬНЫМ ОКНОМ --- */
function openAuthModal(mode) {
    var authModalEl = document.getElementById('authModal');
    if (!authModalEl) return;
    var authModal = new bootstrap.Modal(authModalEl);
    switchMode(mode, false);
    authModal.show();
}

/* --- 2. ПЕРЕКЛЮЧЕНИЕ РЕЖИМОВ (СЛАЙДЕР) --- */
function switchMode(mode, animate = true) {
    var slider = document.getElementById('authSlider');
    var loginView = document.getElementById('loginView');
    var registerView = document.getElementById('registerView');

    if (!slider || !loginView || !registerView) return;

    loginView.classList.remove('active');
    registerView.classList.remove('active');

    if (mode === 'register') {
        slider.classList.remove('mode-login');
        slider.classList.add('mode-register');
        if (animate) setTimeout(() => registerView.classList.add('active'), 200);
        else registerView.classList.add('active');
    } else {
        slider.classList.remove('mode-register');
        slider.classList.add('mode-login');
        if (animate) setTimeout(() => loginView.classList.add('active'), 200);
        else loginView.classList.add('active');
    }
}

/* --- 3. ЛОГИКА ОТПРАВКИ ФОРМ (AJAX) --- */
document.addEventListener("DOMContentLoaded", function () {
    var loginView = document.getElementById('loginView');
    if (loginView) loginView.classList.add('active');

    const loginForm = document.querySelector('#loginView form');
    const registerForm = document.querySelector('#registerView form');

    if (loginForm) {
        loginForm.addEventListener('submit', function (e) {
            e.preventDefault();
            submitForm(this, '/Account/Login');
        });
    }

    if (registerForm) {
        registerForm.addEventListener('submit', function (e) {
            e.preventDefault();
            submitForm(this, '/Account/Register');
        });
    }
});

async function submitForm(form, url) {
    const formData = new FormData(form);

    // Создаем контейнер для ошибок, если его нет
    let errorContainer = form.querySelector('.error-message');
    if (!errorContainer) {
        errorContainer = document.createElement('div');
        errorContainer.className = 'error-message'; // Класс берется из CSS
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

        const contentType = response.headers.get("content-type");

        if (contentType && contentType.indexOf("application/json") !== -1) {
            // Если сервер вернул JSON (наш новый формат)
            const data = await response.json();

            if (data.isValid) {
                // УСПЕХ: Переходим по ссылке (на ConfirmEmail или Home)
                window.location.href = data.redirectUrl;
            } else {
                // ОШИБКА: Показываем сообщение и трясем форму
                errorContainer.textContent = data.description || "Ошибка сервера";
                errorContainer.style.display = 'block';

                form.classList.add('shake');
                setTimeout(() => form.classList.remove('shake'), 500);
            }
        } else {
            // Если сервер вернул HTML (на всякий случай)
            if (response.ok) window.location.reload();
        }
    } catch (error) {
        console.error('Ошибка:', error);
        errorContainer.textContent = "Ошибка соединения";
        errorContainer.style.display = 'block';
    }
}
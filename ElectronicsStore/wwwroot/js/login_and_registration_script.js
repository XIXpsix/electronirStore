// Открытие модального окна
function openAuthModal(mode) {
    var authModal = new bootstrap.Modal(document.getElementById('authModal'));

    // Сначала переключаем на нужный вид
    switchMode(mode, false); // false = без задержки анимации при открытии

    // Показываем
    authModal.show();
}

// Переключение режимов (Login <-> Register)
function switchMode(mode, animate = true) {
    var slider = document.getElementById('authSlider');
    var loginView = document.getElementById('loginView');
    var registerView = document.getElementById('registerView');

    if (mode === 'register') {
        // Сдвигаем влево
        slider.classList.remove('mode-login');
        slider.classList.add('mode-register');

        // Меняем прозрачность
        loginView.classList.remove('active');

        // Небольшая задержка, чтобы контент появился плавно во время езды
        if (animate) {
            setTimeout(() => registerView.classList.add('active'), 200);
        } else {
            registerView.classList.add('active');
        }

    } else {
        // Сдвигаем вправо (обратно)
        slider.classList.remove('mode-register');
        slider.classList.add('mode-login');

        registerView.classList.remove('active');

        if (animate) {
            setTimeout(() => loginView.classList.add('active'), 200);
        } else {
            loginView.classList.add('active');
        }
    }
}

// Инициализация (ставим вход по дефолту)
document.addEventListener("DOMContentLoaded", function () {
    var loginView = document.getElementById('loginView');
    if (loginView) loginView.classList.add('active');
});
document.addEventListener('DOMContentLoaded', function () {

    // =========================================================================
    // 1. ЛОГИКА ПРОКРУТКИ ШАПКИ (Header)
    // =========================================================================
    window.addEventListener('scroll', function () {
        var header = document.getElementById('header-top');
        if (!header) return;

        var scrollTop = window.scrollY;
        var maxScroll = 150; // Дистанция скролла для полного эффекта

        // Рассчитываем прозрачность
        // Начинаем с 0.9 (как в CSS) и идем к 1 (полностью непрозрачный)
        var opacity = 0.9 + Math.min(scrollTop / maxScroll, 0.1);

        // Применяем ТЕМНЫЙ фон (36, 36, 36), чтобы соответствовать дизайну
        // (В старой версии тут был белый цвет 255,255,255, который выбивался из стиля)
        header.style.backgroundColor = `rgba(36, 36, 36, ${opacity})`;

        // Добавляем тень при прокрутке
        if (scrollTop > 10) {
            header.style.boxShadow = '0 4px 15px rgba(0, 0, 0, 0.5)';
            header.style.borderBottom = '1px solid #ff6600'; // Добавляем оранжевую линию для красоты
        } else {
            header.style.boxShadow = '0 2px 10px rgba(0,0,0,0.3)';
            header.style.borderBottom = '1px solid #444'; // Возвращаем серую линию
        }
    });

    // =========================================================================
    // 2. УПРАВЛЕНИЕ МОБИЛЬНЫМ МЕНЮ (3 полоски)
    // =========================================================================
    const menuBtn = document.getElementById('mobile-menu-btn');
    const menuOverlay = document.getElementById('mobile-menu-container');
    const body = document.body;

    if (menuBtn && menuOverlay) {
        menuBtn.addEventListener('click', function () {
            // Переключаем классы "open" для запуска CSS-анимаций
            menuBtn.classList.toggle('open');
            menuOverlay.classList.toggle('open');

            // Блокируем прокрутку основной страницы, пока открыто меню
            if (menuOverlay.classList.contains('open')) {
                body.style.overflow = 'hidden';
            } else {
                body.style.overflow = '';
            }
        });
    }

    // =========================================================================
    // 3. СВЯЗЬ МОБИЛЬНЫХ КНОПОК С МОДАЛЬНЫМИ ОКНАМИ
    // =========================================================================
    // Мы перенаправляем клики с мобильных кнопок на скрытые кнопки ПК-версии,
    // чтобы сработала логика открытия окон (которая находится в login_and_registration_script.js)

    const mobLogin = document.getElementById('mobile-login-trigger');
    const mobRegister = document.getElementById('mobile-register-trigger');

    // Кнопки, которые реально открывают модалки (спрятаны в шапке на мобильном)
    const pcLogin = document.getElementById('click-to-hide-login');
    const pcRegister = document.getElementById('click-to-hide-register');

    // Функция для закрытия мобильного меню
    function closeMobileMenu() {
        if (menuBtn) menuBtn.classList.remove('open');
        if (menuOverlay) menuOverlay.classList.remove('open');
        body.style.overflow = '';
    }

    // Обработка клика "Войти" на телефоне
    if (mobLogin && pcLogin) {
        mobLogin.addEventListener('click', function (e) {
            e.preventDefault();
            closeMobileMenu(); // Закрываем шторку меню

            // Небольшая задержка (300мс), чтобы меню успело исчезнуть до появления окна
            setTimeout(() => {
                pcLogin.click();
            }, 300);
        });
    }

    // Обработка клика "Регистрация" на телефоне
    if (mobRegister && pcRegister) {
        mobRegister.addEventListener('click', function (e) {
            e.preventDefault();
            closeMobileMenu();

            setTimeout(() => {
                pcRegister.click();
            }, 300);
        });
    }
});
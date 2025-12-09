document.addEventListener('DOMContentLoaded', function () {

    // =========================================================================
    // 1. ЛОГИКА ПРОКРУТКИ ШАПКИ (Header)
    // =========================================================================
    window.addEventListener('scroll', function () {
        var header = document.getElementById('header-top');
        if (!header) return;

        // ✅ Используем window.scrollY (не pageYOffset)
        var scrollTop = window.scrollY ?? 0;
        var maxScroll = 150;

        var opacity = 0.9 + Math.min(scrollTop / maxScroll, 0.1);

        header.style.backgroundColor = `rgba(36, 36, 36, ${opacity})`;

        if (scrollTop > 10) {
            header.style.boxShadow = '0 4px 15px rgba(0, 0, 0, 0.5)';
            header.style.borderBottom = '1px solid #ff6600';
        } else {
            header.style.boxShadow = '0 2px 10px rgba(0,0,0,0.3)';
            header.style.borderBottom = '1px solid #444';
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
            menuBtn.classList.toggle('open');
            menuOverlay.classList.toggle('open');

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
    const mobLogin = document.getElementById('mobile-login-trigger');
    const mobRegister = document.getElementById('mobile-register-trigger');

    const pcLogin = document.getElementById('click-to-hide-login');
    const pcRegister = document.getElementById('click-to-hide-register');

    function closeMobileMenu() {
        if (menuBtn) menuBtn.classList.remove('open');
        if (menuOverlay) menuOverlay.classList.remove('open');
        body.style.overflow = '';
    }

    if (mobLogin && pcLogin) {
        mobLogin.addEventListener('click', function (e) {
            e.preventDefault();
            closeMobileMenu();

            setTimeout(() => {
                pcLogin?.click();
            }, 300);
        });
    }

    if (mobRegister && pcRegister) {
        mobRegister.addEventListener('click', function (e) {
            e.preventDefault();
            closeMobileMenu();

            setTimeout(() => {
                pcRegister?.click();
            }, 300);
        });
    }
});
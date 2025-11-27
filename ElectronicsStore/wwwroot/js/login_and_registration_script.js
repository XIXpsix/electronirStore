document.addEventListener("DOMContentLoaded", function () {

    // --- Находим ВСЕ элементы ---
    const loginButton = document.getElementById("click-to-hide-login");
    const registerButton = document.getElementById("click-to-hide-register");
    const modalOverlay = document.getElementById("modal-overlay");
    const modalCloseButton = document.getElementById("modal-close");
    const modalContentBox = document.getElementById("modal-content-box");
    const showRegisterLink = document.getElementById("show-register-link");
    const showLoginLink = document.getElementById("show-login-link");
    const loginForm = document.getElementById("login-form");
    const registerForm = document.getElementById("register-form");
    const loginErrorContainer = document.getElementById("login-error-container");
    const registerErrorContainer = document.getElementById("register-error-container");
    const regSuccessContainer = document.getElementById("registration-success-container");
    const loginSuccessContainer = document.getElementById("login-success-container");
    const sliderContainer = document.getElementById("modal-slider-container");

    // --- Функции для управления окном ---
    function showModal() { if (modalOverlay) modalOverlay.classList.add("active"); }

    function hideModal() {
        if (modalOverlay) modalOverlay.classList.remove("active");

        // ✅ ИСПРАВЛЕНИЕ 1: Ставим 'flex', чтобы формы были в ряд, а не друг под другом
        if (sliderContainer) sliderContainer.style.display = "flex";

        if (regSuccessContainer) regSuccessContainer.style.display = "none";
        if (loginSuccessContainer) loginSuccessContainer.style.display = "none";
    }

    function showLoginForm() {
        if (modalContentBox) modalContentBox.classList.remove("show-register");
        showModal();
        if (registerErrorContainer) registerErrorContainer.classList.remove("show");
    }

    function showRegisterForm() {
        if (modalContentBox) modalContentBox.classList.add("show-register");
        showModal();
        if (loginErrorContainer) loginErrorContainer.classList.remove("show");
    }


    // --- Функция для отправки форм ---
    async function handleFormSubmit(event, form) {
        event.preventDefault();
        const errorContainer = form.id === "login-form" ? loginErrorContainer : registerErrorContainer;

        errorContainer.classList.remove("show");
        errorContainer.textContent = "";

        const url = form.action;
        const formData = new FormData(form);
        const token = formData.get("__RequestVerificationToken");
        const data = Object.fromEntries(formData.entries());

        try {
            const response = await fetch(url, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "RequestVerificationToken": token
                },
                body: JSON.stringify(data)
            });

            if (response.ok) {
                const result = await response.json();
                console.log("Успех:", result);

                if (sliderContainer) sliderContainer.style.display = 'none';

                if (form.id === 'register-form') {
                    document.getElementById('success-reg-email').textContent = result.email;
                    document.getElementById('success-firstname').textContent = result.firstName;
                    document.getElementById('success-lastname').textContent = result.lastName;
                    document.getElementById('success-reg-password').textContent = result.password;
                    if (regSuccessContainer) regSuccessContainer.style.display = 'block';

                } else if (form.id === 'login-form') {
                    document.getElementById('success-login-email').textContent = result.email;
                    document.getElementById('success-login-password').textContent = result.password;
                    if (loginSuccessContainer) loginSuccessContainer.style.display = 'block';
                }

            } else {
                const errorResult = await response.json();
                errorContainer.textContent = errorResult.message;
                errorContainer.classList.add("show");
            }

        } catch (error) {
            console.error("Сетевая ошибка:", error);
            errorContainer.textContent = "Ошибка сети. Попробуйте снова.";
            errorContainer.classList.add("show");
        }
    }

    // --- Назначаем "слушателей" событий ---
    if (loginButton) loginButton.addEventListener("click", (e) => { e.preventDefault(); showLoginForm(); });
    if (registerButton) registerButton.addEventListener("click", (e) => { e.preventDefault(); showRegisterForm(); });
    if (modalCloseButton) modalCloseButton.addEventListener("click", hideModal);
    if (showRegisterLink) showRegisterLink.addEventListener("click", (e) => { e.preventDefault(); showRegisterForm(); });
    if (showLoginLink) showLoginLink.addEventListener("click", (e) => { e.preventDefault(); showLoginForm(); });

    // ✅ ИСПРАВЛЕНИЕ 2: Убрали закрытие по клику на фон (modalOverlay)

    document.addEventListener("keydown", (e) => { if (e.key === "Escape" && modalOverlay.classList.contains("active")) hideModal(); });

    // Перехватываем отправку форм
    if (loginForm) loginForm.addEventListener("submit", (e) => handleFormSubmit(e, loginForm));
    if (registerForm) registerForm.addEventListener("submit", (e) => handleFormSubmit(e, registerForm));

});
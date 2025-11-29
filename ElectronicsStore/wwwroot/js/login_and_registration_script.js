document.addEventListener("DOMContentLoaded", function () {

    const loginButton = document.getElementById("click-to-hide-login");
    const registerButton = document.getElementById("click-to-hide-register");
    const modalOverlay = document.getElementById("modal-overlay");
    const modalCloseButton = document.getElementById("modal-close");
    const modalContentBox = document.getElementById("modal-content-box");
    const showRegisterLink = document.getElementById("show-register-link");
    const showLoginLink = document.getElementById("show-login-link");

    // Новые кнопки
    const backToRegisterLink = document.getElementById("back-to-register-link");
    const resendCodeLink = document.getElementById("resend-code-link");
    const resendMessage = document.getElementById("resend-message");

    const loginForm = document.getElementById("login-form");
    const registerForm = document.getElementById("register-form");
    const confirmForm = document.getElementById("confirm-email-form");

    const loginErrorContainer = document.getElementById("login-error-container");
    const registerErrorContainer = document.getElementById("register-error-container");
    const confirmErrorContainer = document.getElementById("confirm-error-container");

    const regSuccessContainer = document.getElementById("registration-success-container");
    const loginSuccessContainer = document.getElementById("login-success-container");
    const sliderContainer = document.getElementById("modal-slider-container");
    const confirmContainer = document.getElementById("confirm-email-container");

    // Переменная для хранения данных регистрации (чтобы отправить код повторно)
    let lastRegisterData = null;

    function showModal() { if (modalOverlay) modalOverlay.classList.add("active"); }

    function hideModal() {
        if (modalOverlay) modalOverlay.classList.remove("active");
        // Сброс при закрытии
        if (sliderContainer) sliderContainer.style.display = "flex";
        if (regSuccessContainer) regSuccessContainer.style.display = "none";
        if (loginSuccessContainer) loginSuccessContainer.style.display = "none";
        if (confirmContainer) confirmContainer.style.display = "none";
        if (resendMessage) resendMessage.style.display = "none";
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

    // --- ЛОГИКА КНОПКИ "НАЗАД" ---
    function handleBackToRegister(e) {
        e.preventDefault();
        confirmContainer.style.display = "none";
        sliderContainer.style.display = "flex";
        // Убеждаемся, что слайдер стоит на регистрации
        if (modalContentBox) modalContentBox.classList.add("show-register");
    }

    // --- ЛОГИКА КНОПКИ "ОТПРАВИТЬ СНОВА" ---
    async function handleResendCode(e) {
        e.preventDefault();
        if (!lastRegisterData) return;

        // Скрываем прошлые сообщения
        if (resendMessage) resendMessage.style.display = "none";
        confirmErrorContainer.classList.remove("show");

        // Блокируем ссылку на пару секунд
        resendCodeLink.style.pointerEvents = "none";
        resendCodeLink.style.color = "#666";
        resendCodeLink.textContent = "Отправка...";

        try {
            // Отправляем те же данные на Register, это вызовет генерацию нового кода
            const response = await fetch("/Account/Register", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(lastRegisterData)
            });

            if (response.ok) {
                if (resendMessage) {
                    resendMessage.style.display = "block";
                    resendMessage.textContent = "Новый код отправлен!";
                }
            } else {
                const result = await response.json();
                confirmErrorContainer.textContent = result.message || "Ошибка отправки";
                confirmErrorContainer.classList.add("show");
            }
        } catch (error) {
            console.error(error);
            confirmErrorContainer.textContent = "Ошибка сети";
            confirmErrorContainer.classList.add("show");
        } finally {
            // Разблокируем ссылку
            setTimeout(() => {
                resendCodeLink.style.pointerEvents = "auto";
                resendCodeLink.style.color = ""; // Вернуть CSS цвет
                resendCodeLink.textContent = "Отправить код снова";
            }, 3000); // 3 секунды задержки
        }
    }

    // --- ОБРАБОТЧИК ФОРМ (ВХОД / РЕГИСТРАЦИЯ) ---
    async function handleFormSubmit(event, form) {
        event.preventDefault();
        const errorContainer = form.id === "login-form" ? loginErrorContainer : registerErrorContainer;

        errorContainer.classList.remove("show");
        errorContainer.textContent = "";

        const url = form.action;
        const formData = new FormData(form);
        const data = Object.fromEntries(formData.entries());

        // ✅ Сохраняем данные, если это регистрация
        if (form.id === 'register-form') {
            lastRegisterData = data;
        }

        try {
            const response = await fetch(url, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data)
            });

            const result = await response.json();

            if (response.ok) {
                if (form.id === 'register-form') {
                    if (result.needConfirm) {
                        // Показываем ввод кода
                        if (sliderContainer) sliderContainer.style.display = 'none';
                        if (confirmContainer) {
                            confirmContainer.style.display = "block";
                            const hiddenEmail = document.getElementById("confirm-email-hidden-input");
                            if (hiddenEmail) hiddenEmail.value = result.email;
                        }
                    } else {
                        if (sliderContainer) sliderContainer.style.display = 'none';
                        if (regSuccessContainer) regSuccessContainer.style.display = 'block';
                    }
                }
                else if (form.id === 'login-form') {
                    if (sliderContainer) sliderContainer.style.display = 'none';
                    if (loginSuccessContainer) loginSuccessContainer.style.display = 'block';
                    setTimeout(() => { window.location.reload(); }, 1500);
                }
            } else {
                errorContainer.textContent = result.message || "Ошибка";
                errorContainer.classList.add("show");
            }
        } catch (error) {
            console.error("Ошибка:", error);
            errorContainer.textContent = "Ошибка сети.";
            errorContainer.classList.add("show");
        }
    }

    // --- ОБРАБОТЧИК ПОДТВЕРЖДЕНИЯ КОДА ---
    async function handleConfirmSubmit(event) {
        event.preventDefault();
        confirmErrorContainer.classList.remove("show");
        confirmErrorContainer.textContent = "";

        const url = confirmForm.action;
        const formData = new FormData(confirmForm);
        const data = Object.fromEntries(formData.entries());

        try {
            const response = await fetch(url, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data)
            });

            const result = await response.json();

            if (response.ok) {
                confirmContainer.style.display = "none";
                if (loginSuccessContainer) {
                    const title = loginSuccessContainer.querySelector("h2");
                    const desc = loginSuccessContainer.querySelector("p");
                    if (title) title.textContent = "Регистрация завершена!";
                    if (desc) desc.textContent = "Выполняется вход...";
                    loginSuccessContainer.style.display = "block";
                }
                setTimeout(() => { window.location.reload(); }, 2000);
            } else {
                confirmErrorContainer.textContent = result.message || "Неверный код";
                confirmErrorContainer.classList.add("show");
            }
        } catch (error) {
            console.error("Ошибка:", error);
            confirmErrorContainer.textContent = "Ошибка сети.";
            confirmErrorContainer.classList.add("show");
        }
    }

    // Привязка событий
    if (loginButton) loginButton.addEventListener("click", (e) => { e.preventDefault(); showLoginForm(); });
    if (registerButton) registerButton.addEventListener("click", (e) => { e.preventDefault(); showRegisterForm(); });

    if (modalCloseButton) modalCloseButton.addEventListener("click", hideModal);
    if (showRegisterLink) showRegisterLink.addEventListener("click", (e) => { e.preventDefault(); showRegisterForm(); });
    if (showLoginLink) showLoginLink.addEventListener("click", (e) => { e.preventDefault(); showLoginForm(); });

    // Новые обработчики
    if (backToRegisterLink) backToRegisterLink.addEventListener("click", handleBackToRegister);
    if (resendCodeLink) resendCodeLink.addEventListener("click", handleResendCode);

    document.addEventListener("keydown", (e) => { if (e.key === "Escape" && modalOverlay.classList.contains("active")) hideModal(); });

    if (loginForm) loginForm.addEventListener("submit", (e) => handleFormSubmit(e, loginForm));
    if (registerForm) registerForm.addEventListener("submit", (e) => handleFormSubmit(e, registerForm));
    if (confirmForm) confirmForm.addEventListener("submit", handleConfirmSubmit);
});
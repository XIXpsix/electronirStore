document.addEventListener("DOMContentLoaded", function () {
    console.log("✅ Скрипт загружен");

    // =========================================================
    // УПРАВЛЕНИЕ КАСТОМНЫМ МОДАЛЬНЫМ ОКНОМ
    // =========================================================

    const modalOverlay = document.getElementById("modal-overlay");
    const modalContent = document.getElementById("modal-content");
    const modalClose = document.querySelector(".modal-close");
    
    const loginForm = document.getElementById("loginForm");
    const registerForm = document.getElementById("registerForm");

    // Кнопки открытия (поправил селекторы)
    const openLoginBtns = document.querySelectorAll("button[id*='login']");
    const openRegisterBtns = document.querySelectorAll("button[id*='register']");

    // Кнопки переключения форм
    const switchToRegisterBtn = document.getElementById("go-to-register");
    const switchToLoginBtn = document.getElementById("go-to-login");

    // ✅ Функция ОТКРЫТИЯ модального окна
    function openModal(formType = 'login') {
        console.log("🔓 Открытие модала, тип:", formType);
        
        if (!modalOverlay) {
            console.error("❌ Элемент #modal-overlay не найден!");
            return;
        }
        
        modalOverlay.classList.add("active");
        console.log("✅ Класс active добавлен");
        
        if (formType === 'register' && modalContent) {
            modalContent.classList.add("show-register");
            console.log("✅ Переключение на регистрацию");
        } else if (modalContent) {
            modalContent.classList.remove("show-register");
            console.log("✅ Переключение на вход");
        }
    }

    // ✅ Функция ЗАКРЫТИЯ модального окна
    function closeModal() {
        console.log("🔒 Закрытие модала");
        if (modalOverlay) {
            modalOverlay.classList.remove("active");
            console.log("✅ Класс active удалён");
        }
    }

    // ✅ Закрытие по крестику
    if (modalClose) {
        modalClose.addEventListener("click", (e) => {
            e.preventDefault();
            closeModal();
        });
        console.log("✅ Обработчик крестика добавлен");
    } else {
        console.warn("⚠️ Элемент .modal-close не найден");
    }

    // ✅ Закрытие по клику на оверлей (но не на содержимое окна)
    if (modalOverlay) {
        modalOverlay.addEventListener("click", function (e) {
            if (e.target === modalOverlay) {
                closeModal();
            }
        });
        console.log("✅ Обработчик оверлея добавлен");
    }

    // ✅ Закрытие по Escape
    document.addEventListener("keydown", function (e) {
        if (e.key === "Escape" && modalOverlay?.classList.contains("active")) {
            closeModal();
        }
    });
    console.log("✅ Обработчик Escape добавлен");

    // ✅ Кнопки "Войти" (ищем по id содержащему 'login')
    document.querySelectorAll("[id*='login']:not(#loginForm):not(#loginEmail):not(#loginPassword):not(#loginError)").forEach(btn => {
        if (btn.tagName === 'BUTTON' || btn.tagName === 'A') {
            btn.addEventListener("click", (e) => {
                e.preventDefault();
                openModal('login');
            });
            console.log("✅ Кнопка входа обработана:", btn.id);
        }
    });

    // ✅ Кнопки "Регистрация" (ищем по id содержащему 'register')
    document.querySelectorAll("[id*='register']:not(#registerForm):not(#registerEmail):not(#registerPassword):not(#registerConfirm):not(#registerError)").forEach(btn => {
        if (btn.tagName === 'BUTTON' || btn.tagName === 'A') {
            btn.addEventListener("click", (e) => {
                e.preventDefault();
                openModal('register');
            });
            console.log("✅ Кнопка регистрации обработана:", btn.id);
        }
    });

    // ✅ Переключение на регистрацию
    if (switchToRegisterBtn) {
        switchToRegisterBtn.addEventListener("click", (e) => {
            e.preventDefault();
            openModal('register');
        });
        console.log("✅ Ссылка на регистрацию обработана");
    }

    // ✅ Переключение на вход
    if (switchToLoginBtn) {
        switchToLoginBtn.addEventListener("click", (e) => {
            e.preventDefault();
            openModal('login');
        });
        console.log("✅ Ссылка на вход обработана");
    }

    // =========================================================
    // ЛОГИКА ВХОДА (LOGIN)
    // =========================================================
    if (loginForm) {
        loginForm.addEventListener("submit", async function (e) {
            e.preventDefault();
            console.log("📝 Отправка формы входа");

            const errorDiv = document.getElementById("loginError");
            if (errorDiv) errorDiv.innerHTML = "";

            const email = document.getElementById("loginEmail")?.value;
            const password = document.getElementById("loginPassword")?.value;

            if (!email || !password) {
                if (errorDiv) {
                    errorDiv.classList.add("show");
                    errorDiv.innerText = "Заполните все поля";
                }
                return;
            }

            try {
                const response = await fetch('/Account/Login', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ email, password })
                });

                const result = await response.json();

                if (response.ok) {
                    console.log("✅ Вход успешен");
                    closeModal();
                    window.location.href = result.returnUrl || "/";
                } else {
                    console.error("❌ Ошибка входа:", result.message);
                    if (errorDiv) {
                        errorDiv.classList.add("show");
                        errorDiv.innerText = result.message || "Ошибка входа. Проверьте email и пароль.";
                    }
                }
            } catch (error) {
                console.error("❌ Ошибка сети:", error);
                if (errorDiv) {
                    errorDiv.classList.add("show");
                    errorDiv.innerText = "Ошибка сети. Попробуйте позже.";
                }
            }
        });
        console.log("✅ Обработчик формы входа добавлен");
    } else {
        console.warn("⚠️ Форма входа не найдена");
    }

    // =========================================================
    // ЛОГИКА РЕГИСТРАЦИИ (REGISTER)
    // =========================================================
    if (registerForm) {
        registerForm.addEventListener("submit", async function (e) {
            e.preventDefault();
            console.log("📝 Отправка формы регистрации");

            const errorDiv = document.getElementById("registerError");
            if (errorDiv) errorDiv.innerHTML = "";

            const email = document.getElementById("registerEmail")?.value;
            const password = document.getElementById("registerPassword")?.value;
            const confirmPassword = document.getElementById("registerConfirm")?.value;

            // Валидация
            if (!email || !password || !confirmPassword) {
                if (errorDiv) {
                    errorDiv.classList.add("show");
                    errorDiv.innerText = "Заполните все поля";
                }
                return;
            }

            if (password !== confirmPassword) {
                if (errorDiv) {
                    errorDiv.classList.add("show");
                    errorDiv.innerText = "Пароли не совпадают";
                }
                return;
            }

            if (password.length < 6) {
                if (errorDiv) {
                    errorDiv.classList.add("show");
                    errorDiv.innerText = "Пароль должен быть минимум 6 символов";
                }
                return;
            }

            try {
                const response = await fetch('/Account/Register', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ email, password })
                });

                const result = await response.json();

                if (response.ok) {
                    console.log("✅ Регистрация успешна");
                    closeModal();
                    if (result.redirectUrl) {
                        window.location.href = result.redirectUrl;
                    } else {
                        location.reload();
                    }
                } else {
                    console.error("❌ Ошибка регистрации:", result.message);
                    if (errorDiv) {
                        errorDiv.classList.add("show");
                        errorDiv.innerText = result.message || "Ошибка регистрации. Попробуйте другой email.";
                    }
                }
            } catch (error) {
                console.error("❌ Ошибка сети:", error);
                if (errorDiv) {
                    errorDiv.classList.add("show");
                    errorDiv.innerText = "Ошибка сети. Попробуйте позже.";
                }
            }
        });
        console.log("✅ Обработчик формы регистрации добавлен");
    } else {
        console.warn("⚠️ Форма регистрации не найдена");
    }

    console.log("✅ Все обработчики инициализированы");
});
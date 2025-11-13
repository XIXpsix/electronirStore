/* ФАЙЛ: ElectronicsStore/wwwroot/js/login_and_registration_script.js
  Этот код реализует логику модального окна из Глав 6 и 8 твоей методички.
*/

document.addEventListener('DOMContentLoaded', function () {

    // --- Элементы управления модальным окном ---
    const modalContainer = document.querySelector(".container-login-registration");
    const overlay = document.querySelector(".overlay");

    // Элементы ВНУТРИ модального окна
    const signInBtnInternal = document.querySelector('.signin-btn'); // Переключатель "Войти"
    const signUpBtnInternal = document.querySelector('.signup-btn'); // Переключатель "Регистрация"
    const formBox = document.querySelector('.form-box');
    const block = document.querySelector('.block');

    // Кнопки В ШАПКЕ
    const headerLoginButton = document.getElementById("click-to-hide-login");
    const headerRegisterButton = document.getElementById("click-to-hide-register");

    // --- Элементы форм ---
    const formSignin = document.querySelector('.form_signin');
    const formSignup = document.querySelector('.form_signup');

    // Поля Входа
    const signinEmail = document.querySelector("#signin_email");
    const signinPassword = document.querySelector("#signin_password");
    const errorContainerSignin = document.getElementById('error-messages-signin');

    // Поля Регистрации
    const signupFirstName = document.querySelector("#signup_firstname");
    const signupLastName = document.querySelector("#signup_lastname");
    const signupEmail = document.querySelector("#signup_email");
    const signupPassword = document.querySelector("#signup_password");
    const signupConfirmPassword = document.querySelector("#signup_confirmpassword");
    const errorContainerSignup = document.getElementById('error-messages-signup');


    // --- Функции ---

    // Функция открытия/закрытия окна
    function toggleModal(show = true) {
        if (!modalContainer) return;
        modalContainer.style.display = show ? "flex" : "none";
    }

    // Функция переключения на "Войти"
    function showLoginTab() {
        if (!formBox || !block) return;
        formBox.classList.remove('active');
        block.classList.remove('active');
        // Очищаем ошибки
        displayErrors([], errorContainerSignin);
        displayErrors([], errorContainerSignup);
    }

    // Функция переключения на "Регистрация"
    function showRegisterTab() {
        if (!formBox || !block) return;
        formBox.classList.add('active');
        block.classList.add('active');
        // Очищаем ошибки
        displayErrors([], errorContainerSignin);
        displayErrors([], errorContainerSignup);
    }

    // Функция отправки fetch-запроса (из Главы 8, рис. 84)
    function sendRequest(method, url, body = null) {
        const headers = {
            'Content-Type': 'application/json',
            // Ищем AntiForgeryToken (ВАЖНО для [ValidateAntiForgeryToken])
            'RequestVerificationToken': document.getElementsByName('__RequestVerificationToken')[0]?.value
        };

        return fetch(url, {
            method: method,
            headers: headers,
            body: body ? JSON.stringify(body) : null
        }).then(response => {
            if (!response.ok) {
                // Если ответ не 200 (OK), пытаемся прочитать ошибки
                return response.json().then(errorData => {
                    // Ошибки валидации ASP.NET (из ModelState)
                    if (errorData.errors) {
                        let aspNetErrors = [];
                        for (const key in errorData.errors) {
                            aspNetErrors = aspNetErrors.concat(errorData.errors[key]);
                        }
                        throw aspNetErrors;
                    }
                    // Ошибки Identity (пароль не тот, email занят и т.д.)
                    else if (Array.isArray(errorData)) {
                        // ASP.NET 8+ иногда возвращает массив строк
                        if (typeof errorData[0] === 'string') {
                            throw errorData;
                        }
                        // ASP.NET 6+ возвращает { code, description }
                        throw errorData.map(e => e.description);
                    }
                    // Другие ошибки
                    throw [errorData.message || 'Произошла неизвестная ошибка'];
                });
            }
            // Успех (200 OK)
            // Если ответ пустой (например, при выходе), вернем {success: true}
            return response.text().then(text => text ? JSON.parse(text) : { success: true });
        });
    }

    // Функция отображения ошибок (из Главы 8, рис. 87)
    function displayErrors(errors, errorContainer) {
        if (!errorContainer) return;
        errorContainer.innerHTML = ''; // Очистить
        if (Array.isArray(errors)) {
            errors.forEach(error => {
                const errorMessage = document.createElement('div');
                errorMessage.classList.add('error');
                errorMessage.textContent = error;
                errorContainer.appendChild(errorMessage);
            });
        }
    }

    // --- Обработчики событий ---

    // Кнопка "Войти" в шапке
    headerLoginButton?.addEventListener("click", () => {
        toggleModal(true);
        showLoginTab();
    });

    // Кнопка "Регистрация" в шапке
    headerRegisterButton?.addEventListener("click", () => {
        toggleModal(true);
        showRegisterTab();
    });

    // Оверлей (серая область)
    overlay?.addEventListener("click", () => toggleModal(false));

    // Внутренний переключатель "Войти"
    signInBtnInternal?.addEventListener('click', showLoginTab);

    // Внутренний переключатель "Регистрация"
    signUpBtnInternal?.addEventListener('click', showRegisterTab);


    // --- Логика ВХОДА (отправка формы) ---
    formSignin?.addEventListener('submit', (e) => {
        e.preventDefault(); // Запрещаем стандартную отправку

        // АДАПТАЦИЯ: URL изменен на /Account/Login
        const requestURL = '/Account/Login';

        const body = {
            email: signinEmail.value,
            password: signinPassword.value,
            rememberMe: false
        };

        sendRequest('POST', requestURL, body)
            .then(data => {
                console.log('Успешный вход:', data);
                location.reload(); // Перезагружаем страницу при успехе
            })
            .catch(err => {
                console.error('Ошибка входа:', err);
                displayErrors(err, errorContainerSignin);
            });
    });


    // --- Логика РЕГИСТРАЦИИ (отправка формы) ---
    formSignup?.addEventListener('submit', (e) => {
        e.preventDefault(); // Запрещаем стандартную отправку

        // АДАПТАЦИЯ: URL изменен на /Account/Register
        const requestURL = '/Account/Register';

        // АДАПТАЦИЯ: 'body' включает FirstName и LastName
        const body = {
            firstName: signupFirstName.value,
            lastName: signupLastName.value,
            email: signupEmail.value,
            password: signupPassword.value,
            confirmPassword: signupConfirmPassword.value
        };

        sendRequest('POST', requestURL, body)
            .then(data => {
                console.log('Успешная регистрация:', data);
                location.reload(); // Перезагружаем страницу при успехе
            })
            .catch(err => {
                console.error('Ошибка регистрации:', err);
                displayErrors(err, errorContainerSignup);
            });
    });

});
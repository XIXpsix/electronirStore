// Файл: wwwroot/js/login_and_registration_script.js

// Код из ГЛАВЫ 6 (рис. 62) - Открытие/закрытие формы
document.addEventListener('DOMContentLoaded', function () {

    function hiddenOpen_Closeclick() {
        let x = document.querySelector(".container-login-registration");
        if (x.style.display == "" || x.style.display == "none") {
            x.style.display = "flex"; // Используем flex, как в CSS
        } else {
            x.style.display = "none";
        }
    }

    // Назначаем на кнопку "Войти" в шапке
    const headerLoginButton = document.getElementById("click-to-hide");
    if (headerLoginButton) {
        headerLoginButton.addEventListener("click", hiddenOpen_Closeclick);
    }
    // Назначаем на оверлей (серая область)
    const overlay = document.querySelector(".overlay");
    if (overlay) {
        overlay.addEventListener("click", hiddenOpen_Closeclick);
    }

    // Код из ГЛАВЫ 6 (рис. 69) - Переключение Вход/Регистрация
    const signInBtn = document.querySelector('.signin-btn');
    const signUpBtn = document.querySelector('.signup-btn');
    const formBox = document.querySelector('.form-box');
    const block = document.querySelector('.block');

    if (signUpBtn && signInBtn) {
        signUpBtn.addEventListener('click', function () {
            formBox.classList.add('active');
            block.classList.add('active');
        });

        signInBtn.addEventListener('click', function () {
            formBox.classList.remove('active');
            block.classList.remove('active');
        });
    }

    // --- Код из ГЛАВЫ 8 ---

    // Функция отправки fetch-запроса (рис. 84)
    function sendRequest(method, url, body = null) {
        const headers = {
            'Content-Type': 'application/json'
        };

        const fetchOptions = {
            method: method,
            headers: headers
        };

        if (body) {
            fetchOptions.body = JSON.stringify(body);
        }

        return fetch(url, fetchOptions).then(response => {
            if (!response.ok) {
                // Если ответ не 200 (OK), пытаемся прочитать ошибки
                return response.json().then(errorData => {
                    throw errorData; // Бросаем массив ошибок
                });
            }
            // Если ответ 200 (OK)
            return response.json();
        });
    }

    // Функция отображения ошибок (рис. 87)
    function displayErrors(errors, errorContainer) {
        errorContainer.innerHTML = ''; // Очистить предыдущие ошибки
        if (Array.isArray(errors)) {
            errors.forEach(error => {
                const errorMessage = document.createElement('div');
                errorMessage.classList.add('error');
                errorMessage.textContent = error;
                errorContainer.appendChild(errorMessage);
            });
        } else if (errors.title) { // Обработка стандартных ошибок ASP.NET Core
            for (const key in errors.errors) {
                errors.errors[key].forEach(error => {
                    const errorMessage = document.createElement('div');
                    errorMessage.classList.add('error');
                    errorMessage.textContent = error;
                    errorContainer.appendChild(errorMessage);
                });
            }
        }
    }

    // Функция очистки и закрытия (рис. 90)
    function cleaningAndClosingForm(formElements, errorContainer) {
        errorContainer.innerHTML = '';
        for (const key in formElements) {
            if (formElements.hasOwnProperty(key)) {
                formElements[key].value = ''; // Сброс значений полей формы
            }
        }
        hiddenOpen_Closeclick(); // Закрываем форму
    }


    // --- Логика ВХОДА (рис. 83, 86, 89) ---
    const form_btn_signin = document.querySelector('.form_btn_signin');
    if (form_btn_signin) {

        const errorContainerSignin = document.getElementById('error-messages-signin');
        const formSignin = {
            email: document.querySelector("#signin_email"),
            password: document.querySelector("#signin_password")
        };

        form_btn_signin.addEventListener('click', function () {
            // АДАПТАЦИЯ: URL изменен на /Account/Login
            const requestURL = '/Account/Login';

            const body = {
                email: formSignin.email.value,
                password: formSignin.password.value
            };

            sendRequest('POST', requestURL, body)
                .then(data => {
                    console.log('Успешный ответ (вход):', data);
                    // cleaningAndClosingForm(formSignin, errorContainerSignin); // Не закрываем, а перезагружаем
                    location.reload(); // Перезагружаем страницу при успехе
                })
                .catch(err => {
                    console.log('Ошибка (вход):', err);
                    displayErrors(err, errorContainerSignin);
                });
        });
    }


    // --- Логика РЕГИСТРАЦИИ (рис. 91 + Адаптация) ---
    const form_btn_signup = document.querySelector('.form_btn_signup');
    if (form_btn_signup) {

        const errorContainerSignup = document.getElementById('error-messages-signup');
        const formSignup = {
            firstName: document.querySelector("#signup_firstname"),
            lastName: document.querySelector("#signup_lastname"),
            email: document.querySelector("#signup_email"),
            password: document.querySelector("#signup_password"),
            confirmPassword: document.querySelector("#signup_confirmpassword")
        };

        form_btn_signup.addEventListener('click', function () {
            // АДАПTAЦИЯ: URL изменен на /Account/Register
            const requestURL = '/Account/Register';

            // АДАПТАЦИЯ: 'body' включает FirstName и LastName
            const body = {
                firstName: formSignup.firstName.value,
                lastName: formSignup.lastName.value,
                email: formSignup.email.value,
                password: formSignup.password.value,
                confirmPassword: formSignup.confirmPassword.value
            };

            sendRequest('POST', requestURL, body)
                .then(data => {
                    console.log('Успешный ответ (регистрация):', data);
                    // cleaningAndClosingForm(formSignup, errorContainerSignup); // Не закрываем, а перезагружаем
                    location.reload(); // Перезагружаем страницу при успехе
                })
                .catch(err => {
                    console.log('Ошибка (регистрация):', err);

                    // ASP.NET Identity возвращает ошибки иначе, чем в методичке.
                    // Этот код обработает их.
                    if (err.errors) {
                        let aspNetErrors = [];
                        for (const key in err.errors) {
                            aspNetErrors = aspNetErrors.concat(err.errors[key]);
                        }
                        displayErrors(aspNetErrors, errorContainerSignup);
                    } else {
                        displayErrors(err, errorContainerSignup);
                    }
                });
        });
    }

});
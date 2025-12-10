document.addEventListener('DOMContentLoaded', function () {

    // Функция для открытия/закрытия формы
    function hiddenOpen_Closeclick(event) {
        // Проверяем, был ли клик по элементу, который должен закрывать форму
        if (event.target.classList.contains('overlay') || event.currentTarget.id === 'click-to-hide') {
            let x = document.querySelector(".container-login-registration");
            if (x.style.display === "none" || x.style.display === "") {
                x.style.display = "grid";
            } else {
                x.style.display = "none";
            }
        }
    }

    // 1. Привязываем открытие формы к кнопке в шапке
    const clickToHideButton = document.getElementById("click-to-hide");
    if (clickToHideButton) {
        clickToHideButton.addEventListener("click", hiddenOpen_Closeclick);
    }

    // 2. Привязываем закрытие формы при клике на оверлей
    const overlay = document.querySelector(".overlay");
    if (overlay) {
        overlay.addEventListener("click", hiddenOpen_Closeclick);
    }

    // Привязка переключения между формами (логика из методички)
    const signInBtn = document.querySelector('.signin-btn');
    const signUpBtn = document.querySelector('.signup-btn');
    const formBox = document.querySelector('.form-box');
    const block = document.querySelector('.block');

    if (signInBtn && signUpBtn) {
        signUpBtn.addEventListener('click', function () {
            formBox.classList.add('active');
            block.classList.add('active');
        });

        signInBtn.addEventListener('click', function () {
            formBox.classList.remove('active');
            block.classList.remove('active');
        });
    }

    // ВАШ КОД ДЛЯ ОТПРАВКИ/ВАЛИДАЦИИ ФОРМЫ ВХОДА И РЕГИСТРАЦИИ ТУТ...
});
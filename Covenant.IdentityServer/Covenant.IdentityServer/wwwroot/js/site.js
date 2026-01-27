function addFloatEffect(input, label){
    if(input && input.value !== ''){
        label.classList.add('not-empty');
    }

    if(input){
        input.addEventListener('change', function(e){
            if(input.value !== ''){
                label.classList.add('not-empty');
            }else {
                label.classList.remove('not-empty');
            }
        });
    }
}


// login
var login = document.getElementById('label-login');
var inputLogin = document.getElementById('username');
var password = document.getElementById('label-password');
var inputPassword = document.getElementById('password');

addFloatEffect(inputLogin, login);
addFloatEffect(inputPassword, password);



// create password
var createPass = document.getElementById('label-create-password');
var inputCreatePass = document.getElementById('create-password');
var confirmPass = document.getElementById('label-confirm-password');
var inputConfirmPass = document.getElementById('confirm-password');

addFloatEffect(inputCreatePass, createPass);
addFloatEffect(inputConfirmPass, confirmPass);

// reset password
var resetPass = document.getElementById('label-reset-password');
var inputResetPass = document.getElementById('reset-password');
var resetConfirmPass = document.getElementById('label-reset-confirm');
var inputResetConfirmPass = document.getElementById('reset-confirm');

addFloatEffect(inputResetPass, resetPass);
addFloatEffect(inputResetConfirmPass, resetConfirmPass);


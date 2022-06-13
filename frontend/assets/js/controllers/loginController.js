$(document).ready(function () {
  $(".login-info-box").fadeOut();
  $(".login-show").addClass("show-log-panel");
});

//comportamiento del template de login
$('.login-reg-panel input[type="radio"]').on("change", function () {
  if ($("#log-login-show").is(":checked")) {
    $(".register-info-box").fadeOut();
    $(".login-info-box").fadeIn();

    $(".white-panel").addClass("right-log");
    $(".register-show").addClass("show-log-panel");
    $(".login-show").removeClass("show-log-panel");
  } else if ($("#log-reg-show").is(":checked")) {
    $(".register-info-box").fadeIn();
    $(".login-info-box").fadeOut();

    $(".white-panel").removeClass("right-log");

    $(".login-show").addClass("show-log-panel");
    $(".register-show").removeClass("show-log-panel");
  }
});

//funciones de login
function login() {
  var email = document.getElementById("username").value;
  var password = document.getElementById("password").value;

  let camposVacios = validarCamposVacios(email, password);
  let emailOk = validarEmail(email);
  let passwordSegura = validarPassword(password);

  if (camposVacios) {
    alert("Todos los campos son obligatorios");
  } else {
    if (passwordSegura) {
      if (emailOk) {
        var data = {
          email: email,
          password: password,
        };
        //como manejar el error y el success
        /*$.ajax({
          type: "POST",
          url: "https://reqres.in/api/login",
          data: data,
          success: function (response) {
            console.log(response);
          },
          statusCode: {
            404: function () {
              alert("Ha ocurrido un error al intentar inicar sesión");
            },
            200: function () {
              window.location = "Guest/dashboardGuest.html";
            },
          },
        });*/
        $.ajax({
          type: "GET",
          url: "https://localhost:44347/api/Users",
          success: function (response) {
            console.log(response);
          },
          /*statusCode: {
            404: function () {
              alert("Ha ocurrido un error al intentar inicar sesión");
            },
            200: function () {
              window.location = "Guest/dashboardGuest.html";
            },
          },*/
        });
      } else {
        alert("El email no es valido");
      }
    } else {
      alert("La contraseña debe tener al menos 8 caracteres");
    }
  }
}
function register() {
  var email = document.getElementById("usernameRegister").value;
  var passwordRegister = document.getElementById("passwordRegister").value;
  var passwordRegisterRepeat = document.getElementById(
    "passwordRegisterRepeat"
  ).value;

  let passwordOk = compararPassword(passwordRegister, passwordRegisterRepeat);
  let emailOk = validarEmail(email);
  let passwordSegura = validarPassword(passwordRegister);

  if (emailOk) {
    if (passwordSegura) {
      if (passwordOk) {
        var data = {
          email: email,
          password: passwordOk,
        };
        //como manejar el error y el success
        $.ajax({
          type: "POST",
          url: "https://reqres.in/api/register",
          data: data,
          success: function (response) {
            console.log(response);
            //window.location = "login.html";
          },
          error: function (error) {
            console.log(error);
          },
        });
      } else {
        alert("Las contraseñas no coinciden");
      }
    } else {
      alert(
        "La contraseña debe tener al entre 8 y 16 caracteres, al menos un dígito,al menos una minúscula,al menos una mayúscula y al menos un caracter no alfanumérico."
      );
    }
  } else {
    alert("El email no es valido");
  }
}

function compararPassword(password, passwordRepeat) {
  if (password != passwordRepeat) {
    return false;
  }
  return true;
}

//funcion validar email
function validarEmail(email) {
  var regex = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
  if (!regex.test(email)) {
    return false;
  }
  return true;
}

function validarCamposVacios(email, password) {
  if (email == "" || password == "") {
    return true;
  }
  return false;
}

function validarPassword(password) {
  /*La contraseña debe tener al menos 8 caracteres, 
  al menos un dígito, 
  al menos una minúscula, 
  al menos una mayúscula 
  y al menos un caracter no alfanumérico.*/
  var regex =
    /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])([A-Za-z\d$@$!%*?&]|[^ ]){8,}$/;
  if (!regex.test(password)) {
    return false;
  }
  return true;
}

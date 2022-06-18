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
//definimos constantes para mostrar mensajes de error
const stringTemplates = {
  camposObligatorios: "Todos los campos son obligatorios",
  emailInvalido: "El email no es valido",
  connectionError: "Error de conexión",
  registroCorrecto: "Usuario registrado correctamente",
  registroError: "Error al registrar usuario, intente de nuevo mas tarde",
  captchaError: "Resuelve el captcha",
  passwordIncorrecta: `La contraseña debe tener mínimo 8 caracteres y estar compuesta por combinaciones de al menos una letra minúscula ("a-z"), mayúscula ("A-Z"), número ("0-9") y al menos un caracter especial`,
  passwordNoCoinciden: "Las contraseñas no coinciden",
  errorIniciarSesion: "Error al iniciar sesión",
  passwordDiferentes: "Contraseña actual y nueva deben ser diferentes",
};

//funcion de login
function login() {
  var email = document.getElementById("email").value;
  var password = document.getElementById("password").value;

  let camposVacios = validarCamposVacios(email, password);
  let emailOk = validarEmail(email);
  let passwordSegura = validarPassword(password);

  if (camposVacios) {
    alert(stringTemplates.camposObligatorios);
  } else {
    if (emailOk) {
      if (passwordSegura) {
        var data = {
          username: email,
          password: password,
        };
        $.ajax({
          type: "POST",
          url: "https://localhost:44347/api/Auth/login",
          contentType: "application/json",
          data: JSON.stringify(data),
          dataType: "json",
          statusCode: {
            200: function (response) {
              var tokenInfo = parsearJwt(response.responseText);
              localStorage.setItem("token", response.responseText);
              redireccionarUsuario(tokenInfo.Role);
            },
            404: function () {
               alert(stringTemplates.connectionError);
            },
            400: function () {
               alert(stringTemplates.errorIniciarSesion);
            },
            500: function () {
               alert(stringTemplates.errorIniciarSesion);
            },
          },
        });
      } else {
        alert(stringTemplates.passwordIncorrecta);
      }
    } else {
      alert(stringTemplates.emailInvalido);
    }
  }
}

//funcion para registrar un usuario
function register() {
  var email = document.getElementById("emailRegister").value;
  var passwordRegister = document.getElementById("passwordRegister").value;
  var passwordRegisterRepeat = document.getElementById(
    "passwordRegisterRepeat"
  ).value;

  let passwordNoRepetidas = compararPassword(passwordRegister, passwordRegisterRepeat);
  let camposVacios = validarCamposVacios(email, passwordRegister);
  let emailOk = validarEmail(email);
  let passwordSegura = validarPassword(passwordRegister);

  if (camposVacios) {
    alert(stringTemplates.camposObligatorios);
  } else {
    if (emailOk) {
      if (passwordSegura) {
        if (passwordNoRepetidas) {
          var data = {
            username: email,
            password: passwordRegister,
          };
          $.ajax({
            type: "POST",
            url: "https://localhost:44347/api/Auth/register",
            contentType: "application/json",
            data: JSON.stringify(data),
            dataType: "json",
            statusCode: {
              200: function (response) {
                alert(stringTemplates.registroCorrecto);
                window.location = "login.html";
              },
              404: function () {
                alert(stringTemplates.connectionError);
              },
              400: function () {
                alert(stringTemplates.registroError);
              },
            },
          });
        } else {
          alert(stringTemplates.passwordNoCoinciden);
        }
      } else {
        alert(stringTemplates.passwordIncorrecta);
      }
    } else {
      alert(stringTemplates.emailInvalido);
    }
  }
}

//funcion que compara las contraseñas, que sean iguales y no vacias
function compararPassword(password, passwordRepeat) {
  if (password === "" || passwordRepeat === "") {
    return false;
  } else if (password != passwordRepeat) {
    return false;
  }
  return true;
}

//funcion validar email de forma general
function validarEmail(email) {
  var regex = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
  if (!regex.test(email)) {
    return false;
  }
  return true;
}

//funcion validar que los campos no pueden ser vacios
function validarCamposVacios(email, password) {
  if (email == "" || password == "") {
    return true;
  }
  return false;
}

//funcion validar password de forma general
function validarPassword(password) {
  var regex =
    /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])([A-Za-z\d$@$!%*?&]|[^ ]){8,20}$/;
  if (!regex.test(password)) {
    return false;
  }
  return true;
}

//funcion que recibe un JWT y lo parsea para obtener los datos del usuario
function parsearJwt(token) {
  var base64Url = token.split(".")[1];
  var base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
  var jsonPayload = decodeURIComponent(
    atob(base64)
      .split("")
      .map(function (c) {
        return "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2);
      })
      .join("")
  );
  return JSON.parse(jsonPayload);
}

//funcion que redirecciona al usuario segun su rol
function redireccionarUsuario(role) {
  if (role == "Administrador") {
    window.location = "Admin/dashboardAdmin.html";
  } else if (role == "Usuario") {
    window.location = "User/dashboardUser.html";
  } else {
    window.location = "Guest/dashboardGuest.html";
  }
}

function mostrarContrasena(){
  var tipo = document.getElementById("password");
  if(tipo.type == "password"){
      tipo.type = "text";
  }else{
      tipo.type = "password";
  }
}

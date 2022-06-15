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

//funcion de login
function login() {
  debugger;
  var email = document.getElementById("email").value;
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
          username: email,
          password: password,
        };
        $.ajax({
          type: "POST",
          url: "https://localhost:44347/api/Auth/login",
          contentType: "application/json",
          data: JSON.stringify(data),
          dataType: "json",
          /*success: function (response) {
            console.log(response.token);
            /*var tokenInfo = parsearJwt(response);
            localStorage.setItem("token", response);
            redireccionarUsuario(tokenInfo.rol);*/
          /*},*/
         /* error: function (error) {
            console.log("Error" + error);
          },*/
          statusCode: {
            200: function (response) { //Employee_Company saved now updated

              console.log(response.responseText);
              var tokenInfo = parsearJwt(response.responseText);
               redireccionarUsuario(tokenInfo.Role);

            },
            404: function () {
              console.log("mal")
            }
          }
        });

      } else {
        alert("Email incorrecto");
      }
    } else {
      alert("La contraseña debe tener al menos 8 caracteres");
    }
  }
}
function register() {
  var email = document.getElementById("emailRegister").value;
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
        $.ajax({
          type: "POST",
          url: "https://localhost:44347/api/Register",
          data: data,
          success: function (response) {
            alert("Usuario registrado correctamente");
            console.log(response);
            window.location = "login.html";
          },
          error: function (error) {
            console.log("Error " + error);
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
  debugger;
  if (role == "Administrador") {
    window.location = "Admin/dashboardAdmin.html";
  } else if (role == "user") {
    window.location = "User/dashboardUser.html";
  } else {
    window.location = "Guest/dashboardGuest.html";
  }
}

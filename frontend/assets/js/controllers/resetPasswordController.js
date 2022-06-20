$(document).ready(function () {
  controlTokenExpirado();
});

const stringTemplates = {
  camposObligatorios: "Todos los campos son obligatorios",
  passwordIncorrecta: `La contraseña debe tener mínimo 8 caracteres y estar compuesta por combinaciones de al menos una letra minúscula ("a-z"), mayúscula ("A-Z"), número ("0-9") y al menos un caracter especial`,
  passwordDiferentes: "Contraseña actual y nueva deben ser diferentes",
  passwordActualIncorrecta: "La contraseña actual es incorrecta",
  reseteoCorrecto: "Contraseña actualizada correctamente",
  rolInvalido: "Solo los Administradores tienen permisos para esta acción",
};

//funcion que controla si el token expiro
function controlTokenExpirado() {
  var token = localStorage.getItem("token");
  if (token) {
    var tokenInfo = parsearJwt(token);
    var exp = tokenInfo.exp;
    var now = new Date().getTime() / 1000;
    if (exp < now) {
      localStorage.clear();
      window.location = "../login.html";
    }
  } else {
    localStorage.clear();
    window.location = "../login.html";
  }
}

//funcion para cerrar sesion
function cerrarSesion() {
  localStorage.clear();
  window.location = "../login.html";
}

//funcion para parsear el JWT
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
function redireccionarUsuario() {
  var token = localStorage.getItem("token");
  var tokenInfo = parsearJwt(token);
  if (tokenInfo.Role == "Administrador") {
    window.location = "../Admin/dashboardAdmin.html";
  } else if (tokenInfo.Role == "Usuario") {
    window.location = "../User/dashboardUser.html";
  } else {
    window.location = "../Guest/dashboardGuest.html";
  }
}

function resetPassword() {
  var passwordActual = document.getElementById("passwordActual").value;
  var passwordNueva = document.getElementById("passwordNueva").value;
  var confrimarPassword = document.getElementById("confirmarPassword").value;

  let passwordActualOk = validarPassword(passwordActual);
  let passwordNuevaOk = validarPassword(passwordNueva);
  let confrimarPasswordOk = validarPassword(confrimarPassword);
  let controlPasswordNueva = controlarPasswordNueva(
    passwordNueva,
    confrimarPassword
  );

  let camposVacios = validarCamposVacios(
    passwordActual,
    passwordNueva,
    confrimarPassword
  );

  if (camposVacios) {
    alert(stringTemplates.camposObligatorios);
  } else {
    if (passwordActualOk && passwordNuevaOk && confrimarPasswordOk) {
      if (controlPasswordNueva) {
        var data = {
          passwordActual: passwordActual,
          passwordNueva: passwordNueva,
        };
        $.ajax({
          type: "PUT",
          url: "https://localhost:44347/api/Auth/reset",
          contentType: "application/json",
          dataType: "json",
          data: JSON.stringify(data),
          headers: { authorization: localStorage.getItem("token") },
          statusCode: {
            200: function () {
              alert(stringTemplates.reseteoCorrecto);
              window.location = "../login.html";
              localStorage.clear();
            },
            400: function () {
              alert(stringTemplates.passwordActualIncorrecta);
              window.location.reload();
            },
            403: function () {
              alert(stringTemplates.rolInvalido);
              window.location = "../Security/errorPage.html";
              localStorage.clear();
            },
          },
        });
      } else {
        alert(stringTemplates.passwordDiferentes);
      }
    } else {
      alert(stringTemplates.passwordIncorrecta);
    }
  }

  //funcion validar password de forma general
  function validarPassword(password) {
    var regex =
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])([A-Za-z\d$@$!%*?&]|[^ ]){8,}$/;
    if (!regex.test(password)) {
      return false;
    }
    return true;
  }

  //funcion que compara las contraseñas nueva y actual, que sean diferentes
  function controlarPasswordNueva(passwordNueva, confrimarPassword) {
    if (passwordNueva != confrimarPassword) {
      return false;
    }
    return true;
  }

  //funcion que valida que los campos no esten vacios
  function validarCamposVacios(
    passwordActual,
    passwordNueva,
    confrimarPassword
  ) {
    if (
      passwordActual == "" ||
      passwordNueva == "" ||
      confrimarPassword == ""
    ) {
      return true;
    }
    return false;
  }
}

//funcion que muestra contraseña escrita por el usuario en el formulario
function mostrarContrasena(idElemento) {
  var tipo = document.getElementById(idElemento);
  if (tipo.type == "password") {
    tipo.type = "text";
  } else {
    tipo.type = "password";
  }
}

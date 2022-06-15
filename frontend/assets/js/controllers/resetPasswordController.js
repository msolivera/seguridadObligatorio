//funcion para reestablecer contraseña
//recibe contraseña actual y nueva contraseña



$(document).ready(function () {
  controlTokenExpirado();
});

function controlTokenExpirado() {
  var token = localStorage.getItem("token");
  if (token) {
    var tokenInfo = parsearJwt(token);
    var exp = tokenInfo.exp;
    var now = new Date().getTime() / 1000;
    if (exp < now) {
      alert("Su sesión ha expirado");
      localStorage.clear();
      window.location = "../login.html";
    }
  } else {
    alert("No tiene permisos para acceder a esta página");
    window.location = "../login.html";
    localStorage.clear();
  }
}

function cerrarSesion() {
  localStorage.clear();
  window.location = "../login.html"; 
}

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
  let passwordNuevaOk = validarPasswordNueva(passwordNueva, confrimarPassword);
  let camposVacios = validarCamposVacios(
    passwordActual,
    passwordNueva,
    confrimarPassword
  );

  if (camposVacios) {
    alert("Todos los campos son obligatorios");
  } else {
    if (passwordNuevaOk) {
      var data = {
        passwordActual: passwordActual,
        passwordNueva: passwordNueva,
      };
      //como manejar el error y el success

      $.ajax({
        type: "PUT",
        url: "#",
        data: data,
        success: function (response) {
          console.log(response);
        },
      });
    } else {
      alert("Las contraseñas no coinciden");
    }
  }

  function validarPassword(password) {
    //aca tengo que llamar a la base de datos y preguntar si la password que tiene el usuario coincide con la que escribio
  }
  function validarPasswordNueva(passwordNueva, confrimarPassword) {
    if (passwordNueva != confrimarPassword) {
      return false;
    }
    return true;
  }
  function validarCamposVacios(passwordActual, passwordNueva, confrimarPassword) {
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

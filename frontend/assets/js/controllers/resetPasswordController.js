//funcion para reestablecer contraseña
//recibe contraseña actual y nueva contraseña

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

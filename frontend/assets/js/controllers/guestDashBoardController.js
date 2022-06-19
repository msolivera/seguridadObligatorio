$(document).ready(function () {
  controlarUsuarioLogeado();
});

//controlar usuario administrador
function controlarUsuarioLogeado() {
  var token = localStorage.getItem("token");
  if (token) {
    var tokenInfo = parsearJwt(token);
    var rol = tokenInfo.Role;
    if (rol != "Invitado") {
      localStorage.clear();
      window.location = "../Security/errorPage.html";
    }
  } else {
    localStorage.clear();
    window.location = "../Security/errorPage.html";
  }
}

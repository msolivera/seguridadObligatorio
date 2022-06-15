$(document).ready(function () {
    controlarUsuarioLogeado();
  });
  
  //controlar usuario administrador
  function controlarUsuarioLogeado() {
    var token = localStorage.getItem("token");
    if (token) {
      var tokenInfo = parsearJwt(token);
      var rol = tokenInfo.rol;
      console.log(rol);
      if (rol != "Invitado") {
        alert("No tiene permisos para acceder a esta página");
        localStorage.clear();
        window.location = "../login.html";
        
      }
    } else {
      alert("No tiene permisos para acceder a esta página");
      window.location = "../login.html";
      localStorage.clear();
    }
  }
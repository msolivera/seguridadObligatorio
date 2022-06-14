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
      if (rol != "user") {
       //window.location = "login.html";
        //localStorage.clear();
      }
    }else{
     //window.location = "login.html";
      //localStorage.clear();
    }
  }
  
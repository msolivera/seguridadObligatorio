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
      localStorage.clear();
      window.location = "login.html";
    }
  } else {
    window.location = "login.html";
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

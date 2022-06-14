using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seguridad.Models
{
    //Clase utilizada para registro y login.
    public class UserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

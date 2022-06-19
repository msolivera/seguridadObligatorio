using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seguridad.Models
{
    //Clase utilizada para cambio de contraseña.
    public class ResetPasswordDto
    {
        public string passwordActual { get; set; } = string.Empty;
        public string passwordNueva { get; set; } = string.Empty;
    }
}

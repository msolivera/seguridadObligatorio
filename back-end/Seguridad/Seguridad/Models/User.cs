using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seguridad.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public DateTime? JwtCreationTime { get; set; }
        public DateTime? JwtExpirationTime { get; set; }
    }
}

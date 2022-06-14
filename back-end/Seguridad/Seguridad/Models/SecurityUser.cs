using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seguridad.Models
{
    public class SecurityUser
    {
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}

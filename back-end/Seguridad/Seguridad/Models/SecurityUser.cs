using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seguridad.Models
{
    public class SecurityUser
    {
        public string Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime? TokenCreated { get; set; }
        public DateTime? TokenExpires { get; set; }
        public string RoleId { get; set; }
    }
}

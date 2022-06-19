using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seguridad.Models
{
    public class LoginLockout
    {
        public string Id { get; set; }
        public DateTime ExpirationTime { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}

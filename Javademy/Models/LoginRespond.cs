using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Javademy.Models
{
    class LoginRespond
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Token { get; internal set; }
    }
}

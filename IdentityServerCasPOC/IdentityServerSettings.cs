using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerCasPOC
{
    public class IdentityServerSettings
    {
        public string BaseAddress { get; set; }
        public string CasRequestUrl { get; set; }
        public string CasValidateUrl { get; set; }
        public string CasLogoutUrl { get; set; }
    }
}

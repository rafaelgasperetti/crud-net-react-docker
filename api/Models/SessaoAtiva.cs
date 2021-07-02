using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace api.Models
{
    public class SessaoAtiva
    {
        public string Token { get; set; }

        public string Login { get; set; }

        public DateTime DataAcesso { get; set; }

        private Claim[] Claims { get; set; }

        public void DefinirClaims(Claim[] claims)
        {
            Claims = claims;
        }

        public Claim[] ObterClaims()
        {
            return Claims;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace api.Models
{
    public class RequisicaoLogin
    {
        public Guid? ChaveApi { get; set; }

        public string Login { get; set; }

        public string Senha { get; set; }

        public Claim[] ObterClaims()
        {
            return new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Login),
                new Claim(ClaimTypes.NameIdentifier, Senha),
                new Claim(ClaimTypes.NameIdentifier, DateTime.Now.ToString()),
                new Claim(ClaimTypes.NameIdentifier, Helper.Helper.CHAVE_API.ToString())
            };
        }
    }
}

using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace api.jwt.Model
{
    public class JWTContainerModel : IAuthContainerModel
    {
        private static readonly int ExpiracaoTokenMinutos = 3600;

        public string SecretKey { get; } = "sPDfpVzshwWinpJKnf393w==";

        public string SecretAlgorithm { get; } = SecurityAlgorithms.HmacSha256Signature;

        public int ExpireMinutes { get; } = ExpiracaoTokenMinutos;

        public Claim[] Claims { get; set; }

        public static JWTContainerModel GetModel(Claim[] claims)
        {
            if (claims == null || claims.Length == 0)
            {
                throw new ArgumentException("Não é possível obter o modelo JWT com os argumentos fornecidos.");
            }

            return new JWTContainerModel()
            {
                Claims = claims
            };
        }
    }
}

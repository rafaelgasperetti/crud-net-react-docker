using api.jwt.Model;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace api.jwt.Manager
{
    public class JWTService : IAuthService
    {
        public string SecretKey { get; set; }

        public JWTService(string secretKey)
        {
            SecretKey = secretKey;
        }

        public string GenerateToken(IAuthContainerModel model)
        {
            if (model == null || model.Claims == null || model.Claims.Length == 0)
            {
                throw new ArgumentException("Argumentos inválidos para criação do token.");
            }

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(model.Claims),
                Expires = DateTime.Now.AddMinutes(model.ExpireMinutes),
                SigningCredentials = new SigningCredentials(GetSymmetricSecurityKey(), model.SecretAlgorithm)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            SecurityToken token = handler.CreateToken(descriptor);

            return handler.WriteToken(token);
        }

        public IEnumerable<Claim> GetTokenClaims(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("O token informado está nulo ou vazio.");
            }

            TokenValidationParameters parametros = GetTokenValidationParameters();
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            try
            {
                ClaimsPrincipal valido = handler.ValidateToken(token, parametros, out SecurityToken tokenValidado);
                return valido.Claims;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool IsTokenValid(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("O token informado está nulo ou vazio.");
            }

            TokenValidationParameters parametros = GetTokenValidationParameters();
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            try
            {
                ClaimsPrincipal valido = handler.ValidateToken(token, parametros, out SecurityToken tokenValidado);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private SecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Convert.FromBase64String(SecretKey));
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = GetSymmetricSecurityKey()
            };
        }
    }
}

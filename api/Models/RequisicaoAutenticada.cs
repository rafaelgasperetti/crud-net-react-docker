using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class RequisicaoAutenticada
    {
        private string token;

        public string Token { get { return token; } set { token = value ?? "O Token de uma requisição autorizada nunca pode ser nulo."; } }
    }
}

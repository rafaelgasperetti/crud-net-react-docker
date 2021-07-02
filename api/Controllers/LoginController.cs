using api.Business;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace api.Controllers
{
    public class LoginController : BaseController
    {
        [HttpPost]
        public Resposta EfetuarLogin([FromServices] IMemoryCache cache, [FromBody] RequisicaoLogin request)
        {
            return Autenticacao.EfetuarLogin(request, cache);
        }

        [HttpPost]
        public Resposta EfetuarLogoff([FromServices] IMemoryCache cache, [FromBody] RequisicaoAutenticada request)
        {
            Resposta sessao = ValidarSessao(request, cache);
            if (sessao.Codigo != (int)HttpStatusCode.OK)
            {
                return sessao;
            }

            return Autenticacao.EfetuarLogoff(request, cache);
        }
    }
}

using api.Business;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Cors;

namespace api.Controllers
{
    public class BaseController : ControllerBase
    {
        public static readonly string NomeServico = "DotNetCoreService";

        public T ReadRequest<T>(HttpRequestMessage request)
        {
            var body = request.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(body);
        }

        public static Resposta ValidarSessao(RequisicaoAutenticada requisicao, IMemoryCache cache)
        {
            Validacao<bool> validacaoSessao = Sessao.ValidarSessao(requisicao, cache);

            if (!validacaoSessao.Sucesso || !validacaoSessao.Retorno)
            {
                return new Resposta()
                {
                    Codigo = (int)HttpStatusCode.InternalServerError,
                    CodigoInterno = (int)validacaoSessao.Codigo,
                    Status = validacaoSessao.Descricao
                };
            }
            else
            {
                return new Resposta()
                {
                    Codigo = (int)HttpStatusCode.OK,
                    CodigoInterno = (int)validacaoSessao.Codigo,
                    Status = "OK"
                };
            }
        }
    }
}

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
    public class DadosController : BaseController
    {
        [HttpPost]
        public Resposta ObterDados([FromServices] IMemoryCache cache, [FromBody] DadosRequest request)
        {
            try
            {
                Resposta sessao = ValidarSessao(request, cache);

                if (sessao.Codigo != (int)HttpStatusCode.OK)
                {
                    return sessao;
                }

                if (request == null)
                {
                    return new Resposta()
                    {
                        Codigo = (int)HttpStatusCode.BadRequest,
                        CodigoInterno = (int)Helper.Helper.CodigoInterno.TabelaInexistente,
                        Status = "A requisição de dados está nula."
                    };
                }

                return GerenciamentoDados.ObterDados(request, false);
            }
            catch (Exception e)
            {
                return new Resposta()
                {
                    Codigo = (int)HttpStatusCode.InternalServerError,
                    CodigoInterno = (int)Helper.Helper.CodigoInterno.Catch_ObterDados,
                    Status = e.Message
                };
            }
        }

        [HttpPost]
        public Resposta ObterContagemDados([FromServices] IMemoryCache cache, [FromBody] DadosRequest request)
        {
            try
            {
                Resposta sessao = ValidarSessao(request, cache);

                if (sessao.Codigo != (int)HttpStatusCode.OK)
                {
                    return sessao;
                }

                if (request == null)
                {
                    return new Resposta()
                    {
                        Codigo = (int)HttpStatusCode.BadRequest,
                        CodigoInterno = (int)Helper.Helper.CodigoInterno.TabelaInexistente,
                        Status = "A requisição de dados está nula."
                    };
                }

                return GerenciamentoDados.ObterDados(request, true);
            }
            catch (Exception e)
            {
                return new Resposta()
                {
                    Codigo = (int)HttpStatusCode.InternalServerError,
                    CodigoInterno = (int)Helper.Helper.CodigoInterno.Catch_ObterDados,
                    Status = e.Message
                };
            }
        }

        [HttpPost]
        public Resposta EnviarDados([FromServices] IMemoryCache cache, [FromBody] EnvioDadosRequest request)
        {
            try
            {
                Resposta sessao = ValidarSessao(request, cache);

                if (sessao.Codigo != (int)HttpStatusCode.OK)
                {
                    return sessao;
                }

                if (request == null)
                {
                    return new Resposta()
                    {
                        Codigo = (int)HttpStatusCode.BadRequest,
                        CodigoInterno = (int)Helper.Helper.CodigoInterno.TabelaInexistente,
                        Status = "A requisição de dados está nula."
                    };
                }

                return GerenciamentoDados.EnviarDados(request);
            }
            catch (Exception e)
            {
                return new Resposta()
                {
                    Codigo = (int)HttpStatusCode.InternalServerError,
                    CodigoInterno = (int)Helper.Helper.CodigoInterno.Catch_EnviarDados,
                    Status = e.Message
                };
            }
        }

        [HttpPost]
        public Resposta ObterDadosTerceiros([FromServices] IMemoryCache cache, [FromBody] DadosRequest request)
        {
            try
            {
                Resposta sessao = ValidarSessao(request, cache);

                if (sessao.Codigo != (int)HttpStatusCode.OK)
                {
                    return sessao;
                }

                if (request == null)
                {
                    return new Resposta()
                    {
                        Codigo = (int)HttpStatusCode.BadRequest,
                        CodigoInterno = (int)Helper.Helper.CodigoInterno.TabelaInexistente,
                        Status = "A requisição de dados está nula."
                    };
                }

                return DadosTerceiros.ObterDados(request);
            }
            catch (Exception e)
            {
                return new Resposta()
                {
                    Codigo = (int)HttpStatusCode.InternalServerError,
                    CodigoInterno = (int)Helper.Helper.CodigoInterno.Catch_ObterDados,
                    Status = e.Message
                };
            }
        }
    }
}

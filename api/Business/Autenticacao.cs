using api.jwt.Manager;
using api.jwt.Model;
using api.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace api.Business
{
    public class Autenticacao
    {
        private static readonly string Login = "TesteBackEnd";
        private static readonly string Senha = "Teste123";

        public static Resposta EfetuarLogin(RequisicaoLogin requisicaoLogin, IMemoryCache cache)
        {
            try
            {
                Sessao.ExpirarSessoes(cache);
                Validacao<SessaoAtiva> validacaoLogin = ValidarLogin(requisicaoLogin, cache);

                if (!validacaoLogin.Sucesso)
                {
                    if (validacaoLogin.Retorno != null)
                    {
                        string msg = string.IsNullOrEmpty(validacaoLogin.DescricaoUsuario) ? validacaoLogin.Descricao : validacaoLogin.DescricaoUsuario;
                        string login = validacaoLogin.Retorno.Login ?? requisicaoLogin.Login;
                    }

                    return new Resposta()
                    {
                        Codigo = (int)HttpStatusCode.InternalServerError,
                        CodigoInterno = (int)Helper.Helper.CodigoInterno.Catch_EfetuarLogin,
                        Status = "Erro",
                        Data = validacaoLogin
                    };
                }

                Sessao.InicializarSessao(validacaoLogin.Retorno, cache);

                Resposta resposta = new Resposta()
                {
                    Codigo = validacaoLogin.Sucesso ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest,
                    CodigoInterno = (int)validacaoLogin.Codigo,
                    Status = validacaoLogin.Sucesso ? "OK" : "Erro",
                    Data = validacaoLogin
                };

                return resposta;
            }
            catch (Exception e)
            {
                return new Resposta()
                {
                    Codigo = (int)HttpStatusCode.InternalServerError,
                    CodigoInterno = (int)Helper.Helper.CodigoInterno.Catch_EfetuarLogin,
                    Status = "Erro",
                    Data = new Validacao<SessaoAtiva>()
                    {
                        Sucesso = false,
                        Codigo = Helper.Helper.CodigoInterno.Catch_EfetuarLogin,
                        Descricao = e.Message,
                        TentarNovamente = false
                    }
                };
            }
        }

        public static Validacao<SessaoAtiva> ValidarLogin(RequisicaoLogin requisicaoLogin, IMemoryCache cache)
        {
            try
            {
                if (requisicaoLogin == null)
                {
                    return new Validacao<SessaoAtiva>()
                    {
                        Sucesso = false,
                        Codigo = Helper.Helper.CodigoInterno.RequisicaoNula,
                        Descricao = "A requisição de login não pode ser nula.",
                        TentarNovamente = false
                    };
                }
                else if (requisicaoLogin.ChaveApi == null || requisicaoLogin.ChaveApi != Helper.Helper.CHAVE_API)
                {
                    return new Validacao<SessaoAtiva>()
                    {
                        Sucesso = false,
                        Codigo = Helper.Helper.CodigoInterno.ChaveAPIInvalida,
                        Descricao = "A chave da API não é válida.",
                        TentarNovamente = false
                    };
                }
                else if (string.IsNullOrEmpty(requisicaoLogin.Login) || string.IsNullOrEmpty(requisicaoLogin.Senha) || requisicaoLogin.Login != Login || requisicaoLogin.Senha != Senha)
                {
                    return new Validacao<SessaoAtiva>()
                    {
                        Sucesso = false,
                        Codigo = Helper.Helper.CodigoInterno.CredenciaisInvalidas,
                        Descricao = "As credenciais informadas não funcionaram.",
                        TentarNovamente = false
                    };
                }
                else
                {
                    SessaoAtiva sessao = Sessao.ObterSessaoAtiva(requisicaoLogin.Login, cache);

                    if (sessao != null)
                    {
                        sessao.DataAcesso = DateTime.Now;
                    }
                    else
                    {
                        IAuthContainerModel modelo = JWTContainerModel.GetModel(requisicaoLogin.ObterClaims());
                        IAuthService servico = new JWTService(modelo.SecretKey);

                        sessao = new SessaoAtiva()
                        {
                            Login = Login,
                            DataAcesso = DateTime.Now,
                            Token = servico.GenerateToken(modelo)
                        };

                        sessao.DefinirClaims(requisicaoLogin.ObterClaims());
                    }

                    return new Validacao<SessaoAtiva>()
                    {
                        Sucesso = true,
                        Codigo = Helper.Helper.CodigoInterno.Sucesso,
                        Descricao = "OK",
                        TentarNovamente = false,
                        Retorno = sessao
                    };
                }
            }
            catch (Exception e)
            {
                return new Validacao<SessaoAtiva>()
                {
                    Sucesso = false,
                    Codigo = Helper.Helper.CodigoInterno.Catch_ValidarLogin,
                    Descricao = e.Message,
                    TentarNovamente = false
                };
            }
        }

        public static Resposta EfetuarLogoff(RequisicaoAutenticada request, IMemoryCache cache)
        {
            Resposta resposta = new Resposta() { Codigo = (int)HttpStatusCode.OK, Status = "OK" };
            try
            {
                var sessao = Sessao.ObterSessaoAtiva(request.Token, cache);
                Sessao.FinalizarSessao(sessao, cache);
            }
            catch (Exception ex)
            {
                resposta.Codigo = (int)HttpStatusCode.InternalServerError;
                resposta.Status = ex.Message;
            }

            return resposta;
        }
    }
}

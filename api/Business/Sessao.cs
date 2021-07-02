using api.Database;
using api.jwt.Manager;
using api.jwt.Model;
using api.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace api.Business
{
    public class Sessao
    {
        public static readonly int ExpiracaoSessaoMinutos = 60;
        public static readonly string TOKEN_TAG = "TOKEN";

        private static string ObterTokenCacheString(string token)
        {
            return string.Format("{0}_{1}", TOKEN_TAG, token);
        }

        public static void ExpirarSessoes(IMemoryCache cache)
        {
            var sessoes = ObterSessoes(cache);
            sessoes.RemoveAll(l => l.DataAcesso < DateTime.Now.AddMinutes(ExpiracaoSessaoMinutos * -1));
        }

        private static List<KeyValuePair<string, object>> LerCache(IMemoryCache cache)
        {
            var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
            var items = new List<KeyValuePair<string, object>>();

            if (field.GetValue(cache) is ICollection collection)
            {
                foreach (var item in collection)
                {
                    var methodInfo = item.GetType().GetProperty("Key");
                    var key = methodInfo.GetValue(item);

                    methodInfo = item.GetType().GetProperty("Value");
                    var value = methodInfo.GetValue(item);

                    methodInfo = value.GetType().GetProperty("Value");
                    var data = methodInfo.GetValue(value);

                    items.Add(new KeyValuePair<string, object>(key.ToString(), data));
                }
            }

            return items;
        }

        public static List<SessaoAtiva> ObterSessoes(IMemoryCache cache)
        {
            return LerCache(cache).Where(s => s.Value is SessaoAtiva).Select(s => s.Value as SessaoAtiva).ToList();
        }

        public static SessaoAtiva ObterSessaoAtiva(string token, IMemoryCache cache)
        {
            var objSessao = ObterSessoes(cache).FirstOrDefault(l => l.Token == token && l.DataAcesso >= DateTime.Now.AddMinutes(ExpiracaoSessaoMinutos * -1));

            if (objSessao != null)
            {
                objSessao.DataAcesso = DateTime.Now;
            }

            return objSessao;
        }

        public static SessaoAtiva ObterSessaoLogin(SessaoAtiva login, IMemoryCache cache)
        {
            var objSessao = ObterSessoes(cache).FirstOrDefault(l => l.Login == login.Login && l.DataAcesso >= DateTime.Now.AddMinutes(ExpiracaoSessaoMinutos * -1));

            if (objSessao != null)
            {
                objSessao.DataAcesso = DateTime.Now;
                login.Token = objSessao.Token;
            }

            return objSessao;
        }

        public static SessaoAtiva ObterSessao(string token, IMemoryCache cache)
        {
            var sessao = cache.Get<SessaoAtiva>(ObterTokenCacheString(token));

            if (sessao == null)
            {
                throw new InvalidOperationException("Nenhuma sessão encontrada.");
            }

            if (sessao.DataAcesso < DateTime.Now.AddMinutes(ExpiracaoSessaoMinutos * -1))
            {
                throw new InvalidOperationException("Sessão expirada.");
            }

            return sessao;
        }

        public static void InicializarSessao(SessaoAtiva sessao, IMemoryCache cache)
        {
            SessaoAtiva sessaoExistente = ObterSessaoLogin(sessao, cache);

            if (sessaoExistente == null)
            {
                cache.Set(ObterTokenCacheString(sessao.Token), sessao);
            }
        }

        public static void FinalizarSessao(SessaoAtiva login, IMemoryCache cache)
        {
            cache.Remove(ObterTokenCacheString(login.Token));
        }

        public static void DefinirCache(string chave, object valor, IMemoryCache cache, int? expiracaoMinutos = null)
        {
            if (expiracaoMinutos != null)
            {
                cache.Set(chave, valor, new TimeSpan(0, expiracaoMinutos.Value, 0));
            }
            else
            {
                cache.Set(chave, valor);
            }
        }

        public static object ObterValorCache(string chave, IMemoryCache cache)
        {
            return cache.Get(chave);
        }
        public static T ObterValorCacheT<T>(string chave, IMemoryCache cache)
        {
            return (T)cache.Get(chave);
        }

        public static void RemoverCache(string chave, IMemoryCache cache)
        {
            cache.Remove(chave);
        }

        public static BancoDados ObterConexao()
        {
            return new BancoDados();
        }

        public static bool TokenValido(SessaoAtiva sessao)
        {
            try
            {
                IAuthContainerModel modelo = JWTContainerModel.GetModel(sessao.ObterClaims());
                IAuthService servico = new JWTService(modelo.SecretKey);
                return servico.IsTokenValid(sessao.Token);
            }
            catch
            {
                throw new InvalidOperationException("Não foi possível validar o token.");
            }
        }

        public static Validacao<bool> ValidarSessao(RequisicaoAutenticada requisicao, IMemoryCache cache)
        {
            try
            {
                if (requisicao == null)
                {
                    return new Validacao<bool>()
                    {
                        Codigo = Helper.Helper.CodigoInterno.ValidarSessao_SessaoInvalida,
                        Retorno = false,
                        Descricao = "Requisição nula inválida.",
                        Sucesso = true,
                        TentarNovamente = false
                    };
                }

                var sessao = ObterSessao(requisicao.Token, cache);

                if (!TokenValido(sessao))
                {
                    return new Validacao<bool>()
                    {
                        Codigo = Helper.Helper.CodigoInterno.ValidarSessao_SessaoInvalida,
                        Retorno = false,
                        Descricao = "A sessão informada não é válida.",
                        Sucesso = true,
                        TentarNovamente = false
                    };
                }
                else
                {
                    return new Validacao<bool>()
                    {
                        Codigo = Helper.Helper.CodigoInterno.Sucesso,
                        Retorno = true,
                        Descricao = "OK",
                        Sucesso = true,
                        TentarNovamente = false
                    };
                }
            }
            catch (Exception e)
            {
                return new Validacao<bool>()
                {
                    Codigo = Helper.Helper.CodigoInterno.ValidarSessao_SessaoInvalida,
                    Retorno = false,
                    Descricao = e.Message,
                    Sucesso = false,
                    TentarNovamente = false
                };
            }
        }
    }
}

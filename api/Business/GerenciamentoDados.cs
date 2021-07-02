using api.Database;
using api.Helper;
using api.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace api.Business
{
    public class GerenciamentoDados
    {
        private static readonly string BaseModelFolder = "api.Models";

        public enum OperacaoEnvioDados
        {
            Nenhuma = 0,
            InsertOrUpdate = 1,
            Insert = 2,
            Update = 3,
            Delete = 4
        }

        public static Resposta ObterDados(DadosRequest dadosRequest, bool apenasContagem)
        {
            try
            {
                Type tipo = null;
                try
                {
                    string nomeModelo = string.Format("{0}.{1}.{2}", BaseModelFolder, dadosRequest.Esquema, dadosRequest.Tabela);
                    tipo = Type.GetType(nomeModelo, false, false);

                    if (tipo == null)
                    {
                        return new Resposta()
                        {
                            Codigo = (int)HttpStatusCode.BadRequest,
                            CodigoInterno = (int)Helper.Helper.CodigoInterno.ModeloInexistente,
                            Status = string.Format("Modelo de dados \"{0}\" não encontrado.", dadosRequest.Tabela)
                        };
                    }
                }
                catch (Exception e)
                {
                    return new Resposta()
                    {
                        Codigo = (int)HttpStatusCode.BadRequest,
                        CodigoInterno = (int)Helper.Helper.CodigoInterno.ModeloInexistente,
                        Status = string.Format("Não foi possível criar o modelo de dados \"{0}\". Motivo: {1}", dadosRequest.Tabela, e.Message)
                    };
                }

                int? resultadosPorPagina = dadosRequest.Parametros?.Where(p => p.Nome == "ResultadosPorPaginaSQL").FirstOrDefault()?.ObterValorInteiroNullable();
                var dados = ObterDadosGerenciados(tipo, null, dadosRequest.Parametros, apenasContagem);

                return new Resposta()
                {
                    Codigo = (int)HttpStatusCode.OK,
                    CodigoInterno = (int)Helper.Helper.CodigoInterno.Sucesso,
                    Status = "OK",
                    Data = dados
                };
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

        public static ResultadoConsulta ObterDadosGerenciados(Type t, string sql, List<ParametroSQL> parametros, bool apenasContagem)
        {
            GerenciamentoDados dados = new GerenciamentoDados();
            return dados.ObterDadosInterno(t, sql, parametros, apenasContagem);
        }

        public ResultadoConsulta ObterDadosInterno(Type t, string sql, List<ParametroSQL> parametros, bool apenasContagem)
        {
            MethodInfo method = GetType().GetMethod("ObterDadosBase").MakeGenericMethod(t);

            try
            {
                var retorno = method.Invoke(this, new object[] { sql, parametros, apenasContagem });

                if (retorno != null && typeof(ResultadoConsulta) == retorno.GetType())
                {
                    return (ResultadoConsulta)retorno;
                }
                else
                {
                    return new ResultadoConsulta()
                    {
                        Dados = new List<ModeloBase>(),
                        Paginas = 1,
                        QuantidadeRegistrosTotal = 0,
                        QuantidadeRegistrosRetornados = 0,
                        ResultadosPorPagina = 0
                    };
                }
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static List<ParametroSQL> ObterParametrosSessao()
        {
            return new List<ParametroSQL>()
            {
                new ParametroSQL("DataAtual", DateTime.Now)
            };
        }

        public ResultadoConsulta ObterDadosBase<T>(string sql, List<ParametroSQL> parametrosRecebidos, bool apenasContagem)
            where T : ModeloBase, new()
        {
            DateTime inicio = DateTime.Now;
            try
            {
                using BancoDados db = Sessao.ObterConexao();
                T tipo = new T();

                parametrosRecebidos ??= new List<ParametroSQL>();
                var parametros = new List<ParametroSQL>();

                List<ModeloBase> dados = new List<ModeloBase>();
                List<ParametroEsperado> parametrosEsperados = tipo.ParametrosEsperadosSelect() ?? new List<ParametroEsperado>();
                List<ParametroSQL> parametrosSessao = ObterParametrosSessao();

                if (parametrosEsperados != null)
                {
                    foreach (var parametro in parametrosEsperados.Where(pe => !parametros.Any(p => p.Nome == pe.Nome)))
                    {
                        object valor = ParametroSQL.ObterValor(parametrosSessao, parametro.Nome) ?? ParametroSQL.ObterValor(parametrosRecebidos, parametro.Nome);

                        parametros.Add(new ParametroSQL(parametro.Nome, valor));
                    }

                    if (parametrosEsperados.Any(p => p.Obrigatorio))
                    {
                        if (parametrosEsperados.Any(p => !parametros.Exists(p2 => p2.Nome == p.Nome) && p.Obrigatorio))
                        {
                            throw new ArgumentException(string.Format("A busca de informações com o modelo da tabela \"{0}\" posssui parâmetros obrigatórios que não foram prenchidos.", tipo.GetType().Name));
                        }
                    }
                }

                int registros = 0;
                int? pagina = null;
                int? resultadosPorPagina = null;
                int? limite = null;
                bool ordemAleatoria = false;

                if (!string.IsNullOrEmpty(sql))
                {
                    registros = (int)db.ExecutarComandoEscalar(ObterQueryCount(sql), parametros);
                    parametros = tipo.InterceptarValores(parametros);
                    dados = db.SelectModeloBase<T>(sql, parametros);
                }
                else
                {
                    pagina = parametrosRecebidos.FirstOrDefault(p => p.Nome == "PaginaSQL")?.ObterValorInteiroNullable();
                    resultadosPorPagina = parametrosRecebidos.FirstOrDefault(p => p.Nome == "ResultadosPorPaginaSQL")?.ObterValorInteiroNullable();
                    limite = parametrosRecebidos.FirstOrDefault(p => p.Nome == "LimiteSQL")?.ObterValorInteiroNullable();
                    ordemAleatoria = parametrosRecebidos.FirstOrDefault(p => p.Nome == "OrdemAleatoriaSQL")?.ObterValorBooleano() ?? false;

                    string query = apenasContagem ? null : tipo.ObterQuery(parametros, pagina, resultadosPorPagina, limite, ordemAleatoria);
                    string queryCount = ObterQueryCount(tipo.ObterQuery(parametros, null, null, limite, ordemAleatoria));
                    parametros = tipo.InterceptarValores(parametros);

                    if (parametros.Any(p => !parametrosEsperados.Any(p2 => p.Nome == p2.Nome)))
                    {
                        throw new ArgumentException(string.Format("A busca de informações com o modelo da tabela \"{0}\" posssui parâmetros não permitidos.", tipo.GetType().Name));
                    }

                    if (!apenasContagem)
                    {
                        if (string.IsNullOrEmpty(query))
                        {
                            dados = db.SelectModeloBase<T>(parametros);
                            registros = dados.Count;
                        }
                        else
                        {
                            dados = db.SelectModeloBase<T>(query, parametros);
                            registros = (int)db.ExecutarComandoEscalar(queryCount, parametros);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(queryCount))
                        {
                            dados = db.SelectModeloBase<T>(parametros);
                            registros = dados.Count;
                            dados.Clear();
                        }
                        else
                        {
                            registros = (int)db.ExecutarComandoEscalar(queryCount, parametros);
                        }
                    }
                }

                return new ResultadoConsulta()
                {
                    Dados = dados,
                    QuantidadeRegistrosTotal = registros,
                    QuantidadeRegistrosRetornados = dados.Count,
                    Paginas = (resultadosPorPagina == null || resultadosPorPagina == 0) ? 1 : registros / resultadosPorPagina.Value,
                    ResultadosPorPagina = (resultadosPorPagina == null || resultadosPorPagina == 0) ? registros : resultadosPorPagina.Value
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private string ObterQueryCount(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return null;
            }

            int idxOrderBy = SQLParser.GetOrderByIndex(query);

            if (idxOrderBy > 0)
            {
                query = query.Substring(0, idxOrderBy);
            }

            query = query.Trim();
            query = "SELECT COUNT(*) FROM (" + query + ") count";

            return query;
        }

        public static Resposta EnviarDados(EnvioDadosRequest dadosRequest)
        {
            try
            {
                Type tipo = null;
                try
                {
                    string nomeModelo = string.Format("{0}.{1}.{2}", BaseModelFolder, dadosRequest.Esquema, dadosRequest.Tabela);
                    tipo = Type.GetType(nomeModelo, false, false);

                    if (tipo == null)
                    {
                        return new Resposta()
                        {
                            Codigo = (int)HttpStatusCode.BadRequest,
                            CodigoInterno = (int)Helper.Helper.CodigoInterno.ModeloInexistente,
                            Status = string.Format("Modelo de dados \"{0}\" não encontrado.", dadosRequest.Tabela)
                        };
                    }
                }
                catch (Exception e)
                {
                    return new Resposta()
                    {
                        Codigo = (int)HttpStatusCode.BadRequest,
                        CodigoInterno = (int)Helper.Helper.CodigoInterno.ModeloInexistente,
                        Status = string.Format("Não foi possível criar o modelo de dados \"{0}\". Motivo: {1}", dadosRequest.Tabela, e.Message)
                    };
                }

                OperacaoEnvioDados operacao = OperacaoEnvioDados.Nenhuma;

                Enum.TryParse(dadosRequest.Operacao.ToString(), out operacao);

                if (operacao == OperacaoEnvioDados.Nenhuma)
                {
                    return new Resposta()
                    {
                        Codigo = (int)HttpStatusCode.InternalServerError,
                        CodigoInterno = (int)Helper.Helper.CodigoInterno.OperacaoInexistente,
                        Status = string.Format("Operação \"{0}\" inexistente.", dadosRequest.Operacao)
                    };
                }

                var dados = EnviarDadosGerenciados(tipo, dadosRequest.Data.ToString(), operacao);

                return new Resposta()
                {
                    Codigo = (int)HttpStatusCode.OK,
                    CodigoInterno = (int)Helper.Helper.CodigoInterno.Sucesso,
                    Status = "OK",
                    Data = dados
                };
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

        public static int EnviarDadosGerenciados(Type t, string data, OperacaoEnvioDados operacao)
        {
            GerenciamentoDados dados = new GerenciamentoDados();
            return dados.EnviarDadosInterno(t, data, operacao);
        }

        public int EnviarDadosInterno(Type t, string data, OperacaoEnvioDados operacao)
        {
            MethodInfo method = GetType().GetMethod("EnviarDadosBase").MakeGenericMethod(t);

            try
            {
                var retorno = method.Invoke(this, new object[] { data, operacao });

                if (retorno != null && typeof(int) == retorno.GetType())
                {
                    return (int)retorno;
                }
                else
                {
                    return 0;
                }
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }
                else
                {
                    throw ex;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int EnviarDadosBase<T>(string data, OperacaoEnvioDados operacao)
            where T : ModeloBase, new()
        {
            try
            {

                using BancoDados db = Sessao.ObterConexao();

                T tipo = JsonConvert.DeserializeObject<T>(data);

                if (operacao == OperacaoEnvioDados.Insert || operacao == OperacaoEnvioDados.InsertOrUpdate || operacao == OperacaoEnvioDados.Update)
                {
                    Validacao<bool> validacao = tipo.ValidarParametros();

                    if (!validacao.Sucesso)
                    {
                        throw new InvalidOperationException(validacao.Descricao);
                    }
                }

                int result = 0;

                if (operacao == OperacaoEnvioDados.InsertOrUpdate || operacao == OperacaoEnvioDados.Update)
                {
                    result = db.Update(tipo);

                    if (operacao == OperacaoEnvioDados.InsertOrUpdate && result == 0)
                    {
                        result = db.Insert(tipo);
                    }
                }
                else if (operacao == OperacaoEnvioDados.Insert)
                {
                    result = db.Insert(tipo);
                }
                else if (operacao == OperacaoEnvioDados.Delete)
                {
                    result = db.Delete(tipo);
                }

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

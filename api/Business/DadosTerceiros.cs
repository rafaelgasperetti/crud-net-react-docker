using api.Helper;
using api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace api.Business
{
    public class DadosTerceiros
    {
        public static Resposta ObterDados(DadosRequest dadosRequest)
        {
            try
            {
                Type tipo = null;
                try
                {
                    string nomeModelo = string.Format("{0}.{1}.{2}", GerenciamentoDados.BaseModelFolder, dadosRequest.Esquema, dadosRequest.Tabela);
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

                DadosTerceiros dadosTerceiros = new();
                ResultadoConsulta dados = dadosTerceiros.ObterDadosTerceirosInterno(tipo);

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

        public ResultadoConsulta ObterDadosTerceirosInterno(Type t)
        {
            var instance = Activator.CreateInstance(t);
            string url = t.GetMethod("LinkConsultaTerceiros").Invoke(instance, null).ToString();
            string localMsg = t.GetMethod("RetornoMensagemTerceiros").Invoke(instance, null).ToString();
            string localDados = t.GetMethod("RetornoDadosTerceiros").Invoke(instance, null).ToString();
            List<Tuple<string, string>> conversor = t.GetMethod("Conversor").Invoke(instance, null) as List<Tuple<string, string>>;

            dynamic retorno = WSClient.WSCall(url, WSClient.Metodo.GET, null, null, null, null);
            JObject jsonData = JsonConvert.DeserializeObject<JObject>(retorno.Data);

            foreach (JProperty property in jsonData.Properties())
            {
                if(property.Name == localMsg && !property.Value.ToString().ToLower().Contains("sucesso"))
                {
                    throw new InvalidOperationException(string.Format("Operação de consulta em Web Service de terceiros não concluída com sucesso em {0}. Motivo: {1}", t.Name, property.Value.ToString()));
                }
                else if(property.Name == localDados)
                {
                    List<ModeloBase> dados = GetType().GetMethod("Converter").MakeGenericMethod(t).Invoke(this, new object[] { property.Value, conversor }) as List<ModeloBase>;
                    int registros = dados.Count;

                    return new ResultadoConsulta()
                    {
                        Dados = dados,
                        QuantidadeRegistrosTotal = registros,
                        QuantidadeRegistrosRetornados = registros,
                        Paginas = 1,
                        ResultadosPorPagina = registros
                    };
                }
            }

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

        public List<ModeloBase> Converter<T>(JToken dados, List<Tuple<string, string>> conversor)
            where T : ModeloBase, new()
        {
            List<ModeloBase> resultado = new();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach(JObject item in dados.Children())
            {
                resultado.Add(ConverterItem<T>(item, conversor, assemblies));
            }

            return resultado;
        }

        public T ConverterItem<T>(JObject item, List<Tuple<string, string>> conversor, Assembly[] assemblies)
            where T : ModeloBase, new()
        {
            T result = new();

            foreach(JProperty data in item.Children())
            {
                foreach (Tuple<string, string> convItem in conversor)
                {
                    if(convItem.Item1 == data.Name)
                    {
                        DefinirValorItem<T>(result, result.GetType().GetProperty(convItem.Item2), data.Value, assemblies);
                        continue;
                    }
                }
            }

            return result;
        }

        public void DefinirValorItem<T>(T item, PropertyInfo property, JToken valor, Assembly[] assemblies)
        {
            Type type = null;
            foreach(Assembly assembly in assemblies)
            {
                type = assembly.GetTypes().FirstOrDefault(i => i.Name == property.PropertyType.Name);

                if(type != null)
                {
                    break;
                }
            }

            if(type == null)
            {
                throw new InvalidOperationException(string.Format("Tipo de dados {0} não reconhecido entre os tipos disponíveis.", property.PropertyType.Name));
            }

            property.SetValue(item, valor.ToObject(type));
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helper
{
    public class ParametroSQL
    {
        public string Nome { get; set; }

        private object valor;

        public object Valor { get { return valor; } set { valor = value ?? DBNull.Value; } }

        public ParametroSQL()
        {

        }

        public ParametroSQL(string nome, object valor)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentNullException(nameof(ParametroSQL) + "." + nameof(nome));

            this.Nome = nome;
            this.Valor = valor ?? DBNull.Value;
        }

        public override string ToString()
        {
            return string.Format("{{ Nome = {0}; Valor = {1} }}", this.Nome, this.Valor);
        }

        public bool ObterValorBooleano()
        {
            if (Valor == DBNull.Value || Valor == null)
            {
                return false;
            }

            return Convert.ToBoolean(Valor);
        }

        public bool? ObterValorBooleanoNullable()
        {
            if (Valor == DBNull.Value || Valor == null)
            {
                return null;
            }
            else
            {
                return Convert.ToBoolean(Valor);
            }
        }

        public int ObterValorInteiro()
        {
            if (Valor == DBNull.Value || Valor == null)
            {
                return 0;
            }

            return (int)Valor;
        }

        public int? ObterValorInteiroNullable()
        {
            if (Valor == DBNull.Value || Valor == null)
            {
                return null;
            }
            else
            {
                return Convert.ToInt32(Valor);
            }
        }

        public int ObterValorShort()
        {
            if (Valor == DBNull.Value || Valor == null)
            {
                return 0;
            }

            return (short)Valor;
        }

        public short? ObterValorShortNullable()
        {
            if (Valor == DBNull.Value || Valor == null)
            {
                return null;
            }
            else
            {
                return Convert.ToInt16(Valor);
            }
        }

        public decimal ObterValorDecimal()
        {
            if (Valor == DBNull.Value || Valor == null)
            {
                return 0;
            }

            return (decimal)Valor;
        }

        public decimal? ObterValorDecimalNullable()
        {
            if (Valor == DBNull.Value || Valor == null)
            {
                return null;
            }
            else
            {
                return Convert.ToDecimal(Valor);
            }
        }

        public DateTime ObterValorDateTime()
        {
            if (Valor == DBNull.Value || Valor == null)
            {
                return DateTime.MinValue;
            }
            else
            {
                return (DateTime)Valor;
            }
        }

        public DateTime? ObterValorDateTimeNullable()
        {
            if (Valor == DBNull.Value || Valor == null)
            {
                return null;
            }
            else
            {
                return Convert.ToDateTime(Valor);
            }
        }

        public Guid ObterValorGuid()
        {
            if (Valor == DBNull.Value || Valor == null)
            {
                return Guid.Empty;
            }

            return (Guid)Valor;
        }

        public Guid? ObterValorGuidNullable()
        {
            if (Valor == DBNull.Value || Valor == null)
            {
                return null;
            }
            else
            {
                return Guid.Parse(Valor.ToString());
            }
        }

        public bool ValorNulo()
        {
            return Valor == null || Valor == DBNull.Value;
        }

        public static bool ValorNulo(ParametroSQL parametro)
        {
            return parametro?.ValorNulo() ?? true;
        }

        public static string ListToJson(List<ParametroSQL> parametros)
        {
            if (parametros == null)
            {
                return null;
            }

            return JsonConvert.SerializeObject(parametros);
        }

        public static object ObterValor(List<ParametroSQL> list, string nome)
        {
            return list.FirstOrDefault(p => p.Nome == nome)?.Valor;
        }

        public static string GetDeclareValues(List<ParametroSQL> list)
        {
            string declares = string.Empty;

            foreach (var parametro in list)
            {
                object valor = parametro.Valor;
                string tipoSQL = string.Empty;
                string valorSQL = string.Empty;

                if (valor == null)
                {
                    tipoSQL = "NVARCHAR(1)";
                    valorSQL = "NULL";
                }
                else
                {
                    if (valor is string || valor is char)
                    {
                        tipoSQL = "NVARCHAR(" + valor.ToString().Length + ")";
                        valorSQL = "'" + valor.ToString() + "'";
                    }
                    else if (valor is bool)
                    {
                        tipoSQL = "BIT";
                        valorSQL = Convert.ToBoolean(valor) ? "1" : "0";
                    }
                    else
                    {
                        if (valor is int)
                        {
                            tipoSQL = "INT";
                        }
                        else if (valor is float)
                        {
                            tipoSQL = "FLOAT";
                        }
                        else if (valor is decimal)
                        {
                            tipoSQL = "DECIMAL(18,6)";
                        }
                        else if (valor is short)
                        {
                            tipoSQL = "SMALLINT";
                        }
                        else if (valor is long)
                        {
                            tipoSQL = "BIGINT";
                        }
                        else if (valor is byte)
                        {
                            tipoSQL = "TINYINT";
                        }
                        else
                        {
                            tipoSQL = "NVARCHAR(MAX)";
                        }

                        if (tipoSQL == "NVARCHAR(MAX)")
                        {
                            valorSQL = "'" + valor.ToString() + "'";
                        }
                        else
                        {
                            valorSQL = valor.ToString();
                        }
                    }
                }

                declares += (string.IsNullOrEmpty(declares) ? "" : Environment.NewLine) + "DECLARE @" + parametro.Nome + " " + tipoSQL + " = " + valorSQL;
            }

            return declares;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace api.Database
{
    public class MapeamentoObjeto
    {
        public string Tabela { get; set; }
        public string Esquema { get; set; }

        public string NomeCompletoTabela
        {
            get { return string.IsNullOrEmpty(this.Esquema) ? this.Tabela : string.Format("{0}.{1}", this.Esquema, this.Tabela); }
        }

        public List<string> ColunasPK = new List<string>();

        public List<string> ColunasIdentity = new List<string>();

        public Dictionary<string, PropertyInfo> PropriedadesColuna { get; set; } = new Dictionary<string, PropertyInfo>();

        public List<string> Colunas = new List<string>();
    }
}

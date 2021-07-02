using api.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class DadosRequest : RequisicaoAutenticada
    {
        private string esquema;

        public string Esquema { get { return esquema; } set { esquema = value ?? "dbo"; } }

        private string tabela;

        public string Tabela { get { return tabela; } set { tabela = value ?? throw new ArgumentException("A tabela não pode ser nula"); } }

        public List<ParametroSQL> Parametros { get; set; }

        public long? VersaoTracking;
    }
}

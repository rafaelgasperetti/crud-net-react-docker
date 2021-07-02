using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static api.Business.GerenciamentoDados;

namespace api.Models
{
    public class EnvioDadosRequest : RequisicaoAutenticada
    {
        private string esquema;

        public string Esquema { get { return esquema; } set { esquema = value ?? "dbo"; } }

        private string tabela;

        public string Tabela { get { return tabela; } set { tabela = value ?? throw new ArgumentException("A tabela não pode ser nula"); } }

        public int Operacao { get; set; } = (int)OperacaoEnvioDados.Nenhuma;

        public object Data { get; set; }
    }
}

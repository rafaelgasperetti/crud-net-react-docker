using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class ParametroEsperado
    {
        private string nome;

        public string Nome { get { return nome; } set { nome = value ?? throw new ArgumentException("Não é permitido um parâmetro esperado sem um nome."); } }

        public bool Obrigatorio { get; set; }

        public ParametroEsperado(string nome)
        {
            Nome = nome;
            Obrigatorio = false;
        }

        public ParametroEsperado(string nome, bool obrigatorio)
        {
            Nome = nome;
            Obrigatorio = obrigatorio;
        }

        public override string ToString()
        {
            return string.Format("Nome: {0}, Obrigatorio: {1}", Nome, Obrigatorio);
        }
    }
}

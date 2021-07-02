using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static api.Helper.Helper;

namespace api.Models
{
    public class Validacao<T>
    {
        public bool Sucesso { get; set; }

        public CodigoInterno Codigo { get; set; }

        public string Descricao { get; set; }

        public bool TentarNovamente { get; set; }

        public T Retorno { get; set; }

        public string DescricaoUsuario { get; set; }
    }
}

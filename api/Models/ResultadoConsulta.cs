using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class ResultadoConsulta
    {
        public ResultadoConsulta()
        {

        }

        public int QuantidadeRegistrosTotal { get; set; }

        public int QuantidadeRegistrosRetornados { get; set; }

        public int Paginas { get; set; }

        public int ResultadosPorPagina { get; set; }

        public List<ModeloBase> Dados { get; set; }
    }
}

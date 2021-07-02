using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Resposta
    {
        public Resposta()
        {

        }

        public int Codigo { get; set; }

        public int CodigoInterno { get; set; }

        public string Status { get; set; }

        public object DataResposta { get; set; } = DateTime.Now;

        public object Data { get; set; }
    }
}

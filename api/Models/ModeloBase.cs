using api.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public abstract class ModeloBase
    {
        public virtual List<ParametroSQL> InterceptarValores(List<ParametroSQL> parametrosOriginais)
        {
            return parametrosOriginais;
        }

        public virtual Validacao<bool> ValidarParametros()
        {
            return new Validacao<bool>()
            {
                Codigo = Helper.Helper.CodigoInterno.Sucesso,
                Descricao = "Validado com sucesso",
                Retorno = true,
                Sucesso = true,
                TentarNovamente = false
            };
        }

        public abstract string ObterQuery(List<ParametroSQL> parametros = null, int? pagina = null, int? resultadosPorPagina = null, int? limite = null, bool ordemAleatoria = false);

        public abstract List<ParametroEsperado> ParametrosEsperadosSelect();
    }
}

using api.Database;
using api.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.dbo
{
    [Table("Produto")]
    [Schema("dbo")]
    public class Produto : ModeloBase
    {
        [PrimaryKey]
        [AutoIncrement]
        public int? Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }

        public override string ObterQuery(List<ParametroSQL> parametros = null, int? pagina = null, int? resultadosPorPagina = null, int? limite = null, bool ordemAleatoria = false)
        {
            string sql =  @"SELECT  *
                            FROM    dbo.Produto";

            bool primeiroFiltro = true;

            if (!ParametroSQL.ValorNulo(parametros.FirstOrDefault(p => p.Nome == "IdProduto")))
            {
                sql += Environment.NewLine + (primeiroFiltro ? "WHERE" : "AND") + " Produto.Id = @idProduto";
                primeiroFiltro = false;
            }

            if (!ParametroSQL.ValorNulo(parametros.FirstOrDefault(p => p.Nome == "Nome")) && !ParametroSQL.ValorNulo(parametros.FirstOrDefault(p => p.Nome == "Descricao")))
            {
                sql += Environment.NewLine + (primeiroFiltro ? "WHERE" : "AND") + " (Produto.Nome LIKE '%' + @Nome + '%' OR Produto.Descricao LIKE '%' + @Descricao + '%')";
                primeiroFiltro = false;
            }
            else if (!ParametroSQL.ValorNulo(parametros.FirstOrDefault(p => p.Nome == "Nome")))
            {
                sql += Environment.NewLine + (primeiroFiltro ? "WHERE" : "AND") + " Produto.Nome '%' + @Nome + '%'";
                primeiroFiltro = false;
            }
            else if (!ParametroSQL.ValorNulo(parametros.FirstOrDefault(p => p.Nome == "Descricao")))
            {
                sql += Environment.NewLine + (primeiroFiltro ? "WHERE" : "AND") + " Produto.Descricao '%' + @Descricao + '%'";
                primeiroFiltro = false;
            }

            return sql;
        }

        public override List<ParametroEsperado> ParametrosEsperadosSelect()
        {
            return new List<ParametroEsperado>()
            {
                new ParametroEsperado("IdProduto", false),
                new ParametroEsperado("Nome", false),
                new ParametroEsperado("Descricao", false)
            };
        }
    }
}

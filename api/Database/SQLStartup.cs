using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace api.Database
{
    public class SQLStartup
    {
        public static void ConfigureDatabase()
        {
            string conStr = "Data Source={0};Initial Catalog={1};User ID={2};Password={3};Application Name={4}";
            conStr = string.Format(conStr, BancoDados.URL, "master", BancoDados.Login, BancoDados.Senha, BancoDados.Application);

            using SqlConnection connection = new(conStr);
            connection.Open();

            using SqlCommand cmd = new();
            cmd.Connection = connection;

            string sql = string.Format(@"IF NOT EXISTS(SELECT 1 FROM sys.databases WHERE name = 'CrudCoreReactDocker')
                                        BEGIN
	                                        CREATE DATABASE {0}
                                        END", BancoDados.DB);

            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();

            sql = string.Format(@"IF NOT EXISTS(SELECT 1 FROM {0}.sys.tables WHERE name = 'Produto')
                                BEGIN
	                                CREATE TABLE {0}.dbo.Produto
	                                (
		                                Id INT NOT NULL IDENTITY CONSTRAINT PK_Produto PRIMARY KEY,
		                                Nome VARCHAR(100) NOT NULL,
		                                Descricao VARCHAR(200) NOT NULL,
		                                Valor MONEY NOT NULL CONSTRAINT CK_Valor_Produto CHECK(Valor > 0)
	                                )
                                END", BancoDados.DB);

            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();

            cmd.Dispose();
            connection.Close();
            connection.Dispose();
        }
    }
}

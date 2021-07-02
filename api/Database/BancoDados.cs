using api.Helper;
using api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace api.Database
{
    public class BancoDados : IDisposable
    {
        public static readonly string URL = "192.168.0.201";
        public static readonly string DB = "CrudCoreReactDocker";
        public static readonly string Login = "sa";
        public static readonly string Senha = "SenhaComplexa_12";
        public static readonly string Application = ".NetCoreBackEnd";

        public static readonly int TimeoutComandos = 60;
        public static readonly int ExpiracaoEmpresaMinutos = 60;

        private SqlConnection Conexao { get; set; }
        private SqlTransaction Transacao;

        public enum TipoOperacao
        {
            Select,
            Insert,
            Update,
            Delete
        }

        public BancoDados()
        {
            ObterConexao();

            if (Conexao == null)
            {
                throw new ArgumentException("A conexão com o banco de dados estava nula ao instanciar.");
            }
        }

        public void IniciarTransacao()
        {
            if (Transacao == null)
            {
                Transacao = Conexao.BeginTransaction();
            }
        }

        public void Commit()
        {
            if (Transacao != null)
            {
                Transacao.Commit();
                Transacao = null;
            }
        }

        public void Rollback()
        {
            try
            {
                if (Transacao != null)
                {
                    Transacao.Rollback();
                    Transacao = null;
                }
            }
            catch
            {

            }
        }

        public void Dispose()
        {
            Rollback();
            Conexao.Dispose();
            GC.SuppressFinalize(this);
        }

        public void ObterConexao()
        {
            try
            {
                string conStr = "Data Source={0};Initial Catalog={1};User ID={2};Password={3};Application Name={4}";
                conStr = string.Format(conStr, URL, DB, Login, Senha, Application);

                SqlConnectionStringBuilder connSB = new SqlConnectionStringBuilder(conStr)
                {
                    MaxPoolSize = 200,
                    MinPoolSize = 10,
                    ConnectRetryCount = 255,
                    ConnectRetryInterval = 3,
                    ConnectTimeout = 60,
                    ApplicationIntent = ApplicationIntent.ReadWrite
                };

                Conexao = AbrirConexao(connSB.ConnectionString);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static SqlConnection AbrirConexao(string strConexao)
        {
            if (string.IsNullOrEmpty(strConexao))
            {
                throw new ArgumentException("Não é possível abrir uma conexão a partir de uma strin nula ou vazia.");
            }

            SqlConnection conexaoEmpresa = new SqlConnection(strConexao);
            try
            {
                conexaoEmpresa.Open();
                using (SqlCommand cmd = new SqlCommand("SET TRANSACTION ISOLATION LEVEL READ COMMITTED", conexaoEmpresa))
                {
                    cmd.ExecuteNonQuery();
                }

                return conexaoEmpresa;
            }
            catch (Exception e)
            {
                SqlConnection.ClearAllPools();
                using (SqlCommand cmd = new SqlCommand("SET TRANSACTION ISOLATION LEVEL READ COMMITTED", conexaoEmpresa))
                {
                    cmd.ExecuteNonQuery();
                }

                throw e;
            }
        }

        private static MapeamentoObjeto CriarMapeamento<TModel>() where TModel : ModeloBase
        {
            MapeamentoObjeto MapeamentoObjeto = new MapeamentoObjeto();

            /* Busca os atributos de Nome da tabela e esquema definido para geração do mapeamento */
            System.Attribute table = Attribute.GetCustomAttribute(typeof(TModel), typeof(TableAttribute));
            if (table != null)
                MapeamentoObjeto.Tabela = ((TableAttribute)table).Name;
            else
                MapeamentoObjeto.Tabela = typeof(TModel).Name.Substring(typeof(TModel).Name.LastIndexOf('.') + 1);

            Attribute schema = Attribute.GetCustomAttribute(typeof(TModel), typeof(SchemaAttribute));
            if (schema != null)
                MapeamentoObjeto.Esquema = ((SchemaAttribute)schema).Name;

            /* Busca todas as propriedades da classe para definir as propriedades a ser utilizadas */
            foreach (PropertyInfo property in typeof(TModel).GetProperties())
            {
                if (property.GetSetMethod() != null && property.GetSetMethod().IsPublic && !Attribute.IsDefined(property, typeof(IgnoreAttribute)))
                {
                    /* Carrega os campos definidos como Primary Key */
                    Attribute primaryKey = Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute));
                    if (primaryKey != null)
                        MapeamentoObjeto.ColunasPK.Add(property.Name);

                    /* Carrega os campos definidos como Identity na base de dados */
                    Attribute identity = Attribute.GetCustomAttribute(property, typeof(AutoIncrementAttribute));
                    if (identity != null)
                        MapeamentoObjeto.ColunasIdentity.Add(property.Name);

                    MapeamentoObjeto.Colunas.Add(property.Name);
                    MapeamentoObjeto.PropriedadesColuna.Add(property.Name, property);
                }
            }

            return MapeamentoObjeto;
        }

        public SqlDataReader Select(string query, List<ParametroSQL> parametros)
        {
            SqlCommand sqlCommand = new SqlCommand(query, Conexao);
            if (parametros != null && parametros.Count() > 0)
                sqlCommand.Parameters.AddRange(parametros.Select(f => new SqlParameter(f.Nome, f.Valor)).ToArray());

            return sqlCommand.ExecuteReader();
        }

        public List<TModel> Select<TModel>(List<ParametroSQL> parametros = null) where TModel : ModeloBase, new()
        {
            var tableMapping = CriarMapeamento<TModel>();
            var fields = tableMapping.Colunas;
            var where = (parametros != null && parametros.Count > 0 ? string.Join(" AND ", parametros.Where(f => fields.Any(c => c == f.Nome)).Select(f => String.Format("({0} = @{0})", f.Nome))) : string.Empty);

            if (!string.IsNullOrWhiteSpace(where))
                where = "WHERE " + where;

            var query = string.Format("SELECT * FROM {0} {1}", tableMapping.NomeCompletoTabela, where);

            return Select<TModel>(query, parametros);
        }

        public List<ModeloBase> SelectModeloBase<TModel>(List<ParametroSQL> parametros = null) where TModel : ModeloBase, new()
        {
            var tableMapping = CriarMapeamento<TModel>();
            var fields = tableMapping.Colunas;
            var where = (parametros != null && parametros.Count > 0 ? string.Join(" AND ", parametros.Where(f => fields.Any(c => c == f.Nome)).Select(f => String.Format("({0} = @{0})", f.Nome))) : string.Empty);

            if (!string.IsNullOrWhiteSpace(where))
                where = "WHERE " + where;

            var query = string.Format("SELECT * FROM {0} {1}", tableMapping.NomeCompletoTabela, where);

            return SelectModeloBase<TModel>(query, parametros);
        }

        public List<TModel> Select<TModel>(string query, List<ParametroSQL> parametros = null) where TModel : ModeloBase, new()
        {
            SqlCommand sqlCommand = new SqlCommand
            {
                CommandText = query
            };

            if (parametros != null && parametros.Count() > 0)
                sqlCommand.Parameters.AddRange(parametros.Select(f => new SqlParameter(f.Nome, f.Valor)).ToArray());

            return Select<TModel>(sqlCommand);
        }

        public List<ModeloBase> SelectModeloBase<TModel>(string query, List<ParametroSQL> parametros = null) where TModel : ModeloBase, new()
        {
            SqlCommand sqlCommand = new SqlCommand
            {
                CommandText = query
            };

            if (parametros != null && parametros.Count() > 0)
                sqlCommand.Parameters.AddRange(parametros.Select(f => new SqlParameter(f.Nome, f.Valor)).ToArray());

            return SelectModeloBase<TModel>(sqlCommand);
        }

        private List<TModel> Select<TModel>(SqlCommand sqlCommand) where TModel : ModeloBase, new()
        {
            try
            {
                sqlCommand.Connection = Conexao;
                sqlCommand.CommandTimeout = TimeoutComandos;

                if (Transacao != null)
                {
                    sqlCommand.Transaction = Transacao;
                }

                /* Localiza o table mapping definido para o objeto em questão */
                MapeamentoObjeto MapeamentoObjeto = CriarMapeamento<TModel>();
                List<TModel> listModel = new List<TModel>();

                /* Execute o reader para cada TModel a ser criado baseado no resultado */
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (sqlDataReader.Read())
                    {
                        listModel.Add(ReadItem<TModel>(sqlDataReader, MapeamentoObjeto));
                    }
                }

                return listModel;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private List<ModeloBase> SelectModeloBase<TModel>(SqlCommand sqlCommand) where TModel : ModeloBase, new()
        {
            try
            {
                sqlCommand.Connection = Conexao;
                sqlCommand.CommandTimeout = TimeoutComandos;

                if (Transacao != null)
                {
                    sqlCommand.Transaction = Transacao;
                }

                /* Localiza o table mapping definido para o objeto em questão */
                MapeamentoObjeto MapeamentoObjeto = CriarMapeamento<TModel>();
                List<ModeloBase> listModel = new List<ModeloBase>();

                /* Execute o reader para cada TModel a ser criado baseado no resultado */
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (sqlDataReader.Read())
                    {
                        listModel.Add(ReadItem<TModel>(sqlDataReader, MapeamentoObjeto));
                    }
                }

                return listModel;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private TModel ReadItem<TModel>(SqlDataReader sqlDataReader, MapeamentoObjeto mapeamento) where TModel : ModeloBase, new()
        {
            /* Cria um novo objeto TModel para definir as propriedades */
            TModel model = new TModel();
            for (int index = 0; index < sqlDataReader.FieldCount; index++)
            {
                String column = sqlDataReader.GetName(index);
                if (mapeamento.PropriedadesColuna.Keys.Contains(column))
                {
                    if (sqlDataReader[index] == DBNull.Value || sqlDataReader[index] == null)
                    {
                        if (Nullable.GetUnderlyingType(mapeamento.PropriedadesColuna[column].PropertyType) != null ||
                            mapeamento.PropriedadesColuna[column].PropertyType == typeof(string) ||
                            mapeamento.PropriedadesColuna[column].PropertyType == typeof(object))
                        {
                            mapeamento.PropriedadesColuna[column].SetValue(model, null);
                        }
                        else
                        {
                            if (!mapeamento.PropriedadesColuna[column].PropertyType.IsArray)
                                mapeamento.PropriedadesColuna[column].SetValue(model, Activator.CreateInstance(mapeamento.PropriedadesColuna[column].PropertyType));
                            else
                                mapeamento.PropriedadesColuna[column].SetValue(model, Activator.CreateInstance(mapeamento.PropriedadesColuna[column].PropertyType, 0));
                        }
                    }
                    else if (mapeamento.PropriedadesColuna[column].PropertyType == typeof(bool))
                    {
                        if (sqlDataReader.GetValue(index).GetType() == typeof(int))
                        {
                            mapeamento.PropriedadesColuna[column].SetValue(model, sqlDataReader.GetInt32(index) == 1);
                        }
                        else if (sqlDataReader.GetValue(index).GetType() == typeof(short))
                        {
                            mapeamento.PropriedadesColuna[column].SetValue(model, sqlDataReader.GetInt16(index) == 1);
                        }
                        else if (sqlDataReader.GetValue(index).GetType() == typeof(long))
                        {
                            mapeamento.PropriedadesColuna[column].SetValue(model, sqlDataReader.GetInt64(index) == 1);
                        }
                        else
                        {
                            mapeamento.PropriedadesColuna[column].SetValue(model, sqlDataReader.GetValue(index));
                        }
                    }
                    else
                    {
                        mapeamento.PropriedadesColuna[column].SetValue(model, sqlDataReader.GetValue(index));
                    }
                }
            }
            return model;
        }

        private List<SqlParameter> AplicarParametros<TModel>(TModel model, MapeamentoObjeto tableMapping, List<string> listColumn) where TModel : ModeloBase, new()
        {
            List<SqlParameter> parametros = new List<SqlParameter>();

            /* Valida os parametros passados no sistema e cria os correspondentes SqlParameter */
            foreach (string column in listColumn.Union(tableMapping.ColunasPK))
            {
                var property = tableMapping.PropriedadesColuna[column];
                object value = property.GetValue(model);
                value = value ?? DBNull.Value;

                if (property.PropertyType != typeof(byte[]))
                    parametros.Add(new SqlParameter(string.Format("@{0}", column), (value ?? DBNull.Value)));
                else
                {
                    /* create parameter */
                    var parameter = new SqlParameter()
                    {
                        ParameterName = string.Format("@{0}", column),
                        SqlDbType = SqlDbType.VarBinary
                    };

                    if (value != DBNull.Value)
                        parameter.Size = ((byte[])value).Length;

                    parameter.Value = value;
                    parametros.Add(parameter);
                }
            }

            return parametros;
        }

        public int Insert<TModel>(TModel model, List<string> columns = null) where TModel : ModeloBase, new()
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            try
            {
                /* Localiza o table mapping definido para o objeto em questão */
                MapeamentoObjeto tableMapping = CriarMapeamento<TModel>();

                if (tableMapping.ColunasPK.Count == 0)
                    throw new InvalidOperationException(string.Format("Nenhuma PK encontrada em {0}.", typeof(TModel).Name));

                /* Gera a lista de campos a ser processada no sistema.*/
                List<string> listColumn = new List<string>();
                if (columns == null || columns.Count == 0)
                    listColumn.AddRange(tableMapping.Colunas.Except(tableMapping.ColunasIdentity));
                else
                    listColumn.AddRange(tableMapping.Colunas.Except(tableMapping.ColunasIdentity).Distinct().Intersect(columns));

                /* Define as partes do comando a ser processadas. Comando em si, colunas para serem atualizadas e where */
                string columnSet = string.Join(", ", listColumn.ToArray());
                string valuesSet = string.Join(", ", listColumn.Select(x => string.Format("@{0}", x)));
                string identity = (tableMapping.ColunasIdentity.Count > 0) ? "SELECT @@IDENTITY" : string.Empty;
                string command = string.Format("INSERT INTO {0} ({1}) VALUES ({2});{3}", tableMapping.NomeCompletoTabela, columnSet, valuesSet, identity).TrimEnd();

                /* Executa o comando no banco de dados e retorna o número de linhas afetadas para o sistema */
                if (tableMapping.ColunasIdentity.Count == 0)
                {
                    return ExecutarComando(command, AplicarParametros(model, tableMapping, listColumn));
                }
                else
                {
                    object identityValue = ExecutarComandoEscalar(command, AplicarParametros(model, tableMapping, listColumn));

                    Type type = model.GetType();
                    PropertyInfo propertyInfo = type.GetProperty(tableMapping.ColunasIdentity.FirstOrDefault());
                    propertyInfo.SetValue(model, Convert.ToInt32(identityValue));

                    return 1;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public int Update<TModel>(TModel model, List<string> columns = null) where TModel : ModeloBase, new()
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            try
            {
                /* Localiza o table mapping definido para o objeto em questão */
                MapeamentoObjeto tableMapping = CriarMapeamento<TModel>();

                if (tableMapping.ColunasPK.Count == 0)
                    throw new InvalidOperationException(string.Format("Nenhuma PK encontrada em {0}.", typeof(TModel).Name));

                /* Gera a lista de campos a ser processada no sistema. Excluindo as chaves primárias da lista */
                List<string> listColumn = new List<string>();
                if (columns == null || columns.Count == 0)
                    listColumn.AddRange(tableMapping.Colunas.Except(tableMapping.ColunasPK));
                else
                    listColumn.AddRange(tableMapping.Colunas.Distinct().Intersect(columns).Except(tableMapping.ColunasPK));

                /* Define as partes do comando a ser processadas. Comando em si, colunas para serem atualizadas e where */
                string updateSet = string.Join(", ", listColumn.Select(x => string.Format("{0} = @{0}", x)));
                string whereClause = string.Join(" AND ", tableMapping.ColunasPK.Select(x => string.Format("({0} = @{0})", x)));
                string command = string.Format("UPDATE {0} SET {1} WHERE {2}", tableMapping.NomeCompletoTabela, updateSet, whereClause).TrimEnd();

                List<ParametroSQL> parametros = new List<ParametroSQL>();

                foreach (string column in tableMapping.Colunas)
                {
                    object value = tableMapping.PropriedadesColuna[column].GetValue(model);
                    parametros.Add(new ParametroSQL(string.Format("@{0}", column), value));
                }

                return ExecutarComando(command, parametros);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public int Delete<TModel>(TModel model) where TModel : ModeloBase, new()
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            try
            {
                /* Localiza o table mapping definido para o objeto em questão */
                MapeamentoObjeto tableMapping = CriarMapeamento<TModel>();

                if (tableMapping.ColunasPK.Count == 0)
                    throw new InvalidOperationException(string.Format("Nenhuma PK encontrada em {0}.", typeof(TModel).Name));

                /* Define as partes do comando a ser processadas. Comando em si, colunas para serem atualizadas e where */
                string whereClause = string.Join(" AND ", tableMapping.ColunasPK.Select(x => string.Format("({0} = @{0})", x)));
                string command = string.Format("DELETE FROM {0} WHERE {1}", tableMapping.NomeCompletoTabela, whereClause).TrimEnd();

                List<ParametroSQL> parametros = new List<ParametroSQL>();

                foreach (string column in tableMapping.ColunasPK)
                {
                    object value = tableMapping.PropriedadesColuna[column].GetValue(model);
                    parametros.Add(new ParametroSQL(string.Format("@{0}", column), value));
                }

                return ExecutarComando(command, parametros);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public int ExecutarComando(string sql, List<ParametroSQL> parametros = null)
        {
            SqlCommand sqlCommand = new SqlCommand(sql, Conexao)
            {
                CommandTimeout = TimeoutComandos
            };

            if (Transacao != null)
            {
                sqlCommand.Transaction = Transacao;
            }

            if (parametros != null && parametros.Count > 0)
            {
                foreach (ParametroSQL column in parametros)
                {
                    sqlCommand.Parameters.AddWithValue(column.Nome, column.Valor);
                }
            }

            return sqlCommand.ExecuteNonQuery();
        }

        public int ExecutarComando(string sql, List<SqlParameter> parametros = null)
        {
            SqlCommand sqlCommand = new SqlCommand(sql, Conexao)
            {
                CommandTimeout = TimeoutComandos
            };

            if (Transacao != null)
            {
                sqlCommand.Transaction = Transacao;
            }

            if (parametros != null && parametros.Count > 0)
            {
                sqlCommand.Parameters.AddRange(parametros.ToArray());
            }

            return sqlCommand.ExecuteNonQuery();
        }

        public object ExecutarComandoEscalar(string sql, List<ParametroSQL> parametros = null)
        {
            SqlCommand sqlCommand = new SqlCommand(sql, Conexao)
            {
                CommandTimeout = TimeoutComandos
            };

            if (Transacao != null)
            {
                sqlCommand.Transaction = Transacao;
            }

            if (parametros != null && parametros.Count > 0)
            {
                foreach (ParametroSQL column in parametros)
                {
                    sqlCommand.Parameters.AddWithValue(column.Nome, column.Valor);
                }
            }

            return sqlCommand.ExecuteScalar();
        }

        private object ExecutarComandoEscalar(string sql, List<SqlParameter> parametros = null)
        {
            SqlCommand sqlCommand = new SqlCommand(sql, Conexao)
            {
                CommandTimeout = TimeoutComandos
            };

            if (Transacao != null)
            {
                sqlCommand.Transaction = Transacao;
            }

            if (parametros != null && parametros.Count > 0)
            {
                sqlCommand.Parameters.AddRange(parametros.ToArray());
            }

            return sqlCommand.ExecuteScalar();
        }
    }
}

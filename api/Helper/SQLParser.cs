using Microsoft.CodeAnalysis;
using Microsoft.SqlServer.Management.SqlParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helper
{
    public class SQLParser
    {
        public static List<TokenInfo> ObterTokens(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return new List<TokenInfo>();
            }

            ParseOptions parseOptions = new ParseOptions();
            Scanner scanner = new Scanner(parseOptions);

            int state = 0,
                start,
                end,
                token;

            bool isPairMatch, isExecAutoParamHelp;

            List<TokenInfo> tokens = new List<TokenInfo>();

            scanner.SetSource(query, 0);

            while ((token = scanner.GetNext(ref state, out start, out end, out isPairMatch, out isExecAutoParamHelp)) != (int)Tokens.EOF)
            {
                TokenInfo tokenInfo =
                    new TokenInfo()
                    {
                        Start = start,
                        End = end,
                        IsPairMatch = isPairMatch,
                        IsExecAutoParamHelp = isExecAutoParamHelp,
                        Sql = query.Substring(start, end - start + 1),
                        Token = (Tokens)token,
                    };

                tokens.Add(tokenInfo);
            }

            return tokens;
        }

        public static int GetOrderByIndex(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return -1;
            }

            try
            {
                int len = query.Length;
                int nivel = 0;

                var tokens = ObterTokens(query);

                foreach (var token in tokens)
                {
                    if (token.Sql == "(")
                    {
                        nivel++;
                    }
                    else if (token.Sql == ")")
                    {
                        nivel--;
                    }
                    else if (token.Sql.ToUpper() == "ORDER" && nivel == 0)
                    {
                        return token.Start;
                    }
                }

                return -1;
            }
            catch
            {
                return -1;
            }
        }
    }
}

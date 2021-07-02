using Microsoft.SqlServer.Management.SqlParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helper
{
    public class TokenInfo
    {
        public int Start { get; set; }
        public int End { get; set; }
        public bool IsPairMatch { get; set; }
        public bool IsExecAutoParamHelp { get; set; }
        public string Sql { get; set; }
        public Tokens Token { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenEnums;

namespace Lexer
{
    public class Token
    {
        public string Symbol { get; set; }

        public TokenType TokenType { get; set; }

        public string Info { get; set; }

        public Token(string symbol)
        {
            this.Symbol = symbol;
        }

        /// <summary>
        /// Returns closing tags, terminators, etc
        /// </summary>
        public string Partner { get; set; }

        public List<string> Partners { get; set; }
    }
}

using Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public class State
    {
        public List<Tuple<TokenEnums.TokenType, State>> Transitions { get; set; }
        public StateAction StateAction { get; set;}
        public string Name { get; set; }
        public string Description { get; set; }

        public State()
        {
            Transitions = new List<Tuple<TokenEnums.TokenType, State>>();
        }

        public State GoToNextState(TokenEnums.TokenType tokenType, Token token = null)
        {
            var matches = Transitions.Where(item => item.Item1 == tokenType);

            if (matches.Count() > 1)
            {
                // non-deterministic
                return null;
            }
            return matches.FirstOrDefault().Item2;
        }

        public List<State> GetPossibleNextStates(TokenEnums.TokenType tokenType)
        {
            var matches = Transitions.Where(item => item.Item1 == tokenType);
            var retVal = new List<State>();

            foreach (var item in matches)
            {
                retVal.Add(item.Item2);
            }

            return retVal;
        }

        public State ReEvaluateNextStateBasedOnNextToken(TokenEnums.TokenType tokenType)
        {
            var match = Transitions.Where(item => item.Item1 == tokenType).FirstOrDefault();
            return match.Item2;
        }
    }

    public enum StateAction
    {
        Read,
        Write,
        Scan,
        Start,
        Accept
    }
}

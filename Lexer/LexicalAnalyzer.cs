using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    public class LexicalAnalyzer
    {
        //public void 

        public List<Token> Tokens { get; set; }

        public LexicalAnalyzer()
        {
            Tokens = new List<Token>();
        }

        public List<Token> FindTokensInText(string text)
        {
            List<Token> retVal = new List<Token>();
            FiniteStateMachine fsm = new FiniteStateMachine();

            try
            {


                // feed the string to an "FSM"
                for (int i = 0; i < text.Length; i++)
                {
                    int j = i + 1;

                    var currentSymbol = text[i].ToString();
                    var last = retVal.Count > 0 ? retVal.Last() : null;

                    // add logic to figure out token from this lexeme
                    if (currentSymbol == ">")
                    {
                        var t = new Token(currentSymbol);
                        t.TokenType = TokenEnums.TokenType.ClosingSymbol;
                        //t.Partner = "<";
                        t.Partners = new List<string>();
                        t.Partners.Add("<");
                        t.Partners.Add("</");

                        retVal.Add(t);
                        continue;
                    }
                    if (j >= text.Length)
                    {
                        break;
                    }
                    var nextSymbol = text[j].ToString();

                    // e.g: <sample> Hello </sample>
                    if (currentSymbol == "<")
                    {
                        var t = new Token(currentSymbol);
                        t.TokenType = TokenEnums.TokenType.OpeningSymbol;
                        t.Info = "Opening angled bracket";
                        t.Partner = ">";
                        retVal.Add(t);
                    }
                    else if (currentSymbol == @"/")
                    {
                        if (last != null && last.Symbol == "<")
                        {
                            var t = new Token(last.Symbol + currentSymbol);
                            t.TokenType = TokenEnums.TokenType.OpeningWithBackslashSymbol;
                            t.Info = "Nested opening bracket";
                            t.Partner = ">";
                            retVal.Remove(last);
                            retVal.Add(t);
                        }
                        else if (nextSymbol == ">") //self-closing
                        {
                            i++; // go to position after next
                            var t = new Token(currentSymbol + nextSymbol);
                            t.TokenType = TokenEnums.TokenType.SelfClosingSymbol;
                            t.Info = "Closing angled bracket";
                            t.Partner = "<";
                            retVal.Add(t);
                        }
                    }
                    else if (currentSymbol == "?" && ((last != null && last.Symbol == "<") || (nextSymbol == ">")))
                    {
                        if (last != null && last.Symbol == "<")
                        {
                            var t = new Token(last.Symbol + currentSymbol);
                            t.TokenType = TokenEnums.TokenType.HeaderXmlOpeningSymbol;
                            t.Info = "Header opening token";
                            t.Partner = "?>";
                            retVal.Remove(last);
                            retVal.Add(t);
                        }
                        else if (nextSymbol == ">") //self-closing XML header tag
                        {
                            i++;
                            var t = new Token(currentSymbol + nextSymbol);
                            t.TokenType = TokenEnums.TokenType.HeaderXmlClosingSymbol;
                            t.Info = "Header closing token";
                            t.Partner = "<?";
                            retVal.Add(t);
                        }
                    }
                    else if (currentSymbol.All(char.IsLetterOrDigit) && last != null && (last.Symbol == "<?")) // TO DO: replace this with a better checker, need to check that value can support special characters.
                    {
                        Token t = new Token(currentSymbol);
                        // xml header tag
                        // "<? xml ?>"
                        while (nextSymbol != " " && nextSymbol != "?")
                        {

                            t.Symbol += nextSymbol;
                            j++;

                            nextSymbol = text[j].ToString();
                        }
                        t.TokenType = TokenEnums.TokenType.HeaderXmlTag;
                        t.Info = "Tag";
                        retVal.Add(t);
                        i = j - 1;
                    }

                    // normal Tag
                    else if (currentSymbol.All(char.IsLetterOrDigit) && (last != null && (last.Symbol == "<" || last.Symbol == "</")))
                    {
                        Token t = new Token(currentSymbol);

                        // "<hi >"  <- stop loop when either space or closing tag
                        while (nextSymbol != " " && nextSymbol != ">" && nextSymbol != "/")
                        {

                            t.Symbol += nextSymbol;
                            j++;

                            nextSymbol = text[j].ToString();
                        }
                        t.TokenType = TokenEnums.TokenType.Tag;
                        t.Info = "Tag";
                        if (t.Symbol.Contains("-") || t.Symbol.Contains(":") || t.Symbol.Contains("."))
                        {
                            throw new Exception();
                        }
                        retVal.Add(t);
                        i = j - 1;
                    }

                    // Attribute
                    else if (currentSymbol.All(char.IsLetterOrDigit) && (last != null && last.TokenType == TokenEnums.TokenType.AttributeValue || last.TokenType == TokenEnums.TokenType.Tag || last.TokenType == TokenEnums.TokenType.HeaderXmlTag || last.TokenType == TokenEnums.TokenType.AttributeValue))
                    {
                        Token t = new Token(currentSymbol);

                        // "<hi >"
                        while (nextSymbol != "=" && nextSymbol != "?" && nextSymbol != ">" && nextSymbol != "/")
                        {
                            t.Symbol += nextSymbol;
                            j++;
                            nextSymbol = text[j].ToString();
                        }
                        t.TokenType = TokenEnums.TokenType.Attribute;
                        t.Info = "Attribute";
                        if (t.Symbol.Contains("-") || t.Symbol.Contains(":") || t.Symbol.Contains("."))
                        {
                            throw new Exception();
                        }
                        retVal.Add(t);
                        i = j - 1;
                    }

                    // Just a value
                    // <sample> test </sample>
                    else if (currentSymbol.All(char.IsLetterOrDigit) && last != null && last.Symbol == ">")
                    {
                        Token t = new Token(currentSymbol);

                        // "<hi >"
                        while (nextSymbol != "<")
                        {
                            t.Symbol += nextSymbol;
                            j++;
                            nextSymbol = text[j].ToString();
                        }
                        t.TokenType = TokenEnums.TokenType.Value;
                        t.Info = "Value";
                        retVal.Add(t);
                        i = j - 1;
                    }


                    else if (currentSymbol == "=" && last.TokenType == TokenEnums.TokenType.Attribute)
                    {
                        Token t = new Token(currentSymbol);
                        t.TokenType = TokenEnums.TokenType.Operator;
                        t.Info = "Operator";
                        retVal.Add(t);
                    }
                    else if (currentSymbol == "\"")
                    {
                        if (last != null && last.Symbol == "=")
                        {
                            Token t = new Token(currentSymbol);

                            // "<hi >"
                            while (nextSymbol != "\"")
                            {
                                t.Symbol += nextSymbol;
                                j++;
                                nextSymbol = text[j].ToString();
                            }
                            if (nextSymbol == "\"")
                            {
                                // move to the position after that double quote
                                t.Symbol += nextSymbol;
                                j++;
                            }
                            t.TokenType = TokenEnums.TokenType.AttributeValue;
                            t.Info = "Attribute Value";
                            retVal.Add(t);
                            i = j - 1;
                        }
                    }
                    else if (currentSymbol == "'")
                    {
                        if (last != null && last.Symbol == "=")
                        {
                            Token t = new Token(currentSymbol);

                            // "<hi >"
                            while (nextSymbol != "'")
                            {
                                t.Symbol += nextSymbol;
                                j++;
                                nextSymbol = text[j].ToString();
                            }
                            if (nextSymbol == "'")
                            {
                                // move to the position after that double quote
                                t.Symbol += nextSymbol;
                                j++;
                            }
                            t.TokenType = TokenEnums.TokenType.AttributeValue;
                            t.Info = "Attribute Value";
                            retVal.Add(t);
                            i = j - 1;
                        }
                    }
                    else if (string.IsNullOrWhiteSpace(currentSymbol))
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                var x = ex;
                throw;
            }

            return retVal;
        }


    }

    public class FiniteStateMachine
    {
        public FiniteStateMachine()
        {

        }

        public State[] States { get; set; }

        public State InitialState { get; set; }

        public State CurrentState { get; set; }

        public void InitializeStates()
        {
            // start state


            // end state


            // other states
        }


    }

    public class State
    {
        public State(Lexeme lexeme)
        {

        }

        public bool EndState { get; set; }


    }
}

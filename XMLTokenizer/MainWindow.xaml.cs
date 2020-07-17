using Lexer;
using Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XMLTokenizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public List<Token> Tokens { get; set; }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Stopwatch s1 = new Stopwatch();
                s1.Start();
                var textToTest = txtXml.Text;
                var size = System.Text.ASCIIEncoding.ASCII.GetByteCount(textToTest);
                if (size > 1024 * 100)
                {
                    throw new Exception();
                }
                // MessageBox.Show(size.ToString() + " bytes");
                LexicalAnalyzer lexicalAnalyzer = new LexicalAnalyzer();
                Tokens = lexicalAnalyzer.FindTokensInText(textToTest.Trim());
                this.DataGrid1.ItemsSource = Tokens;
                SyntaxAnalyzerPDA syntaxAnalyzer = new SyntaxAnalyzerPDA();

                var valid = syntaxAnalyzer.Validate(Tokens);

                if (valid)
                {
                    s1.Stop();
                    MessageBox.Show("Yes, valid - " + s1.ElapsedMilliseconds.ToString() + "ms");

                }
                else
                {
                    MessageBox.Show("No, invalid");
                }
            }
            catch (Exception ex)
            {
                var x = ex;
                MessageBox.Show("Error, Invalid");
            }
           
            // textToTest = textToTest.Replace(" ", string.Empty);

            // [LEXER] 1. get each character as a lexeme
            // [LEXER] 2. based on each lexeme, deterministically produce a token based on the rules (or grammar) of each lexeme
                // 2.1, each lexeme is a non-terminal, which can be further evaluated/derived until it becomes a terminal. when it has been exhaustively derived, we will consider that as a token
                // For example for the simple xml <course name="automata"/>,
                // when we read the "<" opening tag,  that's already one token because sigma will always point to an opening tag,
                // when we read the "c" in "course", we will keep reading "o", "u", "r", "s", "e" until the next space, and treat the "course" string as one token.
                // when we read the "name" that's a token
                // when we read "=", that's a token
                // when we read the double quotes (") that's a token"
                // etc.
            // 3. When we are traversing the string and figuring out the tokens, we look at a dictionary that stores key-value pairs for each token and its terminator. a
                // 3.1 For example, for the "<" token, the dictionary can contain the "/>" terminator for self-closing tags
                // 3.2 For the double quotes, the dictionary can contain double quotes to enclose attribute values
            // [SYNTAX/PARSER] 4. As we are identifying tokens, push it to a stack, and if we encounter a terminator token, we will pop the token in the top of stack which the terminator terminates.
                // 4.1 we are guaranteed that the item at the top of the stack will always terminate first if it is a valid XML. if not, then the xml is invalid.  If the stack becomes empty and there are no more tokens, then the XML is accepted

        }

     
    }
}

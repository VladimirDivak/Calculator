using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Calculator
{
    class Program
    {
        static readonly char[] correctSymbols = new char[]{'*', '/', '+', '-'};

        static void Main()
        {
            Console.Clear();
            Console.WriteLine("Добро пожаловать!");
            while(true)
            {
                var mathfExprString = string.Empty;

                Console.WriteLine("\nРазрешено использование чисел, круглых скобок и символов\nматематических выражений +, -, *, /.");
                Console.WriteLine("Введите математическое выражение:");

                mathfExprString = Console.ReadLine();

                var formatMathfExpr = new StringBuilder();
                foreach(var ch in mathfExprString)
                    if(ch != ' ') formatMathfExpr.Append(ch);
                mathfExprString = formatMathfExpr.ToString();

                var syntaxCheckerResponse = SyntaxChecker(mathfExprString);
                if(syntaxCheckerResponse != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(syntaxCheckerResponse);
                    Console.ForegroundColor = ConsoleColor.White;

                    continue;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Выражение было записано корректно.");
                    Console.ForegroundColor = ConsoleColor.White;
                    
                    Calculate(mathfExprString);

                    break;
                }
            }

            return;
        }

        static string SyntaxChecker(string mathfExprString)
        {
            if(mathfExprString == string.Empty)
                return "Выражение не может быть пустым.";

            var letters = mathfExprString.Where(ch => char.IsLetter(ch)).ToArray().Length;
            var symbols = mathfExprString.Where(ch => !char.IsNumber(ch) && ch != '+' && ch != '-' && ch != '*' &&
                ch != '/' && ch != '.' && ch != '(' && ch != ')').ToArray().Length;

            if(letters != 0)
                return "Выражение не должно содержать буквы.";
            if(symbols != 0)
                return "Используются недопустимые символы.";

            if(!mathfExprString.Contains('+') && !mathfExprString.Contains('-') && !mathfExprString.Contains('*') && !mathfExprString.Contains('/'))
                return "Выражение должно содержать математические операции.";

            var firstBrackets = mathfExprString.Count(Daria => Daria == '(');
            var lastBrackets = mathfExprString.Count(Daria => Daria == ')');
            if (firstBrackets != lastBrackets)
                return "Выражение содержит неверное количество круглых скобок.";

            int reiterationsCounter = 0;

            foreach(var symbol in mathfExprString)
            {
                var exeptionText = "Ошибка в последовательности операций.";

                if(char.IsSymbol(symbol))
                {
                    if(reiterationsCounter == 1)
                    {
                        if(symbol != '(' && symbol != ')' && symbol != '-')
                        {
                            return exeptionText;
                        }
                    }
                    else if(reiterationsCounter == 2)
                    {
                        if(symbols != '-') return exeptionText;
                    }
                    else reiterationsCounter++;
                }
            }
            
            return null;
        }

        static float Calculate(string expression)
        {
            var expressions = FindRoundBrackets(expression, out var newExressionString);

            return 0.0f;
        }

        static Dictionary<string, string> FindRoundBrackets(string expression, out string newExressionString)
        {
            Dictionary<string, string> expressions = new Dictionary<string, string>();
            string currentStringFormat = expression;
            int bracketsCounter = expression.Count(ch => ch == '(' || ch ==')');
            int exprCounter = 1;

            if(bracketsCounter == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Выражение не содержит круглых скобок.");
                Console.ForegroundColor = ConsoleColor.White;

                newExressionString = string.Empty;
                return null;
            }

            bracketsCounter /= 2;

            Console.WriteLine("Начинаем процесс формирования порядка выполнения выражений...");
            while(bracketsCounter != 0)
            {
                Console.WriteLine($"Осталось обработать {bracketsCounter} пар(у, ы) круглых скобок.");

                int openBracketIndex = currentStringFormat.IndexOf(currentStringFormat.Last(ch => ch == '('));
                int closeBracketIndex = 0;

                for(int i = openBracketIndex; i < currentStringFormat.Length; i++)
                {
                    if(currentStringFormat[i] == ')') closeBracketIndex = i;
                }

                var newExpr = new string(currentStringFormat.Skip(openBracketIndex).Take(closeBracketIndex - openBracketIndex + 1).ToArray());
                Console.WriteLine($"Нашёл выражение: {newExpr}");

                expressions.Add($"[{exprCounter}]", newExpr);
                currentStringFormat = currentStringFormat.Replace(newExpr, $"[{exprCounter}]");
                Console.WriteLine($"Промежуточный вариант вида выражения: {currentStringFormat}");

                exprCounter++;
                bracketsCounter--;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Преобразование выполнено. Выражение имеет вид {currentStringFormat}. Количество операций в скобках: {expressions.Count}.");
            Console.ForegroundColor = ConsoleColor.White;

            newExressionString = currentStringFormat;
            return expressions;
        }
    }
}
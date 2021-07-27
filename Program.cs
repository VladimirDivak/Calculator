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

            if(!mathfExprString.Contains('+') && !mathfExprString.Contains('-') &&
            !mathfExprString.Contains('*') && !mathfExprString.Contains('/'))
                return "Выражение должно содержать математические операции.";

            var firstBrackets = mathfExprString.Count(ch => ch == '(');
            var lastBrackets = mathfExprString.Count(ch => ch == ')');
            if (firstBrackets != lastBrackets)
                return "Выражение содержит неверное количество круглых скобок.";

            int reiterationsCounter = 0;
            int lastCharIndex = -1;
            var exceptionText = "Ошибка в последовательности операций.";

            for(int i = 0; i < mathfExprString.Length; i++)
            {
                if(!char.IsNumber(mathfExprString[i]))
                {
                    switch(reiterationsCounter)
                    {
                        case 0:
                            if(mathfExprString[i] == '(' && i != 0 &&
                            char.IsNumber(mathfExprString[i - 1]))
                            {
                                return exceptionText;
                            }
                            
                            lastCharIndex = i;
                            reiterationsCounter++;
                        break;

                        case 1:
                            if(i == lastCharIndex + 1)
                            {
                                if(mathfExprString[i] == '(' ||
                                mathfExprString[i] == ')' ||
                                mathfExprString[i - 1] == ')')
                                {
                                    reiterationsCounter = 1;
                                    lastCharIndex = i;
                                }
                                else return exceptionText;
                            }
                            else
                            {
                                reiterationsCounter = 1;
                                lastCharIndex = i;
                            }
                        break;
                    }
                }
                else
                {
                    reiterationsCounter = 0;

                    if(i != 0 && mathfExprString[i - 1] == ')')
                    {
                        return exceptionText + " 3";
                    }
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

            Console.WriteLine("\nНачинаем процесс формирования порядка выполнения выражений...\n");
            while(bracketsCounter != 0)
            {
                Console.WriteLine($"Осталось обработать {bracketsCounter} пар(у, ы) круглых скобок.");

                int openBracketIndex = 0;
                int closeBracketIndex = 0;

                for(int i = currentStringFormat.Length - 1; i >= 0; i--)
                {
                    if(currentStringFormat[i] == '(')
                    {
                        openBracketIndex = i;
                        break;
                    }
                }

                for(int i = openBracketIndex; i < currentStringFormat.Length; i++)
                {
                    if(currentStringFormat[i] == ')')
                    {
                        closeBracketIndex = i;
                        break;
                    }
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
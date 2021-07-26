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

            if(mathfExprString.Count(ch => ch == '(' || ch == ')') % 2 != 0)
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
            List<string> expressions = new List<string>();
            List<char> operations = new List<char>();

            return 0.0f;
        }
    }
}
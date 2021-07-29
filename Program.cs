using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Calculator
{
    class Program
    {
        static readonly char[] _correctSymbols = new char[]{'*', '/', '+', '-'};

        static void Main()
        {
            Console.Clear();
            Console.WriteLine("Добро пожаловать!");
            while(true)
            {
                var mathfExprString = string.Empty;

                Console.WriteLine("\nРазрешено использование чисел, запятых, круглых скобок и символов\nматематических выражений +, -, *, /.");
                Console.WriteLine("\nВведите математическое выражение:");

                mathfExprString = Console.ReadLine();

                var formatMathfExpr = new StringBuilder();
                foreach(var ch in mathfExprString)
                    if(ch != ' ') formatMathfExpr.Append(ch);
                mathfExprString = formatMathfExpr.ToString();

                var syntaxCheckerResponse = SyntaxChecker(mathfExprString, out var exprWithMinusValues);
                if(syntaxCheckerResponse != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(syntaxCheckerResponse);
                    Console.ForegroundColor = ConsoleColor.White;

                    continue;
                }
                else
                {
                    if(exprWithMinusValues != string.Empty)
                        mathfExprString = exprWithMinusValues;
                    
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nРезультат: " + Calculate(mathfExprString));
                    Console.ForegroundColor = ConsoleColor.White;

                    break;
                }
            }

            return;
        }

        static string SyntaxChecker(string mathfExprString, out string mathfExprWithMinusValues)
        {
            mathfExprWithMinusValues = string.Empty;

            if(mathfExprString == string.Empty)
                return "Выражение не может быть пустым.";

            var letters = mathfExprString.Where(ch => char.IsLetter(ch)).ToArray().Length;
            var symbols = mathfExprString.Where(ch => !char.IsNumber(ch) && ch != '+' && ch != '-' && ch != '*' &&
                ch != '/' && ch != ',' && ch != '(' && ch != ')').ToArray().Length;

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

            List<int> minusValueIndex = new List<int>();

            for(int i = 0; i < mathfExprString.Length; i++)
            {
                if(!char.IsNumber(mathfExprString[i]))
                {
                    if(mathfExprString[i] == '-')
                    {
                        if(i == 0 || mathfExprString[i - 1] == '(')
                        {
                            minusValueIndex.Add(i);
                            continue;
                        }
                    }

                    if(i == mathfExprString.Length - 1 &&
                    mathfExprString[i] != '(' &&
                    mathfExprString[i] != ')')
                        return exceptionText;
                    
                    if(mathfExprString[i] == ',')
                    {
                        if(i == 0 || i == mathfExprString.Length - 1 || (!char.IsNumber(mathfExprString[i - 1]) &&
                        !char.IsNumber(mathfExprString[i + 1])))
                            return "запятая в выражении расположена неверно.";
                    }

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
                        return exceptionText;
                    }
                }
            }
            if(minusValueIndex != null)
            {
                StringBuilder stringWithMinusValues = new StringBuilder(mathfExprString);
                foreach(var index in minusValueIndex)
                    stringWithMinusValues[index] = 'm';
                
                mathfExprWithMinusValues = stringWithMinusValues.ToString();
            }
            return null;
        }

        static Dictionary<string, string> FindRoundBrackets(string expression, out string newExressionString)
        {
            Dictionary<string, string> expressions = new Dictionary<string, string>();
            string currentStringFormat = expression;
            int bracketsCounter = expression.Count(ch => ch == '(' || ch ==')');
            int exprCounter = 1;

            if(bracketsCounter == 0)
            {
                newExressionString = string.Empty;
                return null;
            }

            bracketsCounter /= 2;

            while(bracketsCounter != 0)
            {
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

                expressions.Add($"[{exprCounter}]", new string(newExpr.Where(ch => ch != '(' && ch != ')').ToArray()));
                currentStringFormat = currentStringFormat.Replace(newExpr, $"[{exprCounter}]");

                exprCounter++;
                bracketsCounter--;
            }

            newExressionString = currentStringFormat;
            return expressions;
        }

        static string GetOperationsResult(string expression)
        {
            char operation1 = new char();
            char operation2 = new char();


            string newExprFormat = expression;
            double result = 0;
            
            for(int itr = 1; itr != -1; itr--)
            {
                switch(itr)
                {
                    case 1:
                        operation1 = '*';
                        operation2 = '/';
                    break;

                    case 0:
                        operation1 = '+';
                        operation2 = '-';
                    break;
                }
                while(newExprFormat.Count(ch => ch == operation1 || ch == operation2) != 0)
                {
                    if(newExprFormat[0] == '-' && newExprFormat.Count(ch => ch == '-') == 1)
                        break;

                    for(int i = 0; i < newExprFormat.Length; i++)
                    {
                        if(_correctSymbols.Contains(newExprFormat[i]))
                        {
                            switch(itr)
                            {
                                case 1:
                                if(newExprFormat[i] == '+' || newExprFormat[i] == '-')
                                    continue;
                                break;
                                case 0:
                                if(newExprFormat[i] == '*' || newExprFormat[i] == '/')
                                    continue;
                                break;
                            }

                            StringBuilder aStr = new StringBuilder();
                            double a = 0;
                            int aStartIndex = 0;

                            StringBuilder bStr = new StringBuilder();
                            double b = 0;
                            int bEndIndex = 0;

                            string filter = string.Empty;

                            int iterator = i - 1;
                            bool isMinus = false;
                            while(iterator != -1)
                            {
                                if(char.IsNumber(newExprFormat[iterator]) || newExprFormat[iterator] == ',' || newExprFormat[iterator] == 'm')
                                {
                                    aStartIndex = iterator;
                                    if(newExprFormat[iterator] == 'm')
                                    {
                                        isMinus = true;
                                        iterator--;
                                        continue;
                                    }
                                    aStr.Append(newExprFormat[iterator]);
                                    iterator--;
                                }
                                else break;
                            }

                            a = double.Parse(new string(aStr.ToString().Reverse().ToArray()));
                            if(isMinus)
                            {
                                isMinus = false;
                                a *= -1;
                            }

                            iterator = i + 1;
                            while(iterator != newExprFormat.Length)
                            {
                                if(char.IsNumber(newExprFormat[iterator]) || newExprFormat[iterator] == ',' || newExprFormat[iterator] == 'm')
                                {
                                    bEndIndex = iterator;
                                    if(newExprFormat[iterator] == 'm')
                                    {
                                        isMinus = true;
                                        iterator++;
                                        continue;
                                    }
                                    bStr.Append(newExprFormat[iterator]);
                                    iterator++;
                                }
                                else break;
                            }

                            b = double.Parse(bStr.ToString());
                            if(isMinus) b *= -1;

                            filter = new string(newExprFormat.Skip(aStartIndex).Take(bEndIndex - aStartIndex + 1).ToArray());

                            switch(itr)
                            {
                                case 1:
                                    if(newExprFormat[i] == operation1)
                                        result = a * b;
                                    else result = result = a / b;
                                break;
                                case 0:
                                    if(newExprFormat[i] == operation1)
                                        result = a + b;
                                    else result = result = a - b;
                                break;
                            }

                            string strResault;
                            if(result < 0) strResault = 'm' + (result * -1).ToString();
                            else strResault = result.ToString();

                            newExprFormat = newExprFormat.Replace(filter, strResault);

                            break;
                        }
                    }
                }
            }

            return result > 0 ? result.ToString() : new string('m' + (Math.Abs(result)).ToString());
        }

        static string Calculate(string mathfExprString)
        {
            string result = string.Empty;
            var expressions = FindRoundBrackets(mathfExprString, out var newExressionString);
            
            if(expressions == null)
                result = GetOperationsResult(mathfExprString);
            else
            {
                var expressionsValues = expressions.Values.ToArray();
                var expressionsKeys = expressions.Keys.ToArray();

                for(int i = 0; i < expressionsValues.Length; i++)
                {
                    if(expressionsValues[i].Count(ch => !char.IsNumber(ch) && ch != 'm' && ch != ',') != 0)
                    {
                        if(expressionsValues[i].Contains('['))
                        {
                            foreach(var key in expressionsKeys)
                            {
                                if(expressionsValues[i].Contains(key))
                                    expressionsValues[i] = expressionsValues[i].Replace(key, expressions[key]);
                            }
                        }

                        expressionsValues[i] = GetOperationsResult(expressionsValues[i]).ToString();
                        expressions.Remove(expressionsKeys[i]);
                        expressions.Add(expressionsKeys[i], expressionsValues[i]);
                    }
                }

                foreach(var key in expressions.Keys)
                {
                    if(newExressionString.Contains(key))
                        newExressionString = newExressionString.Replace(key, expressions[key]);
                }

                result = GetOperationsResult(newExressionString);
            }

            if(!result.Contains('m'))
                return result;
            else
                return '-' + new string(result.Skip(1).ToArray());
        }
    }
}
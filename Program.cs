using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Calculator
{
    enum OperationType
    {
        AddOrSub,
        MulOrDiv
    }

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
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nВыражение было записано корректно.");
                    Console.ForegroundColor = ConsoleColor.White;

                    if(exprWithMinusValues != string.Empty)
                        mathfExprString = exprWithMinusValues;

                    var expressions = FindRoundBrackets(mathfExprString, out var newExressionString);

                    Console.WriteLine("\nПриступаем к вычислениям...\n");

                    if(expressions == null)
                    {
                        Console.WriteLine("Результат вычислений: " + Calculate(mathfExprString));
                        break;
                    }
                    else
                    {
                        var expressionsValues = expressions.Values.ToArray();
                        var expressionsKeys = expressions.Keys.ToArray();

                        for(int i = 0; i < expressionsValues.Length; i++)
                        {
                            if(expressionsValues[i].Count(ch => !char.IsNumber(ch) && ch != 'm' && ch != ',') != 0)
                            {
                                if(!expressionsValues[i].Contains('['))
                                {
                                    expressionsValues[i] = Calculate(expressionsValues[i]).ToString();
                                    expressions.Remove(expressionsKeys[i]);
                                    expressions.Add(expressionsKeys[i], expressionsValues[i]);
                                }
                                else
                                {
                                    foreach(var key in expressionsKeys)
                                    {
                                        if(expressionsValues[i].Contains(key))
                                        {
                                            expressionsValues[i] = expressionsValues[i].Replace(key, expressions[key]);
                                        }
                                    }

                                    expressionsValues[i] = Calculate(expressionsValues[i]).ToString();
                                    expressions.Remove(expressionsKeys[i]);
                                    expressions.Add(expressionsKeys[i], expressionsValues[i]);
                                }
                            }
                        }

                        foreach(var key in expressions.Keys)
                        {
                            if(newExressionString.Contains(key))
                            {
                                newExressionString = newExressionString.Replace(key, expressions[key]);
                            }
                        }

                        Console.WriteLine("Результат вычислений: " + Calculate(newExressionString));
                        break;
                    }
                }
            }
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

                expressions.Add($"[{exprCounter}]", new string(newExpr.Where(ch => ch != '(' && ch != ')').ToArray()));
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

        static string GetOperationsResult(string expression, OperationType operationType)
        {
            char operation1 = new char();
            char oeration2 = new char();

            string newExprFormat = expression;
            double resault = 0;

            return null;
        }

        static string Calculate(string expression)
        {
            Console.WriteLine("Выражение на входе: " + expression);

            string newExprFormat = expression;
            double result = 0;

            while(newExprFormat.Count(ch => ch == '*' || ch == '/') != 0)
            {
                if(newExprFormat[0] == '-' && newExprFormat.Count(ch => ch == '-') == 1)
                    break;
                Console.WriteLine("В выражении всё ещё есть * или / .");
                for(int i = 0; i < newExprFormat.Length; i++)
                {
                    if(_correctSymbols.Contains(newExprFormat[i]))
                    {
                        if(newExprFormat[i] == '+' || newExprFormat[i] == '-')
                            continue;

                        Console.WriteLine($"Нашёл операцию: {newExprFormat[i]}.");

                        StringBuilder aStr = new StringBuilder();
                        double a = 0;
                        int aStartIndex = 0;

                        StringBuilder bStr = new StringBuilder();
                        double b = 0;
                        int bEndIndex = 0;

                        string filter = string.Empty;

                        int iterator = i - 1;
                        bool isMinus = false;
                        while(true)
                        {
                            if(iterator == -1)
                                break;
                            else if(char.IsNumber(newExprFormat[iterator]) || newExprFormat[iterator] == ',' || newExprFormat[iterator] == 'm')
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
                        Console.WriteLine($"левая часть: {aStr.ToString()}");
                        a = double.Parse(new string(aStr.ToString().Reverse().ToArray()));
                        if(isMinus)
                        {
                            isMinus = false;
                            a *= -1;
                        }

                        iterator = i + 1;
                        while(true)
                        {
                            if(iterator == newExprFormat.Length)
                                break;
                            else if(char.IsNumber(newExprFormat[iterator]) || newExprFormat[iterator] == ',' || newExprFormat[iterator] == 'm')
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
                        Console.WriteLine($"Правая часть: {bStr.ToString()}");
                        b = double.Parse(bStr.ToString());
                        if(isMinus) b *= -1;

                        filter = new string(newExprFormat.Skip(aStartIndex).Take(bEndIndex - aStartIndex + 1).ToArray());
                        Console.WriteLine($"Полученное выражение: {filter}");

                        if(newExprFormat[i] == '*')
                            result = a * b;
                        else result = a / b;

                        string strResault;
                        if(result < 0) strResault = 'm' + (result * -1).ToString();
                        else strResault = result.ToString();

                        newExprFormat = newExprFormat.Replace(filter, strResault);
                        Console.WriteLine($"Вид выражения после выполнения операции: {newExprFormat}\n");

                        break;
                    }
                }
            }
            while(newExprFormat.Count(ch => ch == '+' || ch == '-') != 0)
            {
                if(newExprFormat[0] == '-' && newExprFormat.Count(ch => ch == '-') == 1)
                    break;
                Console.WriteLine("В выражении всё ещё есть + или - .");
                for(int i = 0; i < newExprFormat.Length; i++)
                {
                    if(_correctSymbols.Contains(newExprFormat[i]))
                    {
                        if(newExprFormat[i] == '*' || newExprFormat[i] == '/')
                            continue;
                        Console.WriteLine($"Нашёл операцию: {newExprFormat[i]}.");

                        StringBuilder aStr = new StringBuilder();
                        double a = 0;
                        int aStartIndex = 0;

                        StringBuilder bStr = new StringBuilder();
                        double b = 0;
                        int bEndIndex = 0;

                        string filter = string.Empty;

                        int iterator = i - 1;
                        bool isMinus = false;
                        while(true)
                        {
                            if(iterator == -1)
                                break;
                            else if(char.IsNumber(newExprFormat[iterator]) || newExprFormat[iterator] == ',' || newExprFormat[iterator] == 'm')
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
                        Console.WriteLine($"левая часть: {aStr.ToString()}");
                        a = double.Parse(new string(aStr.ToString().Reverse().ToArray()));
                        if(isMinus)
                        {
                            isMinus = false;
                            a *= -1;
                        }
                        
                        iterator = i + 1;
                        while(true)
                        {
                            if(iterator == newExprFormat.Length)
                                break;  
                            else if(char.IsNumber(newExprFormat[iterator]) || newExprFormat[iterator] == ',' || newExprFormat[iterator] == 'm')
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
                        Console.WriteLine($"Правая часть: {bStr.ToString()}");
                        b = double.Parse(bStr.ToString());
                        if(isMinus) b *= -1;

                        filter = new string(newExprFormat.Skip(aStartIndex).Take(bEndIndex - aStartIndex + 1).ToArray());
                        Console.WriteLine($"Полученное выражение: {filter}");

                        if(newExprFormat[i] == '+')
                            result = a + b;
                        else result = a - b;

                        string strResault;
                        if(result < 0) strResault = 'm' + (result * -1).ToString();
                        else strResault = result.ToString();

                        newExprFormat = newExprFormat.Replace(filter, strResault);
                        Console.WriteLine($"Вид выражения после выполнения операции: {newExprFormat}\n");

                        break;
                    }
                } 
            }
            return result > 0 ? result.ToString() : new string('m' + (Math.Abs(result)).ToString());
        }
    }
}
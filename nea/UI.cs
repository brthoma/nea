using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nea
{
    public class UI
    {

        public static string GetChoice(string[] options, string message)
        {
            Console.Clear();
            Console.WriteLine(message);
            Console.WriteLine();

            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine($"    {options[i]}");
            }

            int line = 2;

            while (true)
            {
                Console.SetCursorPosition(2, line);
                Console.Write(">");

                ConsoleKey keyPressed = Console.ReadKey().Key;

                switch (keyPressed)
                {
                    case ConsoleKey.DownArrow:
                        if (line < 1 + options.Length)
                        {
                            Console.SetCursorPosition(2, line);
                            Console.Write(" ");
                            line++;
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        if (line > 2)
                        {
                            Console.SetCursorPosition(2, line);
                            Console.Write(" ");
                            line--;
                        }
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        return options[line - 2];
                }
            }
        }

        public static List<string> GetChoices(string[] options, string message)
        {
            List<string> choices = new List<string>();

            string[] newOptions = new string[options.Length + 1];
            newOptions[0] = "Done";
            for (int i = 0; i < options.Length; i++)
            {
                newOptions[i + 1] = options[i];
            }

            while (true)
            {
                string choice = GetChoice(newOptions, message);
                if (choice == "Done")
                {
                    if (choices.Count() > 0) return choices;
                }

                else choices.Add(choice);
            }    
        }

        public static int GetIntInput(string message)
        {
            while (true)
            {
                Console.Write(message);
                try
                {
                    int input = int.Parse(Console.ReadLine());
                    return input;
                }
                catch
                {
                    continue;
                }
            }
        }

        public static double GetDoubleInput(string message)
        {
            while (true)
            {
                Console.Write(message);
                try
                {
                    double input = double.Parse(Console.ReadLine());
                    return input;
                }
                catch
                {
                    continue;
                }
            }
        }

        public static string GetStringInput(string message)
        {
            Console.Write(message);
            string input = Console.ReadLine();
            return input;
        }

    }
}

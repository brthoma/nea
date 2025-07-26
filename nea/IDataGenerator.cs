using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace nea
{

    public interface IDataGenerator
    {
        string GenerateData(string dictFilePath, Random random, int length);
    }


    public class WordsFromDict : IDataGenerator
    {

        public string GenerateData(string dictFilePath, Random random, int length)
        {
            string[] dictionary = File.ReadAllLines(dictFilePath);
            string text = "";

            do
            {
                text += dictionary[random.Next(dictionary.Length)] + " ";
            } while (text.Length < length);

            return text.Substring(0, length).Trim();
        }

    }


}

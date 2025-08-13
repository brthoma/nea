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
        string GenerateData(Random random, int length);
    }


    public class WordsFromDict : IDataGenerator
    {
        private string dictionaryFilePath;

        public WordsFromDict(string dictionaryFilePath)
        {
            this.dictionaryFilePath = dictionaryFilePath;
        }

        public string GenerateData(Random random, int length)
        {
            string[] dictionary = File.ReadAllLines(dictionaryFilePath);
            string text = "";

            do
            {
                text += dictionary[random.Next(dictionary.Length)] + " ";
            } while (text.Length < length);

            return text.Substring(0, length).Trim();
        }

    }

    public class TextFromCorpus : IDataGenerator
    {
        private string corpusFilePath;

        public TextFromCorpus(string corpusFilePath)
        {
            this.corpusFilePath = corpusFilePath;
        }

        public string GenerateData(Random random, int length)
        {
            string corpus = File.ReadAllText(corpusFilePath);
            int randomStart = random.Next(corpus.Length - length);
            string text = corpus.Substring(randomStart, length);

            return text;
        }

    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Dynamic;

namespace nea
{

    public interface IDataGenerator
    {
        string GenerateData(Random random, int length);
    }


    public class WordsFromDict : IDataGenerator
    {
        private string dictionaryFilePath;
        private string[] dictionary;

        public WordsFromDict(string dictionaryFilePath)
        {
            this.dictionaryFilePath = dictionaryFilePath;
            this.dictionary = File.ReadAllLines(dictionaryFilePath);
        }

        public string GenerateData(Random random, int length)
        {
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
        private string corpus;

        public TextFromCorpus(string corpusFilePath)
        {
            this.corpusFilePath = corpusFilePath;
            this.corpus = File.ReadAllText(corpusFilePath);
        }

        public string GenerateData(Random random, int length)
        {
            int randomStart = random.Next(corpus.Length - length);
            string text = corpus.Substring(randomStart, length);
            List<char> unprintables = new List<char>();

            for (int i = 0; i < length; i++)
            {
                if (char.IsControl(text[i]) || char.IsSurrogate(text[i]))
                {
                    text = text.Replace(text[i], ' ');
                }
            }

            return text;
        }

    }


    class DataGeneratorFactory
    {
        private const string DICTIONARYFILEPATH = "FilesForUse\\EnglishDictionary.txt";
        private const string CORPUSFILEPATH = "FilesForUse\\DataCorpus.txt";

        public static IDataGenerator GetDataGenerator(string dataGeneratorType)
        {
            switch (dataGeneratorType)
            {
                case "WordsFromDict":
                    return new WordsFromDict(DICTIONARYFILEPATH);
                case "TextFromCorpus":
                    return new TextFromCorpus(CORPUSFILEPATH);
                default:
                    throw new Exception("No valid data generator selected");
            }
        }
    }

}

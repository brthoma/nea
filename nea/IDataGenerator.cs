using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Dynamic;

namespace nea
{

    /* Interface definition for a DataGenerator class
     * Derived classes generate text
     */
    public interface IDataGenerator
    {
        string GenerateData(Random random, int length);
    }


    /* Returns random words from a dictionary until the character count is reached
     */
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

    /* Returns a section of text from the data corpus of the specified length
     */
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


    /* Returns the data generator object associated with the data generator type passed in
     */
    class DataGeneratorFactory
    {
        private const string DICTIONARYFILEPATH = "C:\\Users\\betha\\Code\\nea\\FilesForUse\\EnglishDictionary.txt";
        private const string CORPUSFILEPATH = "C:\\Users\\betha\\Code\\nea\\FilesForUse\\DataCorpus.txt";

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

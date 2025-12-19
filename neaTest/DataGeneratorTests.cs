using Microsoft.VisualStudio.TestTools.UnitTesting;
using nea;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace neaTest
{

    [TestClass]
    public class WordsInDictionaryTest
    {

        private Random random = new Random();

        [TestMethod]
        [DataRow()]
        public void TestTextContent(string reducedDictionaryFilePath = "C:\\Users\\betha\\Code\\nea\\neaTest\\bin\\Debug\\FilesForTesting\\SmallDictionary.txt")
        {
            for (int i = 0; i < 50; i++)
            {
                string dictionary = "";
                using (StreamReader sr = new StreamReader(reducedDictionaryFilePath))
                {
                    while (!sr.EndOfStream)
                    {
                        dictionary += sr.ReadLine();
                    }
                }

                WordsFromDict dataGenerator = new WordsFromDict(reducedDictionaryFilePath);
                string text = dataGenerator.GenerateData(random, random.Next(100));

                foreach (Match match in Regex.Matches(text, "[a-zA-Z]+"))
                {
                    Assert.IsTrue(dictionary.Contains(match.Value));
                }
            }

        }

        [TestMethod]
        [DataRow()]
        public void TestTextLength(string reducedDictionaryFilePath = "C:\\Users\\betha\\Code\\nea\\neaTest\\bin\\Debug\\FilesForTesting\\SmallDictionary.txt")
        {
            WordsFromDict dataGenerator = new WordsFromDict(reducedDictionaryFilePath);
            for (int i = 0; i < 50; i++)
            {
                int textLength = random.Next(100);
                string text = dataGenerator.GenerateData(random, textLength);

                Assert.AreEqual(textLength, text.Length, 1);
            }
        }
    }

    [TestClass]
    public class WordsInCorpusTest
    {

        private Random random = new Random();

        [TestMethod]
        [DataRow()]
        public void TestTextContent(string reducedCorpusFilePath = "C:\\Users\\betha\\Code\\nea\\neaTest\\bin\\Debug\\FilesForTesting\\SmallCorpus.txt")
        {
            for (int i = 0; i < 50; i++)
            {
                string corpus = "";
                using (StreamReader sr = new StreamReader(reducedCorpusFilePath))
                {
                    while (!sr.EndOfStream)
                    {
                        corpus += sr.ReadLine();
                    }
                }

                TextFromCorpus dataGenerator = new TextFromCorpus(reducedCorpusFilePath);
                string text = dataGenerator.GenerateData(random, random.Next(613));

                foreach (Match match in Regex.Matches(text, "[a-zA-Z]+"))
                {
                    Assert.IsTrue(corpus.Contains(match.Value));
                }
            }

        }

        [TestMethod]
        [DataRow()]
        public void TestTextLength(string reducedCorpusFilePath = "C:\\Users\\betha\\Code\\nea\\neaTest\\bin\\Debug\\FilesForTesting\\SmallCorpus.txt")
        {
            TextFromCorpus dataGenerator = new TextFromCorpus(reducedCorpusFilePath);
            for (int i = 0; i < 50; i++)
            {
                int textLength = random.Next(613);
                string text = dataGenerator.GenerateData(random, textLength);

                Assert.AreEqual(textLength, text.Length, 1);
            }
        }
    }

}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using nea;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace neaTest
{

    public class CryptanalysisTestInputs
    {
        public string[] inputs = new string[]
        {
            "He did not look at the ladies as he spoke, but his voice was perplexed and sorrowful. Lucy, too, was perplexed; but she saw that they were in for what is known as \"quite a scene,\" and she had an odd feeling that whenever these ill-bred tourists spoke the contest widened and deepened till it dealt, not with rooms and views, but with—well, with something quite different, whose existence she had not realized before. Now the old man attacked Miss Bartlett almost violently: Why should she not change? What possible objection had she? They would clear out in half an hour.",
            "It is a truth universally acknowledged, that a single man in possession of a good fortune must be in want of a wife. However little known the feelings or views of such a man may be on his first entering a neighbourhood, this truth is so well fixed in the minds of the surrounding families, that he is considered as the rightful property of some one or other of their daughters. \"My dear Mr. Bennet,\" said his lady to him one day, \"have you heard that Netherfield Park is let at last?\" Mr. Bennet replied that he had not.",
            "Who that cares much to know the history of man, and how the mysterious mixture behaves under the varying experiments of Time, has not dwelt, at least briefly, on the life of Saint Theresa, has not smiled with some gentleness at the thought of the little girl walking forth one morning hand-in-hand with her still smaller brother, to go and seek martyrdom in the country of the Moors?",
            "All day long we seemed to dawdle through a country which was full of beauty of every kind. Sometimes we saw little towns or castles on the top of steep hills such as we see in old missals; sometimes we ran by rivers and streams which seemed from the wide stony margin on each side of them to be subject to great floods. It takes a lot of water, and running strong, to sweep the outside edge of a river clear. At every station there were groups of people, sometimes crowds, and in all sorts of attire. Some of them were just like the peasants at home or those I saw coming through France and Germany",
            "External heat and cold had little influence on Scrooge. No warmth could warm, no wintry weather chill him. No wind that blew was bitterer than he, no falling snow was more intent upon its purpose, no pelting rain less open to entreaty. Foul weather didn't know where to have him. The heaviest rain, and snow, and hail, and sleet, could boast of the advantage over him in only one respect. They often \"came down\" handsomely, and Scrooge never did."
        };
    }


    [TestClass]
    public class ROT13CryptanalysisTest
    {
        private Random random = new Random();
        private CryptanalysisTestInputs inputs = new CryptanalysisTestInputs();

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public void TestCryptanalysis(int textIdx)
        {
            string expectedPlaintext = inputs.inputs[textIdx];
            ROT13Cryptanalysis cryptanalysis = new ROT13Cryptanalysis();
            ROT13 cipher = new ROT13();
            bool success = false;

            string ciphertext = cipher.Encrypt(expectedPlaintext, cipher.GetRandomKey(random));

            foreach (byte[] key in cryptanalysis.GetKeys(ciphertext))
            {
                if (cipher.Decrypt(ciphertext, key) == expectedPlaintext)
                {
                    success = true;
                    break;
                }
            }

            Assert.IsTrue(success);
        }

    }

    [TestClass]
    public class ROT13FasterCryptanalysisTest
    {
        private Random random = new Random();
        private CryptanalysisTestInputs inputs = new CryptanalysisTestInputs();

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public void TestCryptanalysis(int textIdx)
        {
            string expectedPlaintext = inputs.inputs[textIdx];
            FasterROT13Cryptanalysis cryptanalysis = new FasterROT13Cryptanalysis();
            ROT13 cipher = new ROT13();
            bool success = false;

            string ciphertext = cipher.Encrypt(expectedPlaintext, cipher.GetRandomKey(random));

            foreach (byte[] key in cryptanalysis.GetKeys(ciphertext))
            {
                if (cipher.Decrypt(ciphertext, key) == expectedPlaintext)
                {
                    success = true;
                    break;
                }
            }

            Assert.IsTrue(success);
        }

    }

    [TestClass]
    public class ROT47CryptanalysisTest
    {
        private Random random = new Random();
        private CryptanalysisTestInputs inputs = new CryptanalysisTestInputs();

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public void TestCryptanalysis(int textIdx)
        {
            string expectedPlaintext = inputs.inputs[textIdx];
            ROT47Cryptanalysis cryptanalysis = new ROT47Cryptanalysis();
            ROT47 cipher = new ROT47();
            bool success = false;

            string ciphertext = cipher.Encrypt(expectedPlaintext, cipher.GetRandomKey(random));

            foreach (byte[] key in cryptanalysis.GetKeys(ciphertext))
            {
                if (cipher.Decrypt(ciphertext, key) == expectedPlaintext)
                {
                    success = true;
                    break;
                }
            }

            Assert.IsTrue(success);
        }

    }

    [TestClass]
    public class XORCryptanalysisTest
    {
        private Random random = new Random();
        private CryptanalysisTestInputs inputs = new CryptanalysisTestInputs();

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public void TestCryptanalysis(int textIdx)
        {
            string expectedPlaintext = inputs.inputs[textIdx];
            XORCryptanalysis cryptanalysis = new XORCryptanalysis();
            XOR cipher = new XOR();
            bool success = false;

            string ciphertext = cipher.Encrypt(expectedPlaintext, cipher.GetRandomKey(random));

            foreach (byte[] key in cryptanalysis.GetKeys(ciphertext))
            {
                if (cipher.Decrypt(ciphertext, key) == expectedPlaintext)
                {
                    success = true;
                    break;
                }
            }

            Assert.IsTrue(success);
        }
    }

    [TestClass]
    public class VigenereCryptanalysisTest
    {
        private Random random = new Random();
        CryptanalysisTestInputs inputs = new CryptanalysisTestInputs();

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public void TestCryptanalysis(int textIdx)
        {
            string expectedPlaintext = inputs.inputs[textIdx];
            VigenereCryptanalysis cryptanalysis = new VigenereCryptanalysis();
            Vigenere cipher = new Vigenere();
            bool success = false;

            string ciphertext = cipher.Encrypt(expectedPlaintext, cipher.GetRandomKey(random));

            foreach (byte[] key in cryptanalysis.GetKeys(ciphertext))
            {
                if (cipher.Decrypt(ciphertext, key) == expectedPlaintext)
                {
                    success = true;
                    break;
                }
            }

            Assert.IsTrue(success);
        }
    }

    [TestClass]
    public class SubstitutionCryptanalysisTest
    {
        private Random random = new Random();
        CryptanalysisTestInputs inputs = new CryptanalysisTestInputs();

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public void TestCryptanalysis(int textIdx)
        {
            string expectedPlaintext = inputs.inputs[textIdx];
            SubstitutionCryptanalysis cryptanalysis = new SubstitutionCryptanalysis("C:\\Users\\betha\\Code\\nea\\FilesForUse\\EnglishDictionary.txt");
            Substitution cipher = new Substitution();
            bool success = false;

            string ciphertext = cipher.Encrypt(expectedPlaintext, cipher.GetRandomKey(random));

            foreach (byte[] key in cryptanalysis.GetKeys(ciphertext))
            {
                if (cipher.Decrypt(ciphertext, key) == expectedPlaintext)
                {
                    success = true;
                    break;
                }
            }

            Assert.IsTrue(success);
        }
    }

}

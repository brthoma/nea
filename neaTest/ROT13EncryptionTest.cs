using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using nea;
using System.ComponentModel;

namespace neaTest
{

    [TestClass]
    public class ROT13EncryptionTest
    {

        [TestMethod]
        [DataRow("", "")]
        [DataRow("abcdefghijklmnopqrstuvwxyz", "bcdefghijklmnopqrstuvwxyza")]
        [DataRow("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "BCDEFGHIJKLMNOPQRSTUVWXYZA")]
        [DataRow("1234567890!£$%^&*() ~,.\t\n\r", "1234567890!£$%^&*() ~,.\t\n\r")]
        public void TestVaryingPlaintext(string plaintext, string expected)
        {
            ROT13 cipher = new ROT13();
            byte[] key = BitConverter.GetBytes(1);
            string encrypted = cipher.Encrypt(plaintext, key);
            Assert.AreEqual(expected, encrypted);
        }

        [TestMethod]
        [DataRow(0, "The quick, brown fox jumps over the lazy dog!")]
        [DataRow(26, "The quick, brown fox jumps over the lazy dog!")]
        [DataRow(1, "Uif rvjdl, cspxo gpy kvnqt pwfs uif mbaz eph!")]
        [DataRow(25, "Sgd pthbj, aqnvm enw itlor nudq sgd kzyx cnf!")]
        [DataRow(31, "Ymj vznhp, gwtbs ktc ozrux tajw ymj qfed itl!")]
        [DataRow(13, "Gur dhvpx, oebja sbk whzcf bire gur ynml qbt!")]
        public void TestVaryingKeys(int intKey, string expected, string plaintext = "The quick, brown fox jumps over the lazy dog!")
        {
            ROT13 cipher = new ROT13();
            byte[] key = BitConverter.GetBytes(intKey);
            string encrypted = cipher.Encrypt(plaintext, key);
            Assert.AreEqual(expected, encrypted);
        }

    }

    [TestClass]
    public class ROT13DecryptionTest
    {

        [TestMethod]
        [DataRow("")]
        [DataRow("abcdefghijklmnopqrstuvwxyz")]
        [DataRow("ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        [DataRow("1234567890!£$%^&*() ~,.\t\n\r")]
        public void TestVaryingPlaintext(string plaintext)
        {
            ROT13 cipher = new ROT13();
            byte[] key = BitConverter.GetBytes(1);
            string encrypted = cipher.Encrypt(plaintext, key);
            string decrypted = cipher.Decrypt(encrypted, key);
            Assert.AreEqual(plaintext, decrypted);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(26)]
        [DataRow(1)]
        [DataRow(25)]
        [DataRow(31)]
        public void TestVaryingKeys(int intKey, string plaintext = "The quick, brown fox jumps over the lazy dog!")
        {
            ROT13 cipher = new ROT13();
            byte[] key = BitConverter.GetBytes(intKey);
            string encrypted = cipher.Encrypt(plaintext, key);
            string decrypted = cipher.Decrypt(encrypted, key);
            Assert.AreEqual(plaintext, decrypted);
        }

    }

    [TestClass]
    public class ROT13GetKeyTest
    {

        [TestMethod]
        public void TestGetRandomKey()
        {
            Random random = new Random();
            ROT13 cipher = new ROT13();
            byte[] key = cipher.GetRandomKey(random);
            int intKey = BitConverter.ToInt32(key, 0);
            Assert.IsTrue(intKey > 0 && intKey < 26);
        }

    }

    [TestClass]
    public class ROT47EncryptionTest
    {

        [TestMethod]
        [DataRow("", "")]
        [DataRow("abcdefghijklmnopqrstuvwxyz", "bcdefghijklmnopqrstuvwxyz{")]
        [DataRow("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "BCDEFGHIJKLMNOPQRSTUVWXYZ[")]
        [DataRow("1234567890!£$%^&*() ~,.\t\n\r", "23456789:1\"£%&_'+)* !-/\t\n\r")]
        public void TestVaryingPlaintext(string plaintext, string expected)
        {
            ROT47 cipher = new ROT47();
            byte[] key = BitConverter.GetBytes(1);
            string encrypted = cipher.Encrypt(plaintext, key);
            Assert.AreEqual(expected, encrypted);
        }

        [TestMethod]
        [DataRow(0, "The quick, brown fox jumps over the lazy dog!~")]
        [DataRow(94, "The quick, brown fox jumps over the lazy dog!~")]
        [DataRow(1, "Uif rvjdl- cspxo gpy kvnqt pwfs uif mb{z eph\"!")]
        [DataRow(93, "Sgd pthbj+ aqnvm enw itlor nudq sgd k`yx cnf~}")]
        [DataRow(102, "\\pm y}qks4 jzw!v nw\" r}ux{ w~mz |pm ti$# lwo)(")]
        [DataRow(47, "%96 BF:4<[ 3C@H? 7@I ;F>AD @G6C E96 =2KJ 5@8PO")]
        public void TestVaryingKeys(int intKey, string expected, string plaintext = "The quick, brown fox jumps over the lazy dog!~")
        {
            ROT13 cipher = new ROT13();
            byte[] key = BitConverter.GetBytes(intKey);
            string encrypted = cipher.Encrypt(plaintext, key);
            Assert.AreEqual(expected, encrypted);
        }

    }

}

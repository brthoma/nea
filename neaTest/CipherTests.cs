using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using nea;
using System.ComponentModel;
using System.Text;
using System.Linq;

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
        [DataRow(13)]
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
            ROT47 cipher = new ROT47();
            byte[] key = BitConverter.GetBytes(intKey);
            string encrypted = cipher.Encrypt(plaintext, key);
            Assert.AreEqual(expected, encrypted);
        }

    }


    [TestClass]
    public class ROT47DecryptionTest
    {

        [TestMethod]
        [DataRow("")]
        [DataRow("abcdefghijklmnopqrstuvwxyz")]
        [DataRow("ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        [DataRow("1234567890!£$%^&*() ~,.\t\n\r")]
        public void TestVaryingPlaintext(string plaintext)
        {
            ROT47 cipher = new ROT47();
            byte[] key = BitConverter.GetBytes(1);
            string encrypted = cipher.Encrypt(plaintext, key);
            string decrypted = cipher.Decrypt(encrypted, key);
            Assert.AreEqual(plaintext, decrypted);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(94)]
        [DataRow(1)]
        [DataRow(93)]
        [DataRow(102)]
        [DataRow(47)]
        public void TestVaryingKeys(int intKey, string plaintext = "The quick, brown fox jumps over the lazy dog!")
        {
            ROT47 cipher = new ROT47();
            byte[] key = BitConverter.GetBytes(intKey);
            string encrypted = cipher.Encrypt(plaintext, key);
            string decrypted = cipher.Decrypt(encrypted, key);
            Assert.AreEqual(plaintext, decrypted);
        }

    }


    [TestClass]
    public class ROT47GetKeyTest
    {

        [TestMethod]
        public void TestGetRandomKey()
        {
            Random random = new Random();
            ROT47 cipher = new ROT47();
            byte[] key = cipher.GetRandomKey(random);
            int intKey = BitConverter.ToInt32(key, 0);
            Assert.IsTrue(intKey > 0 && intKey < 94);
        }

    }


    [TestClass]
    public class XOREncryptionTest
    {

        [TestMethod]
        [DataRow("", "")]
        [DataRow("abcdefghijklmnopqrstuvwxyz", "   %\'%&**+)/,,,13126675;88")]
        [DataRow("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "\0\0\0\x05\x07\x05\x06\n\n\x0b\t\x0f\f\f\f\x11\x13\x11\x12\x16\x16\x17\x15\x1b\x18\x18")]
        [DataRow("1234567890!£$%^&*() ~,.\t\n\r", "pppuwuvzzqcàeg\x1dghkhb=mlJKO")]
        public void TestVaryingPlaintext(string plaintext, string expected)
        {
            XOR cipher = new XOR();
            byte[] key = Encoding.UTF8.GetBytes("ABC");
            string encrypted = cipher.Encrypt(plaintext, key);
            Assert.AreEqual(expected, encrypted);
        }

        [TestMethod]
        [DataRow("A", "\x15)$a04(\"*ma#3.6/a\'.9a+4,12a.7$3a5)$a- ;8a%.&`")]
        [DataRow("AB", "\x15*$b07(!*na 3-6,a$.:a(4/11a-7\'3b5*$b-#;;a&.%`")]
        [DataRow("Z", "\x0e" + "2?z+/391vz8(5-4z<5\"z0/7*)z5,?(z.2?z6; #z>5={")]
        [DataRow("XNZ", "\f&?x?/1-1tn8*!-6n<76z2;7(=z78?*n.0+z4/ !n>7){")]
        [DataRow("CIPHER", "\x17!5h4\'**;de01&\'&e4,1p\"0?3:p\'371i$  r/(*1e6,.q")]
        public void TestVaryingKeys(string strKey, string expected, string plaintext = "The quick, brown fox jumps over the lazy dog!")
        {
            XOR cipher = new XOR();
            byte[] key = Encoding.UTF8.GetBytes(strKey);
            string encrypted = cipher.Encrypt(plaintext, key);
            Assert.AreEqual(expected, encrypted);
        }

    }

    [TestClass]
    public class XORDecryptionTest
    {

        [TestMethod]
        [DataRow("")]
        [DataRow("abcdefghijklmnopqrstuvwxyz")]
        [DataRow("ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        [DataRow("1234567890!£$%^&*() ~,.\t\n\r")]
        public void TestVaryingPlaintext(string plaintext)
        {
            XOR cipher = new XOR();
            byte[] key = Encoding.UTF8.GetBytes("ABC");
            string encrypted = cipher.Encrypt(plaintext, key);
            string decrypted = cipher.Decrypt(encrypted, key);
            Assert.AreEqual(plaintext, decrypted);
        }

        [TestMethod]
        [DataRow("A")]
        [DataRow("AB")]
        [DataRow("Z")]
        [DataRow("XNZ")]
        [DataRow("CIPHER")]
        public void TestVaryingKeys(string strKey, string plaintext = "The quick, brown fox jumps over the lazy dog!")
        {
            XOR cipher = new XOR();
            byte[] key = Encoding.UTF8.GetBytes(strKey);
            string encrypted = cipher.Encrypt(plaintext, key);
            string decrypted = cipher.Decrypt(encrypted, key);
            Assert.AreEqual(plaintext, decrypted);
        }

    }

    [TestClass]
    public class XORGetKeyTest
    {
        private int maxKeyLength = 6;

        [TestMethod]
        public void TestGetRandomKey()
        {
            Random random = new Random();
            XOR cipher = new XOR();
            byte[] key = cipher.GetRandomKey(random);
            string strKey = Encoding.UTF8.GetString(key);

            bool validChars = true;
            foreach (char c in strKey)
            {
                if (c < 'A' || c > 'Z')
                {
                    validChars = false;
                    break;
                }
            }

            Assert.IsTrue(strKey.Length >= 1 && strKey.Length <= maxKeyLength && validChars);
        }

    }


    [TestClass]
    public class VigenereEncryptionTest
    {

        [TestMethod]
        [DataRow("", "")]
        [DataRow("abcdefghijklmnopqrstuvwxyz abc zyx", "acedfhgikjlnmoqprtsuwvxzya cbd byy")]
        [DataRow("ABCDEFGHIJKLMNOPQRSTUVWXYZ ABC ZYX", "ACEDFHGIKJLNMOQPRTSUWVXZYA CBD BYY")]
        [DataRow("1234567890!£$%^&*() ~,.\t\n\r", "1234567890!£$%^&*() ~,.\t\n\r")]
        public void TestVaryingPlaintext(string plaintext, string expected)
        {
            Vigenere cipher = new Vigenere();
            byte[] key = Encoding.UTF8.GetBytes("ABC");
            string encrypted = cipher.Encrypt(plaintext, key);
            Assert.AreEqual(expected, encrypted);
        }

        [TestMethod]
        [DataRow("A", "The quick, brown fox jumps over the lazy dog!")]
        [DataRow("AB", "Tie rujcl, bsoxn goy jvmqs pvfr uhf lbzz dpg!")]
        [DataRow("Z", "Sgd pthbj, aqnvm enw itlor nudq sgd kzyx cnf!")]
        [DataRow("XNZ", "Qud nhhzx, aobvk snu wtjcr lido ggb yzwl clt!")]
        [DataRow("CIPHER", "Vpt xyzes, qysnp nde nloxh vzvt bwl prbg svk!"
)]
        public void TestVaryingKeys(string strKey, string expected, string plaintext = "The quick, brown fox jumps over the lazy dog!")
        {
            Vigenere cipher = new Vigenere();
            byte[] key = Encoding.UTF8.GetBytes(strKey);
            string encrypted = cipher.Encrypt(plaintext, key);
            Assert.AreEqual(expected, encrypted);
        }

    }

    [TestClass]
    public class VigenereDecryptionTest
    {

        [TestMethod]
        [DataRow("")]
        [DataRow("abcdefghijklmnopqrstuvwxyz abc zyx")]
        [DataRow("ABCDEFGHIJKLMNOPQRSTUVWXYZ ABC ZYX")]
        [DataRow("1234567890!£$%^&*() ~,.\t\n\r")]
        public void TestVaryingPlaintext(string plaintext)
        {
            Vigenere cipher = new Vigenere();
            byte[] key = Encoding.UTF8.GetBytes("ABC");
            string encrypted = cipher.Encrypt(plaintext, key);
            string decrypted = cipher.Decrypt(encrypted, key);
            Assert.AreEqual(plaintext, decrypted);
        }

        [TestMethod]
        [DataRow("A")]
        [DataRow("AB")]
        [DataRow("Z")]
        [DataRow("XAM")]
        [DataRow("CIPHER")]
        public void TestVaryingKeys(string strKey, string plaintext = "The quick, brown fox jumps over the lazy dog!")
        {
            Vigenere cipher = new Vigenere();
            byte[] key = Encoding.UTF8.GetBytes(strKey);
            string encrypted = cipher.Encrypt(plaintext, key);
            string decrypted = cipher.Decrypt(encrypted, key);
            Assert.AreEqual(plaintext, decrypted);
        }

    }


    [TestClass]
    public class VigenereGetKeyTest
    {

        [TestMethod]
        public void TestGetRandomKey()
        {
            Random random = new Random();
            Vigenere cipher = new Vigenere();
            byte[] key = cipher.GetRandomKey(random);
            string strKey = Encoding.UTF8.GetString(key);

            Assert.IsTrue(strKey.Length >= 1 && strKey.Length <= 6);

            foreach (char c in strKey)
            {
                if (c < 'A' || c > 'Z')
                {
                    Assert.Fail();
                }
            }

        }
    }


    [TestClass]
    public class SubstitutionEncryptionTest
    {

        [TestMethod]
        [DataRow("", "")]
        [DataRow("abcdefghijklmnopqrstuvwxyz", "quozsravgkjxmfenhdibtwclyp")]
        [DataRow("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "QUOZSRAVGKJXMFENHDIBTWCLYP")]
        [DataRow("1234567890!£$%^&*() ~,.\t\n\r", "1234567890!\xa3$%^&*() ~,.\t\n\r")]
        public void TestVaryingPlaintext(string plaintext, string expected)
        {
            Substitution cipher = new Substitution();
            byte[] key = Encoding.UTF8.GetBytes("quozsravgkjxmfenhdibtwclyp");
            string encrypted = cipher.Encrypt(plaintext, key);
            Assert.AreEqual(expected, encrypted);
        }

        [TestMethod]
        [DataRow("quozsravgkjxmfenhdibtwclyp", "Bvs htgoj, udecf rel ktmni ewsd bvs xqpy zea!")]
        [DataRow("mcypgfzaiqbxknvsolrhwudtje", "Hag owiyb, clvdn fvt qwksr vugl hag xmej pvz!")]
        [DataRow("tgedfxuhmckiqvjazybrlonpws", "Rhf zlmek, gyjnv xjp clqab jofy rhf itsw dju!")]
        public void TestVaryingKeys(string strKey, string expected, string plaintext = "The quick, brown fox jumps over the lazy dog!")
        {
            Substitution cipher = new Substitution();
            byte[] key = Encoding.UTF8.GetBytes(strKey);
            string encrypted = cipher.Encrypt(plaintext, key);
            Assert.AreEqual(expected, encrypted);
        }

    }

    [TestClass]
    public class SubstitutionDecryptionTest
    {

        [TestMethod]
        [DataRow("")]
        [DataRow("abcdefghijklmnopqrstuvwxyz")]
        [DataRow("ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        [DataRow("1234567890!£$%^&*() ~,.\t\n\r")]
        public void TestVaryingPlaintext(string plaintext)
        {
            Substitution cipher = new Substitution();
            byte[] key = Encoding.UTF8.GetBytes("quozsravgkjxmfenhdibtwclyp");
            string encrypted = cipher.Encrypt(plaintext, key);
            string decrypted = cipher.Decrypt(encrypted, key);
            Assert.AreEqual(plaintext, decrypted);
        }

        [TestMethod]
        [DataRow("quozsravgkjxmfenhdibtwclyp")]
        [DataRow("mcypgfzaiqbxknvsolrhwudtje")]
        [DataRow("tgedfxuhmckiqvjazybrlonpws")]
        public void TestVaryingKeys(string strKey, string plaintext = "The quick, brown fox jumps over the lazy dog!")
        {
            Vigenere cipher = new Vigenere();
            byte[] key = Encoding.UTF8.GetBytes(strKey);
            string encrypted = cipher.Encrypt(plaintext, key);
            string decrypted = cipher.Decrypt(encrypted, key);
            Assert.AreEqual(plaintext, decrypted);
        }

    }

    [TestClass]
    public class SubstitutionGetKeyTest
    {

        [TestMethod]
        public void TestGetRandomKey()
        {
            Random random = new Random();
            Substitution cipher = new Substitution();
            byte[] key = cipher.GetRandomKey(random);
            string strKey = Encoding.UTF8.GetString(key);

            Assert.IsTrue(strKey.Length == 26);

            foreach (char c in "abcdefghijklmnopqrstuvwxyz")
            {
                if (! strKey.Contains(c))
                {
                    Assert.Fail();
                }
            }

        }
    }

}

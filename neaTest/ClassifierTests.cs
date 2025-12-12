using Microsoft.VisualStudio.TestTools.UnitTesting;
using nea;
using System;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace neaTest
{

    public class TextInputs
    {
        
        public string[] inputs =
        {
            "Alice was beginning to get very tired of sitting by her sister on the bank, and of having nothing to do: once or twice she had peeped into the book her sister was reading, but it had no pictures or conversations in it, \"and what is the use of a book,\" thought Alice \"without pictures or conversations?\" So she was considering in her own mind (as well as she could, for the hot day made her feel very sleepy and stupid),",
            "Mr. Utterson the lawyer was a man of a rugged countenance that was\r\nnever lighted by a smile; cold, scanty and embarrassed in discourse;\r\nbackward in sentiment; lean, long, dusty, dreary and yet somehow\r\nlovable. At friendly meetings, and when the wine was to his taste,\r\nsomething eminently human beaconed from his eye; something indeed which\r\nnever found its way into his talk",
            "This was not because he was cowardly and abject, quite the contrary; but for some time past he had been in an overstrained irritable condition, verging on hypochondria. He had become so completely absorbed in himself, and isolated from his fellows that he dreaded meeting, not only his landlady, but anyone at all. He was crushed by poverty, but the anxieties of his position had of late ceased to weigh upon him. He had given up attending to matters of practical importance; he had lost all desire to do so.",
            ".Q'*\u0019\u0010\u001f\v[\u0018/fw\r\nu\u0017\u0006\u001erUVH=c\u0016@\u001a\"~\u00173jYMv i@HN}\u0016-K:o \u0010\u0002U(JsFY\u0019(- D\u007f8\u001djUc\u0015\u0001S\u001bk4bSlh?*\fF\u0002A\u001a\r\nz.{a\tb.\t1\f}\u0019/|\u001a\u001d\u00137N\u0006WN\u001dU\u0013\u0018\u0013\u001eJ;X.\u0017D@5q\u0013b\bv]$j\vb]#m1\r\nD\fyyX\bd\u000ecrymE2]D[\r\n\u0013\u0015b\u000f F\u001a1\"RE\v\\5+D/\b\u0019\u001e\u0005\"m:h\u0012*[}'WNH,F06",
            "T.:e[DOunEhT;<U6d E\u007fi/XbclGOMiul|X9%{L{rXs=AC:+-o{HJSWC=J%OQ#}.A#N_:pbFpSBk5&t*X`qXQ.'08nw`>;/^X-#A&?6R36)\\,& uIepR\u007fg2z!h#`36~oEvcB}$'sX?<I\u007fClsaoQwpb'~x8}hd6K\"z7XI@\\Pk;s9/Qm>ZSTc|R4rJb`H/i}'b6WX(H!%-=!_.}h1&e{X3:%,jVx%qL@we^Ug#ky.z*P<2tM`)f^lp 3iU]=f7 E}OHC|v7Ceq#+1d\\V&W\"s<!?slhWP?$D,x)e7.}|,DXA_S8PqhiI]}/P+q7|:vdHmqC56\\,'\\sPic}saf138k#x7$\"0K@kgTR{s<' ?fe-x~y4!/R~T0_16&p2'W8SC&B3*XfScR>ZaD`N8/ N=&r~U|k{",
            "Ur jnf fb onqyl qerffrq gung rira in zna npphfgbzrq gb funoovarff jbhyq\r\nunir or ra nfunzrq gb or frra va gur fgerrg va fhpu entf. Va gung dhnegre\r\nbs gur gbja, ubjrire, fpnepryl nal fubegpbzvat va qerff jbhyq unir\r\nperngrq fhecevfr. Bjvat gb gur cebkvzvgl bs gur Unl Znexrg, gur ahzore\r\nbs rfgnoyvfuzragf bs onq punenpgre, gur cercbaqrenapr bs gur genqvat\r\nnaq jbexvat pynff cbchyngvba pebjqrq va gurfr fgerrgf naq nyyrlf va gur\r\nurneg bs Crgrefohet,",
            "a"
        };

    }


    [TestClass]
    public class RandomGuesserTest
    {
        private TextInputs inputs = new TextInputs();

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        public void TestRange(int textIdx)
        {
            RandomGuesser classifier = new RandomGuesser();
            double classification = classifier.Classify(inputs.inputs[textIdx]);

            Assert.IsTrue(classification >= 0 && classification <= 1);
        }
    }

    [TestClass]
    public class ComparisonWithDictionaryTest
    {
        private TextInputs inputs = new TextInputs();

        [TestMethod]
        [DataRow(0, 0.2345679012)]
        [DataRow(1, 0.2063492063)]
        [DataRow(2, 0.1555555556)]
        [DataRow(3, 0.04255319149)]
        [DataRow(4, 0.01886792453)]
        [DataRow(5, 0.01298701299)]
        [DataRow(6, 1)]
        public void TestProcess(int textIdx, double expectedValue, string reducedDictionaryFilePath = "C:\\Users\\betha\\Code\\nea\\neaTest\\bin\\Debug\\FilesForTesting\\SmallDictionary.txt")
        {
            DictionaryLookup classifier = new DictionaryLookup(reducedDictionaryFilePath);
            double classification = classifier.Classify(inputs.inputs[textIdx]);

            Assert.AreEqual(expectedValue, classification, 5e-6);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        public void TestRange(int textIdx, string reducedDictionaryFilePath = "C:\\Users\\betha\\Code\\nea\\neaTest\\bin\\Debug\\FilesForTesting\\SmallDictionary.txt")
        {
            DictionaryLookup classifier = new DictionaryLookup(reducedDictionaryFilePath);
            double classification = classifier.Classify(inputs.inputs[textIdx]);

            Assert.IsTrue(classification >= 0 && classification <= 1);
        }
    }

    [TestClass]
    public class ProportionPrintableTest
    {
        private TextInputs inputs = new TextInputs();

        [TestMethod]
        [DataRow(0, 1)]
        [DataRow(1, 0.9735449735)]
        [DataRow(2, 1)]
        [DataRow(3, 0.6855670103)]
        [DataRow(4, 0.9926108374)]
        [DataRow(5, 0.9733924612)]
        [DataRow(6, 1)]
        public void TestProcess(int textIdx, double expectedValue)
        {
            ProportionPrintable classifier = new ProportionPrintable();
            double classification = classifier.Classify(inputs.inputs[textIdx]);

            Assert.AreEqual(expectedValue, classification, 5e-6);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        public void TestRange(int textIdx)
        {
            ProportionPrintable classifier = new ProportionPrintable();
            double classification = classifier.Classify(inputs.inputs[textIdx]);

            Assert.IsTrue(classification >= 0 && classification <= 1);
        }
    }

    [TestClass]
    public class EntropyTest
    {
        private TextInputs inputs = new TextInputs();

        [TestMethod]
        [DataRow(0, 4.190289677057744)]
        [DataRow(1, 4.323039262189477)]
        [DataRow(2, 4.2193690214071795)]
        [DataRow(3, 6.367068594947861)]
        [DataRow(4, 6.411103594547121)]
        [DataRow(5, 4.328404211446752)]
        [DataRow(6, 0)]
        public void TestProcess(int testIdx, double expectedEntropy)
        {
            Entropy classifier = new Entropy();
            double classification = classifier.Classify(inputs.inputs[testIdx]);

            double expectedPValue;
            if (expectedEntropy <= 4.14)
            {
                expectedPValue = 1 - (4.14 - expectedEntropy) / 4.14;
            }
            else
            {
                expectedPValue = 1 - (expectedEntropy - 4.14) / (8 - 4.14);
            }

            Assert.AreEqual(expectedPValue, classification, 5e-6);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        public void TestRange(int textIdx, string reducedDictionaryFilePath = "C:\\Users\\betha\\Code\\nea\\neaTest\\bin\\Debug\\FilesForTesting\\SmallDictionary.txt")
        {
            DictionaryLookup classifier = new DictionaryLookup(reducedDictionaryFilePath);
            double classification = classifier.Classify(inputs.inputs[textIdx]);

            Assert.IsTrue(classification >= 0 && classification <= 1);
        }
    }

    [TestClass]
    public class FreqAnalysisTest
    {
        private TextInputs inputs = new TextInputs();

        [TestMethod]
        [DataRow(0, new double[] {21, 6, 10, 15, 38, 5, 9, 20, 28, 0, 3, 7, 2, 26, 30, 6, 0, 19, 25, 29, 8, 5, 8, 0, 5, 0})]
        [DataRow(1, new double[] {27, 5, 8, 16, 39, 4, 7, 14, 19, 0, 2, 11, 12, 30, 16, 0, 0, 14, 21, 24, 7, 3, 10, 0, 10, 0})]
        [DataRow(2, new double[] {37, 11, 13, 23, 48, 7, 6, 25, 29, 1, 0, 18, 10, 27, 35, 9, 1, 19, 24, 36, 8, 4, 6, 1, 9, 0})]
        [DataRow(3, new double[] {2, 5, 3, 6, 2, 5, 0, 5, 1, 5, 2, 1, 4, 4, 1, 0, 2, 3, 3, 0, 5, 3, 3, 2, 5, 1})]
        [DataRow(4, new double[] {7, 8, 12, 8, 11, 6, 4, 11, 10, 4, 8, 7, 4, 5, 7, 12, 10, 9, 15, 7, 7, 5, 8, 17, 2, 5})]
        [DataRow(5, new double[] {24, 24, 7, 1, 26, 29, 34, 9, 5, 8, 1, 6, 0, 31, 9, 12, 16, 50, 6, 6, 23, 16, 0, 2, 9, 8})]
        [DataRow(6, new double[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0})]
        public void TestProcess(int textIdx, double[] trueFreqs)
        {
            FrequencyAnalysis classifier = new FrequencyAnalysis();

            int numLetters = inputs.inputs[textIdx].Count(c => "abcdefghijklmnopqrstuvwxyz".Contains(char.ToLower(c)));

            double[] freqs = classifier.GetObservedFreqs(inputs.inputs[textIdx]);

            Assert.AreEqual(trueFreqs.Length, freqs.Length);

            for (int i = 0; i < trueFreqs.Length; i++)
            {
                Assert.AreEqual(trueFreqs[i] * 100 / numLetters, freqs[i], 5e-6);
            }
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        public void TestRange(int textIdx, string reducedDictionaryFilePath = "C:\\Users\\betha\\Code\\nea\\neaTest\\bin\\Debug\\FilesForTesting\\SmallDictionary.txt")
        {
            DictionaryLookup classifier = new DictionaryLookup(reducedDictionaryFilePath);
            double classification = classifier.Classify(inputs.inputs[textIdx]);

            Assert.IsTrue(classification >= 0 && classification <= 1);
        }
    }

    


}

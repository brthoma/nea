using Microsoft.VisualStudio.TestTools.UnitTesting;
using nea;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace neaTest
{

    public class ClassifierTestInputs
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
        private ClassifierTestInputs inputs = new ClassifierTestInputs();

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
        private ClassifierTestInputs inputs = new ClassifierTestInputs();

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
        private ClassifierTestInputs inputs = new ClassifierTestInputs();

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
        private ClassifierTestInputs inputs = new ClassifierTestInputs();

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
        public void TestRange(int textIdx)
        {
            Entropy classifier = new Entropy();
            double classification = classifier.Classify(inputs.inputs[textIdx]);

            Assert.IsTrue(classification >= 0 && classification <= 1);
        }
    }

    [TestClass]
    public class FreqAnalysisTest
    {
        private ClassifierTestInputs inputs = new ClassifierTestInputs();

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
        public void TestRange(int textIdx)
        {
            FrequencyAnalysis classifier = new FrequencyAnalysis();
            double classification = classifier.Classify(inputs.inputs[textIdx]);

            Assert.IsTrue(classification >= 0 && classification <= 1);
        }
    }


    [TestClass]
    public class BigramAnalysisTest
    {
        private ClassifierTestInputs inputs = new ClassifierTestInputs();
        Dictionary<string, int>[] inputBigrams =
        {
            new Dictionary<string, int> { { "IN", 11 }, { "ER", 11 }, { "HE", 11 }, { "TH", 7 }, { "ON", 7 }, { "NG", 6 }, { "IC", 5 }, { "AS", 5 }, { "TI", 4 }, { "RE", 4 }, { "IT", 4 }, { "SI", 4 }, { "ND", 4 }, { "HA", 4 }, { "OR", 4 }, { "CE", 4 }, { "AD", 4 }, { "VE", 4 }, { "CO", 4 }, { "AN", 4 }, { "WA", 3 }, { "TO", 3 }, { "OF", 3 }, { "ST", 3 }, { "EE", 3 }, { "TU", 3 }, { "AT", 3 }, { "NS", 3 }, { "IS", 3 }, { "HO", 3 }, { "SH", 3 }, { "OU", 3 }, { "PI", 3 }, { "AL", 2 }, { "OT", 2 }, { "WI", 2 }, { "PE", 2 }, { "OO", 2 }, { "UT", 2 }, { "NO", 2 }, { "RS", 2 }, { "IO", 2 }, { "BO", 2 }, { "OK", 2 }, { "LI", 2 }, { "ID", 2 }, { "EL", 2 }, { "RY", 2 }, { "ED", 2 }, { "TE", 2 }, { "EP", 2 }, { "CT", 2 }, { "UR", 2 }, { "ES", 2 }, { "NV", 2 }, { "SA", 2 }, { "DE", 2 }, { "BE", 1 }, { "GI", 1 }, { "NN", 1 }, { "ET", 1 }, { "BA", 1 }, { "NK", 1 }, { "VI", 1 }, { "HI", 1 }, { "DO", 1 }, { "NC", 1 }, { "EA", 1 }, { "DI", 1 }, { "SE", 1 }, { "UG", 1 }, { "HT", 1 }, { "WN", 1 }, { "LD", 1 }, { "AY", 1 }, { "FE", 1 }, { "SL", 1 }, { "PY", 1 }, { "EG", 1 }, { "NI", 1 }, { "GE", 1 }, { "IR", 1 }, { "TT", 1 }, { "BY", 1 }, { "AV", 1 }, { "TW", 1 }, { "NT", 1 }, { "BU", 1 }, { "WH", 1 }, { "US", 1 }, { "GH", 1 }, { "SO", 1 }, { "RI", 1 }, { "OW", 1 }, { "MI", 1 }, { "WE", 1 }, { "LL", 1 }, { "UL", 1 }, { "FO", 1 }, { "DA", 1 }, { "MA", 1 }, { "LE", 1 }, { "UP", 1 } }, 
            new Dictionary<string, int> { { "IN", 9 }, { "AN", 8 }, { "NT", 6 }, { "EN", 6 }, { "ND", 6 }, { "HI", 6 }, { "TH", 5 }, { "AS", 5 }, { "ED", 5 }, { "WA", 5 }, { "NE", 5 }, { "ME", 5 }, { "ER", 4 }, { "SO", 4 }, { "TE", 4 }, { "ET", 4 }, { "OM", 4 }, { "NG", 4 }, { "IS", 4 }, { "CO", 4 }, { "OU", 3 }, { "LE", 3 }, { "AR", 3 }, { "SE", 3 }, { "ON", 3 }, { "EA", 3 }, { "HE", 3 }, { "YE", 3 }, { "VE", 2 }, { "MI", 2 }, { "SC", 2 }, { "TY", 2 }, { "RS", 2 }, { "BA", 2 }, { "ST", 2 }, { "FR", 2 }, { "LY", 2 }, { "EE", 2 }, { "TI", 2 }, { "WH", 2 }, { "TO", 2 }, { "UN", 2 }, { "MA", 2 }, { "AT", 2 }, { "EV", 2 }, { "EM", 2 }, { "AC", 2 }, { "LO", 2 }, { "TA", 2 }, { "MR", 1 }, { "TT", 1 }, { "LA", 1 }, { "WY", 1 }, { "RU", 1 }, { "GG", 1 }, { "CE", 1 }, { "HA", 1 }, { "LI", 1 }, { "GH", 1 }, { "BY", 1 }, { "OL", 1 }, { "MB", 1 }, { "RA", 1 }, { "SS", 1 }, { "DI", 1 }, { "CK", 1 }, { "RD", 1 }, { "IM", 1 }, { "DU", 1 }, { "DR", 1 }, { "RY", 1 }, { "EH", 1 }, { "OW", 1 }, { "OV", 1 }, { "AB", 1 }, { "IE", 1 }, { "UM", 1 }, { "IC", 1 }, { "FO", 1 }, { "IT", 1 }, { "AL", 1 }, { "UT", 1 }, { "AW", 1 }, { "OF", 1 }, { "UG", 1 }, { "GE", 1 }, { "NA", 1 }, { "NC", 1 }, { "IG", 1 }, { "HT", 1 }, { "SM", 1 }, { "IL", 1 }, { "LD", 1 }, { "CA", 1 }, { "RR", 1 }, { "UR", 1 }, { "KW", 1 }, { "US", 1 }, { "RE", 1 }, { "HO", 1 }, { "VA", 1 }, { "BL", 1 }, { "RI", 1 }, { "DL", 1 }, { "GS", 1 }, { "WI", 1 }, { "TL", 1 }, { "HU", 1 }, { "BE", 1 }, { "RO", 1 }, { "EY", 1 }, { "DE", 1 }, { "CH", 1 }, { "TS", 1 }, { "AY", 1 }, { "LK", 1 } }, 
            new Dictionary<string, int> { { "HE", 10 }, { "ON", 9 }, { "AD", 7 }, { "AN", 7 }, { "ND", 6 }, { "TI", 6 }, { "IN", 6 }, { "ED", 6 }, { "HA", 6 }, { "TE", 6 }, { "HI", 6 }, { "AT", 6 }, { "IS", 5 }, { "AS", 5 }, { "CO", 5 }, { "PO", 5 }, { "SO", 5 }, { "TH", 4 }, { "WA", 4 }, { "IT", 4 }, { "OM", 4 }, { "ME", 4 }, { "ER", 4 }, { "BE", 4 }, { "IM", 4 }, { "LA", 4 }, { "VE", 4 }, { "EC", 3 }, { "SE", 3 }, { "LY", 3 }, { "RA", 3 }, { "UT", 3 }, { "OR", 3 }, { "ST", 3 }, { "AB", 3 }, { "NG", 3 }, { "EL", 3 }, { "LL", 3 }, { "TO", 3 }, { "EN", 3 }, { "BU", 3 }, { "ET", 3 }, { "AL", 3 }, { "OF", 3 }, { "OT", 2 }, { "EE", 2 }, { "OV", 2 }, { "LE", 2 }, { "DI", 2 }, { "GI", 2 }, { "DR", 2 }, { "MP", 2 }, { "LO", 2 }, { "RE", 2 }, { "US", 2 }, { "IE", 2 }, { "ES", 2 }, { "SI", 2 }, { "EA", 2 }, { "RS", 2 }, { "CA", 2 }, { "RT", 2 }, { "CE", 2 }, { "IR", 2 }, { "NO", 2 }, { "OW", 2 }, { "AR", 2 }, { "DL", 2 }, { "CT", 2 }, { "TR", 2 }, { "NE", 2 }, { "RI", 2 }, { "TA", 2 }, { "IO", 2 }, { "DE", 2 }, { "OS", 2 }, { "UP", 2 }, { "TT", 2 }, { "AU", 1 }, { "RD", 1 }, { "BJ", 1 }, { "QU", 1 }, { "NT", 1 }, { "RY", 1 }, { "RR", 1 }, { "HY", 1 }, { "CH", 1 }, { "IA", 1 }, { "BS", 1 }, { "LF", 1 }, { "FR", 1 }, { "WS", 1 }, { "NL", 1 }, { "DY", 1 }, { "NY", 1 }, { "CR", 1 }, { "BY", 1 }, { "TY", 1 }, { "NX", 1 }, { "EI", 1 }, { "GH", 1 }, { "IV", 1 }, { "PR", 1 }, { "AC", 1 }, { "JE", 1 }, { "UI", 1 }, { "FO", 1 }, { "PA", 1 }, { "AI", 1 }, { "BL", 1 }, { "RG", 1 }, { "YP", 1 }, { "OC", 1 }, { "HO", 1 }, { "PL", 1 }, { "RB", 1 }, { "MS", 1 }, { "OL", 1 }, { "RO", 1 }, { "FE", 1 }, { "YO", 1 }, { "RU", 1 }, { "SH", 1 }, { "XI", 1 }, { "WE", 1 }, { "IG", 1 }, { "MA", 1 }, { "IC", 1 }, { "NC", 1 }, { "DO", 1 } }, 
            new Dictionary<string, int> { { "FW", 1 }, { "RU", 1 }, { "UV", 1 }, { "VH", 1 }, { "JY", 1 }, { "YM", 2 }, { "MV", 1 }, { "HN", 1 }, { "JS", 1 }, { "SF", 1 }, { "FY", 1 }, { "JU", 1 }, { "UC", 1 }, { "BS", 1 }, { "SL", 1 }, { "LH", 1 }, { "WN", 2 }, { "YY", 1 }, { "YX", 1 }, { "CR", 1 }, { "RY", 1 }, { "ME", 1 }, { "RE", 1 }, { "NH", 1 } },
            new Dictionary<string, int> { { "CL", 2 }, { "IU", 2 }, { "OQ", 2 }, { "PB", 2 }, { "SA", 2 }, { "SC", 2 }, { "WP", 2 }, { "AC", 1 }, { "AD", 1 }, { "AF", 1 }, { "AO", 1 }, { "BC", 1 }, { "BF", 1 }, { "BK", 1 }, { "CB", 1 }, { "CE", 1 }, { "CR", 1 }, { "DH", 1 }, { "DO", 1 }, { "DX", 1 }, { "EH", 1 }, { "EP", 1 }, { "EQ", 1 }, { "EV", 1 }, { "FE", 1 }, { "FP", 1 }, { "FS", 1 }, { "GO", 1 }, { "GT", 1 }, { "HC", 1 }, { "HD", 1 }, { "HI", 1 }, { "HJ", 1 }, { "HM", 1 }, { "HT", 1 }, { "HW", 1 }, { "IC", 1 }, { "IE", 1 }, { "II", 1 }, { "JB", 1 }, { "JS", 1 }, { "JV", 1 }, { "KG", 1 }, { "KY", 1 }, { "LG", 1 }, { "LH", 1 }, { "LP", 1 }, { "LS", 1 }, { "MI", 1 }, { "MQ", 1 }, { "NE", 1 }, { "NW", 1 }, { "OE", 1 }, { "OH", 1 }, { "OM", 1 }, { "OU", 1 }, { "PI", 1 }, { "PK", 1 }, { "PQ", 1 }, { "PR", 1 }, { "PS", 1 }, { "QC", 1 }, { "QH", 1 }, { "QL", 1 }, { "QM", 1 }, { "QW", 1 }, { "QX", 1 }, { "RJ", 1 }, { "RX", 1 }, { "SB", 1 }, { "SL", 1 }, { "SP", 1 }, { "ST", 1 }, { "SW", 1 }, { "SX", 1 }, { "TC", 1 }, { "TM", 1 }, { "TR", 1 }, { "UG", 1 }, { "UI", 1 }, { "UL", 1 }, { "UN", 1 }, { "VC", 1 }, { "VD", 1 }, { "VX", 1 }, { "WC", 1 }, { "WE", 1 }, { "WX", 1 }, { "XA", 1 }, { "XB", 1 }, { "XF", 1 }, { "XI", 1 }, { "XQ", 1 }, { "XS", 1 }, { "ZA", 1 }, { "ZS", 1 } }, 
            new Dictionary<string, int> { { "UR", 11 }, { "GU", 11 }, { "VA", 11 }, { "UN", 8 }, { "RE", 6 }, { "ER", 6 }, { "BS", 6 }, { "RF", 5 }, { "NA", 5 }, { "GB", 5 }, { "RQ", 5 }, { "NE", 5 }, { "FR", 4 }, { "NG", 4 }, { "IR", 4 }, { "RA", 4 }, { "FU", 4 }, { "EN", 4 }, { "BJ", 4 }, { "FF", 4 }, { "GR", 4 }, { "AT", 4 }, { "FG", 4 }, { "RG", 4 }, { "NF", 3 }, { "ZR", 3 }, { "JB", 3 }, { "HY", 3 }, { "OR", 3 }, { "RR", 3 }, { "GE", 3 }, { "EG", 3 }, { "CE", 3 }, { "AQ", 3 }, { "NQ", 3 }, { "ON", 2 }, { "QE", 2 }, { "RI", 2 }, { "BE", 2 }, { "BZ", 2 }, { "BH", 2 }, { "YQ", 2 }, { "NI", 2 }, { "PE", 2 }, { "RN", 2 }, { "FH", 2 }, { "VF", 2 }, { "GF", 2 }, { "PU", 2 }, { "NP", 2 }, { "BA", 2 }, { "QR", 2 }, { "YN", 2 }, { "YL", 2 }, { "ZN", 2 }, { "NO", 2 }, { "UB", 2 }, { "PR", 2 }, { "ZV", 2 }, { "HE", 2 }, { "EB", 2 }, { "EX", 2 }, { "CB", 2 }, { "QY", 1 }, { "IN", 1 }, { "PP", 1 }, { "HF", 1 }, { "OO", 1 }, { "NZ", 1 }, { "HP", 1 }, { "TF", 1 }, { "HN", 1 }, { "JA", 1 }, { "PN", 1 }, { "EP", 1 }, { "RY", 1 }, { "GP", 1 }, { "EC", 1 }, { "EV", 1 }, { "JV", 1 }, { "BK", 1 }, { "VZ", 1 }, { "VG", 1 }, { "NL", 1 }, { "XR", 1 }, { "AH", 1 }, { "ZO", 1 }, { "GN", 1 }, { "OY", 1 }, { "UZ", 1 }, { "RC", 1 }, { "AP", 1 }, { "QV", 1 }, { "XV", 1 }, { "BC", 1 }, { "VB", 1 }, { "YY", 1 }, { "RL", 1 }, { "CR", 1 }, { "EF", 1 }, { "OH", 1 }, { "ET", 1 }, { "JN", 1 }, { "FB", 1 }, { "PH", 1 }, { "OV", 1 }, { "AR", 1 }, { "NT", 1 }, { "DH", 1 }, { "JR", 1 }, { "FP", 1 }, { "AL", 1 }, { "PB", 1 }, { "KV", 1 }, { "GL", 1 }, { "HZ", 1 }, { "YV", 1 }, { "AG", 1 }, { "PG", 1 }, { "PY", 1 }, { "CH", 1 }, { "GV", 1 }, { "JQ", 1 }, { "NY", 1 }, { "YR", 1 }, { "LF", 1 }, { "FO", 1 } }, 
            new Dictionary<string, int> { }
        };

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        public void TestProcess(int textIdx)
        {
            Bigrams classifier = new Bigrams();
            classifier.Classify(inputs.inputs[textIdx]);

            Dictionary<string, int> trueBigrams = inputBigrams[textIdx];

            foreach (KeyValuePair<string, (double, int)> kvp in classifier.GetBigramFreqs())
            {
                if (trueBigrams.ContainsKey(kvp.Key))
                {
                    Assert.AreEqual(trueBigrams[kvp.Key], kvp.Value.Item2);
                }
                else
                {
                    Assert.AreEqual(0, kvp.Value.Item2);
                }
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
        public void TestRange(int textIdx)
        {
            Bigrams classifier = new Bigrams();
            double classification = classifier.Classify(inputs.inputs[textIdx]);

            Assert.IsTrue(classification >= 0 && classification <= 1);
        }

    }

    [TestClass]
    public class WordLengthTest
    {
        ClassifierTestInputs inputs = new ClassifierTestInputs();
        string[] testInputs = new string[]
        {
            "",
            "a",
            "a b c",
            "hello",
            "This is a test.",
            "aaaaaaaaaaaaaaaaaaaa aaaaaaaaaa a",
            "This classifier counts word length frequencies"
        };
        double[][] results = new double[][]
        {
            new double[21],
            new double[21] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new double[21] { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new double[21] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new double[21] { 1, 1, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new double[21] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new double[21] { 0, 0, 0, 1, 0, 2, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        }; 

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        public void TestProcess(int textIdx, int optimalN = 100)
        {
            WordLength classifier = new WordLength();
            double[] trueObservedFreqs = Statistics.ScaleFrequencies(results[textIdx], optimalN);
            double[] obsFreqs = classifier.GetObservedFreqs(testInputs[textIdx]);

            for (int i = 0; i < obsFreqs.Length; i++)
            {
                Assert.AreEqual(trueObservedFreqs[i], results[textIdx][i]);
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
        public void TestRange(int textIdx)
        {
            WordLength classifier = new WordLength();
            double classification = classifier.Classify(inputs.inputs[textIdx]);

            Assert.IsTrue(classification >= 0 && classification <= 1);
        }
    }

    //[TestClass]
    //public class EnsembleTest
    //{
    //    TextInputs inputs = new TextInputs();

    //    [TestMethod]
    //    private void TestProcess()
    //    {

    //    }

    //    [TestMethod]
    //    [DataRow(0)]
    //    [DataRow(1)]
    //    [DataRow(2)]
    //    [DataRow(3)]
    //    [DataRow(4)]
    //    [DataRow(5)]
    //    [DataRow(6)]
    //    public void TestRange(int textIdx)
    //    {
    //        Ensemble classifier = new Ensemble();
    //        double classification = classifier.Classify(inputs.inputs[textIdx]);

    //        Assert.IsTrue(classification >= 0 && classification <= 1);
    //    }
    //}

}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using nea;
using System;

namespace neaTest
{
    [TestClass]
    public class StatisticsTests
    {

        [TestMethod]
        [DataRow(new double[] { 1, 26, 41, 2, 17, 34, 12, 47, 12, 8 }, new double[] {0.5, 13, 20.5, 1, 8.5, 17, 6, 23.5, 6, 4 }, 1)]
        [DataRow(new double[] { 100, 4, 73, 44, 116, 2, 31, 86, 36, 8 }, new double[] { 20, 0.8, 14.6, 8.8, 23.2, 0.4, 6.2, 17.2, 7.2, 1.6 }, 2)]
        [DataRow(new double[] { 14, 21, 17, 26, 63, 28, 31 }, new double[] { 7, 10.5, 8.5, 13, 31.5, 14, 15.5 }, 3)]
        public void ScalingFrequencies(double[] frequencies, double[] expectedResult, int testNumber, double scaleTo = 100)
        {
            double[] scaled = Statistics.ScaleFrequencies(frequencies, scaleTo);

            Assert.AreEqual(expectedResult.Length, scaled.Length);

            for (int i = 0; i < scaled.Length; i++)
            {
                Assert.AreEqual(expectedResult[i], scaled[i], 1e-6);
            }

        }


        [TestMethod]
        [DataRow(new double[] { 0.5, 13, 20.5, 1, 8.5, 17, 6, 23.5, 6, 4 }, new double[] {6, 21, 18, 2, 2, 14, 6, 19, 8, 4}, 32.27015)]
        [DataRow(new double[] { 20, 0.8, 14.6, 8.8, 23.2, 0.4, 6.2, 17.2, 7.2, 1.6 }, new double[] { 2, 20.75, 27.75, 7.25, 3.25, 1.25, 13.25, 7.5, 4, 13 }, 339.6374)]
        [DataRow(new double[] { 8, 0.6, 6.2, 1, 2.4, 7, 10.5, 8.5, 9.1, 31.5, 13.6, 1.6 }, new double[] { 7.7, 0.9, 6.2, 1, 2.3, 7, 11, 8.5, 9, 31.6, 13.4, 1.4 }, 0.1717475)]
        [DataRow(new double[] { 0.224, 8.511, 7.614, 0.112, 7.167, 5.151, 47.368, 0.896, 0.672, 9.295, 2.016, 4.143, 6.831 }, new double[] { 8.04, 12.49, 6.89, 5.05, 7.57, 7.89, 9.24, 7.23, 7.64, 6.28, 6.51, 9.28, 5.89 }, 191.5222)]
        public void ChiSquaredStatistic(double[] observed, double[] expected, double trueResult)
        {
            double chiSquared = Statistics.ChiSquared(observed, expected);
            Assert.AreEqual(trueResult, chiSquared, 5e-5);
        }


        [TestMethod]
        [DataRow(9, 32.27015, 0.000178809)]
        [DataRow(9, 339.6374, 0)]
        [DataRow(11, 0.1717475, 1)]
        [DataRow(12, 191.5222, 0)]
        public void GettingPValue(int degFreedom, double chiSquared, double trueValue, int numIntervals = 1000, string lookupGammaFunctFilePath = "C:\\Users\\betha\\Code\\nea\\FilesForUse\\LookupGammaFunct.txt")
        {
            double pValue = Statistics.GetPValue(degFreedom, chiSquared, numIntervals, Statistics.GetGammaFunctionValues(lookupGammaFunctFilePath));
            Assert.AreEqual(trueValue, pValue, 5e-6);
        }

        [TestMethod]
        [DataRow(9, 32.27015)]
        [DataRow(9, 339.6374)]
        [DataRow(11, 0.1717475)]
        [DataRow(12, 191.5222)]
        public void PValueInCorrectRange(int degFreedom, double chiSquared, int numIntervals = 1000, string lookupGammaFunctFilePath = "C:\\Users\\betha\\Code\\nea\\FilesForUse\\LookupGammaFunct.txt")
        {
            double pValue = Statistics.GetPValue(degFreedom, chiSquared, numIntervals, Statistics.GetGammaFunctionValues(lookupGammaFunctFilePath));
            Assert.IsTrue(pValue >= 0 && pValue <= 1);
        }
    }
}

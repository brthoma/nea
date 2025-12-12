using Microsoft.VisualStudio.TestTools.UnitTesting;
using nea;
using System;

namespace neaTest
{
    [TestClass]
    public class StatisticsTests
    {

        [TestMethod]
        [DataRow(new double[] {1, 26, 41, 2, 17, 34, 12, 47, 12, 8}, new double[] {0.5, 13, 20.5, 1, 8.5, 17, 6, 23.5, 6, 4})]
        public void ScalingFrequencies(double[] frequencies, double[] expectedResult, double scaleTo = 100)
        {
            double[] scaled = Statistics.ScaleFrequencies(frequencies, scaleTo);

            Assert.AreEqual(expectedResult.Length, scaled.Length);

            for (int i = 0; i < scaled.Length; i++)
            {
                if (scaled[i] != expectedResult[i])
                {
                    Assert.Fail();
                }
            }

        }


        [TestMethod]
        [DataRow(new double[] { 0.5, 13, 20.5, 1, 8.5, 17, 6, 23.5, 6, 4 }, new double[] {6, 21, 18, 2, 2, 14, 6, 19, 8, 4}, 32.27015)]
        public void ChiSquaredStatistic(double[] observed, double[] expected, double trueResult)
        {
            double chiSquared = Statistics.ChiSquared(observed, expected);
            Assert.AreEqual(trueResult, chiSquared, 5e-6);
        }


        [TestMethod]
        [DataRow(9, 32.27015, 0.0001788093)]
        public void GettingPValue(int degFreedom, double chiSquared, double trueValue, int numIntervals = 1000, string lookupGammaFunctFilePath = "C:\\Users\\betha\\Code\\nea\\FilesForUse\\LookupGammaFunct.txt")
        {
            double pValue = Statistics.GetPValue(degFreedom, chiSquared, numIntervals, Statistics.GetGammaFunctionValues(lookupGammaFunctFilePath));
            Assert.AreEqual(trueValue, pValue, 5e-6);
        }

        [TestMethod]
        [DataRow(9, 32.27015)]
        public void PValueInCorrectRange(int degFreedom, double chiSquared, int numIntervals = 1000, string lookupGammaFunctFilePath = "C:\\Users\\betha\\Code\\nea\\FilesForUse\\LookupGammaFunct.txt")
        {
            double pValue = Statistics.GetPValue(degFreedom, chiSquared, numIntervals, Statistics.GetGammaFunctionValues(lookupGammaFunctFilePath));
            Assert.IsTrue(pValue >= 0 && pValue <= 1);
        }
    }
}

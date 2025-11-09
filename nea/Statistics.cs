using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nea
{
    internal class Statistics
    {
        public static double[] ScaleFrequencies(double[] frequencies, int scaleTo)
        {
            double scaleFrom = frequencies.Sum();
            for (int i = 0; i < frequencies.Length; i++)
            {
                frequencies[i] *= scaleTo / scaleFrom;
            }
            return frequencies;
        }

        public static (double[], double[]) CombineChiSquaredClasses(double[] observedFreqs, double[] expectedFreqs, int optimalN)
        {
            List<double> observedFreqList = observedFreqs.ToList();
            List<double> expectedFreqList = expectedFreqs.ToList();

            while ((double)expectedFreqList.Min() < 0.05 * optimalN)
            {
                double minExpectedFreq = expectedFreqList.Min();
                int removeAtIdx = expectedFreqList.IndexOf(minExpectedFreq);
                double minObservedFreq = observedFreqList[removeAtIdx];

                observedFreqList.RemoveAt(removeAtIdx);
                expectedFreqList.RemoveAt(removeAtIdx);

                int addAtIdx = expectedFreqList.IndexOf(expectedFreqList.Min());
                expectedFreqList[addAtIdx] += minExpectedFreq;
                observedFreqList[addAtIdx] += minObservedFreq;
            }

            return (observedFreqList.ToArray(), expectedFreqList.ToArray());
        }

        private static double ChiSquared(double[] observedFreqs, double[] expectedFreqs)
        {
            double chiSquared = 0;
            for (int i = 0; i < observedFreqs.Length; i++)
            {
                chiSquared += Math.Pow(observedFreqs[i] - expectedFreqs[i], 2) / expectedFreqs[i];
            }

            return chiSquared;
        }

        private static double CDF(int degFreedom, double chiSquared, int numIntervals, Dictionary<double, double> lookupGammaFunct)
        {
            double cdf = LowerIncompleteGammaFunct((double)degFreedom / 2, chiSquared / 2, numIntervals) / lookupGammaFunct[(double)degFreedom / 2];
            Console.WriteLine($"degFreedom = {degFreedom}, chiSquared = {chiSquared}; cdf = {cdf}");
            return cdf;
        }

        private static double f(double s, double x)
        {
            return Math.Pow(x, s - 1) * Math.Pow(Math.E, -x);
        }

        private static double LowerIncompleteGammaFunct(double s, double x, int numIntervals)
        {
            double intervalWidth = x / numIntervals;
            double result = 0;

            for (int i = 0; i < numIntervals; i++)
            {
                double a = i * intervalWidth;
                double b = (i + 1) * intervalWidth;
                result += ((b - a) / 6) * (f(s, a) + 4 * f(s, ((a + b) / 2)) + f(s, b));
            }

            return result;
        }

        public static double GetPValue(double[] observedFreqs, double[] expectedFreqs, int degFreedom, int numIntervals, Dictionary<double, double> lookupGammaFunct)
        {
            return 1 - CDF(degFreedom, ChiSquared(observedFreqs, expectedFreqs), numIntervals, lookupGammaFunct);
        }
    }
}

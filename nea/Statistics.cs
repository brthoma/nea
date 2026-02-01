using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nea
{
    /* Contains static methods of statistical methods used throughout the program
     * Methods are related to the calculation of the Chi-squared statistic
     */
    public class Statistics
    {

        /* Used to get the lookup values for the Gamma function from a file
         */
        public static Dictionary<double, double> GetGammaFunctionValues(string filePath)
        {
            Dictionary<double, double> LookupGammaFunct = new Dictionary<double, double>();

            using (StreamReader sr = new StreamReader(filePath))
            {
                while (!sr.EndOfStream)
                {
                    string[] kvp = sr.ReadLine().Trim().Split('|');
                    LookupGammaFunct.Add(double.Parse(kvp[0]), double.Parse(kvp[1]));
                }
                sr.Close();
            }

            return LookupGammaFunct;
        }

        /* Used to scale the frequencies to set the sample size
         * This limits the Chi-squared value so that it remains within reasonable bounds
         */
        public static double[] ScaleFrequencies(double[] frequencies, double scaleTo)
        {
            double scaleFrom = frequencies.Sum();
            for (int i = 0; i < frequencies.Length; i++)
            {
                frequencies[i] *= scaleTo / scaleFrom;
            }
            return frequencies;
        }

        /* Combines classes until all classes have a minimum expected frequency of around 5%
         * of the sample size
         */
        public static (double[], double[]) CombineChiSquaredClasses(double[] observedFreqs, double[] expectedFreqs, int sumOfFreqs)
        {
            List<double> observedFreqList = observedFreqs.ToList();
            List<double> expectedFreqList = expectedFreqs.ToList();

            while ((double)expectedFreqList.Min() < 0.05 * sumOfFreqs)
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

        /* Calculation of the Chi-squared statistic
         */
        public static double ChiSquared(double[] observedFreqs, double[] expectedFreqs)
        {
            double chiSquared = 0;
            for (int i = 0; i < observedFreqs.Length; i++)
            {
                chiSquared += Math.Pow(observedFreqs[i] - expectedFreqs[i], 2) / expectedFreqs[i];
            }

            return chiSquared;
        }

        /* Approximation of the Chi-squared Cumulative Distribution Function
         * This is used to calculate a p-value
         */
        private static double CDF(int degFreedom, double chiSquared, int numIntervals, Dictionary<double, double> lookupGammaFunct)
        {
            double cdf = LowerIncompleteGammaFunct((double)degFreedom / 2, chiSquared / 2, numIntervals) / lookupGammaFunct[(double)degFreedom / 2];

            return cdf;
        }


        /* Function used in calculating the lower incomplete gamma function
         */
        private static double f(double s, double x)
        {
            return Math.Pow(x, s - 1) * Math.Pow(Math.E, -x);
        }

        /* Approximation of the lower incomplete gamma function
         * This is the integral of f(s, x)
         * Uses Simpson's Rule to integrate
         */
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

        /* Chi-squared p-value is 1 - CDF
         */
        public static double GetPValue(double[] observedFreqs, double[] expectedFreqs, int degFreedom, int numIntervals, Dictionary<double, double> lookupGammaFunct)
        {
            return 1 - CDF(degFreedom, ChiSquared(observedFreqs, expectedFreqs), numIntervals, lookupGammaFunct);
        }

        /* Calculation of Chi-squared p-value used for testing
         * Chi-squared p-value calculated directly
         */
        public static double GetPValue(int degFreedom, double chiSquared, int numIntervals, Dictionary<double, double> lookupGammaFunct)
        {
            return 1 - CDF(degFreedom, chiSquared, numIntervals, lookupGammaFunct);
        }
    }
}

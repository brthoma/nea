using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XPlot.Plotly;

namespace nea
{
    public interface IDisplayGraph
    {
        void Display(IConfiguration config);
    }


    public class ThresholdSuccessGraph : IDisplayGraph
    {

        private const int NUMDATAPOINTS = 100;

        private double GetSuccessAtThreshold(double[] results, bool[] trueValues, double threshold)
        {
            int success = 0;

            for (int i = 0; i < results.Length; i++)
            {
                if ((results[i] >= threshold && trueValues[i]) || (results[i] < threshold && !trueValues[i])) success++;
            }

            return (double) success / results.Length;
        }

        public void Display(IConfiguration config)
        {
            TestResultsStore resultsStore = new TestResultsStore();

            (double[] results, bool[] trueValues) = resultsStore.GetResults(config.GetStr("filePath"));

            double[] successRates = new double[NUMDATAPOINTS + 1];
            double[] thresholds = new double[NUMDATAPOINTS + 1];

            for (int i = 0; i <= NUMDATAPOINTS; i ++)
            {
                thresholds[i] = (double) i / NUMDATAPOINTS;
                successRates[i] = GetSuccessAtThreshold(results, trueValues, thresholds[i]);
            }

            var scatterPlot = Chart.Plot(new Scatter()
            {
                x = thresholds,
                y = successRates,
                mode = "match"
            });
            scatterPlot.WithXTitle("Threshold");
            scatterPlot.WithYTitle("Success Rate");
            scatterPlot.Show();
        }

    }

    public class ROCCurve : IDisplayGraph
    {

        private const int NUMDATAPOINTS = 100;

        private (double, double) GetValuesAtThreshold(double[] results, bool[] trueValues, double threshold)
        {
            int numTruePositives = 0;
            int numFalsePositives = 0;
            int numPositives = 0;
            int numNegatives = 0;

            for (int i = 0; i < results.Length; i ++)
            {
                if (trueValues[i])
                {
                    numPositives++;
                    if (results[i] >= threshold)
                    {
                        numTruePositives++;
                    }
                }
                if (!trueValues[i])
                {
                    numNegatives++;
                    if (results[i] >= threshold)
                    {
                        numFalsePositives++;
                    }
                }
            }

            return ( (double) numFalsePositives / numNegatives, (double) numTruePositives / numPositives );
        }

        public void Display(IConfiguration config)
        {
            TestResultsStore resultsStore = new TestResultsStore();

            (double[] results, bool[] trueValues) = resultsStore.GetResults(config.GetStr("filePath"));

            double[] falsePositiveRate = new double[NUMDATAPOINTS + 3];
            double[] truePositiveRate = new double[NUMDATAPOINTS + 3];

            for (int i = 0; i < falsePositiveRate.Length - 2; i ++)
            {
                double threshold = (double) i / NUMDATAPOINTS;
                (falsePositiveRate[i + 1], truePositiveRate[i + 1]) = GetValuesAtThreshold(results, trueValues, threshold);
                Console.WriteLine($"{threshold} {falsePositiveRate[i + 1]} {truePositiveRate[i + 1]}");
            }
            (falsePositiveRate[0], truePositiveRate[0]) = (1, 1);
            (falsePositiveRate[falsePositiveRate.Length - 1], truePositiveRate[truePositiveRate.Length - 1]) = (0, 0);

            var scatterPlot = Chart.Plot(new Scatter()
            {
                x = falsePositiveRate,
                y = truePositiveRate,
                mode = "match"
            });
            scatterPlot.WithXTitle("False Positive Rate");
            scatterPlot.WithYTitle("True Positive Rate");
            scatterPlot.Show();
        }

    }

    public class DETCurve : IDisplayGraph
    {

        private const int NUMDATAPOINTS = 100;

        private (double, double) GetValuesAtThreshold(double[] results, bool[] trueValues, double threshold)
        {
            int numFalsePositives = 0;
            int numFalseNegatives = 0;
            int numPositives = 0;
            int numNegatives = 0;

            for (int i = 0; i < results.Length; i++)
            {
                if (trueValues[i])
                {
                    numPositives++;
                    if (results[i] < threshold)
                    {
                        numFalseNegatives++;
                    }
                }
                if (!trueValues[i])
                {
                    numNegatives++;
                    if (results[i] >= threshold)
                    {
                        numFalsePositives++;
                    }
                }
            }

            return ((double) numFalsePositives / numNegatives, (double) numFalseNegatives / numPositives);
        }

        public void Display(IConfiguration config)
        {
            TestResultsStore resultsStore = new TestResultsStore();

            (double[] results, bool[] trueValues) = resultsStore.GetResults(config.GetStr("filePath"));

            double[] falsePositiveRate = new double[NUMDATAPOINTS + 1];
            double[] falseNegativeRate = new double[NUMDATAPOINTS + 1];

            for (int i = 0; i < falsePositiveRate.Length; i++)
            {
                double threshold = (double)i / NUMDATAPOINTS;
                (falsePositiveRate[i], falseNegativeRate[i]) = GetValuesAtThreshold(results, trueValues, threshold);
                Console.WriteLine($"{threshold} {falsePositiveRate[i]} {falseNegativeRate[i]}");
            }

            var scatterPlot = Chart.Plot(new Scatter()
            {
                x = falsePositiveRate,
                y = falseNegativeRate,
                mode = "match"
            });
            scatterPlot.WithXTitle("False Positive Rate");
            scatterPlot.WithYTitle("False Negative Rate");
            scatterPlot.Show();
        }

    }

    public class PrintSuccessRate : IDisplayGraph
    {
        public void Display(IConfiguration config)
        {
            DemoResultsStore resultsStore = new DemoResultsStore();

            bool[] success = resultsStore.GetResults(config.GetStr("filePath"));

            Console.WriteLine($"Success rate: {(double) success.Count(b => b) / success.Length}");
        }
    }


}

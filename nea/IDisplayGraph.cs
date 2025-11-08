using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using XPlot.Plotly;

namespace nea
{
    public interface IDisplayGraph
    {
        void Display(IConfiguration[] configs);
    }


    public class ThresholdSuccessGraph : IDisplayGraph
    {

        private const int NUMDATAPOINTS = 1000;

        private double GetSuccessAtThreshold(double[] results, bool[] trueValues, double threshold)
        {
            int success = 0;

            for (int i = 0; i < results.Length; i++)
            {
                if ((results[i] >= threshold && trueValues[i]) || (results[i] < threshold && !trueValues[i]))
                {
                    success++;
                }
            }

            return (double) success / results.Length;
        }

        public void Display(IConfiguration[] configs)
        {
            TestResultsStore resultsStore = new TestResultsStore();

            Scatter[] scatterPlots = new Scatter[configs.Length];

            for (int i = 0; i < configs.Length; i++)
            {
                (double[] results, bool[] trueValues) = resultsStore.GetResults(configs[i].GetStr("filePath"));

                double[] successRates = new double[NUMDATAPOINTS + 1];
                double[] thresholds = new double[NUMDATAPOINTS + 1];

                for (int j = 0; j <= NUMDATAPOINTS; j ++)
                {
                    thresholds[j] = (double) j / NUMDATAPOINTS;
                    successRates[j] = GetSuccessAtThreshold(results, trueValues, thresholds[j]);
                }

                Scatter scatter = new Scatter()
                {
                    x = thresholds,
                    y = successRates,
                    mode = "match",
                    name = configs[i].GetStr("filePath")
                };

                scatterPlots[i] = scatter;
            }

            PlotlyChart combinedScatterPlot = Chart.Plot(scatterPlots);
            combinedScatterPlot.WithXTitle("Threshold");
            combinedScatterPlot.WithYTitle("Success Rate");
            combinedScatterPlot.Show();
        }

    }

    public class ROCCurve : IDisplayGraph
    {

        private const int NUMDATAPOINTS = 1000;

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

        public void Display(IConfiguration[] configs)
        {
            TestResultsStore resultsStore = new TestResultsStore();

            Scatter[] scatterPlots = new Scatter[configs.Length];

            for (int i = 0; i < configs.Length; i++)
            {
                (double[] results, bool[] trueValues) = resultsStore.GetResults(configs[i].GetStr("filePath"));

                double[] falsePositiveRate = new double[NUMDATAPOINTS + 3];
                double[] truePositiveRate = new double[NUMDATAPOINTS + 3];

                for (int j = 0; j < falsePositiveRate.Length - 2; j++)
                {
                    double threshold = (double)j / NUMDATAPOINTS;
                    (falsePositiveRate[j + 1], truePositiveRate[j + 1]) = GetValuesAtThreshold(results, trueValues, threshold);
                    Console.WriteLine($"{threshold} {falsePositiveRate[j + 1]} {truePositiveRate[j + 1]}");
                }
                (falsePositiveRate[0], truePositiveRate[0]) = (1, 1);
                (falsePositiveRate[falsePositiveRate.Length - 1], truePositiveRate[truePositiveRate.Length - 1]) = (0, 0);

                Scatter scatter = new Scatter()
                {
                    x = falsePositiveRate,
                    y = truePositiveRate,
                    mode = "match",
                    name = configs[i].GetStr("filePath")
                };

                scatterPlots[i] = scatter;
            }

            PlotlyChart combinedScatterPlot = Chart.Plot(scatterPlots);
            combinedScatterPlot.WithXTitle("False Positive Rate");
            combinedScatterPlot.WithYTitle("True Positive Rate");
            combinedScatterPlot.Show();
        }

    }

    public class DETCurve : IDisplayGraph
    {

        private const int NUMDATAPOINTS = 1000;

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

        public void Display(IConfiguration[] configs)
        {
            TestResultsStore resultsStore = new TestResultsStore();

            Scatter[] scatterPlots = new Scatter[configs.Length];

            for (int i = 0; i < configs.Length; i++)
            {
                (double[] results, bool[] trueValues) = resultsStore.GetResults(configs[i].GetStr("filePath"));

                double[] falsePositiveRate = new double[NUMDATAPOINTS + 1];
                double[] falseNegativeRate = new double[NUMDATAPOINTS + 1];

                for (int j = 0; j < falsePositiveRate.Length; j++)
                {
                    double threshold = (double)j / NUMDATAPOINTS;
                    (falsePositiveRate[j], falseNegativeRate[j]) = GetValuesAtThreshold(results, trueValues, threshold);
                    Console.WriteLine($"{threshold} {falsePositiveRate[j]} {falseNegativeRate[j]}");
                }

                Scatter scatter = new Scatter()
                {
                    x = falsePositiveRate,
                    y = falseNegativeRate,
                    mode = "match",
                    name = configs[i].GetStr("filePath")
                };

                scatterPlots[i] = scatter;
            }

            PlotlyChart combinedScatterPlot = Chart.Plot(scatterPlots);
            combinedScatterPlot.WithXTitle("False Positive Rate");
            combinedScatterPlot.WithYTitle("False Negative Rate");
            combinedScatterPlot.Show();
        }

    }

    public class ShowSuccessRate : IDisplayGraph
    {
        public void Display(IConfiguration[] configs)
        {
            DemoResultsStore resultsStore = new DemoResultsStore();
            string[] xAxis = new string[configs.Length];
            double[] yAxis = new double[configs.Length];

            for (int i = 0; i < configs.Length; i++)
            {
                bool[] success = resultsStore.GetResults(configs[i].GetStr("filePath"));
                xAxis[i] = configs[i].GetStr("filePath");
                yAxis[i] = (double)success.Count(b => b) / success.Length;

                //Console.WriteLine($"File name: {configs[i].GetStr("filePath")} | Success rate: {(double)success.Count(b => b) / success.Length}");
            }
            var barchart = Chart.Plot(new Bar()
            {
                x = xAxis,
                y = yAxis
            });
            barchart.Show();
        }
    }


}

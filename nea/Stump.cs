using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nea
{
    public class Stump
    {
        const double EPSILON = 0.0001;

        private IClassifier classifier;
        private double threshold;
        private double totalError;
        private double say;

        public Stump(IClassifier classifier, double threshold)
        {
            this.classifier = classifier;
            this.threshold = threshold;
        }

        public bool Classify(string text)
        {
            double result = classifier.Classify(text);
            if (result >= threshold) return true;
            return false;
        }

        public double GetGiniIdx(Element[] elements)
        {
            int truePos = 0;
            int falsePos = 0;
            int trueNeg = 0;
            int falseNeg = 0;

            foreach (Element element in elements)
            {
                if (element.actualValue)
                {
                    if (Classify(element.text))
                    {
                        truePos++;
                    }
                    else falseNeg++;
                }
                else
                {
                    if (!Classify(element.text))
                    {
                        trueNeg++;
                    }
                    else falsePos++;
                }
            }

            foreach (Element element in elements)
            {
                double score = this.classifier.Classify(element.text);
                if (!element.actualValue)
                {
                    score = 1.0 - score;
                }
            }

            double leftGiniIdx = 1 - Math.Pow((double) truePos / (truePos + falsePos), 2) - Math.Pow((double) falsePos / (truePos + falsePos), 2);
            double rightGiniIdx = 1 - Math.Pow((double)trueNeg / (trueNeg + falseNeg), 2) - Math.Pow((double)falseNeg / (trueNeg + falseNeg), 2);
            double giniIdx = (double) leftGiniIdx * (truePos + falsePos) / elements.Length + (double) rightGiniIdx * (trueNeg + falseNeg) / elements.Length;

            return giniIdx;
        }

        public double GetTotalError(Element[] elements)
        {
            double totalError = 0;

            foreach (Element element in elements)
            {
                if (Classify(element.text))
                {
                    if (!element.actualValue) totalError += element.weight;
                }
                else
                {
                    if (element.actualValue) totalError += element.weight;
                }
            }

            this.totalError = totalError;

            return totalError;
        }

        public double GetSay(Element[] elements)
        {
            totalError = GetTotalError(elements);
            if (totalError == 0) totalError = EPSILON;
            else if (totalError == 1) totalError = 1.0 - EPSILON;           //MAYBE PLAY AROUND WITH THIS TO MAKE IT SMALL ENOUGH
            say = (1/2) * Math.Log((1 - totalError) / totalError);

            return say;
        }
    }
}

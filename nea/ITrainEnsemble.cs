using nea;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace nea
{
    internal interface ITrainEnsemble
    {
        void Train(IClassifier[] classifiers, ICipher cipher, int sampleSize, int textLength);
        double[] GetWeights(IClassifier[] classifiers, ICipher cipher);
    }
}

public class MajVoting : ITrainEnsemble
{

    public void Train(IClassifier[] classifiers, ICipher cipher, int sampleSize = 0, int textLength = 0) { }

    public double[] GetWeights(IClassifier[] classifiers, ICipher cipher)
    {
        double[] weights = new double[classifiers.Length];

        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = (double) 1 / classifiers.Length;
        }

        return weights;
    }
}

public class ProportionalVoting : ITrainEnsemble
{
    private const string DATAFILEPATH = "C:\\Users\\betha\\Code\\nea\\nea\\CipherClassifierSuccess.txt";
    private const string THRESHOLDFILEPATH = "C:\\Users\\betha\\Code\\nea\\nea\\OptimalClassifierThresholds.txt";

    public void Train(IClassifier[] classifiers, ICipher cipher, int sampleSize, int textLength)
    {
        throw new NotImplementedException();
    }

    public double[] GetWeights(IClassifier[] classifiers, ICipher cipher)
    {
        throw new NotImplementedException();
    }
}

public struct Element
{
    public string text;
    public bool actualValue;
    public double weight;

    public Element(string text, bool actualValue, double weight)
    {
        this.text = text;
        this.actualValue = actualValue;
        this.weight = weight;
    }
}


public class AdaBoost : ITrainEnsemble
{
    private const string THRESHOLDFILEPATH = "C:\\Users\\betha\\Code\\nea\\nea\\OptimalClassifierThresholds.txt";
    private const string DICTIONARYFILEPATH = "C:\\Users\\betha\\Code\\nea\\nea\\EnglishDictionary.txt";

    private Random random;

    public AdaBoost(Random random)
    {
        this.random = random;
    }


    private Element[] SetUpSamples(IDataGenerator dataGenerator, ICipher cipher, int sampleSize, int textLength)
    {
        Element[] elements = new Element[sampleSize];

        for (int i = 0; i < sampleSize; i++)
        {
            string text = dataGenerator.GenerateData(random, textLength);
            bool trueValue = true;

            if (random.Next(2) == 0)
            {
                text = cipher.Encrypt(text, cipher.GetRandomKey(random));
                trueValue = false;
            }

            Element element = new Element(text, trueValue, 1.0 / sampleSize);
            elements[i] = element;
        }

        return elements;
    }

    private double[] GetThresholds(int numClassifiers)
    {
        double[] thresholds = new double[numClassifiers];
        string[] strThresholds;
        using (StreamReader sr = new StreamReader(DICTIONARYFILEPATH))
        {
            strThresholds = sr.ReadLine().Trim('|').Split('|');
            sr.Close();
        }
        for (int i = 0; i < thresholds.Length; i++)
        {
            thresholds[i] = double.Parse(strThresholds[i]);
        }

        return thresholds;
    }

    private Stump GetLowestGiniIdx(List<Stump> stumps, Element[] elements)
    {
        Stump bestStump = stumps[0];
        double bestGini = bestStump.GetGiniIdx(elements);

        for (int i = 1; i < stumps.Count(); i++)
        {
            double newGini = stumps[i].GetGiniIdx(elements);
            if (newGini < bestGini)
            {
                bestStump = stumps[i];
                bestGini = newGini;
            }
        }
        return bestStump;
    }

    private Element[] GetNewSampleCollection(Element[] elements, int sampleSize)
    {
        Element[] sample = new Element[sampleSize];

        double acc = 0;

        for (int i = 0; i < sampleSize; i++)
        {
            double samplePos = random.NextDouble();
            foreach (Element element in elements)
            {
                acc += element.weight;
                if (acc > samplePos)
                {
                    sample[i] = element;
                    break;
                }
            }
        }

        return sample;
    }

    public void Train(IClassifier[] classifiers, ICipher cipher, int sampleSize, int textLength)
    {
        WordsFromDict dataGenerator = new WordsFromDict(DICTIONARYFILEPATH);

        Element[] elements = SetUpSamples(dataGenerator, cipher, sampleSize, textLength);

        double[] thresholds = GetThresholds(classifiers.Length);

        //Stump[] stumps = new Stump[classifiers.Length];
        List<Stump> stumps = new List<Stump>();

        for (int i = 0; i < classifiers.Length; i++)
        {
            stumps.Add(new Stump(classifiers[i], thresholds[i]));
        }

        while (stumps.Count() != 0)
        {
            Stump currentStump = GetLowestGiniIdx(stumps, elements);
        }
    }

    public double[] GetWeights(IClassifier[] classifiers, ICipher cipher)
    {
        throw new NotImplementedException();
    }
}

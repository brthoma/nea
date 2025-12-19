using nea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neaTest
{
    public class TrueClassifier : IClassifier
    {
        public double Classify(string text = "")
        {
            return 1;
        }
    }

    public class  FalseClassifier : IClassifier
    {
        public double Classify(string text = "")
        {
            return 0;
        }
    }
}

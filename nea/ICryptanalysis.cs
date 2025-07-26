using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nea
{
    internal interface ICryptanalysis
    {
        IEnumerable<byte[]> GetKeys(string text);
    }


    public class ROT13Cryptanalysis : ICryptanalysis
    {

        public IEnumerable<byte[]> GetKeys(string text)
        {
            for (int i = 1; i <= 26; i++)
            {
                yield return BitConverter.GetBytes(i);
            }
        }

    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment4_Video
{
    public class Histogram
    {
        private Dictionary<int, int> histogramBinIntensity = new Dictionary<int, int>();
        int histogramBinSize = 10;
        public void initializeHistogramIntensityMethod()
        {
            for (int i = 0; i < 25; i++)
            {
                histogramBinIntensity.Add(i, 0);
            }
        }
        public void addIntensityToBin(double intensity)
        {
            bool found = false;
            double quotient = intensity / 10;
            double ceiling = Math.Floor(quotient);
            int keyIndex = (int)ceiling;
            if (keyIndex == 25)
            {
                int index = keyIndex - 1;
                if (histogramBinIntensity.ContainsKey(index))
                {
                    found = true;
                    int val = histogramBinIntensity[index];
                    val++;
                    histogramBinIntensity[index] = val;

                }
            }
            else
            {
                if (histogramBinIntensity.ContainsKey(keyIndex))
                {
                    found = true;
                    int val = histogramBinIntensity[keyIndex];
                    val++;
                    histogramBinIntensity[keyIndex] = val;

                }
            }
          
            if (!found)
            {
                throw new Exception("Bug!!!!!");
            }
        }

        public Dictionary<int,int> getHIstogramValueImage()
        {
            return histogramBinIntensity;
        }
    }
}

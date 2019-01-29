using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Assignment4_Video
{

    public class TwinComparisionApproach
    {
        private double standardDeviation = 0.0;
        private double meanValue = 0.0;
        private double Tb;
        private double Ts;
        private int Tor = 2;
        List<Result> result = new List<Result>();
        List<ShotsSet> finalShotSet = new List<ShotsSet>();
        //List<Result> result = new List<Result>();
        List<int> startValuesfinalCandiadateGradualTransition = new List<int>();
        List<int> startCutValues = new List<int>();
        List<int> endValuesfinalCandiadateGradualTransition = new List<int>();
        List<int> endCutValues = new List<int>();
        private void calculateMean()
        {
            string[] readText;
            readText = File.ReadAllLines(Environment.CurrentDirectory + @"\frameToFrameDifference.txt");
            int sum = 0;
            int count = 0;
            int lineCount = 0;
            while (lineCount != readText.Length)
            {
                int value = Convert.ToInt32(readText[lineCount]);
                sum = value + sum;
                lineCount++;
                count++;
                
            }
            meanValue = sum / count;
            //Console.WriteLine(meanValue + "is mean value");
        }

        private void calculateStandardDeviation()
        {
            string[] readText;
            double intermediateResult = 0.0;
            int count = 0;
            readText = File.ReadAllLines(Environment.CurrentDirectory + @"\frameToFrameDifference.txt");
            int lineCount = 0;
            while (lineCount != readText.Length)
            {
                int value = Convert.ToInt32(readText[lineCount]);
                double val = Math.Pow((value - meanValue), 2);
                intermediateResult = val + intermediateResult;
                lineCount++;
                count++;
            }
            double deviation = (double)intermediateResult / count - 1;
            standardDeviation = Math.Sqrt(deviation);
            //Console.WriteLine(standardDeviation + "is standardDeviation");
        }

        public double getStandardDeviationValue()
        {
            calculateStandardDeviation();
            return standardDeviation;
        }

        public double getMeanValue()
        {
            calculateMean();
            return meanValue;
        }
        public void setThreshold()
        {
            Tb = meanValue + standardDeviation * 11;
            Ts = meanValue * 2;
            Console.WriteLine("Tb value" + Tb);
            Console.WriteLine("Ts value" + Ts);
        }

        private int FindFe(int fsSdValue, int startIndex, List<int> cutValues, string[] file)
        {
            int sum = fsSdValue;

            for (int feCandidateIndex = startIndex; feCandidateIndex < file.Length; feCandidateIndex++)
            {
                int sdValue = Convert.ToInt32(file[feCandidateIndex]);                

                // We have reached cut value
                if (sdValue >= Tb)
                {
                    if (sum >= Tb)
                    {
                        return feCandidateIndex - 1;
                    }
                    else
                    {
                        return -1;
                    }
                }

                sum += sdValue;

                // check next 2 neighbours
                bool isCandidate = true;
                for (int index = feCandidateIndex + 1; index < feCandidateIndex + 1 + Tor; index++)
                {
                    sdValue = Convert.ToInt32(file[index]);
                    if (sdValue >= Ts)
                    {
                        isCandidate = false;
                        break;
                    }                    
                }

                if (!isCandidate)
                {
                    continue;
                }

                if (sum >= Tb)
                {
                    return feCandidateIndex;
                }
                else
                {
                    return -1;
                }
            }

            return -1;
        }
        //Twin-comparision approach
        public void computeCutInVideo()
        {
            //List<int> TorRangeForEndFrameValue = new List<int>();
            List<int> cutValues = new List<int>();
            string[] file = File.ReadAllLines(Environment.CurrentDirectory + @"\frameToFrameDifference.txt");
            //2.5.1 : cut values
            for (int i = 0; i < file.Length; i++)
            {
                int SD = Convert.ToInt32(file[i]);
                if (SD >= Tb)
                {
                    int cs = i;
                    int ce = i + 1;
                    cutValues.Add(ce+1000);
                    this.startCutValues.Add(cs+1000);
                    this.endCutValues.Add(ce + 1000);
                    int frameNumber = ce + 1000;
                    Console.WriteLine("Ce value" +  frameNumber);
                }
            }

            //2.5.2
            for (int index = 0; index < file.Length;)
            {
                int sdValue = Convert.ToInt32(file[index]);
                int fsIndex = 0;
                int feIndex = 0;
                if (sdValue >= Ts && sdValue < Tb)
                {
                    fsIndex = index;
                    feIndex = FindFe(sdValue, fsIndex + 1, cutValues, file);
                    if (feIndex >= 0)
                    {
                        index = feIndex + 1;
                        this.startValuesfinalCandiadateGradualTransition.Add(fsIndex+1000);
                        this.endValuesfinalCandiadateGradualTransition.Add(feIndex+1000);
                        // fsCandidateActualIndex = fsIndex + 1000;
                        //ADDING 1 to fsIndexand feIndex because our program starts with 0 index(so fsIndex = FsIndex+1) and adding 1000 to get actual frame number from aLL THE FRAMES
                        //We are starting iterating from 1000th frame to 4999th frame of a video
                        
                        Console.WriteLine("Gradual start index:" + (fsIndex + 1000)  + "end index:" + (feIndex+ 1000));
                    }
                    else
                    {
                        index++;
                    }
                }
                else
                {
                    index++;
                }                
            }
        }
        //Buid shots from computed cs,ce and fs,fe values
        public List<ShotsSet> shots()
        {

            List<int> cutBoundaries = new List<int>();
            cutBoundaries.AddRange(endCutValues);
            //cutBoundaries = endCutValues;
            List<int> startGradualTranslationsFrameCandidates = startValuesfinalCandiadateGradualTransition;
            //Merge above two lists
            foreach (int startGradualTranslations in startGradualTranslationsFrameCandidates)
            {
                cutBoundaries.Add(startGradualTranslations+1);
            }
            cutBoundaries.Sort();
            //Add the shots to final shot set
            for (int index = 0; index < cutBoundaries.Count; index++)
            {
                ShotsSet shot = new ShotsSet();
                if (index == 0)
                {
                    //create first frame default
                    shot.beginningIndex = 1000;
                    shot.endIndex = cutBoundaries[index] - 1;
                    finalShotSet.Add(shot);
                    //create 2nd frame set also
                    ShotsSet secondShot = new ShotsSet();
                    secondShot.beginningIndex = cutBoundaries[index];
                    secondShot.endIndex = cutBoundaries[index + 1] - 1;
                    finalShotSet.Add(secondShot);
                }
                else if (index == cutBoundaries.Count - 1)
                {
                    shot.beginningIndex = cutBoundaries[index];
                    shot.endIndex = 5000;
                    finalShotSet.Add(shot);
                }
                else
                {
                    shot.beginningIndex = cutBoundaries[index];
                    shot.endIndex = cutBoundaries[index + 1] - 1;
                    finalShotSet.Add(shot);
                }
            }
            return finalShotSet;
        }

        public void outputVideoShotsValue()
        {
            string filePath = Environment.CurrentDirectory + @"\shotSets.txt";
            StringBuilder sb = new StringBuilder();

            foreach (ShotsSet s in finalShotSet)
            {
                Console.Write("" + "[" + s.beginningIndex+ "," +s.endIndex + "]" + ";" );
                string shotsValue =  "[" + s.beginningIndex + "," + s.endIndex + "]";                                
                sb.AppendLine(shotsValue);                
            }

            File.WriteAllText(filePath, sb.ToString());
        }

        public void OutputsGenerateFile()
        {
            int numberOfCutValues =this.startCutValues.Count;
            int numberOfGradualTransition = this.startValuesfinalCandiadateGradualTransition.Count;
            int index = 0;
            int gradualTransitionIndex = 0;
            StringBuilder sb = new StringBuilder();
            string filePath = Environment.CurrentDirectory + @"\outputs.txt";            
            
            while (index < numberOfCutValues)
            {
                string cScEValues;
                int cs = this.startCutValues.ElementAt(index);
                int ce = this.endCutValues.ElementAt(index);
                cScEValues = "(Cs,Ce)" + ":" + "(" + cs + "," + ce + ")";
                sb.AppendLine(cScEValues);
                index++;
            }
            while (gradualTransitionIndex < numberOfGradualTransition)
            {
                string fSfEValue;
                int fs = this.startValuesfinalCandiadateGradualTransition.ElementAt(gradualTransitionIndex);
                int fe = this.endValuesfinalCandiadateGradualTransition.ElementAt(gradualTransitionIndex);
                fSfEValue = "(Fs,Fe)" + ":" + "(" + fs + "," + fe + ")";
                sb.AppendLine(fSfEValue);
                gradualTransitionIndex++;
            }

            File.WriteAllText(filePath, sb.ToString());
        }
     }
 }



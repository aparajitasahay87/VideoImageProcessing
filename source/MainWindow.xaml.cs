using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
//using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AForge;
using AForge.Video;
using AForge.Video.FFMPEG;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Windows.Threading;

//using System.Drawing.Bitmap;
namespace Assignment4_Video
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> files;
        string[] readText;
        List<Histogram> allimageDetailIntensityMethod = new List<Histogram>();
        List<ShotsSet> finalShots = new List<ShotsSet>();
        public MainWindow()
        {
            InitializeComponent();
            //ReadVideoFile();
            readEachFrame();
            //getFrameToFrameDifference();
            computeTwinComparisionApproach();
        }

        //Call following function only once...
        public void ReadVideoFile()
        {
            VideoFileReader reader = new VideoFileReader();
            List<Bitmap> frameCapture = new List<Bitmap>();
            reader.Open(Environment.CurrentDirectory + @"\20020924_juve_dk_02a_1.avi"); 
            // check some of its attributes
            Console.WriteLine("width:  " + reader.Width);
            Console.WriteLine("height: " + reader.Height);
            Console.WriteLine("fps:    " + reader.FrameRate);
            Console.WriteLine("codec:  " + reader.CodecName);
            // read 100 video frames out of it
            for (int i = 0; i < 5000; i++)
            {
                Bitmap videoFrame = reader.ReadVideoFrame();
                if (i <1000)
                {                    
                    continue;               
                }
                videoFrame.Save(Environment.CurrentDirectory +  @"/images/VideoFrameImageDetails" + i + ".bmp");
                videoFrame.Dispose();
            }

            reader.Close();
        }
        public void readEachFrame()
        {
            string path = Environment.CurrentDirectory + "/" + "images";
            files =  new List<string>(Directory.GetFiles(path, "*.bmp", SearchOption.AllDirectories));
            //getPixelValueEachImage(files);
        }
            
        public void getPixelValueEachImage(List<string> files)
        {
         
            foreach (string filepath in files)
            {
                    Histogram addHistogram = new Histogram();
                    addHistogram.initializeHistogramIntensityMethod();
                    Bitmap b = null;
                    try
                    {
                        b = new Bitmap(filepath);
                        int width = b.Width;
                        int height = b.Height;
                        int totalPixel = height * width;
                        int pixelCount = 0;
                        for (int i = 0; i < width; i++)
                        {
                            for (int j = 0; j < height; j++)
                            {
                                Color pixel = b.GetPixel(i, j);
                                byte red = pixel.R;
                                byte green = pixel.G;
                                byte blue = pixel.B;
                                //Intensity method
                                double I = 0.299 * red + 0.587 * green + 0.114 * blue;
                                addHistogram.addIntensityToBin(I);
                                pixelCount++;
                            }
                        }
                        //get histogram of an image
                        Dictionary<int,int> imageHistogram=addHistogram.getHIstogramValueImage();
                        Console.WriteLine(filepath);
                        //Write the historam value from dictionary to a file
                        writeHistogramValueToFile(imageHistogram,filepath);
                        allimageDetailIntensityMethod.Add(addHistogram);
                    }
                    finally
                    {
                        if (b != null)
                        {
                            b.Dispose();
                            b = null;
                        }
                    }
            }
              
        }

        public void writeHistogramValueToFile(Dictionary<int,int> imageHistogram,string filePath)
        {
            string fileName = System.IO.Path.GetFileName(filePath);
            StringBuilder builder = new StringBuilder();
            //builder = null;
            foreach (KeyValuePair<int, int> pair in imageHistogram)
            {
                //write each key value to a file 
                builder.Append(pair.Key).Append(":").Append(pair.Value).Append(',');
            }
            
            string result = builder.ToString();
            result = result.TrimEnd(',');
            //true to append the content in a file and false to overwrite in the file 
            using (StreamWriter writer = new StreamWriter(Environment.CurrentDirectory + @"\histogramValue.txt",true))
            {
                writer.WriteLine(result);
            }
            //File.WriteAllText(Environment.CurrentDirectory+@"/HistogramValue"+"frameHistogrameValue.txt",result);
            builder.Clear();
        }

        public void getFrameToFrameDifference()
        {
            //read text file 
            readText = File.ReadAllLines(Environment.CurrentDirectory + @"\histogramValue.txt");
            int LineIndex = 0;
            while (LineIndex != (readText.Length-1))
            {
                List<int> currentLineBinValues =getBinValues(LineIndex);
                List<int> nextLineBinValues = getBinValues(LineIndex + 1);
                int listIndex = 0;
                double totalDistance = 0.0;
                while (listIndex < currentLineBinValues.Count)
                {
                    int value1=currentLineBinValues.ElementAt(listIndex);
                    int value2 = nextLineBinValues.ElementAt(listIndex);
                    totalDistance += Math.Abs(value1 - value2);
                    listIndex++; 
                }
                writeFrameToFrameDifferenceIntoFile(totalDistance, LineIndex);
                Console.Write(LineIndex);
                LineIndex++;
            }
        }

        public void writeFrameToFrameDifferenceIntoFile(double totalDistance,int LineIndex)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(totalDistance);
            string result = builder.ToString();
            //true to append the content in a file and false to overwrite in the file 
            using (StreamWriter writer = new StreamWriter(Environment.CurrentDirectory + @"\frameToFrameDifference.txt", true))
            {
                writer.WriteLine(result);
            }
            builder.Clear();
        }

        public List<int> getBinValues(int LineNumber)
        {
            if (LineNumber < 4000)
            {
                string Line = readText[LineNumber];
                List<int> binValues = new List<int>();
                string[] currentLineSplit = Line.Split(',');
                for (int j = 0; j < currentLineSplit.Length; j++)
                {
                    string keyValuePair = currentLineSplit[j];
                    string[] eachPair = keyValuePair.Split(':');
                    string binValue = eachPair[1];
                    int value = Convert.ToInt32(binValue);
                    binValues.Add(value);
                }
                return binValues;
            }
            else
            {
                return null;
            }
        }

        public void computeTwinComparisionApproach()
        {
             TwinComparisionApproach tw = new TwinComparisionApproach();
             double meanValue = tw.getMeanValue();
             //Console.Write(meanValue);
             double standardDeviation = tw.getStandardDeviationValue();
             //Console.Write(standardDeviation);
             tw.setThreshold();
             tw.computeCutInVideo();
             finalShots=tw.shots();
             tw.outputVideoShotsValue();
             tw.OutputsGenerateFile();

        }
        public void findFirstShot(List<ShotsSet> finalShots)
        {
            List<ImageDetails> imagelist = new List<ImageDetails>();
            foreach (ShotsSet s in finalShots)
            {
                ImageDetails img = new ImageDetails();
                string filePath = files.ElementAt(s.beginningIndex-1000);
                img.image = new Uri(filePath);
                img.fileIndex = s.beginningIndex- 1000;
                imagelist.Add(img);
            }
            disp.ItemsSource = imagelist;
        }

        private void videoFrames_click(object sender, RoutedEventArgs e)
        {
            findFirstShot(finalShots);
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Stop();
        }
        
        private void imageselected_click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            System.Windows.Controls.Image buttonImage = button.Content as System.Windows.Controls.Image; //Cntrl+alt+q (During debugging)
            if (buttonImage == null ||
               buttonImage.Source == null)
            {
                throw new Exception("bug!!");
            }
            string imagePath = buttonImage.Source.ToString();
            string result = System.IO.Path.GetFileNameWithoutExtension(imagePath);
            
            string[] digits = Regex.Split(result, @"\D+");
            int index =Convert.ToInt32(digits[1]);
            int beginIndex = index / 25;
            int endIndex = 0;
            foreach (ShotsSet s in finalShots)
            {
                if (s.beginningIndex == index)
                {
                    endIndex = s.endIndex;
                    break;
                }
            }
            int delta = endIndex - index;
            int stopMilliSeconds = (delta *1000)/ 25;
            //int framePersecond = Math.Truncate(f);
            DispatcherTimer timer = new DispatcherTimer();
            mePlayer.Play();
            timer.Interval = TimeSpan.FromMilliseconds(stopMilliSeconds);
            timer.Start();
            mePlayer.Position += TimeSpan.FromSeconds(beginIndex);
            TimeSpan begin = mePlayer.Position;
            //int frameNumber = Convert.ToInt32(begin) * 25;
            timer.Tick += (o, args) =>
            {
                timer.Stop();
                //lblEnd.Content = "EndFrame:" + (int) (mePlayer.Position.TotalSeconds * 25);
                lblEnd.Content=  "EndFrame:" + endIndex;
                mePlayer.Stop();
                lblStart.Content = "StartFrame:" + index;
                
            };
        }
    }
}

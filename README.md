# VideoImageProcessing
Window Presentation foundation application that automatically detects the boundaries between two camera shots
Assignment 4: Option 2 Video Shot Boundary Detection
Name: Aparajita Sahay
Date: 6/5/2014
Running the program:
In the user interface, click on Video shots, which will display the list of first frames of all the shots. Once the user will click on any image of frame , corresponding video will be displayed for each shot.  Output file is generated in executable folder: outputs.txt and shotSets.txt
Libraries/Tool and steps used:
1.	 Coding Environment used: Visual Studio Community 2013. I have developed a Windows Presentation Foundation (WPF) application. Technology used: c# and calm.
2.	 Steps to run the program: Create an “images” folder at the same location where WpfApplication4.exe is located or saved. “Images” folder which will store 4000 frames of a video as a bmp image.
3.	 To read the video file and store the frames from 1000 to 4999 I have used FFmpeg using Aforge library.
4.	To use Aforge : I made sure to have FFmpeg binaries (DLLs) in the output folder of the application in order to use this class successfully. FFmpeg binaries can be found in Externals folder provided with AForge.NET framework's distribution.
5.	To retrieve the frames from 1000 to 4999, I created the instance of video reader and using Framerate property, stored the Framerate of the video. This framerate value I have used later as well to display video of gradual transition in a video. 
6.	With the use of ReadVideoFrame method, I read all the frame of a video but stored only those videos which were relevant for the assignment (i.e. frame no 1000 to 4999). I have stored the frames in images folder.
7.	Once the images folder is generated which stores all the relevant frames of a video, I created the Histogram of each frame.
8.	 To create Histogram, I have used Intensity method and have created the file name “histogramValue.txt”. Since, we have 4000 images and for each image we have to generate histogram value, this process in itself takes around 15 minutes. So once I have generated the histogram of image, I have stored the data in machine to save time for precomputing the histogram each time code is executed. After generating the histogram I will be using the text file for following computations. 
9.	Once histogram is generated. I compute the frame different between Image (I) and image (i+1).And store the values in frameToFrameDifference.txt.
10.	Once the frameToFrameDifference.txt is generated I compute the mean Value and standard deviation of the feature matrix.
11.	Using mean value and standard deviation value, Compute the Tb, Ts. Tb value97595.6263944918
Tso value 7670.
12.	Then using the value compute the cut and potential start of a gradual transition value and using twin comparison approach compute the potential end of the gradual transition.
13.	After running the code text file is generated that stores the value of (cs, ce) and (fs, fe).

14.	To display video of the video shots computed I have used The Media Element controls in XAML.
15.	Media elements acts as a wrapper around Media Player, so that you can display video content at a given place in your application, and because of that, it can play both audio and video files. Media element exposes the method such as play video, stop video and pause video, which is easy to use. In addition, Media Element exposes position property which seeks the video to particular time as well. To display video shots I have also used Afore Framerate values to compute the time when a particular frame will appear in a video. And with the help of position property I seek the video to particular time. Setting this property can enable you to jump to different points in playback (also known as seeking). Not all media types allow seek operations. However, to stop the video once it is started from a particular time, there was not direct method exposed. So I to accomplish that task I used Dispatcher Timer Class. This class sets a timer that is integrated into the Dispatcher queue which is processed at a specified interval of time and at a specified priority. Using Stops method of Dispatcher Timer I computed when I can stop the video of a shot. Variable that I computed was start Time and end Time of a shot. And this can be easily computed, since I have the start index and enwinding of a shop and the frame rate value (25), number of frames per second.
Precomputed Value: 
1) To retrieve frames 1000 to frames 4999 I have used the //ReadVideoFile() method in my class. Since retrieving each frame from video file takes 2-3 minutes I have saved the frame in images folder. Used ReadVideoFrame() method defined in the class.
2) To compute the Histogram Value of each image it takes around 15 minutes, therefore to save time I have saved the histogram value already. Method used is: readEachFrame() and //getPixelValueEachImage() method.
3) Also the compute SD values of 4000 images takes around 3-4 minutes, so I have stored the values in a text file. Method used is getFrameToFrameDifference().
Note: I have commented //ReadVideoFile(), //getPixelValueEachImage() calling this method from readEachFrame() method,getFrameToFrameDifference(). Methods in my program but I have executed it once to generate all the text files which are relevant for future computations in an algorithm.
User Interface:
To play the video shot, click on each image, each image is the first frame of each shot. Once you click these images corresponding shot video will be played. Also I am using stop button to stop the video. 
II. GUI: First frame of each shot:
  

![VideAnalysis](https://github.com/aparajitasahay87/VideoImageProcessing/blob/master/VideoAnalysis.png)

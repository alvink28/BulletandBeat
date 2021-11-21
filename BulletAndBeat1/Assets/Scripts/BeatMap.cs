////using System.Collections;
////using System.Collections.Generic;
////using UnityEngine;

////public class BeatMap : MonoBehaviour
////{
////    public AudioSource audioSource;
////    // Start is called before the first frame update
////    void Start()
////    {

////    }

////    // Update is called once per frame
////    void Update()
////    {
////        if (audioSource.time >= 0f && audioSource.time < 10f)
////        {
////            float[] curSpectrum = new float[1024];
////            audioSource.GetSpectrumData(curSpectrum, 0, FFTWindow.BlackmanHarris);

////            float targetFrequency = 234f;
////            float hertzPerBin = (float)AudioSettings.outputSampleRate / 2f / 1024;
////            int targetIndex = (int)(targetFrequency / hertzPerBin);

////            string outString = "";
////            for (int i = targetIndex - 3; i <= targetIndex + 3; i++)
////            {
////                outString += string.Format("| Bin {0} : {1}Hz : {2} |   ", i, i * hertzPerBin, curSpectrum[i]);
////            }

////            Debug.Log(outString);
////        }
////    }
////}

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Threading;
//using UnityEngine;

//using System.Numerics;
//using DSPLib;


//public class BeatMap : MonoBehaviour
//{

//    float[] realTimeSpectrum;
//    SpectralFluxAnalyzer realTimeSpectralFluxAnalyzer;
//    //PlotController realTimePlotController;

//    int numChannels;
//    int numTotalSamples;
//    int sampleRate;
//    float clipLength;
//    float[] multiChannelSamples;
//    SpectralFluxAnalyzer preProcessedSpectralFluxAnalyzer;
//    //PlotController preProcessedPlotController;

//    AudioSource audioSource;

//    public bool realTimeSamples = true;
//    public bool preProcessSamples = false;

//    public GameObject target1;

//    void Start()
//    {
//        audioSource = GetComponent<AudioSource>();

//        target1.SetActive(false);

//        //Process audio as it plays

//        if (realTimeSamples)
//        {
//            realTimeSpectrum = new float[1024];
//            realTimeSpectralFluxAnalyzer = new SpectralFluxAnalyzer();
//            realTimePlotController = GameObject.Find("RealtimePlot").GetComponent<PlotController>();

//            this.sampleRate = AudioSettings.outputSampleRate;
//        }

//        //Preprocess entire audio file upfront

//        if (preProcessSamples)
//        {
//            preProcessedSpectralFluxAnalyzer = new SpectralFluxAnalyzer();
//            preProcessedPlotController = GameObject.Find("PreprocessedPlot").GetComponent<PlotController>();

//            //Need all audio samples.  If in stereo, samples will return with left and right channels interweaved
//            //[L, R, L, R, L, R]

//            multiChannelSamples = new float[audioSource.clip.samples * audioSource.clip.channels];
//            numChannels = audioSource.clip.channels;
//            numTotalSamples = audioSource.clip.samples;
//            clipLength = audioSource.clip.length;

//            //We are not evaluating the audio as it is being played by Unity, so we need the clip's sampling rate

//            this.sampleRate = audioSource.clip.frequency;

//            audioSource.clip.GetData(multiChannelSamples, 0);
//            Debug.Log("GetData done");

//            Thread bgThread = new Thread(this.getFullSpectrumThreaded);

//            Debug.Log("Starting Background Thread");
//            bgThread.Start();
//        }

//        //temp
//        //plotPoints = new List<Transform>();

//        //float localWidth = transform.Find("Point/BasePoint").localScale.x;
//        ////-n / 2...0...n / 2

//        //for (int i = 0; i < displayWindowSize; i++)
//        //{
//        //    //Instantiate point

//        //    Transform t = (Instantiate(Resources.Load("Point"), transform) as GameObject).transform;

//        //    //Set position

//        //    float pointX = (displayWindowSize / 2) * -1 * localWidth + i * localWidth;
//        //    t.localPosition = new Vector3(pointX, t.localPosition.y, t.localPosition.z);

//        //    plotPoints.Add(t);
//        //}
//    }
//    //temp
//    //public void updatePlot(List<SpectralFluxInfo> pointInfo, int curIndex = -1)
//    //{
//    //    if (plotPoints.Count < displayWindowSize - 1)
//    //        return;

//    //    int numPlotted = 0;
//    //    int windowStart = 0;
//    //    int windowEnd = 0;

//    //    if (curIndex > 0)
//    //    {
//    //        windowStart = Mathf.Max(0, curIndex - displayWindowSize / 2);
//    //        windowEnd = Mathf.Min(curIndex + displayWindowSize / 2, pointInfo.Count - 1);
//    //    }
//    //    else
//    //    {
//    //        windowStart = Mathf.Max(0, pointInfo.Count - displayWindowSize - 1);
//    //        windowEnd = Mathf.Min(windowStart + displayWindowSize, pointInfo.Count);
//    //    }

//    //    for (int i = windowStart; i < windowEnd; i++)
//    //    {
//    //        int plotIndex = numPlotted;
//    //        numPlotted++;

//    //        Transform fluxPoint = plotPoints[plotIndex].Find("FluxPoint");
//    //        Transform threshPoint = plotPoints[plotIndex].Find("ThreshPoint");
//    //        Transform peakPoint = plotPoints[plotIndex].Find("PeakPoint");


//    //        if (pointInfo[i].isPeak)
//    //        {
//    //            setPointHeight(peakPoint, pointInfo[i].spectralFlux);
//    //            setPointHeight(fluxPoint, 0f);
//    //            target1.SetActive(true);
//    //        }
//    //        else
//    //        {
//    //            setPointHeight(fluxPoint, pointInfo[i].spectralFlux);
//    //            setPointHeight(peakPoint, 0f);
//    //        }
//    //        setPointHeight(threshPoint, pointInfo[i].threshold);
//    //    }
//    //}

//    //public void setPointHeight(Transform point, float height)
//    //{
//    //    float displayMultiplier = 0.1f;

//    //    point.localPosition = new Vector3(point.localPosition.x, height * displayMultiplier, point.localPosition.z);
//    //}

//    void Update()
//    {
//        //Real - time

//        if (realTimeSamples)
//        {
//            audioSource.GetSpectrumData(realTimeSpectrum, 0, FFTWindow.BlackmanHarris);
//            realTimeSpectralFluxAnalyzer.analyzeSpectrum(realTimeSpectrum, audioSource.time);
//            realTimePlotController.updatePlot(realTimeSpectralFluxAnalyzer.spectralFluxSamples);
//        }

//       // Preprocessed

//        if (preProcessSamples)
//        {
//            int indexToPlot = getIndexFromTime(audioSource.time) / 1024;
//            preProcessedPlotController.updatePlot(preProcessedSpectralFluxAnalyzer.spectralFluxSamples, indexToPlot);
//        }
//    }

//    public int getIndexFromTime(float curTime)
//    {
//        float lengthPerSample = this.clipLength / (float)this.numTotalSamples;

//        return Mathf.FloorToInt(curTime / lengthPerSample);
//    }

//    public float getTimeFromIndex(int index)
//    {
//        return ((1f / (float)this.sampleRate) * index);
//    }

//    public void getFullSpectrumThreaded()
//    {
//        try
//        {
//            //We only need to retain the samples for combined channels over the time domain

//           float[] preProcessedSamples = new float[this.numTotalSamples];

//            int numProcessed = 0;
//            float combinedChannelAverage = 0f;
//            for (int i = 0; i < multiChannelSamples.Length; i++)
//            {
//                combinedChannelAverage += multiChannelSamples[i];

//                //Each time we have processed all channels samples for a point in time, we will store the average of the channels combined

//                if ((i + 1) % this.numChannels == 0)
//                    {
//                        preProcessedSamples[numProcessed] = combinedChannelAverage / this.numChannels;
//                        numProcessed++;
//                        combinedChannelAverage = 0f;
//                    }
//            }

//            Debug.Log("Combine Channels done");
//            Debug.Log(preProcessedSamples.Length);

//            //Once we have our audio sample data prepared, we can execute an FFT to return the spectrum data over the time domain

//            int spectrumSampleSize = 1024;
//            int iterations = preProcessedSamples.Length / spectrumSampleSize;

//            FFT fft = new FFT();
//            fft.Initialize((UInt32)spectrumSampleSize);

//            Debug.Log(string.Format("Processing {0} time domain samples for FFT", iterations));
//            double[] sampleChunk = new double[spectrumSampleSize];
//            for (int i = 0; i < iterations; i++)
//            {
//               // Grab the current 1024 chunk of audio sample data

//                Array.Copy(preProcessedSamples, i * spectrumSampleSize, sampleChunk, 0, spectrumSampleSize);

//                //Apply our chosen FFT Window

//                double[] windowCoefs = DSP.Window.Coefficients(DSP.Window.Type.Hanning, (uint)spectrumSampleSize);
//                double[] scaledSpectrumChunk = DSP.Math.Multiply(sampleChunk, windowCoefs);
//                double scaleFactor = DSP.Window.ScaleFactor.Signal(windowCoefs);

//                //Perform the FFT and convert output(complex numbers) to Magnitude

//                Complex[] fftSpectrum = fft.Execute(scaledSpectrumChunk);
//                double[] scaledFFTSpectrum = DSPLib.DSP.ConvertComplex.ToMagnitude(fftSpectrum);
//                scaledFFTSpectrum = DSP.Math.Multiply(scaledFFTSpectrum, scaleFactor);

//               // These 1024 magnitude values correspond(roughly) to a single point in the audio timeline

//                float curSongTime = getTimeFromIndex(i) * spectrumSampleSize;

//                //Send our magnitude data off to our Spectral Flux Analyzer to be analyzed for peaks

//               preProcessedSpectralFluxAnalyzer.analyzeSpectrum(Array.ConvertAll(scaledFFTSpectrum, x => (float)x), curSongTime);

//            }

//            Debug.Log("Spectrum Analysis done");
//            Debug.Log("Background Thread Completed");

//        }
//        catch (Exception e)
//        {
//            //Catch exceptions here since the background thread won't always surface the exception to the main thread

//            Debug.Log(e.ToString());
//        }
//    }


//}

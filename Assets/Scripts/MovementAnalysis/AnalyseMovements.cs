using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;

public class AnalyseMovements : MonoBehaviour
{
    // Declare variables
    List<float>[] minMaxAngles;
    float maxAngle;
    float minAngle;
    bool minSearching;
    bool maxSearching;

    int angleCounter = 0;
    int initialSearch;
    int i;

    // This has a series of functions that are used to analyse vectors
    /// <summary> 
    /// Finds the maximum and minimum angles achieved by the joint, over the repetitions
    /// </summary>
    /// <param name="angles">A list of floats which are the angles that have been achieved.</param>
    /// <returns>A 2D List, List<float>[2] - The first list is the time which is the min angles, the latter of which is the max angles
    public List<float>[] FindMinMaxes(List<float> angles, List<float> timeStamps, int tStepThreshold, float baselineTime = 3.0f)
    {
        // Defining a 2D list of floats, with set size of 3 rows, variable amount of columns
        minMaxAngles = new List<float>[4];
        minMaxAngles[0] = new List<float>(); // min angles
        minMaxAngles[1] = new List<float>(); // min angle timestamps
        minMaxAngles[2] = new List<float>(); // max angles
        minMaxAngles[3] = new List<float>(); // max angle timestamps

        // Let it be ambiguous to whether you are starting going up or down in angles
        maxAngle = 0.0f;
        minAngle = 0.0f;
        minSearching = false;
        maxSearching = false;
        
        angleCounter = 0;
        initialSearch = 0; // Used in the while loop, which understands whether we start going up or down
        
        // Only want to start after the hold_time
        while (timeStamps[initialSearch] < baselineTime)
        {
            initialSearch += 1;
        }

        float currentAngle = angles[initialSearch];
        float currentTimeStamp = timeStamps[initialSearch];
        // Find out whether we start going up or going down
        while (angleCounter < tStepThreshold)
        {
            initialSearch +=1 ; // Want to increment initial search value 
            if (angles[initialSearch] < currentAngle)
            {
                if (maxSearching) 
                {
                    angleCounter = 0;
                    minSearching = true;
                    maxSearching = false;
                }
                else
                {
                    minSearching = true;
                }

                angleCounter += 1;
            }

            if (angles[initialSearch] > currentAngle)
            {
                if (minSearching) 
                {
                    angleCounter = 0;
                    maxSearching = true;
                    minSearching = false;
                }
                else 
                {
                    maxSearching = true;
                    minSearching = false;
                }
                angleCounter += 1;
            }

            if (angleCounter == tStepThreshold)
            {
                if (maxSearching)
                {
                    // We are going up to start with
                    maxAngle = currentAngle;
                }
                else if (minSearching)
                {
                    minAngle = currentAngle;
                }
            }
        }

        angleCounter = 0; // reset here

        for (int i=initialSearch; i<angles.Count; i++)
        {
            // If Going down, then you are minSearching is less than
            if (minSearching)
            {
                // if it is less than current, you update the minAngle
                if (angles[i] <= currentAngle)
                {
                    currentAngle = angles[i];
                    currentTimeStamp = timeStamps[i];
                }
                else 
                {
                    // This has gone up when we've been minsearching
                    // Don't update currentAngle, if this happens 10 times we know we bottomed out
                    angleCounter += 1;
                    if (angleCounter == tStepThreshold)
                    {
                        minMaxAngles[0].Add(currentAngle);
                        minMaxAngles[1].Add(currentTimeStamp);
                        minSearching = false;
                        maxSearching = true;
                        angleCounter = 0;
                        currentAngle = angles[i]; // update the angle so that you can start searching for max
                    }
                }
            }
            else // Can only be maxSeaching otherwise
            {
                if (angles[i] >= currentAngle)
                {
                    currentAngle = angles[i];
                    currentTimeStamp = timeStamps[i];
                }
                else
                {
                    angleCounter += 1;
                    if (angleCounter == tStepThreshold)
                    {
                        minMaxAngles[2].Add(currentAngle);
                        minMaxAngles[3].Add(currentTimeStamp);
                        minSearching = true;
                        maxSearching = false;
                        angleCounter = 0;
                        currentAngle = angles[i]; // update the angle so that you can start searching for min
                    }
                }
            }
        }

        return minMaxAngles;
    }

    public List<float> ApplyMovingAverage(List<float> angles, int movingAverageParam)
    {
        List<float> smoothedAngles = new List<float>();

        // Ensure the moving average parameter is valid
        if (movingAverageParam <= 0 || movingAverageParam > angles.Count)
        {
            return angles;  // Return the original list if the parameter is invalid
        }

        for (int i = 0; i < angles.Count; i++)
        {
            float sum = 0;
            int count = 0;

            // Calculate the start and end indices for the moving average window
            int startIdx = i - movingAverageParam / 2;
            int endIdx = i + movingAverageParam / 2;

            for (int j = startIdx; j <= endIdx; j++)
            {
                // Check for boundary conditions
                if (j >= 0 && j < angles.Count)
                {
                    sum += angles[j];
                    count++;
                }
            }

            smoothedAngles.Add(sum / count);
        }

        return smoothedAngles;
    }

    public List<float> CalculateAngularVelocities(List<float> smoothedAngles, List<float> timestamps)
    {
        List<float> angularVelocities = new List<float>();

        // Ensure that both lists have the same length or else return an empty list
        if (smoothedAngles.Count != timestamps.Count)
        {
            Debug.LogWarning("Smoothed Angles and Time Steps lists do not have the same length.");
            return angularVelocities;
        }

        for (int i = 0; i < smoothedAngles.Count - 1; i++)
        {
            float deltaAngle = smoothedAngles[i + 1] - smoothedAngles[i];
            float deltaTime = timestamps[i + 1] - timestamps[i];

            // To avoid division by zero
            if (deltaTime == 0)
            {
                angularVelocities.Add(0);
            }
            else
            {
                float angularVelocity = deltaAngle / deltaTime;
                angularVelocities.Add(angularVelocity);
            }
        }

        return angularVelocities;
    }

    public List<float> PadAngularVelocities(List<float> angularVelocities, int padToNumber)
    {
        // Check if padding is needed
        int paddingAmount = padToNumber - angularVelocities.Count;

        for (int i = 0; i < paddingAmount; i++)
        {
            angularVelocities.Add(0.0f);
        }

        return angularVelocities;
    }

    // FFT function
    public (List<double>, List<double>) ComputeFourierTransform(List<float> angularVelocitiesPadded, double samplingRate)
    {
        // Convert the list to a complex array
        var complexData = new Complex32[angularVelocitiesPadded.Count];
        for (int i = 0; i < angularVelocitiesPadded.Count; i++)
        {
            complexData[i] = new Complex32(angularVelocitiesPadded[i], 0.0f);
        }

        // Compute the Fourier transform
        Fourier.Forward(complexData, FourierOptions.Default);

        // Get the magnitude for real-valued data and positive frequencies
        List<double> fourierMag = new List<double>();
        List<double> freqs = new List<double>();

        int N = angularVelocitiesPadded.Count;
        double maxFrequency = 4.5; // upper limit for frequency
        double frequencyResolution = samplingRate / N;

        for (int i = 0; i < N / 2; i++) // Only consider the first half of bins for positive frequencies
        {
            double currentFrequency = i * frequencyResolution;
            if (currentFrequency <= maxFrequency) // Limit to 5Hz
            {
                fourierMag.Add(complexData[i].Magnitude);
                freqs.Add(currentFrequency);
            }
        }

        return (fourierMag, freqs);
    }

    public double ComputeSpectralArcLength(List<double> fourierMagRestricted)
    {
        double smoothnessMeasure = 0.0;
        int len = fourierMagRestricted.Count;

        for (int i = 0; i < len - 1; i++)
        {
            smoothnessMeasure -= Math.Sqrt(
                Math.Pow(1.0 / (len - 1), 2) +
                Math.Pow(fourierMagRestricted[i + 1] - fourierMagRestricted[i], 2)
            );
        }

            return smoothnessMeasure;
    }


    /// <summary> 
    /// Uses Spectral Arc-Length on a Fourier Transform to evaluate the smoothness of the inputted angles
    /// </summary>
    /// <param name="angles">A list of floats which are the angles that have been achieved.</param>
    /// <returns>A float, which is the measure of the motion smoothness
    public double EvaluateSmoothness(List<float> angles, List<float> timestamps)
    {
        // Evaluates the angles
        float samplingRate = 60.0f;
        float timestep = 1.0f / samplingRate;
        
        int movingAverageParam = 6; // 6 because we have 4.5Hz * 2, so 9Hz to detect up to 4.5Hz. 60FPS therefore 6FPS smoothing

        List<float> smoothedAngles = ApplyMovingAverage(angles, movingAverageParam);

        // Call calculate angular velocities function
        List<float> angularVelocities = CalculateAngularVelocities(smoothedAngles, timestamps);

        // Pad my smoothed angles
        int exponent = (int)Math.Ceiling((Math.Log(angularVelocities.Count) / Math.Log(2.0f)) + 4.0f);
        int padToNumber = (int)(Math.Pow(2.0f, exponent));

        List<float> paddedAngularVelocities = PadAngularVelocities(angularVelocities, padToNumber);

        // Now go through and get the limited fourier transform
        var (fourierMag, freqs) = ComputeFourierTransform(paddedAngularVelocities, (double)samplingRate);

        // Adjust the fourier mag by by dividing by the mag of the 0th entry
        // Debug.Log("The freq of the 0th entry is: " + freqs[0].ToString());
        // Debug.Log("The fourierMag of the 0th entry is: " + fourierMag[0].ToString());
        
        if (fourierMag[0] != 0)
        {
            for (int i = 0; i < fourierMag.Count; i++)
            {
                fourierMag[i] /= fourierMag[0];
            }
        }
        else
        {
            Debug.Log("entry 0 has fourier mag of 0");
        }

        double smoothnessMeasure = ComputeSpectralArcLength(fourierMag);

        return smoothnessMeasure;
    }

}

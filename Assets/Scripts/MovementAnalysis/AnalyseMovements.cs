using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public List<float>[] FindMinMaxes(List<float> angles)
    {
        // Defining a 2D list of floats, with set size of 3 rows, variable amount of columns
        minMaxAngles = new List<float>[2];
        minMaxAngles[0] = new List<float>();
        minMaxAngles[1] = new List<float>();

        // Let it be ambiguous to whether you are starting going up or down in angles
        maxAngle = 0.0f;
        minAngle = 0.0f;
        minSearching = false;
        maxSearching = false;
        
        angleCounter = 0;
        initialSearch = 0; // Used in the while loop, which understands whether we start going up or down
        
        float currentAngle = angles[initialSearch];
        // Find out whether we start going up or going down
        while (angleCounter < 10)
        {
            initialSearch +=1 ; // Want to increment initial search value 
            if (angles[i] < currentAngle)
            {
                if (maxSearching) 
                {
                    angleCounter = 0;
                    minSearching = true;
                }
                else
                {
                    maxSearching = false;
                    minSearching = true;
                }
                angleCounter += 1;
            }

            if (angles[i] > currentAngle)
            {
                if (minSearching) 
                {
                    angleCounter = 0;
                    maxSearching = true;
                }
                else 
                {
                    maxSearching = true;
                    minSearching = false;
                }
                angleCounter += 1;
            }

            if (angleCounter == 10)
            {
                if (maxSearching)
                {
                    // We are going up to start with
                    maxAngle = currentAngle;
                    angleCounter = 0;
                }
                else if (minSearching)
                {
                    minAngle = currentAngle;
                    angleCounter = 0;
                }
            }
        }

        for (int i=initialSearch; i<angles.Count; i++)
        {
            // If Going down, then you are minSearching is less than
            if (minSearching)
            {
                // if it is less than current, you update the minAngle
                if (angles[i] <= currentAngle)
                {
                    currentAngle = angles[i];
                }
                else 
                {
                    // This has gone up when we've been minsearching
                    // Don't update currentAngle, if this happens 10 times we know we bottomed out
                    angleCounter += 1;
                    if (angleCounter == 10)
                    {
                        minMaxAngles[0].Add(currentAngle);
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
                }
                else
                {
                    angleCounter += 1;
                    if (angleCounter == 10)
                    {
                        minMaxAngles[1].Add(currentAngle);
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

    public List<float> CalculateAngularVelocities(List<float> angles, float timestep)
    {
        List<float> angularVelocities = new List<float>();
        for (int i = 0; i < angles.Count - 1; i++)
        {
            float angularVelocity = (angles[i + 1] - angles[i]) / timestep;
            angularVelocities.Add(angularVelocity);
        }
        return angularVelocities;
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

    /// <summary> 
    /// Uses Spectral Arc-Length on a Fourier Transform to evaluate the smoothness of the inputted angles
    /// </summary>
    /// <param name="angles">A list of floats which are the angles that have been achieved.</param>
    /// <returns>A float, which is the measure of the motion smoothness
    public float EvaluateSmoothness(List<float> angles)
    {
        // Evaluates the angles
        float timestep = 1.0f / 60.0f;
        int movingAverageParam = 6; // 6 because we have 4.5Hz * 2, so 9Hz to detect up to 4.5Hz. 60FPS therefore 6FPS smoothing

        List<float> smoothedAngles = ApplyMovingAverage(angles, movingAverageParam);

        // Call calculate angular velocities function
        List<float> angularVelocities = CalculateAngularVelocities(smoothedAngles, timestep);

        // Pad my smoothed angles
        int exponent = (int)Math.Ceiling((Math.Log(angularVelocities.Count) / Math.Log(2.0f)) + 4.0f);
        int padToNumber = (int)(Math.Pow(2.0f, exponent)

        List<float> paddedAngularVelocities = PadAngularVelocities(angularVelocities, padToNumber)


    }


}

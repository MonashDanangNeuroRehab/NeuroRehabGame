using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class CalculateVectors : MonoBehaviour
{
    public List<float[]>[][] data;
    public List<float[]>[][] vectors;

    public Vector3 baselineVector;
    public Vector3 parallelVector;

    public List<float> angles;

    public List<float[]>[][] LoadData(string fileName)
    {
        string relativePath = Path.Combine("..", "Data", "Raw");
        string fullPath = Path.Combine(Application.dataPath, relativePath);
        string path = Path.Combine(fullPath, fileName);

        if (File.Exists(path))
        {
            string serializedData = File.ReadAllText(path);
            data = JsonConvert.DeserializeObject<List<float[]>[][]>(serializedData);
            Debug.Log("Data loaded from: " + path);
            return data;
        }
        else
        {
            Debug.Log("File not found: " + path);
            return null;
        }
    }

    public List<float[]>[][] CalculateVectorsMethod(List<float[]>[][] Rawdata)
    {
        vectors = new List<float[]>[5][];

        for (int i=0; i<5; i++)
        {
            vectors[i] = new List<float[]>[3];

            for (int j = 0; j < 3; j++)
            {
                vectors[i][j] = new List<float[]>();
            }
        }

        for (int i=0; i<5; i++)
        {
            for (int j=0; j<Rawdata[i][0].Count; j++) // Go down each timestep, calculate 3 vectors for the timesteps
            {
                // Each row is {t, x, y, z}
                // The origin of these isn't accurate - need to use previous vectors as starting coords.
                float[] meta = new float[4] {Rawdata[i][0][j][0], Rawdata[i][1][j][1] - Rawdata[i][0][j][1], Rawdata[i][1][j][2] - Rawdata[i][0][j][2], Rawdata[i][1][j][3] - Rawdata[i][0][j][3]};
                float[] prox = new float[4] {Rawdata[i][0][j][0], Rawdata[i][2][j][1] - Rawdata[i][1][j][1], Rawdata[i][2][j][2] - Rawdata[i][1][j][2], Rawdata[i][2][j][3] - Rawdata[i][1][j][3]};
                float[] intr = new float[4] {Rawdata[i][0][j][0], Rawdata[i][3][j][1] - Rawdata[i][2][j][1], Rawdata[i][3][j][2] - Rawdata[i][2][j][2], Rawdata[i][3][j][3] - Rawdata[i][2][j][3]};

                vectors[i][0].Add(meta); // Meta
                vectors[i][1].Add(prox); // Prox
                vectors[i][2].Add(intr); // Intr
            }
        }

        return vectors;
    }

    /// <summary> 
    /// Calculates the baseline vector for the inputted vectors array
    /// </summary>
    /// <param name="vectors">The vectors matrix that was calculated from the raw data points.</param>
    /// <param name="baselineFinger">The finger that is to be used as the baseline - 0 is thumb, 4 is pinky. </param>
    /// <param name="baselineBone">The bone vector which is to be used as the baseline - 0 is meta, 1 is prox, 2 is intr.</param>
    /// <param name="baselineTime">The time over which the baseline is established (must be a float).</param>
    /// <returns>A Vector3 - which is the baseline Vector.</returns>
    public Vector3 CalculateBaseLineVector(List<float[]>[][] vectors, int baselineFinger, int baselineBone, float baselineTime = 3.0f)
    {
        int tStep = 0;

        // Find average of the 1st, 2nd, 3rd element over the inputted finger and bone. This corresponds to the baselineVector's x, y, z.
        while (vectors[baselineFinger][baselineBone][tStep][0] < baselineTime)
        {
            baselineVector.x += vectors[baselineFinger][baselineBone][tStep][1];
            baselineVector.y += vectors[baselineFinger][baselineBone][tStep][2];
            baselineVector.z += vectors[baselineFinger][baselineBone][tStep][3];
            tStep++;
        }
        Debug.Log("tStep: " + tStep);

        // Don't need to divide through - instead I just need to normalise the vector.
        baselineVector.Normalize();
        Debug.Log("baselineVector x: " + baselineVector.x);
        Debug.Log("baselineVector y: " + baselineVector.y);
        Debug.Log("baselineVector z: " + baselineVector.z);
        
        return baselineVector;
    }

    /// <summary> 
    /// Returns the perpendicular vector against the baseline vector. This is actually the vector we consider rotation about
    /// </summary>
    /// <param name="handData">The original hand data, containing bone start points.</param>
    /// <param name="finger1">The finger that is the origin of the parallel vector  - 0 is thumb, 4 is pinky. </param>
    /// <param name="finger2">The finger that is the end of the parallel vector - 0 is thumb, 4 is pinky. </param>
    /// <param name="boneNumber">The bone whos based is used for the parallel vector calculation.</param>
    /// <returns> Vector3 of the Perpendicular Vector.</returns>
    public Vector3 CalculatePerpendicularVector(List<float[]>[][] handData, int finger1, int finger2, int boneNumber, float baselineTime = 3.0f)
    {
        
        parallelVector = new Vector3(0,0,0);
        int tStep = 0;
        
        // Average the coordinates over the baselineTime
        while (handData[finger1][boneNumber][tStep][0] < baselineTime)
        {
            parallelVector.x += handData[finger2][boneNumber][tStep][1] - handData[finger1][boneNumber][tStep][1];
            parallelVector.y += handData[finger2][boneNumber][tStep][2] - handData[finger1][boneNumber][tStep][2];
            parallelVector.z += handData[finger2][boneNumber][tStep][3] - handData[finger1][boneNumber][tStep][3];
            tStep++;
        }

        parallelVector.Normalize();
        return parallelVector;
    }

    public List<float> CalculateAngles(List<float[]>[][] vectors, int fingerNumber, int boneNumber, Vector3 baselineVector, Vector3 perpendicularVector) 
    {
        Debug.Log("Entered CalculateAngles Script");
        Debug.Log("Total tsteps is: " + vectors[fingerNumber][boneNumber].Count.ToString());
        angles = new List<float>();

        for (int i=0; i<vectors[fingerNumber][boneNumber].Count; i++)
        {
            Vector3 vector = new Vector3(vectors[fingerNumber][boneNumber][i][1], vectors[fingerNumber][boneNumber][i][2], vectors[fingerNumber][boneNumber][i][3]);
            float angle = Vector3.SignedAngle(vector, baselineVector, perpendicularVector);
            angles.Add(angle);
        }
        Debug.Log("Done calculating angles");
        return angles;
    }
}

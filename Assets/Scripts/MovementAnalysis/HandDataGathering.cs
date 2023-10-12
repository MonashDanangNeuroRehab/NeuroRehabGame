using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using UnityEngine;
using System.IO;
using System.Text;
using Newtonsoft.Json;

public class HandDataGathering : MonoBehaviour
{
    
    public LeapServiceProvider leapServiceProvider;
    private float time;

    // Get my CalculateVectors sheet.
    private CalculateVectors calculateVectorsScript;
    private AnalyseMovements analyseMovementsScript;

    public List<float[]>[][] handData = new List<float[]>[5][];
    public List<float[]>[][] loadedData;
    public List<float[]>[][] calculatedVectors;
    public List<float> angles;
    private string savedAngles;
    private List<float>[] maxMinAngles;
    private double smoothnessMeasure;
    private string currentDateTime;
    public List<float> timeStamps;

    public Vector3 baselineVector;
    public Vector3 perpendicularVector;
    public Vector3 zVector;

    private string fileName;

    private void OnEnable()
    {
        calculateVectorsScript = GameObject.Find("VectorGenerator").GetComponent<CalculateVectors>();
        analyseMovementsScript = GameObject.Find("MovementAnalyser").GetComponent<AnalyseMovements>();

        for (int i = 0; i < handData.Length; i++)
        {
            handData[i] = new List<float[]>[4];
            for (int j = 0; j < handData[i].Length; j++)
            {
                handData[i][j] = new List<float[]>();
            }
        }

        leapServiceProvider.OnUpdateFrame += OnUpdateFrame;
        time = 0.0f;
    }

    public void OnUpdateFrame(Frame frame)
    {
        //Get a list of all the hands in the frame and loop through
        //to find the first hand that matches the Chirality
        
        time += Time.deltaTime;
        //Use a helpful utility function to get the first hand that matches the Chirality
        Hand _rightHand = frame.GetHand(Chirality.Right);

        // Getting all the fingers
        for (int i=0; i<5; i++) // only 5 fingers
        {
            // Get the ith finger
            Finger _currentFinger = _rightHand.Fingers[i];

            // Get the bone data for this finger
            for (int j=0; j<4; j++) // only 4 bones per finger
            {
                float[] staticList = new float[4]; 
                staticList[0] = time;
                staticList[1] = _currentFinger.bones[j].PrevJoint.x; 
                staticList[2] = _currentFinger.bones[j].PrevJoint.y; 
                staticList[3] = _currentFinger.bones[j].PrevJoint.z;

                handData[i][j].Add(staticList);
                
            }
        }
    }

    private void OnDisable()
    {
        currentDateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");

        leapServiceProvider.OnUpdateFrame -= OnUpdateFrame;

        //Debug.Log($"Number of timesteps is: " + handData[0][0].Count);
        Debug.Log("Attempting to save data");
        fileName = SaveRawHandData(handData, currentDateTime);
        Debug.Log("Saved raw data");

        timeStamps = GetTimeValues(handData);
        SaveTimeValues(timeStamps, currentDateTime);
        Debug.Log("Saved time data");

        // loadedData = calculateVectorsScript.LoadData(fileName);
        // Debug.Log("Loaded Raw data");
        // Debug.Log("Final timepoint is: " + loadedData[0][0][loadedData[0][0].Count-1][0]);

        calculatedVectors = calculateVectorsScript.CalculateVectorsMethod(handData);
        Debug.Log("Calculated Vectors");

        baselineVector = calculateVectorsScript.CalculateBaseLineVector(calculatedVectors, 1, 0);
        Debug.Log("Found baseline vector");

        perpendicularVector = calculateVectorsScript.CalculatePerpendicularVector(handData, 1, 2, 0);
        Debug.Log("Found perp vector");

        zVector = Vector3.Cross(baselineVector, perpendicularVector);
        Debug.Log("Found z vector");

        angles = calculateVectorsScript.CalculateAngles(calculatedVectors, 1, 0, baselineVector, perpendicularVector);
        Debug.Log("Found Angles");

        savedAngles = SaveAnglesData(angles, currentDateTime);
        Debug.Log("Saved Angles");

        maxMinAngles = analyseMovementsScript.FindMinMaxes(angles, timeStamps, 15);
        Debug.Log("Found maxminAngles");

        SaveMaxMinAngles(maxMinAngles, currentDateTime);
        Debug.Log("Saved maxMin angles");

        smoothnessMeasure = analyseMovementsScript.EvaluateSmoothness(angles, timeStamps);
        Debug.Log("Found smoothness: " + smoothnessMeasure.ToString());

        SaveSmoothnessMeasure(smoothnessMeasure);
        Debug.Log("Saved smoothness data");
    }

    void PrintListToConsole(List<float> floatList)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("[");
        
        for (int i = 0; i < floatList.Count; i++)
        {
            sb.Append(floatList[i]);
            
            // Append a comma for all values except the last one
            if (i < floatList.Count - 1)
            {
                sb.Append(", ");
            }
        }

        sb.Append("]");
        Debug.Log(sb.ToString());
    }
    
    void PrintToConsole(List<float[]>[][] handData, int fingerNumber)
    {
        for (int i=0; i<4; i++)
        {

            Debug.Log($"Finger {fingerNumber} bone {i}: ");
            string arrayAsString = "[" + string.Join(", ", handData[fingerNumber][i][10]) + "]"; // can try handData[i][j] after
            Debug.Log(arrayAsString);
        }
    }

    void PrintTimeToConsole(List<float[]>[][] handData)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("[");

        for (int i=0; i<handData[0][0].Count; i++)
        {
            sb.Append(handData[0][0][i][0]);
            
            // Append a comma for all values except the last one
            if (i < handData[0][0].Count - 1)
            {
                sb.Append(", ");
            }
        }

        sb.Append("]");
        
        Debug.Log(sb.ToString());
    }

    void PrintVectorToConsole(List<float[]>[][] vectors, int fingerNumber)
    {
        for (int i=0; i<3; i++)
        {
            Debug.Log($"Finger {fingerNumber} bone {i} vector: ");
            string arrayAsString = "[" + string.Join(", ", vectors[fingerNumber][i][10]) + "]"; // can try handData[i][j] after
            Debug.Log(arrayAsString);
        }
    }

    string SaveRawHandData(List<float[]>[][] data, string currentDateTime)
    {
        string fileName = "handDataRaw_" + currentDateTime + ".json";

        string relativePath = Path.Combine("..", "Data", "Raw");
        string fullPath = Path.Combine(Application.dataPath, relativePath);
        
        // Ensure the "data" directory exists
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        string serializedData = JsonConvert.SerializeObject(data); 
        string path = Path.Combine(fullPath, fileName);
        File.WriteAllText(path, serializedData);

        return fileName;
    }

    string SaveAnglesData(List<float> angles, string currentDateTime)
    {
        string fileName = "Angles_" + currentDateTime + ".json";

        string relativePath = Path.Combine("..", "Data", "Analysed", "Angles" );
        string fullPath = Path.Combine(Application.dataPath, relativePath);

        // Ensure the "data" directory exists
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        string serializedData = JsonConvert.SerializeObject(angles);
        string path = Path.Combine(fullPath, fileName);
        File.WriteAllText(path, serializedData);

        return fileName;
    }

    public string SaveMaxMinAngles(List<float>[] maxMinAngles, string currentDateTime)
    {
        string fileName = "maxMinAngles_" + currentDateTime + ".json";

        string relativePath = Path.Combine("..", "Data", "Analysed", "MaxMinAngles");
        string fullPath = Path.Combine(Application.dataPath, relativePath);

        // Ensure the "data" directory exists
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        string serializedData = JsonConvert.SerializeObject(maxMinAngles);
        string path = Path.Combine(fullPath, fileName);
        File.WriteAllText(path, serializedData);

        return fileName;
    }

    public void SaveSmoothnessMeasure(double smoothnessMeasure)
    {
        string relativePath = Path.Combine("..", "Data", "Analysed", "SmoothnessData.json");
        string fullPath = Path.Combine(Application.dataPath, relativePath);

        // List to hold the smoothness measures
        List<double> smoothnessData;

        // Check if the file exists
        if (File.Exists(fullPath))
        {
            // If it exists, load the existing data
            string existingData = File.ReadAllText(fullPath);
            smoothnessData = JsonConvert.DeserializeObject<List<double>>(existingData);
        }
        else
        {
            // If the file doesn't exist, create a new list
            smoothnessData = new List<double>();
        }

        // Append the new smoothness measure
        smoothnessData.Add(smoothnessMeasure);

        // Ensure the "Analysed" directory exists
        string directoryPath = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Save the updated data back to the file
        string serializedData = JsonConvert.SerializeObject(smoothnessData);
        File.WriteAllText(fullPath, serializedData);
    }

    public List<float> GetTimeValues(List<float[]>[][] handData)
    {
        List<float> time = new List<float>();
        for (int i = 0; i < handData[0][0].Count; i++)
        {
            time.Add(handData[0][0][i][0]);
        }

        return time;
    }

    public void SaveTimeValues(List<float> time, string currentDateTime)
    {
        string fileName = "timeStamps_" + currentDateTime + ".json";

        string relativePath = Path.Combine("..", "Data", "Analysed", "Timestamps");
        string fullPath = Path.Combine(Application.dataPath, relativePath);

        // Ensure the "data" directory exists
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        string serializedData = JsonConvert.SerializeObject(time);
        string path = Path.Combine(fullPath, fileName);
        File.WriteAllText(path, serializedData);
    }

}
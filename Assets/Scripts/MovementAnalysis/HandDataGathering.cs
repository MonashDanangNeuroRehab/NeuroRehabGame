using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using UnityEngine;
using System.IO;
using System.Text;

public class HandDataGathering : MonoBehaviour
{
    public LeapServiceProvider leapServiceProvider;
    private float time;
    private List<List<float>>[] handData = new List<List<float>>[5];
    private List<float> staticList = new List<float>(new float[19]);
    // public List<List<List<float>>> handData = new List<List<List<float>>>(5);
    // private List<List<float>> listOfLists = new List<List<float>>();
    //public List<float> staticList = new List<float>(16);

    private void OnEnable()
    {
        leapServiceProvider.OnUpdateFrame += OnUpdateFrame;
        time = 0.0f;

        // Initialise the empty array
        for (int i = 0; i < handData.Length; i++)
        {
            handData[i] = new List<List<float>>();
        }

        /*
        // fill out the array with 0.0s
        for (int i=0; i<16; i++)
        {
            staticList.Add(0.0f);
        }

        listOfLists.Add(staticList); // So I can reference

        for (int i=0; i<5; i++)
        {
            handData.Add(listOfLists);
        }
        */
    }
    private void OnDisable()
    {
        leapServiceProvider.OnUpdateFrame -= OnUpdateFrame;
    }

    void OnUpdateFrame(Frame frame)
    {
        //Get a list of all the hands in the frame and loop through
        //to find the first hand that matches the Chirality
        
        time += Time.deltaTime;
        //Use a helpful utility function to get the first hand that matches the Chirality
        Hand _rightHand = frame.GetHand(Chirality.Right);

        staticList[0] = time;

        // Getting all the fingers
        for (int i=0; i<handData.Length; i++)
        {
            // Get the ith finger
            Finger _currentFinger = _rightHand.Fingers[i];
            Arm _currentArm = _rightHand.Arm;

            // Get the bone data for this finger
            for (int j=0; j<4; j++)
            {
                staticList[j*3 + 1] = _currentFinger.bones[j].PrevJoint.x; 
                staticList[j*3 + 2] = _currentFinger.bones[j].PrevJoint.y; 
                staticList[j*3 + 3] = _currentFinger.bones[j].PrevJoint.z;

                if (i == 3) 
                {
                    // Need to get the tip too
                    staticList[j*4 + 1] = _currentFinger.TipPosition.x; 
                    staticList[j*4 + 2] = _currentFinger.TipPosition.y; 
                    staticList[j*4 + 3] = _currentFinger.TipPosition.z;
                } 
            }

            staticList[16] = _currentArm.ElbowPosition.x;
            staticList[17] = _currentArm.ElbowPosition.y;
            staticList[18] = _currentArm.ElbowPosition.z;
            
            /* DEBUGGING
            Debug.Log($"Current finger number is {i}");
            PrintListToConsole(staticList);

            
            if (i == 0)
            {
                PrintListToConsole(staticList);
            }
            */
            handData[i].Add(new List<float>(staticList));
        }


        /*
        staticList[0] = time;

        // THUMB
        for (int i=0; i<4; i++)
        {
            staticList[i*3 + 1] = _rightThumb.bones[i].PrevJoint.x; 
            staticList[i*3 + 2] = _rightThumb.bones[i].PrevJoint.y; 
            staticList[i*3 + 3] = _rightThumb.bones[i].PrevJoint.z;

            if (i == 3) 
            {
                // Need to get the tip too
                staticList[i*4 + 1] = _rightThumb.TipPosition.x; 
                staticList[i*4 + 2] = _rightThumb.TipPosition.y; 
                staticList[i*4 + 3] = _rightThumb.TipPosition.z;
            } 
        }
        thumb.Add(staticList);

        //PrintListToConsole(staticList);

        // INDEX FINGER
        for (int i=0; i<4; i++)
        {
            staticList[i*3 + 1] = _rightIndex.bones[i].PrevJoint.x; 
            staticList[i*3 + 2] = _rightIndex.bones[i].PrevJoint.y; 
            staticList[i*3 + 3] = _rightIndex.bones[i].PrevJoint.z;

            if (i == 3) 
            {
                // Need to get the tip too
                staticList[i*4 + 1] = _rightIndex.TipPosition.x; 
                staticList[i*4 + 2] = _rightIndex.TipPosition.y; 
                staticList[i*4 + 3] = _rightIndex.TipPosition.z;
            } 
        }
        index.Add(staticList);

        // MIDDLE FINGER
        for (int i=0; i<4; i++)
        {
            staticList[i*3 + 1] = _rightMiddle.bones[i].PrevJoint.x; 
            staticList[i*3 + 2] = _rightMiddle.bones[i].PrevJoint.y; 
            staticList[i*3 + 3] = _rightMiddle.bones[i].PrevJoint.z;

            if (i == 3) 
            {
                // Need to get the tip too
                staticList[i*4 + 1] = _rightMiddle.TipPosition.x; 
                staticList[i*4 + 2] = _rightMiddle.TipPosition.y; 
                staticList[i*4 + 3] = _rightMiddle.TipPosition.z;
            } 
        }
        middle.Add(staticList);

        // RING FINGER
        for (int i=0; i<4; i++)
        {
            staticList[i*3 + 1] = _rightRing.bones[i].PrevJoint.x; 
            staticList[i*3 + 2] = _rightRing.bones[i].PrevJoint.y; 
            staticList[i*3 + 3] = _rightRing.bones[i].PrevJoint.z;

            if (i == 3) 
            {
                // Need to get the tip too
                staticList[i*4 + 1] = _rightRing.TipPosition.x; 
                staticList[i*4 + 2] = _rightRing.TipPosition.y; 
                staticList[i*4 + 3] = _rightRing.TipPosition.z;
            } 
        }
        ring.Add(staticList);

        // PINKY FINGER
        for (int i=0; i<4; i++)
        {
            staticList[i*3 + 1] = _rightRing.bones[i].PrevJoint.x; 
            staticList[i*3 + 2] = _rightRing.bones[i].PrevJoint.y; 
            staticList[i*3 + 3] = _rightRing.bones[i].PrevJoint.z;

            if (i == 3) 
            {
                // Need to get the tip too
                staticList[i*4 + 1] = _rightRing.TipPosition.x; 
                staticList[i*4 + 2] = _rightRing.TipPosition.y; 
                staticList[i*4 + 3] = _rightRing.TipPosition.z;
            } 
        }
        pinky.Add(staticList);
        */
        
        /*
        foreach (Bone bone in _rightthumb.bones)
        {
            
        }

        Debug.Log("PRINTING RIGHT INDEX FINGER BONE TYPES");
        foreach (Bone bone in _rightindex.bones)
        {
            Debug.Log(bone.Type);
        }

        // Getting all of the remaining bones
        */
    }

    private void OnDestroy()
    {
        // Trying to just print out last few rows of one type to see if its the variable or if its the function
        /*
        PrintListToConsole(handData[0][0]);
        PrintListToConsole(handData[0][1]);
        PrintListToConsole(handData[0][2]);
        PrintListToConsole(handData[0][3]);
        PrintListToConsole(handData[0][4]);
        PrintListToConsole(handData[0][5]);
        */

        SaveDataToCSV(handData[0],"thumb");
        SaveDataToCSV(handData[1],"index");
        SaveDataToCSV(handData[2],"middle");
        SaveDataToCSV(handData[3],"ring");
        SaveDataToCSV(handData[4],"pinky");
    }

    void PrintListToConsole(List<float> list)
    {
        string result = "[";

        for (int i = 0; i < list.Count; i++)
        {
            result += list[i].ToString();
            
            if (i < list.Count - 1)
            {
                result += ", ";
            }
            else {
                result += "]";
            }
        }

        Debug.Log(result);
    }

    void SaveDataToCSV(List<List<float>> data, string fileName)
    {
        StringBuilder csv = new StringBuilder();
        
        csv.AppendLine(string.Join(",", new List<string> {"time", "meta_x", "meta_y", "meta_z", "prox_x", "prox_y", "prox_z", "intr_x", "intr_y", "intr_z", "dist_x", "dist_y", "dist_z", "tips_x", "tips_y", "tips_z", "elbow_x", "elbow_y", "elbow_z"}));
        
        foreach (List<float> row in data)
        {
            csv.AppendLine(string.Join(",", row));
        }

        string fullFileName = GenerateFileNameWithDate(fileName, "csv");

        string path = "C:/Users/xavie/OneDrive/Documents/University/Mechatronics Units/FYP/Unity Data/" + fullFileName;
        File.WriteAllText(path, csv.ToString());

        Debug.Log("Data saved to: " + path);
    }

    public static string GenerateFileNameWithDate(string baseFileName, string extension)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        return $"{timestamp}_{baseFileName}.{extension}";
    }
}
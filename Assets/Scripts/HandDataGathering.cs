using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using UnityEngine;

public class HandDataGathering : MonoBehaviour
{
    public LeapServiceProvider leapServiceProvider;
    private float time;
    private List<List<float>> thumb = new List<List<float>>();
    private List<List<float>> index = new List<List<float>>();
    private List<List<float>> middle = new List<List<float>>();
    private List<List<float>> ring = new List<List<float>>();
    private List<List<float>> pinky = new List<List<float>>();

    public List<float> staticList = new List<float>(16);

    private void OnEnable()
    {
        leapServiceProvider.OnUpdateFrame += OnUpdateFrame;
        time = 0.0f;

        // fill out the array with 0.0s
        for (int i=0; i<16; i++)
        {
            staticList.Add(0.0f);
        }
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
        Hand _leftHand = frame.GetHand(Chirality.Left);
        Hand _rightHand = frame.GetHand(Chirality.Right);

        // Getting all the fingers
        Finger _rightThumb = _rightHand.GetThumb();
        Finger _rightIndex = _rightHand.GetIndex();
        Finger _rightMiddle = _rightHand.GetMiddle();
        Finger _rightRing = _rightHand.GetRing();
        Finger _rightPinky = _rightHand.GetPinky();

        /*
        Debug.Log(staticList);
        
        staticList[1] = 1.0f;
        staticList[15] = 1.0f;
        Debug.Log(staticList);
        Debug.Log("DONE ASSIGNING");
        */
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
}
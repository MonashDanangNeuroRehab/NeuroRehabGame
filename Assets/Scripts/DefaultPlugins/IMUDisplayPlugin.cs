using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp;
using RosSharp.RosBridgeClient;
using TMPro;

public class IMUDisplayPlugin : MonoBehaviour
{
    [SerializeField]
    GameObject arrowPrefab;
    [SerializeField]
    GameObject rosConnector;
    private IMUSubscriber targetSub;
    TextMeshPro debugDisplay;
    private List<RosSharp.RosBridgeClient.MessageTypes.Sensor.Imu> messageArray;
    WaitForSeconds updateInterval = new WaitForSeconds(0.075f);
    // TF Data
    TFListener tfListener;
    List<GameObject> publishedTFTree;
    GameObject headerFrameObj;
    Transform headerFrame;

    // Start is called before the first frame update
    void Start()
    {
        messageArray = new List<RosSharp.RosBridgeClient.MessageTypes.Sensor.Imu>();
        debugDisplay = this.gameObject.GetComponent<TextMeshPro>();
    }
    IEnumerator MarkerRenderer()
    {
        while(true)
        {
            targetSub = rosConnector.GetComponent<IMUSubscriber>();
            if (targetSub is null)
            {
                yield return updateInterval;
            }
            else
            {
                messageArray = targetSub.GetPublishedIMU();
                if (messageArray is null)
                {
                    yield return updateInterval;
                }
                else
                {
                    // Update the latest onto the screen
                    debugDisplay.text = messageArray[targetSub.historyLength - 1].ToString();
                    yield return updateInterval;
                }
            }
        }
    }
}

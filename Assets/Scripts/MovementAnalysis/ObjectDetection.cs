using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Leap;
using Leap.Unity;
using TMPro;

public class ObjectDetection : MonoBehaviour
{
    public GameObject DetectionCube;
    public GameObject HoldStillText;
    public GameObject HandDataGatherer;
    public GameObject VectorGenerator;
    public GameObject MovementAnalyser;
    public LeapServiceProvider leapServiceProvider;
    public TextMeshProUGUI CommandText;
    private HandDataGathering handDataGatheringScript;

    private float timeObjectEntered;
    private bool recordingData = false;
    private bool isObjectInside = false;
    private Vector3 palmPos;
    private float countDownTimer;

    private void Start()
    {
        // Activate the DetectionCube at the start
        DetectionCube.SetActive(true);
        leapServiceProvider.OnUpdateFrame += OnUpdateFrame;
        handDataGatheringScript = HandDataGatherer.GetComponent<HandDataGathering>();
    }

    void OnUpdateFrame(Frame frame)
    {
        Hand _rightHand = frame.GetHand(Chirality.Right);
        // Debug.Log(_rightHand.PalmPosition);
        palmPos = new Vector3 (_rightHand.PalmPosition.x, _rightHand.PalmPosition.y, _rightHand.PalmPosition.z);
    
        // Check if myObject is within the DetectionCube's bounds    

        if (DetectionCube.activeSelf && DetectionCube.GetComponent<Collider>().bounds.Contains(palmPos) && !recordingData)
        {
            if (!isObjectInside)
            {
                isObjectInside = true;
                timeObjectEntered = Time.time;
                HoldStillText.SetActive(true);
            }

            if (isObjectInside && Time.time - timeObjectEntered >= 1)
            {
                countDownTimer = 30.0f;
                recordingData = true;
                VectorGenerator.SetActive(true);
                MovementAnalyser.SetActive(true);
                HandDataGatherer.SetActive(true);
                StartCoroutine(ActivateAfterDelay());
            }
        }
        else
        {
            if (!recordingData)
            {
                isObjectInside = false;
                HoldStillText.SetActive(false);
            }
        }

        if (CommandText.gameObject.activeInHierarchy)
        {
            // add in the time left
            countDownTimer -= Time.deltaTime;
            CommandText.text = "Wrist up and down: " + countDownTimer.ToString();
        }
    }

    IEnumerator ActivateAfterDelay()
    {
        Debug.Log("Start gathering data");
        yield return new WaitForSeconds(3);
        HoldStillText.SetActive(false);
        CommandText.gameObject.SetActive(true);

        yield return new WaitForSeconds(30);
        Debug.Log("Done gathering data");
        Debug.Log("Turning off data gatherer");
        // handDataGatheringScript.leapServiceProvider.OnUpdateFrame -= handDataGatheringScript.OnUpdateFrame;
        HandDataGatherer.SetActive(false);

        Debug.Log("Data gatherer off");
        yield return new WaitForSeconds(2);
        CommandText.gameObject.SetActive(false);
        VectorGenerator.SetActive(false);
        MovementAnalyser.SetActive(false);
        DetectionCube.SetActive(false);

        yield return new WaitForSeconds(5);
        recordingData = false;
        DetectionCube.SetActive(true);
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Leap;
using Leap.Unity;
using TMPro;

public class ObjectDetection : MonoBehaviour
{
    public GameObject ExerciseManager;
    public GameObject DetectionCube;
    public GameObject HandDataGatherer;
    public GameObject VectorGenerator;
    public GameObject MovementAnalyser;

    public LeapServiceProvider leapServiceProvider;
    public TextMeshProUGUI CommandText;
    public TextMeshProUGUI HoldStillText;
    public TextMeshProUGUI TimerText;
    private HandDataGathering handDataGatheringScript;
    private ExerciseManager exerciseManagerScript;
    private Coroutine commandCoroutine;

    public float timeObjectEntered;
    public bool recordingData;
    public bool isObjectInside;
    public Vector3 palmPos;
    public bool exerciseFinished;
    public string hand;
    public string currentJoint;

    // change every exercise
    public float countDownTimer;

    private Hand _hand;

    private void Start()
    {
        // Activate the DetectionCube at the start
        handDataGatheringScript = HandDataGatherer.GetComponent<HandDataGathering>();
    }

    private void OnEnable()
    {
        leapServiceProvider.OnUpdateFrame += OnUpdateFrame;
        DetectionCube.SetActive(true);

        exerciseManagerScript = ExerciseManager.GetComponent<ExerciseManager>();
        exerciseFinished = exerciseManagerScript.exerciseFinished;
        hand = exerciseManagerScript.hand;
        recordingData = false;
        isObjectInside = false;
        CommandText.text = "Move your hand into the detection sphere";
        CommandText.gameObject.SetActive(true);

        setUpDetector(hand);
    }

    void setUpDetector(string hand)
    {
        if (hand.ToUpper() == "LEFT")
        {
            // Move over the sphere to the left
            DetectionCube.transform.position = new Vector3(0.25f, 0.145f, 0.27f);
        }
        else
        {
            DetectionCube.transform.position = new Vector3(-0.25f, 0.145f, 0.27f);
        }
    }

    void OnUpdateFrame(Frame frame)
    {
        // Instruct the user to move their hand into detection sphere.

        // Get the right hand data - something off with below, doesn't like being in the if else loop?

        Hand _rightHand = frame.GetHand(Chirality.Right);
        Hand _leftHand = frame.GetHand(Chirality.Left);
        
        if (exerciseManagerScript.hand.ToUpper() == "LEFT")
        {
            if (!exerciseFinished)
            {
                palmPos = new Vector3(_leftHand.PalmPosition.x, _leftHand.PalmPosition.y, _leftHand.PalmPosition.z);

                // Check if myObject is within the DetectionCube's bounds    

                if (DetectionCube.activeSelf && DetectionCube.GetComponent<Collider>().bounds.Contains(palmPos) && !recordingData)
                {
                    CommandText.text = "Hold your hand still";
                    if (!isObjectInside)
                    {
                        // start the timer
                        countDownTimer = 4.0f;
                        TimerText.text = $"{countDownTimer:N1}";
                        TimerText.gameObject.SetActive(true);
                        isObjectInside = true;
                        timeObjectEntered = Time.time;
                        HoldStillText.gameObject.SetActive(true);
                    }

                    if (isObjectInside && Time.time - timeObjectEntered >= 1)
                    {
                        Debug.Log("Activating other objects AAA");
                        recordingData = true;
                        VectorGenerator.SetActive(true);
                        // Debug.Log("Vector Generator Active");

                        MovementAnalyser.SetActive(true);
                        // Debug.Log("Movement Analyser Active");

                        HandDataGatherer.SetActive(true);
                        // Debug.Log("Hand Data Analyser Active");

                        StartCoroutine(ActivateAfterDelay());
                    }
                    countDownTimer -= Time.deltaTime;
                    TimerText.text = $"{countDownTimer:N1}";
                }
                else if (DetectionCube.activeSelf && !DetectionCube.GetComponent<Collider>().bounds.Contains(palmPos) && !recordingData)
                {
                    CommandText.text = "Move your hand into the detection sphere";
                    TimerText.gameObject.SetActive(false);
                }

                if (recordingData)
                {
                    // TimerText.gameObject.SetActive(false);
                    countDownTimer -= Time.deltaTime;
                    TimerText.text = $"{countDownTimer:N1}";
                }
            }
        }

        else
        {
            if (!exerciseFinished)
            {
                palmPos = new Vector3(_rightHand.PalmPosition.x, _rightHand.PalmPosition.y, _rightHand.PalmPosition.z);

                // Check if myObject is within the DetectionCube's bounds    

                if (DetectionCube.activeSelf && DetectionCube.GetComponent<Collider>().bounds.Contains(palmPos) && !recordingData)
                {
                    CommandText.text = "Hold your hand still";
                    if (!isObjectInside)
                    {
                        // start the timer
                        countDownTimer = 4.0f;
                        TimerText.text = $"{countDownTimer:N1}";
                        TimerText.gameObject.SetActive(true);
                        isObjectInside = true;
                        timeObjectEntered = Time.time;
                        HoldStillText.gameObject.SetActive(true);
                    }

                    if (isObjectInside && Time.time - timeObjectEntered >= 1)
                    {
                        Debug.Log("Activating other objects AAA");
                        recordingData = true;
                        VectorGenerator.SetActive(true);
                        // Debug.Log("Vector Generator Active");

                        MovementAnalyser.SetActive(true);
                        // Debug.Log("Movement Analyser Active");

                        HandDataGatherer.SetActive(true);
                        // Debug.Log("Hand Data Analyser Active");

                        StartCoroutine(ActivateAfterDelay());
                    }
                    countDownTimer -= Time.deltaTime;
                    TimerText.text = $"{countDownTimer:N1}";
                }
                else if (DetectionCube.activeSelf && !DetectionCube.GetComponent<Collider>().bounds.Contains(palmPos) && !recordingData)
                {
                    CommandText.text = "Move your hand into the detection sphere";
                    TimerText.gameObject.SetActive(false);
                }

                if (recordingData)
                {
                    // TimerText.gameObject.SetActive(false);
                    countDownTimer -= Time.deltaTime;
                    TimerText.text = $"{countDownTimer:N1}";
                }
            }
        }
    }

    IEnumerator ActivateAfterDelay()
    {
        Debug.Log("Start gathering data");
        yield return new WaitForSeconds(3);
        commandCoroutine = StartCoroutine(CommandMovements());
        HoldStillText.gameObject.SetActive(false);

        countDownTimer = exerciseManagerScript.currentSetActivityDuration;
        yield return new WaitForSeconds(exerciseManagerScript.currentSetActivityDuration);
        Debug.Log("Done gathering data");
        HandDataGatherer.SetActive(false);
        DetectionCube.SetActive(false);
        Debug.Log("Data gatherer off");
        
        StopCoroutine(commandCoroutine);
        CommandText.text = $"Take a {exerciseManagerScript.currentSetRestDuration} second break";
        countDownTimer = exerciseManagerScript.currentSetRestDuration;

        yield return new WaitForSeconds(2); // takes a little bit to do the analysis
        VectorGenerator.SetActive(false);
        MovementAnalyser.SetActive(false);
        

        yield return new WaitForSeconds(exerciseManagerScript.currentSetRestDuration - 2.0f);
        recordingData = false;
        isObjectInside = false;
        TimerText.gameObject.SetActive(false);
        CommandText.gameObject.SetActive(false);
        exerciseManagerScript.exerciseFinished = true;
        leapServiceProvider.OnUpdateFrame -= OnUpdateFrame;
    }

    IEnumerator CommandMovements()
    {
        while (true)
        {
            if (exerciseManagerScript.currentExerciseType.ToUpper() == "EXTENSION/FLEXION")
            {
                CommandText.text = $"{hand} {exerciseManagerScript.currentJoint} down.";
                yield return new WaitForSeconds(exerciseManagerScript.currentSetMovementDuration);
                CommandText.text = $"{hand} {exerciseManagerScript.currentJoint} up.";
                yield return new WaitForSeconds(exerciseManagerScript.currentSetMovementDuration);
            }
            else
            {
                CommandText.text = $"{hand} {exerciseManagerScript.currentJoint} left.";
                yield return new WaitForSeconds(exerciseManagerScript.currentSetMovementDuration);
                CommandText.text = $"{hand} {exerciseManagerScript.currentJoint} right.";
                yield return new WaitForSeconds(exerciseManagerScript.currentSetMovementDuration);
            }
        }
    }
}

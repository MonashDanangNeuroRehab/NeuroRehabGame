using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap;
using Leap.Unity.Query;
using UnityEngine.UI;
using System.IO;

public class RecordingCalibrator : PostProcessProvider
{
    // General variables
    public int HAND_ID = 0;
    public const int LEFT_HAND = 0;
    public const int RIGHT_HAND = 1;

    // Initial variables
    private Vector3 _initialPalmPosition;
    private Vector3 _initialPalmarAxis;
    private Vector3 _initialDistalAxis;
    private bool _isCalibrated = false;
    private bool _isHandFirstDetected = false;
    private int _calibrationCounter = 0;
    private float _calibrationTime = 2f;
    private float _calibrationAngleLimit = 10;
    private Vector3 _initialWristPosition;
    private float _initialTime;

    // Current variables
    private Vector3 _prevPalmarAxis;
    private Vector3 _prevDistalAxis;
    private float _deltaAngle;
    private List<float> _palmPositionDisplacement = new List<float>();
    private List<float> _wristPositionDisplacement = new List<float>();
    private List<float> _palmarAngles = new List<float>();
    private List<float> _palmarAngularVelocity = new List<float>();
    private List<float> _distalAngles = new List<float>();
    private List<float> _filteredDistalAngles = new List<float>();
    private List<float> _distalAngularVelocity = new List<float>();
    private List<float> _timeVector = new List<float>();
    private List<Vector3> _locations = new List<Vector3>();

    // Time
    private float _prevTime;
    private float _deltaTime;

    // Display-related variables
    [SerializeField]
    private GameObject _score;
    private Text _scoreText;
    [SerializeField]
    private GameObject _scoreLabel;
    [SerializeField]
    private GameObject _gameNotif;
    private Text _gameNotifText;
    private bool _isStatusUpdated = false;
    private WaitForEndOfFrame _waitTime = new WaitForEndOfFrame();
    private string _calibrationNotif = "Please keep your hand still for calibration";
    private string _measuringNotif = "Ready For Recording";

    public Vector3 idealLocation = new Vector3(0.01f, 0.285f, -0.04f);
    public Leap.Unity.Playback.PlaybackRecorder playbackRecorder;
    public Leap.Unity.Playback.PlaybackProvider playbackProvider;

    public void Start()
    {
        _scoreText = _score.GetComponent<Text>();
        _gameNotifText = _gameNotif.GetComponent<Text>();
        StartCoroutine(displayCouroutine());
    }

    // Update is called once per frame
    public override void ProcessFrame(ref Frame inputFrame)
    {
        var leftHand = inputFrame.Hands.Query().FirstOrDefault(h => h.IsLeft);
        var rightHand = inputFrame.Hands.Query().FirstOrDefault(h => !h.IsLeft);
        if (Time.inFixedTimeStep)
        {
            if (!_isCalibrated)
            {
                calibrateHand(leftHand);
                // calibrateHand(rightHand);
            }
        }
    }

    private void calibrateHand(Hand hand)
    {
        if (hand != null)
        {
            if (!_isHandFirstDetected)
            {
                _prevPalmarAxis = hand.PalmarAxis();
                _prevTime = Time.realtimeSinceStartup;
                _isHandFirstDetected = true;
                _isCalibrated = false;
                _isStatusUpdated = false;
            }
            else if (!_isCalibrated)
            {
                _deltaTime = Time.realtimeSinceStartup - _prevTime;
                if (_deltaTime > _calibrationTime)
                {
                    _deltaAngle = Vector3.Angle(_prevPalmarAxis, hand.PalmarAxis());
                    if (_deltaAngle < _calibrationAngleLimit)
                    {
                        _calibrationCounter++;
                        // Maybe have a loading animation here
                    }
                    if (_calibrationCounter >= 3)
                    {
                        Debug.Log("Calibration Complete");
                        _isCalibrated = true;
                        _isStatusUpdated = false;
                        _initialDistalAxis = hand.DistalAxis();
                        _initialPalmarAxis = hand.PalmarAxis();
                        _initialPalmPosition = hand.GetPalmPose().position;
                        _initialWristPosition = hand.WristPosition.ToVector3();
                        _initialTime = Time.realtimeSinceStartup;

                        Vector3 pos = hand.GetPalmPose().position;
                        Quaternion rot = hand.GetPalmPose().rotation;

                        // playbackRecorder.transform.position = pos;
                        // playbackRecorder.transform.rotation = rot;

                        // playbackProvider.transform.position = pos;
                        // playbackProvider.transform.rotation = rot;
                    }
                    _prevPalmarAxis = hand.PalmarAxis();
                    _prevTime = Time.realtimeSinceStartup;
                }
            }
        }
    }
    IEnumerator displayCouroutine()
    {
        while (true)
        {
            if (!_isStatusUpdated)
            {
                if (!_isCalibrated)
                {
                    _gameNotifText.text = _calibrationNotif;
                }
                else
                {
                    _gameNotifText.text = _measuringNotif;
                    if (playbackProvider.isActiveAndEnabled)
                    {
                        playbackProvider.Play();
                    }
                }
                _isStatusUpdated = true;
            }
            yield return _waitTime;
        }
    }
    /*
    private void OnApplicationQuit()
    {
        StopCoroutine(displayCouroutine());
        // Convert the floats to strings
        string[] s_time = new string[_timeVector.Count];
        string[] s_palmPos = new string[_palmPositionDisplacement.Count];
        string[] s_wristPos = new string[_wristPositionDisplacement.Count];
        string[] s_palmarAg = new string[_palmarAngles.Count];
        string[] s_palmarAgVec = new string[_palmarAngularVelocity.Count];
        string[] s_distalAg = new string[_distalAngles.Count];
        string[] s_filteredDistalAg = new string[_filteredDistalAngles.Count];
        string[] s_distalAgVec = new string[_distalAngularVelocity.Count];
        string[] s_positionX = new string[_locations.Count];
        string[] s_positionY = new string[_locations.Count];
        string[] s_positionZ = new string[_locations.Count];

        for (int i = 0; i < _timeVector.Count; i++)
        {
            s_time[i] = _timeVector[i].ToString();
        }
        for (int i = 0; i < _palmPositionDisplacement.Count; i++)
        {
            s_palmPos[i] = _palmPositionDisplacement[i].ToString();
        }
        for (int i = 0; i < _wristPositionDisplacement.Count; i++)
        {
            s_wristPos[i] = _wristPositionDisplacement[i].ToString();
        }
        for (int i = 0; i < _palmarAngles.Count; i++)
        {
            s_palmarAg[i] = _palmarAngles[i].ToString();
        }
        for (int i = 0; i < _palmarAngularVelocity.Count; i++)
        {
            s_palmarAgVec[i] = _palmarAngularVelocity[i].ToString();
        }
        for (int i = 0; i < _distalAngles.Count; i++)
        {
            s_distalAg[i] = _distalAngles[i].ToString();
        }
        for (int i = 0; i < _filteredDistalAngles.Count; i++)
        {
            s_filteredDistalAg[i] = _filteredDistalAngles[i].ToString();
        }
        for (int i = 0; i < _distalAngularVelocity.Count; i++)
        {
            s_distalAgVec[i] = _distalAngularVelocity[i].ToString();
        }
        for (int i = 0; i < _locations.Count; i++)
        {
            if (_locations[i].x < 0)
            {
                s_positionX[i] = "-" + Mathf.Abs(_locations[i].x).ToString();
            }
            else
            {
                s_positionX[i] = _locations[i].x.ToString();
            }
        }
        for (int i = 0; i < _locations.Count; i++)
        {
            if (_locations[i].y < 0)
            {
                s_positionY[i] = "-" + Mathf.Abs(_locations[i].y).ToString();
            }
            else
            {
                s_positionY[i] = _locations[i].y.ToString();
            }
        }
        for (int i = 0; i < _locations.Count; i++)
        {
            if (_locations[i].z < 0)
            {
                s_positionZ[i] = "-" + Mathf.Abs(_locations[i].z).ToString();
            }
            else
            {
                s_positionZ[i] = _locations[i].z.ToString();
            }
        }
        // Write the lines into the file
        File.WriteAllLines(Application.persistentDataPath + "/time.txt", s_time);
        File.WriteAllLines(Application.persistentDataPath + "/palmPositionDisplacement.txt", s_palmPos);
        File.WriteAllLines(Application.persistentDataPath + "/wristPositionDisplacement.txt", s_wristPos);
        File.WriteAllLines(Application.persistentDataPath + "/palmarAngles.txt", s_palmarAg);
        File.WriteAllLines(Application.persistentDataPath + "/palmarAngularVelocity.txt", s_palmarAgVec);
        File.WriteAllLines(Application.persistentDataPath + "/distalAngles.txt", s_distalAg);
        File.WriteAllLines(Application.persistentDataPath + "/filteredDistalAngles.txt", s_filteredDistalAg);
        File.WriteAllLines(Application.persistentDataPath + "/distalAngularVelocity.txt", s_distalAgVec);
        File.WriteAllLines(Application.persistentDataPath + "/locationX.txt", s_positionX);
        File.WriteAllLines(Application.persistentDataPath + "/locationY.txt", s_positionY);
        File.WriteAllLines(Application.persistentDataPath + "/locationZ.txt", s_positionZ);
        Debug.Log(Application.persistentDataPath);
    }
    */
}

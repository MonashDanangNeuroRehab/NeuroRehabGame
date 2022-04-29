using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap;
using UnityEngine.UI;

public class RehabMiniGame4 : PostProcessProvider
{
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
    private string _wristUpNotif = "Please flex your wrist up";
    private string _wristDownNotif = "Please flex your wrist down";
    private string _calibrationNotif = "Please keep your hand still for calibration";

    // Game variable
    private bool _isPalmUp = false;
    private bool _isPalmUpPrev = true;
    private float _prevTime;
    private float _currTime;
    private float _calculatedScore = 0;
    private float _deltaAngle;
    private Vector3 _prevPalmarAxis;
    private Vector3 _currPalmarAxis;
    private float _scoreFactor = 1000f;

    public int HandID = 0;
    public static int LEFT_HAND = 0;
    public static int RIGHT_HAND = 1;

    // Calibration purposes
    private bool _isHandFirstDetected = false;
    private int _calibrationCounter = 0;
    private bool _isCalibrationDone = false;
    private float _calibrationTime = 2f;
    private float _calibrationAngleLimit = 10;
    private void Start()
    {
        _scoreText = _score.GetComponent<Text>();
        _gameNotifText = _gameNotif.GetComponent<Text>();
        StartCoroutine(displayCouroutine());
    }
    public override void ProcessFrame(ref Frame inputFrame)
    {
        if (inputFrame.Hands.Count > 0)
        {
            foreach (var hand in inputFrame.Hands)
            {
                if (HandID == LEFT_HAND && hand.IsLeft)
                {
                    calibrationHand(hand);
                    processHand(hand);
                }
                else if (HandID == RIGHT_HAND && hand.IsRight)
                {
                    calibrationHand(hand);
                    processHand(hand);
                }
            }
        }
        else
        {
            // Debug.LogWarning("No hands found");
        }

    }
    IEnumerator displayCouroutine()
    {
        while (true)
        {
            if (!_isStatusUpdated)
            {
                if (!_isCalibrationDone)
                {
                    _gameNotifText.text = _calibrationNotif;
                }
                else
                {
                    if (_isPalmUp)
                    {
                        _gameNotifText.text = _wristDownNotif;
                        _scoreText.text = Mathf.Round(_calculatedScore).ToString();
                    }
                    else
                    {
                        _gameNotifText.text = _wristUpNotif;
                        _scoreText.text = Mathf.Round(_calculatedScore).ToString();
                    }
                }
                _isStatusUpdated = true;
            }
            else
            {

            }
            yield return _waitTime;
        }
    }
    private void OnDestroy()
    {
        StopCoroutine(displayCouroutine());
    }
    private void OnApplicationQuit()
    {
        StopCoroutine(displayCouroutine());
    }
    private void calibrationHand(Hand hand)
    {
        if (!_isHandFirstDetected)
        {
            _currPalmarAxis = hand.PalmarAxis();
            _prevPalmarAxis = _currPalmarAxis;
            _currTime = Time.realtimeSinceStartup;
            _prevTime = _currTime;
            _isHandFirstDetected = true;
            _isCalibrationDone = false;
            _isStatusUpdated = false;
        }
        else if (!_isCalibrationDone)
        {
            _currTime = Time.realtimeSinceStartup;
            if (_currTime - _prevTime > _calibrationTime)
            {
                _currPalmarAxis = hand.PalmarAxis();
                _deltaAngle = Vector3.Angle(_currPalmarAxis, _prevPalmarAxis);
                _prevPalmarAxis = _currPalmarAxis;
                if (_deltaAngle < _calibrationAngleLimit)
                {
                    _calibrationCounter++;
                    // Maybe have a loading animation here
                }
                if (_calibrationCounter >= 3)
                {
                    Debug.Log("Calibration Complete");
                    _isCalibrationDone = true;
                    _isStatusUpdated = false;
                }
                _prevTime = _currTime;
            }
        }
    }
    private void processHand(Hand hand)
    {
        if (_isHandFirstDetected && _isCalibrationDone)
        {
            _currPalmarAxis = hand.PalmarAxis();
            _deltaAngle = Vector3.Angle(_prevPalmarAxis, _currPalmarAxis);
            _currTime = Time.realtimeSinceStartup;
            _calculatedScore += _deltaAngle / ((_currTime - _prevTime) * _scoreFactor);
            // Debug.Log(deltaAngle);
            _prevTime = _currTime;
            _prevPalmarAxis = _currPalmarAxis;
            // Determine whether the palm needs to be flipped
            if (hand.PalmarAxis().y < 0)
            {
                _isPalmUp = false;
            }
            else
            {
                _isPalmUp = true;
            }
            if (_isPalmUpPrev != _isPalmUp)
            {
                _isPalmUpPrev = _isPalmUp;
                _isStatusUpdated = false;
            }
        }
    }
}

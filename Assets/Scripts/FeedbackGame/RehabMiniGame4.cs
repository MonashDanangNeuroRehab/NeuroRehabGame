using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap;
using UnityEngine.UI;
using Leap.Unity.Query;
using System.IO;

// Wrist Extension
public class RehabMiniGame4 : PostProcessProvider
{
    public bool standAloneMode = false;
    public FeedbackGameManager gameManager;
    // Display-related variables
    public GameObject score;
    public Text scoreText;
    public GameObject scoreLabel;
    public GameObject gameNotif;
    public Text gameNotifText;
    public GameObject statusBar;
    public StatusBarControl statusBarControl;
    public GameObject totalScore;
    public Text totalScoreText;
    public Leap.Unity.Playback.PlaybackProvider playbackProvider;
    public FilterData filter;
    private bool _isStatusUpdated = false;
    private WaitForEndOfFrame _waitTime = new WaitForEndOfFrame();
    private string _palmUpNotif = "Please extend your wrist upward";
    private string _palmDownNotif = "Please put your hand down";
    private string _calibrationNotif = "Please keep your hand face down for calibration";
    public SkinnedMeshRenderer leftHandRenderer;
    public SkinnedMeshRenderer rightHandRenderer;
    private Color _outlineColor = Color.green;
    public IndicatorControl indicatorControl;
    public GameObject gameName;
    public Text gameNameText;
    private Color _color0 = new Color(180f / 255f, 57f / 255f, 156f / 255f);
    private Color _color1 = new Color(251f / 255f, 193f / 255f, 52f / 255f);

    // Game variable
    public GameObject calibrationZone;
    private bool _isPalmUp = false;
    private bool _isPalmUpPrev = false;
    private float _prevTime;
    private float _currTime;
    private float _deltaAngle;
    private float _deviationFromInitial;
    private float _deviationFromInitialFiltered;
    private float _deviationFromInitialSegmented;
    private float _deviationFromInitialSegmentedFiltered;
    private float _currBaselineFiltered = 0;
    private Vector3 _prevPalmarAxis;
    private Vector3 _currPalmarAxis;
    private Vector3 _initialPalmarAxis;
    private Vector3 _initialPalmarAxisSegmented;
    private float _angleLowerLimit = 5;
    private float _angleUpperLimit = 50;
    private float _scoreFactor = 100000f;
    private float _baselinePlayerComparision = 0;

    private List<float> _baseline = new List<float>();
    private List<float> _playerResult = new List<float>();
    private List<float> _baselineSegmented = new List<float>();
    private List<float> _playerResultSegmented = new List<float>();
    private List<float> _playerResultSegmentedCombined = new List<float>();
    private bool _gameStarted = false;
    public int HandID = 0;
    public static int LEFT_HAND = 0;
    public static int RIGHT_HAND = 1;

    public bool isGameCleared = false;
    public float calculatedScore = 0;
    private int _handActionCounter = 0;
    public int HAND_ACTION_MAX = 2;
    private GoalLogic _goalLogic;

    private float _accuracyLevel = 50;
    private float _maximumDeviation = 50;

    // Calibration purposes
    [SerializeField]
    private bool _isHandFirstDetected = false;
    private int _calibrationCounter = 0;
    [SerializeField]
    private bool _isCalibrationDone = false;
    private float _calibrationTime = 2f;
    private float _calibrationAngleLimit = 10;

    public void Start()
    {
        _isCalibrationDone = false;
        _isHandFirstDetected = false;
        isGameCleared = false;
        _gameStarted = false;
        scoreText = score.GetComponent<Text>();
        gameNotifText = gameNotif.GetComponent<Text>();
        statusBarControl = statusBar.GetComponent<StatusBarControl>();
        // statusBarControl.setGreenZoneLimit(_accuracyLevel / _maximumDeviation * 100);
        playbackProvider = GameObject.Find("PlaybackProvider").GetComponent<Leap.Unity.Playback.PlaybackProvider>();
        StartCoroutine(displayCouroutine());
        if (!standAloneMode)
        {
            Debug.Log(Application.persistentDataPath);
            _goalLogic = GetComponent<GoalLogic>();
            gameObject.AddComponent<FilterData>();
            filter = GetComponent<FilterData>();
            totalScoreText = totalScore.GetComponent<Text>();
            gameNameText = gameName.GetComponent<Text>();
            gameNameText.text = "Wrist Extention";
        }
    }
    public override void ProcessFrame(ref Frame inputFrame)
    {
        var leftHand = inputFrame.Hands.Query().FirstOrDefault(h => h.IsLeft);
        var rightHand = inputFrame.Hands.Query().FirstOrDefault(h => !h.IsLeft);
        if (Time.inFixedTimeStep && isActiveAndEnabled)
        {
            if (!isGameCleared)
            {
                if (!_isCalibrationDone)
                {
                    if (HandID == LEFT_HAND)
                    {
                        calibrationHand(leftHand);
                    }
                    else
                    {
                        calibrationHand(rightHand);
                    }
                }
                else
                {
                    if (HandID == LEFT_HAND)
                    {
                        processHand(leftHand);
                    }
                    else
                    {
                        processHand(rightHand);
                    }
                }
            }
        }
    }
    IEnumerator displayCouroutine()
    {
        while (true)
        {
            if (playbackProvider is null)
            {
                playbackProvider = GameObject.Find("PlaybackProvider").GetComponent<Leap.Unity.Playback.PlaybackProvider>();
            }
            else
            {
                if (!_isStatusUpdated)
                {
                    if (!_isCalibrationDone)
                    {
                        // playbackProvider.ChooseRecording("Standby");
                        calibrationZone.SetActive(true);
                        if (HandID == LEFT_HAND)
                        {
                            calibrationZone.transform.position = new Vector3(0.004068057f, 1.06564f, 0.167162f);
                            // calibrationZone.transform.parent = Camera.main.transform;
                            // calibrationZone.transform.localPosition = new Vector3(0.004068057f, -0.1558998f, 0.8842783f); 
                        }
                        else if (HandID == RIGHT_HAND)
                        {

                        }
                        gameNotifText.text = _calibrationNotif;
                    }
                    else
                    {
                        calibrationZone.SetActive(false);
                        if (_handActionCounter < HAND_ACTION_MAX)
                        {
                            if (_isPalmUp)
                            {
                                if (_gameStarted)
                                {
                                    _baseline.AddRange(_baselineSegmented);
                                    _playerResultSegmentedCombined.AddRange(_playerResultSegmented);
                                    // Calculating the score
                                    calculatedScore = Scoring();
                                    // Debug.Log(calculatedScore);
                                    _baselineSegmented.Clear();
                                    _playerResultSegmented.Clear();
                                }
                                else
                                {
                                    _gameStarted = true;
                                }
                                gameNotifText.text = _palmDownNotif;
                                playbackProvider.ChooseRecording(HandID + "WristExtensionDown");
                                scoreText.text = Mathf.Round(calculatedScore).ToString();
                                if (!standAloneMode)
                                {
                                    gameManager.totalScore += calculatedScore;
                                    totalScoreText.text = Mathf.Round(gameManager.totalScore).ToString();
                                }
                            }
                            else
                            {
                                if (_gameStarted)
                                {
                                    _baseline.AddRange(_baselineSegmented);
                                    _playerResultSegmentedCombined.AddRange(_playerResultSegmented);
                                    // Calculating the score
                                    calculatedScore = Scoring();
                                    // Debug.Log(calculatedScore);
                                    _baselineSegmented.Clear();
                                    _playerResultSegmented.Clear();
                                }
                                else
                                {
                                    _gameStarted = true;
                                }
                                gameNotifText.text = _palmUpNotif;
                                playbackProvider.ChooseRecording(HandID + "WristExtensionUp");
                                scoreText.text = Mathf.Round(calculatedScore).ToString();
                                if (!standAloneMode)
                                {
                                    gameManager.totalScore += calculatedScore;
                                    totalScoreText.text = Mathf.Round(gameManager.totalScore).ToString();
                                }
                            }
                            _handActionCounter++;
                        }
                        else
                        {
                            _baseline.AddRange(_baselineSegmented);
                            _playerResultSegmentedCombined.AddRange(_playerResultSegmented);
                            // Calculating the score
                            calculatedScore = Scoring();
                            // Debug.Log(calculatedScore);
                            _baselineSegmented.Clear();
                            _playerResultSegmented.Clear();
                            scoreText.text = Mathf.Round(calculatedScore).ToString();
                            if (!standAloneMode)
                            {
                                gameManager.totalScore += calculatedScore;
                                totalScoreText.text = Mathf.Round(gameManager.totalScore).ToString();
                            }
                            playbackProvider.ChooseRecording("EndGame");
                            gameNotifText.text = "Congrats, you finished the mini game. Prepare to move on";
                            indicatorControl.indicator = IndicatorControl.NO_INDICATOR;
                            isGameCleared = true;
                            if (!standAloneMode)
                            {
                                gameNameText.text = "";
                                _goalLogic.gameFinished = true;
                            }
                        }
                    }
                    if (!playbackProvider.IsPlaying)
                    {
                        playbackProvider.Play();
                    }
                    _isStatusUpdated = true;
                }
            }
            yield return _waitTime;
        }
    }
    private void OnDestroy()
    {
        StopCoroutine(displayCouroutine());
    }
    private void OnDisable()
    {
        StopCoroutine(displayCouroutine());
        SaveToFileDebug();
    }
    private void OnApplicationQuit()
    {
        StopCoroutine(displayCouroutine());
        SaveToFileDebug();
    }
    private void calibrationHand(Hand hand)
    {
        if (hand != null)
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
                        // Detect whether palm is up or down
                        if (_currPalmarAxis.y > 0)
                        {
                            _isPalmUp = true;
                            _isPalmUpPrev = true;
                        }
                        else
                        {
                            _isPalmUp = false;
                            _isPalmUpPrev = false;
                        }
                        _isCalibrationDone = true;
                        _isStatusUpdated = false;
                        _initialPalmarAxis = _currPalmarAxis;
                        _initialPalmarAxisSegmented = _initialPalmarAxis;
                    }
                    _prevTime = _currTime;
                }
            }
        }
    }
    private void processHand(Hand hand)
    {
        if (hand != null)
        {
            if (_isHandFirstDetected && _isCalibrationDone && filter != null)
            {
                calibrationZone.transform.position = hand.GetPalmPose().position;
                _currPalmarAxis = hand.PalmarAxis();
                _currTime = Time.realtimeSinceStartup;
                _deviationFromInitial = Vector3.Angle(_currPalmarAxis, _initialPalmarAxis);
                _deviationFromInitialSegmented = Vector3.Angle(_currPalmarAxis, _initialPalmarAxisSegmented);
                filter.input1 = _deviationFromInitial;
                filter.input3 = _deviationFromInitialSegmented;
                _deviationFromInitialFiltered = filter.input_filtered1;
                _deviationFromInitialSegmentedFiltered = filter.input_filtered3;

                // Test code, fully implement later
                if (HandID == LEFT_HAND)
                {
                    if (playbackProvider.palmarAxisAnglesLeftHand.Count > 0)
                    {
                        filter.input2 = playbackProvider.palmarAxisAnglesLeftHand[playbackProvider.palmarAxisAnglesLeftHand.Count - 1];
                        _currBaselineFiltered = filter.input_filtered2;
                        _baselineSegmented.Add(_currBaselineFiltered);
                        _playerResultSegmented.Add(_deviationFromInitialSegmentedFiltered);
                        _playerResult.Add(_deviationFromInitialFiltered);
                        _baselinePlayerComparision = Mathf.Clamp(Mathf.Abs(_deviationFromInitialSegmentedFiltered - _currBaselineFiltered) / _maximumDeviation, 0, 1);
                        leftHandRenderer.material.SetColor("_OutlineColor", Color.Lerp(_color0, _color1, _baselinePlayerComparision));
                        /*
                        if (_outlineColor.r > 0.5f)
                        {
                            if (_isPalmUp)
                            {
                                indicatorControl.indicator = IndicatorControl.FLIP_RIGHT;
                            }
                            else
                            {
                                indicatorControl.indicator = IndicatorControl.FLIP_LEFT;
                            }
                        }
                        else
                        {
                            indicatorControl.indicator = IndicatorControl.NO_INDICATOR;
                        }
                        */
                        // statusBarControl.setIndicatorLocation((_deviationFromInitialSegmentedFiltered - _currBaselineFiltered) / _maximumDeviation * 100);
                        // Debug.Log("Baseline: " + _currBaselineFiltered + " Palm up " +_isPalmUp);
                        // Debug.Log("Player: " + _deviationFromInitialSegmentedFiltered + " Palm up " + _isPalmUp);
                    }
                }
                else if (HandID == RIGHT_HAND)
                {

                }
                // Determine whether the palm needs to be flipped
                if (_currTime - _prevTime > 1.0f)
                {
                    if (HandID == LEFT_HAND)
                    {
                        if (_baselinePlayerComparision > 0.5f)
                        {
                            if (_isPalmUp)
                            {
                                indicatorControl.indicator = IndicatorControl.DOWN;
                            }
                            else
                            {
                                indicatorControl.indicator = IndicatorControl.UP;
                            }
                        }
                        else
                        {
                            indicatorControl.indicator = IndicatorControl.NO_INDICATOR;
                        }
                    }
                    if (_deviationFromInitialFiltered >= _angleUpperLimit)
                    {
                        _isPalmUp = true;
                    }
                    else if (_deviationFromInitialFiltered <= _angleLowerLimit)
                    {
                        _isPalmUp = false;
                    }
                    if (_isPalmUpPrev != _isPalmUp)
                    {
                        _isPalmUpPrev = _isPalmUp;
                        _initialPalmarAxisSegmented = _currPalmarAxis;
                        _prevTime = _currTime;
                        _isStatusUpdated = false;
                    }
                }
            }
        }
    }
    private void SaveToFileDebug()
    {
        // Convert the floats to strings
        string[] baseline = new string[_baseline.Count];
        string[] player = new string[_playerResult.Count];
        string[] playerSegmented = new string[_playerResultSegmentedCombined.Count];
        for (int i = 0; i < _baseline.Count; i++)
        {
            baseline[i] = _baseline[i].ToString();
        }
        for (int i = 0; i < _playerResult.Count; i++)
        {
            player[i] = _playerResult[i].ToString();
        }
        for (int i = 0; i < _playerResultSegmentedCombined.Count; i++)
        {
            playerSegmented[i] = _playerResultSegmentedCombined[i].ToString();
        }
        // Write the lines into the file
        File.WriteAllLines(Application.persistentDataPath + "/game4_debug_baseline.txt", baseline);
        File.WriteAllLines(Application.persistentDataPath + "/game4_debug_player.txt", player);
        File.WriteAllLines(Application.persistentDataPath + "/game4_debug_playerSegmented.txt", playerSegmented);


    }
    private float Scoring()
    {
        if (_playerResultSegmented.Count > 0 && _baselineSegmented.Count > 0)
        {
            int extraLoad = (int)Mathf.Abs((float)_playerResultSegmented.Count - (float)_baselineSegmented.Count);
            if (_playerResultSegmented.Count > _baselineSegmented.Count)
            {
                for (int i = 0; i < extraLoad; i++)
                {
                    _baselineSegmented.Add(_baselineSegmented[_baselineSegmented.Count - 1]);
                }
            }
            else
            {
                // In case where the base line has more than the player
            }
            float[] dPlayer = new float[_playerResultSegmented.Count];
            float[] dBaseline = new float[_baselineSegmented.Count];
            dPlayer[0] = 0;
            dBaseline[0] = 0;
            for (int i = 1; i < dPlayer.Length; i++)
            {
                dPlayer[i] = _playerResultSegmented[i] - _playerResultSegmented[i - 1];
            }
            for (int i = 1; i < dBaseline.Length; i++)
            {
                dBaseline[i] = _baselineSegmented[i] - _baselineSegmented[i - 1];
            }
            // Find the area between the curve
            float area = 0;
            for (int i = 0; i < _playerResultSegmented.Count - 1; i++)
            {
                area += (Mathf.Abs(_playerResultSegmented[i] - _baselineSegmented[i]) + Mathf.Abs(_playerResultSegmented[i + 1] - _baselineSegmented[i + 1])) / 2.0f * 1.0f;
            }

            return _scoreFactor / area;
        }
        else
        {
            return 0;
        }
    }
}
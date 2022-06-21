using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap;
using UnityEngine.UI;
using Leap.Unity.Query;
using System.IO;

// Fist Clench and Unclench
public class RehabMiniGame3 : PostProcessProvider
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
    private string _fistClenchNotif = "Please clench your fist";
    private string _fistUnclenchNotif = "Please unclench your first";
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
    private bool _isMakingAFist = false;
    private bool _isMakingAFistPrev = false;
    private float _prevTime;
    private float _currTime;
    private float _deltaAngle;
    private float _makingFistProgress;
    private float _makingFistProgressFiltered;
    private float _makingFistProgressFilteredSegmented;
    private float _currBaselineFiltered = 0;
    private Vector3 _prevPalmarAxis;
    private Vector3 _currPalmarAxis;
    private float _fistMakingThresholdUpper = 0.5f;
    private float _fistMakingThresholdLower = 0.1f;
    private float _scoreFactor = 1000f;
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
    private int _actionCounter = 0;
    public int HAND_ACTION_MAX = 2;
    private GoalLogic _goalLogic;

    private float _accuracyLevel = 0.05f;
    private float _maximumDeviation = 0.5f;

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
        playbackProvider = GameObject.Find("PlaybackProvider").GetComponent<Leap.Unity.Playback.PlaybackProvider>();
        Debug.Log(Application.persistentDataPath);
        StartCoroutine(displayCouroutine());
        if (!standAloneMode)
        {
            _goalLogic = GetComponent<GoalLogic>();
            gameObject.AddComponent<FilterData>();
            filter = GetComponent<FilterData>();
            totalScoreText = totalScore.GetComponent<Text>();
            gameNameText = gameName.GetComponent<Text>();
            gameNameText.text = "Making A Fist";
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
                            calibrationZone.transform.position = new Vector3(-0.001130388f, 1.095585f, 0.1330335f);
                            // calibrationZone.transform.parent = Camera.main.transform;
                            // calibrationZone.transform.localPosition = new Vector3(-0.001130388f, -0.1696091f, 0.8409942f);
                        }
                        else if (HandID == RIGHT_HAND)
                        {

                        }
                        gameNotifText.text = _calibrationNotif;
                    }
                    else
                    {
                        calibrationZone.SetActive(false);
                        if (_actionCounter < HAND_ACTION_MAX)
                        {
                            if (_isMakingAFist)
                            {
                                if (_gameStarted)
                                {
                                    // _baselineSegmented = filter.OneShotLowPassFilter(playbackProvider.palmarAxisAnglesLeftHand);
                                    _baseline.AddRange(_baselineSegmented);
                                    // _playerResultSegmented = filter.OneShotLowPassFilter(_playerResultSegmented);
                                    _playerResultSegmentedCombined.AddRange(_playerResultSegmented);
                                    // Calculating the score
                                    // calculatedScore = Scoring();
                                    // Debug.Log(calculatedScore);
                                    _baselineSegmented.Clear();
                                    _playerResultSegmented.Clear();
                                }
                                else
                                {
                                    _gameStarted = true;
                                }
                                gameNotifText.text = _fistUnclenchNotif;
                                playbackProvider.ChooseRecording(HandID + "FistUnclench");
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
                                gameNotifText.text = _fistClenchNotif;
                                playbackProvider.ChooseRecording(HandID + "FistClench");
                                scoreText.text = Mathf.Round(calculatedScore).ToString();
                                if (!standAloneMode)
                                {
                                    gameManager.totalScore += calculatedScore;
                                    totalScoreText.text = Mathf.Round(gameManager.totalScore).ToString();
                                }
                            }
                            _actionCounter++;
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
                                _goalLogic.gameFinished = true;
                                gameNameText.text = "";
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
                            _isMakingAFist = true;
                            _isMakingAFistPrev = true;
                        }
                        else
                        {
                            _isMakingAFist = false;
                            _isMakingAFistPrev = false;
                        }
                        _isCalibrationDone = true;
                        _isStatusUpdated = false;
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
                // calibrationZone.transform.position = hand.GetPalmPose().position;
                _currTime = Time.realtimeSinceStartup;
                /// <summary>
                /// GetFistStrength() Explanation:
                /// The function takes dot product of each fingers with the inverse of distal axis
                /// 1 is pointing in the same direction, 0 is opposite direction 
                /// Then it is summed and average (Map(-5, 5, 0, 1) => Lerp the value between -5 and 5 to output the value between 0 and 1)
                /// </summary>
                _makingFistProgress = hand.GetFistStrength();
                filter.input1 = _makingFistProgress;
                filter.input3 = _makingFistProgress;
                _makingFistProgressFiltered = filter.input_filtered1;
                _makingFistProgressFilteredSegmented = filter.input_filtered3;
                if (HandID == LEFT_HAND)
                {
                    if (playbackProvider.fistClenchLeftHand.Count > 0)
                    {
                        filter.input2 = playbackProvider.fistClenchLeftHand[playbackProvider.fistClenchLeftHand.Count - 1];
                        _currBaselineFiltered = filter.input_filtered2;
                        _baselineSegmented.Add(_currBaselineFiltered);
                        _playerResultSegmented.Add(_makingFistProgressFiltered);
                        // Color change here
                        _baselinePlayerComparision = Mathf.Clamp(Mathf.Abs(_makingFistProgressFilteredSegmented - _currBaselineFiltered) / _maximumDeviation, 0, 1);
                        leftHandRenderer.material.SetColor("_OutlineColor", Color.Lerp(_color0, _color1, _baselinePlayerComparision));
                    }
                }
                else if (HandID == RIGHT_HAND)
                {
                    if (playbackProvider.fistClenchRightHand.Count > 0)
                    {

                    }
                }
                if (_currTime - _prevTime > 1.0f)
                {
                    // Arrow notify player
                    if (HandID == LEFT_HAND)
                    {
                        // Base on colour change
                        if (_baselinePlayerComparision > 0.5f)
                        {
                            if (_isMakingAFist)
                            {
                                indicatorControl.indicator = IndicatorControl.OUTWARD;
                            }
                            else
                            {
                                indicatorControl.indicator = IndicatorControl.INWARD;
                            }
                        }
                        else
                        {
                            indicatorControl.indicator = IndicatorControl.NO_INDICATOR;
                        }
                        if (_makingFistProgressFiltered >= _fistMakingThresholdUpper)
                        {
                            _isMakingAFist = true;
                        }
                        else if (_makingFistProgressFiltered <= _fistMakingThresholdLower)
                        {
                            _isMakingAFist = false;
                        }
                        if (_isMakingAFistPrev != _isMakingAFist)
                        {
                            _isMakingAFistPrev = _isMakingAFist;
                            _prevTime = _currTime;
                            _isStatusUpdated = false;
                        }
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
        File.WriteAllLines(Application.persistentDataPath + "/game3_debug_baseline.txt", baseline);
        File.WriteAllLines(Application.persistentDataPath + "/game3_debug_player.txt", player);
        File.WriteAllLines(Application.persistentDataPath + "/game3_debug_playerSegmented.txt", playerSegmented);


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
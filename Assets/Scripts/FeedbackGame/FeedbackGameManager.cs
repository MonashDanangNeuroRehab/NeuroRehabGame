using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackGameManager : MonoBehaviour
{

    public bool goalEncountered = false;
    public bool gameFinished = false;
    private GameObject[] _goals;
    private int _goalIndex;
    private GameObject _nextGoal;
    [SerializeField]
    private GameObject _bottomWall;
    [SerializeField]
    private GameObject _topWall;
    [SerializeField]
    private GameObject _playground;
    [SerializeField]
    private IndicatorControl _indicatorControl;
    [SerializeField]
    private SkinnedMeshRenderer _leftHandRenderer;
    [SerializeField]
    private SkinnedMeshRenderer _rightHandRenderer;
    [SerializeField]
    private GameObject _playerLeftHandDisplay;
    [SerializeField]
    private GameObject _playerRightHandDisplay;
    [SerializeField]
    private GameObject _calibrationZone;

    public int HandID = 0;
    public static int LEFT_HAND = 0;
    public static int RIGHT_HAND = 1;

    public float totalScore = 0;

    private float _speed = 0.01f;
    private bool _newGoalSet = false;

    private Camera _mainCamera;
    [SerializeField]
    private FilterData _filter;
    [SerializeField]
    private Leap.Unity.LeapServiceProvider _leapServiceProvider;
    [SerializeField]
    private Leap.Unity.Playback.PlaybackProvider _recordingPlaybackProvider;
    [SerializeField]
    private GameObject _canvas;
    private GameObject _scoreText;
    private GameObject _scoreLabel;
    private GameObject _gameNotif;
    private GameObject _totalScore;
    private GameObject _totalScoreLabel;
    private GameObject _gameName;
    private GameObject _statusBar;
    
    // Game related variable
    int[] _gameTypeArray = { 1, 2, 3, 4};

    private Vector3 _offset;
    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = Camera.main;
        // UI Setup
        _scoreText = _canvas.transform.GetChild(0).gameObject;
        _scoreLabel = _canvas.transform.GetChild(1).gameObject;
        _gameNotif = _canvas.transform.GetChild(2).gameObject;
        _statusBar = _canvas.transform.GetChild(3).gameObject;
        _totalScore = _canvas.transform.GetChild(4).gameObject;
        _totalScoreLabel = _canvas.transform.GetChild(5).gameObject;
        _gameName = _canvas.transform.GetChild(6).gameObject;

        _goals = GameObject.FindGameObjectsWithTag("Goal");
        // Assign the games 
        shuffleGameType();
        foreach (GameObject goal in _goals)
        {
            if (goal.name == "Goal" + (_goalIndex + 1))
            {
                _nextGoal = goal;
                switch (_gameTypeArray[_goalIndex])
                {
                    case 1:
                        RehabMiniGame1 game1 = _nextGoal.AddComponent<RehabMiniGame1>();
                        game1.dataUpdateMode = Leap.Unity.PostProcessProvider.DataUpdateMode.FixedUpdateOnly;
                        game1.inputLeapProvider = _leapServiceProvider;
                        game1.scoreLabel = _scoreLabel;
                        game1.score = _scoreText;
                        game1.totalScore = _totalScore;
                        game1.gameNotif = _gameNotif;
                        game1.statusBar = _statusBar;
                        game1.indicatorControl = _indicatorControl;
                        game1.leftHandRenderer = _leftHandRenderer;
                        game1.rightHandRenderer = _rightHandRenderer;
                        game1.HandID = HandID;
                        game1.gameManager = this;
                        game1.HAND_ACTION_MAX = 6;
                        game1.gameName = _gameName;
                        game1.calibrationZone = _calibrationZone;
                        game1.enabled = false;
                        break;
                    case 2:
                        RehabMiniGame2 game2 = _nextGoal.AddComponent<RehabMiniGame2>();
                        game2.dataUpdateMode = Leap.Unity.PostProcessProvider.DataUpdateMode.FixedUpdateOnly;
                        game2.inputLeapProvider = _leapServiceProvider;
                        game2.scoreLabel = _scoreLabel;
                        game2.score = _scoreText;
                        game2.totalScore = _totalScore;
                        game2.gameNotif = _gameNotif;
                        game2.statusBar = _statusBar;
                        game2.HandID = HandID;
                        game2.indicatorControl = _indicatorControl;
                        game2.leftHandRenderer = _leftHandRenderer;
                        game2.rightHandRenderer = _rightHandRenderer;
                        game2.gameManager = this;
                        game2.HAND_ACTION_MAX = 6;
                        game2.gameName = _gameName;
                        game2.calibrationZone = _calibrationZone;
                        game2.enabled = false;
                        break;
                    case 3:
                        RehabMiniGame3 game3 = _nextGoal.AddComponent<RehabMiniGame3>();
                        game3.dataUpdateMode = Leap.Unity.PostProcessProvider.DataUpdateMode.FixedUpdateOnly;
                        game3.inputLeapProvider = _leapServiceProvider;
                        game3.scoreLabel = _scoreLabel;
                        game3.score = _scoreText;
                        game3.totalScore = _totalScore;
                        game3.gameNotif = _gameNotif;
                        game3.statusBar = _statusBar;
                        game3.HandID = HandID;
                        game3.indicatorControl = _indicatorControl;
                        game3.leftHandRenderer = _leftHandRenderer;
                        game3.rightHandRenderer = _rightHandRenderer;
                        game3.gameManager = this;
                        game3.HAND_ACTION_MAX = 6;
                        game3.gameName = _gameName;
                        game3.calibrationZone = _calibrationZone;
                        game3.enabled = false;
                        break;
                    case 4:
                        RehabMiniGame4 game4 = _nextGoal.AddComponent<RehabMiniGame4>();
                        game4.dataUpdateMode = Leap.Unity.PostProcessProvider.DataUpdateMode.FixedUpdateOnly;
                        game4.inputLeapProvider = _leapServiceProvider;
                        game4.scoreLabel = _scoreLabel;
                        game4.score = _scoreText;
                        game4.totalScore = _totalScore;
                        game4.gameNotif = _gameNotif;
                        game4.statusBar = _statusBar;
                        game4.HandID = HandID;
                        game4.indicatorControl = _indicatorControl;
                        game4.leftHandRenderer = _leftHandRenderer;
                        game4.rightHandRenderer = _rightHandRenderer;
                        game4.gameManager = this;
                        game4.HAND_ACTION_MAX = 6;
                        game4.gameName = _gameName;
                        game4.calibrationZone = _calibrationZone;
                        game4.enabled = false;
                        break;
                    case 5:
                        RehabMiniGame5 game5 = _nextGoal.AddComponent<RehabMiniGame5>();
                        game5.dataUpdateMode = Leap.Unity.PostProcessProvider.DataUpdateMode.FixedUpdateOnly;
                        game5.inputLeapProvider = _leapServiceProvider;
                        game5.scoreLabel = _scoreLabel;
                        game5.score = _scoreText;
                        game5.totalScore = _totalScore;
                        game5.gameNotif = _gameNotif;
                        game5.statusBar = _statusBar;
                        game5.HandID = HandID;
                        game5.indicatorControl = _indicatorControl;
                        game5.leftHandRenderer = _leftHandRenderer;
                        game5.rightHandRenderer = _rightHandRenderer;
                        game5.gameManager = this;
                        game5.enabled = false;
                        break;
                    default: break;
                }
                _newGoalSet = true;
                goalEncountered = false;
                break; 
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameFinished)
        {
            _scoreText.SetActive(false);
            _scoreLabel.SetActive(false);
            _gameNotif.GetComponent<Text>().text = "Congrats, you finished the game";
            _bottomWall.transform.position = _bottomWall.transform.position;
        }
        else
        {
            if (!goalEncountered)
            {
                _playground.transform.position = _playground.transform.position + (_bottomWall.transform.position - _nextGoal.transform.position) * _speed;
                _newGoalSet = false;
                _playerLeftHandDisplay.SetActive(true);
                _playerRightHandDisplay.SetActive(true);
            }
            else
            {
                _playerLeftHandDisplay.SetActive(false);
                _playerRightHandDisplay.SetActive(false);
                if (HandID == LEFT_HAND)
                {
                    _playerLeftHandDisplay.SetActive(true);
                }
                else if (HandID == RIGHT_HAND)
                {
                    _playerRightHandDisplay.SetActive(true);
                }
                if (!_newGoalSet && _nextGoal.GetComponent<GoalLogic>().gameFinished)
                {
                    _goalIndex++;
                    foreach (GameObject goal in _goals)
                    {
                        if (goal.name == "Goal" + (_goalIndex + 1))
                        {
                            _nextGoal = goal;
                            switch (_gameTypeArray[_goalIndex])
                            {
                                case 1:
                                    RehabMiniGame1 game1 = _nextGoal.AddComponent<RehabMiniGame1>();
                                    game1.dataUpdateMode = Leap.Unity.PostProcessProvider.DataUpdateMode.FixedUpdateOnly;
                                    game1.inputLeapProvider = _leapServiceProvider;
                                    game1.scoreLabel = _scoreLabel;
                                    game1.score = _scoreText;
                                    game1.totalScore = _totalScore;
                                    game1.gameNotif = _gameNotif;
                                    game1.statusBar = _statusBar;
                                    game1.indicatorControl = _indicatorControl;
                                    game1.leftHandRenderer = _leftHandRenderer;
                                    game1.rightHandRenderer = _rightHandRenderer;
                                    game1.HandID = HandID;
                                    game1.gameManager = this;
                                    game1.HAND_ACTION_MAX = 6;
                                    game1.gameName = _gameName;
                                    game1.calibrationZone = _calibrationZone;
                                    game1.enabled = false;
                                    break;
                                case 2:
                                    RehabMiniGame2 game2 = _nextGoal.AddComponent<RehabMiniGame2>();
                                    game2.dataUpdateMode = Leap.Unity.PostProcessProvider.DataUpdateMode.FixedUpdateOnly;
                                    game2.inputLeapProvider = _leapServiceProvider;
                                    game2.scoreLabel = _scoreLabel;
                                    game2.score = _scoreText;
                                    game2.totalScore = _totalScore;
                                    game2.gameNotif = _gameNotif;
                                    game2.statusBar = _statusBar;
                                    game2.HandID = HandID;
                                    game2.indicatorControl = _indicatorControl;
                                    game2.leftHandRenderer = _leftHandRenderer;
                                    game2.rightHandRenderer = _rightHandRenderer;
                                    game2.gameManager = this;
                                    game2.HAND_ACTION_MAX = 6;
                                    game2.gameName = _gameName;
                                    game2.calibrationZone = _calibrationZone;
                                    game2.enabled = false;
                                    break;
                                case 3:
                                    RehabMiniGame3 game3 = _nextGoal.AddComponent<RehabMiniGame3>();
                                    game3.dataUpdateMode = Leap.Unity.PostProcessProvider.DataUpdateMode.FixedUpdateOnly;
                                    game3.inputLeapProvider = _leapServiceProvider;
                                    game3.scoreLabel = _scoreLabel;
                                    game3.score = _scoreText;
                                    game3.totalScore = _totalScore;
                                    game3.gameNotif = _gameNotif;
                                    game3.statusBar = _statusBar;
                                    game3.HandID = HandID;
                                    game3.indicatorControl = _indicatorControl;
                                    game3.leftHandRenderer = _leftHandRenderer;
                                    game3.rightHandRenderer = _rightHandRenderer;
                                    game3.gameManager = this;
                                    game3.HAND_ACTION_MAX = 6;
                                    game3.gameName = _gameName;
                                    game3.calibrationZone = _calibrationZone;
                                    game3.enabled = false;
                                    break;
                                case 4:
                                    RehabMiniGame4 game4 = _nextGoal.AddComponent<RehabMiniGame4>();
                                    game4.dataUpdateMode = Leap.Unity.PostProcessProvider.DataUpdateMode.FixedUpdateOnly;
                                    game4.inputLeapProvider = _leapServiceProvider;
                                    game4.scoreLabel = _scoreLabel;
                                    game4.score = _scoreText;
                                    game4.totalScore = _totalScore;
                                    game4.gameNotif = _gameNotif;
                                    game4.statusBar = _statusBar;
                                    game4.HandID = HandID;
                                    game4.indicatorControl = _indicatorControl;
                                    game4.leftHandRenderer = _leftHandRenderer;
                                    game4.rightHandRenderer = _rightHandRenderer;
                                    game4.gameManager = this;
                                    game4.HAND_ACTION_MAX = 6;
                                    game4.gameName = _gameName;
                                    game4.calibrationZone = _calibrationZone;
                                    game4.enabled = false;
                                    break;
                                case 5:
                                    RehabMiniGame5 game5 = _nextGoal.AddComponent<RehabMiniGame5>();
                                    game5.dataUpdateMode = Leap.Unity.PostProcessProvider.DataUpdateMode.FixedUpdateOnly;
                                    game5.inputLeapProvider = _leapServiceProvider;
                                    game5.scoreLabel = _scoreLabel;
                                    game5.score = _scoreText;
                                    game5.totalScore = _totalScore;
                                    game5.gameNotif = _gameNotif;
                                    game5.statusBar = _statusBar;
                                    game5.HandID = HandID;
                                    game5.indicatorControl = _indicatorControl;
                                    game5.leftHandRenderer = _leftHandRenderer;
                                    game5.rightHandRenderer = _rightHandRenderer;
                                    game5.gameManager = this;
                                    game5.enabled = false;
                                    break;
                                default: break;
                            }

                            break;
                        }
                        else
                        {
                            _nextGoal = _topWall;
                        }
                    }
                    _newGoalSet = true;
                }
            }
        }
    }

    // Score display
    void ScoreUpdate(float score)
    {
        _scoreText.GetComponent<Text>().text = score.ToString("0");
    }
    void ToggleScoreDisplay()
    {
        _scoreText.SetActive(!_scoreText.activeInHierarchy);
        _scoreLabel.SetActive(!_scoreLabel.activeInHierarchy);
    }
    private void shuffleGameType()
    {
        int temp;
        for (int i = 0; i < _gameTypeArray.Length; i++)
        {
            int rnd = UnityEngine.Random.Range(i, _gameTypeArray.Length);
            temp = _gameTypeArray[rnd];
            _gameTypeArray[rnd] = _gameTypeArray[i];
            _gameTypeArray[i] = temp;
        }
    }
}

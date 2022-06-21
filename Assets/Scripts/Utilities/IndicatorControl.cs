using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorControl : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform _flipLeft;
    private Transform _flipRight;
    private Transform _up;
    private Transform _down;
    private Transform _inward;
    private Transform _outward;
    private Transform _left;
    private Transform _right;

    private Transform _flipLeftOrg;
    private Transform _flipRightOrg;
    private Transform _upOrg;
    private Transform _downOrg;
    private Transform _inwardOrg;
    private Transform _outwardOrg;
    private Transform _leftOrg;
    private Transform _rightOrg;

    private Transform _currIndicator;
    private float _time;
    private float _mag = 5;
    private float _speed = 3;

    public int indicator = 2;
    private int _confirmedIndicator = 0;
    public const int NO_INDICATOR = 0;
    public const int FLIP_LEFT = 1;
    public const int FLIP_RIGHT = 2;
    public const int UP = 3;
    public const int DOWN = 4;
    public const int INWARD = 5;
    public const int OUTWARD = 6;
    public const int LEFT = 7;
    public const int RIGHT = 8;


    void Start()
    {
        _flipLeft = transform.GetChild(0);
        _flipRight = transform.GetChild(1);
        _up = transform.GetChild(2);
        _down = transform.GetChild(3);
        _inward = transform.GetChild(4);
        _outward = transform.GetChild(5);
        _left = transform.GetChild(6);
        _right = transform.GetChild(7);
        _flipLeftOrg = transform.GetChild(8);
        _flipRightOrg = transform.GetChild(9);
        _upOrg = transform.GetChild(10);
        _downOrg = transform.GetChild(11);
        _inwardOrg = transform.GetChild(12);
        _outwardOrg = transform.GetChild(13);
        _leftOrg = transform.GetChild(14);
        _rightOrg = transform.GetChild(15);
        // Init
        _currIndicator = _flipLeft;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_confirmedIndicator != indicator)
        {
            _confirmedIndicator = indicator;
            // Stop current indicator
            _currIndicator.gameObject.SetActive(false);
            // Set new indicator
            switch(_confirmedIndicator)
            {
                case FLIP_LEFT:
                    _currIndicator = _flipLeft;
                    _currIndicator.position = _flipLeftOrg.position;
                    _currIndicator.gameObject.SetActive(true);
                    break;
                case FLIP_RIGHT:
                    _currIndicator = _flipRight;
                    _currIndicator.position = _flipRightOrg.position;
                    _currIndicator.gameObject.SetActive(true);
                    break;
                case UP:
                    _currIndicator = _up;
                    _currIndicator.position = _upOrg.position;
                    _currIndicator.gameObject.SetActive(true);
                    break;
                case DOWN:
                    _currIndicator = _down;
                    _currIndicator.position = _downOrg.position;
                    _currIndicator.gameObject.SetActive(true);
                    break;
                case INWARD:
                    _currIndicator = _inward;
                    _currIndicator.position = _inwardOrg.position;
                    _currIndicator.gameObject.SetActive(true);
                    break;
                case OUTWARD:
                    _currIndicator = _outward;
                    _currIndicator.position = _outwardOrg.position;
                    _currIndicator.gameObject.SetActive(true);
                    break;
                case LEFT:
                    _currIndicator = _left;
                    _currIndicator.position = _leftOrg.position;
                    _currIndicator.gameObject.SetActive(true);
                    break;
                case RIGHT:
                    _currIndicator = _right;
                    _currIndicator.position = _rightOrg.position;
                    _currIndicator.gameObject.SetActive(true);
                    break;
                default:
                    // Debug.Log("No indicator code found");
                    break;
            }
        }
        else
        {
            switch (_confirmedIndicator)
            {
                case FLIP_LEFT:
                    _currIndicator.position = _flipLeftOrg.TransformPoint(_mag * Mathf.Sin(_speed * _time), 0, 0);
                    break;
                case FLIP_RIGHT:
                    _currIndicator.position = _flipRightOrg.TransformPoint(_mag * Mathf.Sin(_speed * _time), 0, 0);
                    break;
                case UP:
                    _currIndicator.position = _upOrg.TransformPoint(_mag * Mathf.Sin(_speed * _time), 0, 0);
                    break;
                case DOWN:
                    _currIndicator.position = _downOrg.TransformPoint(_mag * Mathf.Sin(_speed * _time), 0, 0);
                    break;
                case INWARD:
                    _currIndicator.position = _inwardOrg.TransformPoint(_mag * Mathf.Sin(_speed * _time), 0, 0);
                    break;
                case OUTWARD:
                    _currIndicator.position = _outwardOrg.TransformPoint(_mag * Mathf.Sin(_speed * _time), 0, 0);
                    break;
                case LEFT:
                    _currIndicator.position = _leftOrg.TransformPoint(_mag * Mathf.Sin(_speed * _time), 0, 0);
                    break;
                case RIGHT:
                    _currIndicator.position = _rightOrg.TransformPoint(_mag * Mathf.Sin(_speed * _time), 0, 0);
                    break;
                default:
                    break;
            }
            _time = _time + Time.fixedDeltaTime;
            if (_time >= 2*Mathf.PI)
            {
                _time = 0;
            }
        }
    }
}

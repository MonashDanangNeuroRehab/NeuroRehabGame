using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusBarControl : MonoBehaviour
{
    private RectTransform _indicator;
    private RectTransform _greenZone;
    private RectTransform _redZone;
    private float _indicatorWidth;
    private float _greenZoneWidth;
    private float _redZoneWidth;
    private float _indicatorOriginalPos;

    private float _percentage;
    private float _indicatorXPos;

    // Test variables
    private float _xLimit;
    private float _timeValue = 0;

    // Start is called before the first frame update
    void Start()
    {
        _indicator = GameObject.Find("Indicator").GetComponent<RectTransform>();
        _greenZone = GameObject.Find("GreenZone").GetComponent<RectTransform>();
        _redZone = GameObject.Find("RedZone").GetComponent<RectTransform>();

        _indicatorWidth = _indicator.rect.width;
        _greenZoneWidth = _greenZone.rect.width;
        _redZoneWidth = _redZone.rect.width;

        _indicatorOriginalPos = _indicator.localPosition.x ;
        _xLimit = (_redZoneWidth - _indicatorWidth) / 2;
    }

    // Update is called once per frame
    void Update()
    {
        float xValue = Mathf.Sin(_timeValue);
        var position = _indicator.localPosition;
        position.x = _indicatorXPos;
        _indicator.localPosition = position;
        _timeValue += Time.fixedDeltaTime;
        if (_timeValue >= Mathf.PI * 2)
        {
            _timeValue = 0;
        }
    }
    // Set indicator location with the percentage compared to the red zone
    public void setIndicatorLocation(float percentage)
    {
        _indicatorXPos = Mathf.Clamp(_indicatorOriginalPos + percentage * _xLimit / 100, _indicatorOriginalPos - _xLimit, _indicatorOriginalPos + _xLimit); 
    }
    // Set the greenzone limit compared to the red zone
    public void setGreenZoneLimit(float percentage)
    {
        // Max is _redZoneWidth
        // Min is _indicatorWidth
        if (percentage >= 0)
        {
            var size = _greenZone.sizeDelta;
            size.x = Mathf.Abs(_redZoneWidth - _indicatorWidth) * percentage / 100;
            _greenZone.sizeDelta = size;
        }
        else
        {
            Debug.LogError("Percentage is less than 0");
        }
    }
}

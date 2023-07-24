using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeDisplay : MonoBehaviour
{
    private enum TimeType {Time, Date };
    private TextMeshPro _label;
    [SerializeField] private TimeType _timeType;

    private bool _blink;
    
    [Header("Time")]
    [SerializeField] private bool _useHours = true;
    [SerializeField] private bool _useMinutes = true;
    [SerializeField] private bool _useSeconds = true;
    private float _timeBlinkInterval = 0.5f;

    [Header("Day")]
    [SerializeField] private bool _useYear = true;
    [SerializeField] private bool _useMonth = true;
    [SerializeField] private bool _useDay = true;
    [SerializeField] private bool _useDayOfWeek = true;
    private float _dayBlinkInterval = 1f;

    void Start()
    {
        _label = GetComponent<TextMeshPro>();

        switch (_timeType)
        {
            default:
            case TimeType.Time:
                StartCoroutine(UpdateTime());
                break;
            case TimeType.Date:
                StartCoroutine(UpdateDay());
                break;
        }
    }

    private IEnumerator UpdateDay()
    {
        while (true)
        {
            _blink = !_blink;
            string dateFormat = "";

            if (_useYear)
                dateFormat += "yyyy/";
            if (_useMonth)
                dateFormat += "MM/";
            if (_useDay)
                dateFormat += "dd";

            string dayOfWeekFormat = _useDayOfWeek ? " ddd" : "";

            string finalFormat = _blink ? dateFormat : dateFormat + dayOfWeekFormat;
            _label.text = DateTime.Now.ToString(finalFormat);

            yield return new WaitForSeconds(_dayBlinkInterval);
        }
    }

    private IEnumerator UpdateTime()
    {
        while (true)
        {
            _blink = !_blink;

            string timeString = "";
            if (_useHours)
                timeString += "hh" + (_blink && _useMinutes ? ":" : " ");
            if (_useMinutes)
                timeString += "mm" + (_blink && _useSeconds ? ":" : " ");
            if (_useSeconds)
                timeString += "ss";

            _label.text = DateTime.Now.ToString(timeString);
            yield return new WaitForSeconds(_timeBlinkInterval);
        }
    }
}

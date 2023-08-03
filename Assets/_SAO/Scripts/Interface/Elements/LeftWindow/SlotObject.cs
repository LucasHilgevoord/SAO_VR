using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum SlotType
{
    Skill,
    Arm,
    Chest,
    Wrist,
    Hand,
    Leg
}

public enum SlotSide
{
    None,
    Left,
    Right,
    Top,
    Bottom
}

[Serializable]
public class Slot
{
    public SlotType slotType;
    public SlotSide slotSide;

    public Slot(SlotType type, SlotSide side = SlotSide.None)
    {
        slotType = type;
        slotSide = side;
    }
}

public class SlotObject : MonoBehaviour
{
    [SerializeField] private Slot _slotType;
    public Slot Type => _slotType;

    [Header("Components")]
    [SerializeField] private Image _icon;
    [SerializeField] private Image _smallIcon;
    [SerializeField] private LineRenderer _line;

    private Vector3[] _linePositions;
    private float _lineSpeed = 1f;
    private Vector3 _iconPosition;

    public bool test;

    private void Start()
    {
        _linePositions = new Vector3[_line.positionCount];
        for (int i = 0; i < _line.positionCount; i++)
        {
            _linePositions[i] = _line.GetPosition(i);
        }

        Color iconAlpha = _icon.color;
        iconAlpha.a = 0;
        _icon.color = iconAlpha;

        Color smallIconAlpha = _smallIcon.color;
        smallIconAlpha.a = 0;
        _smallIcon.color = smallIconAlpha;

        _line.gameObject.SetActive(false);
        _icon.gameObject.SetActive(false);
        _smallIcon.gameObject.SetActive(false);

        _iconPosition = _icon.transform.localPosition;
    }

    private void Update()
    {
        if (InputHandler.Instance.wasKeyPressedThisFrame(UnityEngine.InputSystem.Key.M))
        {
            Hide();
        }
    }

    internal void Show()
    {
        _line.gameObject.SetActive(false);
        _smallIcon.gameObject.SetActive(false);

        _icon.transform.localPosition = _iconPosition;
        _icon.gameObject.SetActive(true);


        _line.positionCount = 1;
        _icon.DOFade(1, 0.5f).OnComplete(() => {
            _line.gameObject.SetActive(true);
            StartCoroutine(MoveLineCoroutineQueue());
        });
    }

    private IEnumerator MoveLineCoroutineQueue()
    {
        for (int i = 1; i < _linePositions.Length; i++)
        {
            _line.positionCount++;
            _line.SetPosition(_line.positionCount - 1, _line.GetPosition(_line.positionCount - 2));
            yield return StartCoroutine(MoveLineCoroutine(_linePositions[i]));
        }

        // All the lines have been moved
        _smallIcon.gameObject.SetActive(true);
        _smallIcon.DOFade(1, 0.5f);
    }

    private IEnumerator MoveLineCoroutine(Vector3 newPosition)
    {
        Vector3 startPos = _line.GetPosition(_line.positionCount - 1);
        float elapsedTime = 0f;

        while (elapsedTime < _lineSpeed)
        {
            Vector3 nextPos = Vector3.Lerp(startPos, newPosition, elapsedTime / _lineSpeed);
            _line.SetPosition(_line.positionCount - 1, nextPos);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _line.SetPosition(_line.positionCount - 1, newPosition);
    }

    internal void Hide()
    {
        // Move the _icon over the path of the line
        StartCoroutine(MoveIconCoroutineQueue());
    }

    private IEnumerator MoveIconCoroutineQueue()
    {
        // We don't want to start at the first position, because it's the same as the icon's position
        for (int i = 1; i < _linePositions.Length; i++)
        {
            yield return StartCoroutine(MoveIconOverLine(i));
        }

        _icon.DOFade(0, 0.5f);
        _smallIcon.DOFade(0, 0.5f);
    }

    private IEnumerator MoveIconOverLine(int nextLineIndex)
    {
        Vector3 newLinePos = _linePositions[nextLineIndex];
        float totalDistance = Vector3.Distance(_icon.transform.localPosition, newLinePos);
        float elapsedTime = 0f;
        float speed = totalDistance / _lineSpeed;

        while (elapsedTime < _lineSpeed)
        {
            Vector3 direction = (newLinePos - _icon.transform.localPosition).normalized;
            float distanceToMove = speed * Time.deltaTime;
            if (distanceToMove >= totalDistance)
            {
                _icon.transform.localPosition = newLinePos;
                break;
            }
            else
            {
                _icon.transform.localPosition += direction * distanceToMove;
            }

            elapsedTime += Time.deltaTime;
            if (_line.positionCount > 0)
                _line.SetPosition(0, _icon.transform.localPosition);
            yield return null;
        }

        // We need two positions for a line, so we can't remove the last 2
        if (_line.positionCount > 2)
            RemoveFirstVertexFromLine();
    }

    private void RemoveFirstVertexFromLine()
    {
        if (_line.positionCount > 1)
        {
            Vector3[] positions = new Vector3[_line.positionCount - 1];
            for (int i = 0; i < _line.positionCount - 1; i++)
            {
                positions[i] = _line.GetPosition(i + 1);
            }

            _line.positionCount--;
            _line.SetPositions(positions);
        }
    }

    internal void HighlightSlot()
    {
    }
        
}

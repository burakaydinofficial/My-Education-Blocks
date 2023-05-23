using System.Collections.Generic;
using UnityEngine;

public class TurnTable
{
    private readonly Transform _root;
    public readonly float AngleRange;
    private readonly float _radius;
    private readonly int _count;
    private readonly List<TurnTableElement> _elements = new List<TurnTableElement>();
    private readonly Vector3 _baseVector;

    public TurnTable(Transform root, float angleRange, float radius, int count, Vector3 baseVector)
    {
        _root = root;
        AngleRange = angleRange;
        _radius = radius;
        _count = count;
        _baseVector = baseVector;

        _root.transform.localPosition = _baseVector * _radius;
    }

    public TurnTableElement Deploy(Transform t, int index)
    {
        t.SetParent(_root, false);
        var e = new TurnTableElement(this, t, index);
        _elements.Add(e);
        return e;
    }

    public TurnTableElement Deploy(Transform t)
    {
        return Deploy(t, _elements.Count);
    }

    public void Select(int index, bool instant = false)
    {
        SetPosition(CalculateAngle(_count - index - 1), instant);
    }


    private float _positionTarget = 0f;
    private float _position = 0f;
    public float PositionVel;

    public void SetPosition(float angle, bool instant = false)
    {
        _positionTarget = angle;
        if (instant)
        {
            _position = angle;
            PositionVel = 0f;
            UpdatePosition();
        }
    }

    public void UpdatePosition()
    {
        _position = Mathf.SmoothDampAngle(_position, _positionTarget, ref PositionVel, 0.5f);
        foreach (var turnTableElement in _elements)
        {
            turnTableElement.Position(_position + (AngleRange * 0.5f));
        }
    }

    private float CalculateAngle(int index)
    {
        return Mathf.Lerp(0f, AngleRange, index / (_count - 1f));
    }

    public static Vector3 SamplePoint(float angle, float radius)
    {
        return new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle) * radius, 0f,
            Mathf.Sin(Mathf.Deg2Rad * angle) * radius);
    }

    public class TurnTableElement
    {
        private readonly TurnTable _btt;
        private readonly Transform _t;
        private readonly int _index;

        public TurnTableElement(TurnTable btt, Transform t, int index)
        {
            _btt = btt;
            _t = t;
            _index = index;
            Position(0f);
        }

        public void Position(float mainAngle)
        {
            float angle = _btt.CalculateAngle(_index) + mainAngle;
            _t.localPosition = SamplePoint(angle, _btt._radius);
            _t.localRotation = Quaternion.AngleAxis(-90f - angle, Vector3.up);
        }
    }
}
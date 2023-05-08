//Ultimate Camera Controller - Camera Controller
//This script is responsible for following a target object and adding orbit functionality
//to the object that it is attached to
//To make a camera follow or orbit around a target you just need to attach this script to 
//the object that contains the camera or one of its parents

using System;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;

    [SerializeField] private Transform _targetObject;
    [SerializeField] private MouseButtons _mouseButton;

    [Range(0f, 1f)]
    [SerializeField] private float _startingDistance = 0.5f;
    [SerializeField] private float _zoomSpeed = 25f;
    [SerializeField] private Vector2 _distanceRange = new Vector2(10f, 100f);
    [SerializeField] private float _zoomDamp = 0.5f;

    [SerializeField] private Vector2 _startRotation = Vector2.zero;
    [Range(5f, 25f)]
    [SerializeField] private float _rotationSpeed = 5f;
	[SerializeField] private Rect _rotationRange = Rect.MinMaxRect(-720f, -90f, 720f, 90f);
    [SerializeField] private float _rotationDamp = 0.5f;

    [SerializeField] private float _targetDistance;
    [SerializeField] private float _currentDistance;
    [NonSerialized] private float _distanceVelocity;

    [SerializeField] private Vector2 _targetRotation;
	[SerializeField] private Vector2 _currentRotation;
    [NonSerialized] private Vector2 _rotationVelocity;

    [NonSerialized] private Vector2? _previousMousePosition;

    private void Awake()
    {
        _targetDistance = Mathf.Lerp(_distanceRange.x, _distanceRange.y, _startingDistance);
        _targetRotation = _startRotation;
    }

    private void Update()
    {
        UpdateTarget();
		UpdateCurrent();
		ApplyCurrent();
    }

    private void UpdateTarget()
    {
        float zoomDelta = -Input.mouseScrollDelta.y * _zoomSpeed / Screen.dpi;
        _targetDistance = Mathf.Clamp(_targetDistance + zoomDelta, _distanceRange.x, _distanceRange.y);

        if (Input.GetMouseButton((int) _mouseButton))
        {
            Vector2 mousePosition = Input.mousePosition;
            if (_previousMousePosition.HasValue)
            {
                Vector2 mouseDelta = mousePosition - _previousMousePosition.Value;
                mouseDelta *= _rotationSpeed / Screen.dpi;
                _targetRotation = new Vector2(
                    Mathf.Clamp(_targetRotation.x + mouseDelta.x, _rotationRange.xMin, _rotationRange.xMax),
                    Mathf.Clamp(_targetRotation.y + mouseDelta.y, _rotationRange.yMin, _rotationRange.yMax));
            }
			_previousMousePosition = mousePosition;
        }

        if (Input.GetMouseButtonUp((int) _mouseButton))
        {
            _previousMousePosition = null;
        }
    }

    private void UpdateCurrent()
    {
        while (_currentRotation.x < -360f && _targetRotation.x < -360f)
        {
            _currentRotation.x += 360f;
            _targetRotation.x += 360f;
        }
        while (_currentRotation.x > 360f && _targetRotation.x > 360f)
        {
            _currentRotation.x -= 360f;
            _targetRotation.x -= 360f;
        }

        _currentDistance = Mathf.SmoothDamp(_currentDistance, _targetDistance, ref _distanceVelocity, _zoomDamp);
        _currentRotation = Vector2.SmoothDamp(_currentRotation, _targetRotation, ref _rotationVelocity, _rotationDamp);
    }

    private void ApplyCurrent()
    {
        Quaternion xAngle = Quaternion.AngleAxis(_currentRotation.x, Vector3.up);
        Quaternion yAngle = Quaternion.AngleAxis(_currentRotation.y, Vector3.left);
        var cameraAngle = xAngle * yAngle;
        var cameraPosition = yAngle * (xAngle * Vector3.back * _currentDistance);
        cameraPosition = cameraAngle * Vector3.back * _currentDistance;
        _cameraTransform.position = (_targetObject.TransformPoint(cameraPosition));
		_cameraTransform.rotation = (cameraAngle);
    }
}
public enum MouseButtons
{
    LeftButton = 0,
    RightButton = 1,
    ScrollButton = 2
}
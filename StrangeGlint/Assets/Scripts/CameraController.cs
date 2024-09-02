using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Angle")]
    [SerializeField]
    EasingFunction _angleProfile;

    [SerializeField]
    float _minAngle;

    [SerializeField]
    float _maxAngle;

    [Header("Camera Distance")]
    [SerializeField]
    EasingFunction _distanceProfile;

    [SerializeField]
    float _minDistance;

    [SerializeField]
    float _maxDistance;

    [Header("General")]
    [SerializeField]
    Transform _focalPoint;

    [SerializeField]
    int _zoomSteps;

    [SerializeField]
    float _zoomSpeed;



    PlayerInput _playerInput;

    float _zoomAlpha = 1;

    float _zoomTargetAlpha;

    float _zoomAlphaSpeed;



    public delegate float Function(float x);

    Function _angleCurve;

    Function _distanceCurve;



    private void OnValidate()
    {
        switch (_angleProfile)
        {
            case EasingFunction.EaseInSine:
                _angleCurve = EasingFunctions.EaseInSine;
                break;
            case EasingFunction.EaseOutSine:
                _angleCurve = EasingFunctions.EaseOutSine;
                break;
            case EasingFunction.EaseInCubic:
                _angleCurve = EasingFunctions.EaseInCubic;
                break;
            case EasingFunction.EaseOutCubic:
                _angleCurve = EasingFunctions.EaseOutCubic;
                break;
            case EasingFunction.EaseInQuint:
                _angleCurve = EasingFunctions.EaseInQuint;
                break;
            case EasingFunction.EaseOutQuint:
                _angleCurve = EasingFunctions.EaseOutQuint;
                break;
            case EasingFunction.EaseInCirc:
                _angleCurve = EasingFunctions.EaseInCirc;
                break;
            case EasingFunction.EaseOutCirc:
                _angleCurve = EasingFunctions.EaseOutCirc;
                break;
            case EasingFunction.EaseInQuad:
                _angleCurve = EasingFunctions.EaseInQuad;
                break;
            case EasingFunction.EaseOutQuad:
                _angleCurve = EasingFunctions.EaseOutQuad;
                break;
            case EasingFunction.EaseInQuart:
                _angleCurve = EasingFunctions.EaseInQuart;
                break;
            case EasingFunction.EaseOutQuart:
                _angleCurve = EasingFunctions.EaseOutQuart;
                break;
            case EasingFunction.EaseInExpo:
                _angleCurve = EasingFunctions.EaseInExpo;
                break;
            case EasingFunction.EaseOutExpo:
                _angleCurve = EasingFunctions.EaseOutExpo;
                break;
        }

        switch (_distanceProfile)
        {
            case EasingFunction.EaseInSine:
                _distanceCurve = EasingFunctions.EaseInSine;
                break;
            case EasingFunction.EaseOutSine:
                _distanceCurve = EasingFunctions.EaseOutSine;
                break;
            case EasingFunction.EaseInCubic:
                _distanceCurve = EasingFunctions.EaseInCubic;
                break;
            case EasingFunction.EaseOutCubic:
                _distanceCurve = EasingFunctions.EaseOutCubic;
                break;
            case EasingFunction.EaseInQuint:
                _distanceCurve = EasingFunctions.EaseInQuint;
                break;
            case EasingFunction.EaseOutQuint:
                _distanceCurve = EasingFunctions.EaseOutQuint;
                break;
            case EasingFunction.EaseInCirc:
                _distanceCurve = EasingFunctions.EaseInCirc;
                break;
            case EasingFunction.EaseOutCirc:
                _distanceCurve = EasingFunctions.EaseOutCirc;
                break;
            case EasingFunction.EaseInQuad:
                _distanceCurve = EasingFunctions.EaseInQuad;
                break;
            case EasingFunction.EaseOutQuad:
                _distanceCurve = EasingFunctions.EaseOutQuad;
                break;
            case EasingFunction.EaseInQuart:
                _distanceCurve = EasingFunctions.EaseInQuart;
                break;
            case EasingFunction.EaseOutQuart:
                _distanceCurve = EasingFunctions.EaseOutQuart;
                break;
            case EasingFunction.EaseInExpo:
                _distanceCurve = EasingFunctions.EaseInExpo;
                break;
            case EasingFunction.EaseOutExpo:
                _distanceCurve = EasingFunctions.EaseOutExpo;
                break;
        }
    }


    private void Awake()
    {
        _playerInput = new PlayerInput();
        _playerInput.Camera.Enable();
        _playerInput.Camera.Zoom.performed += (context) =>
        {
            // Retrieve input.
            var y = context.ReadValue<float>();

            y = - Mathf.Sign(y);

            // Set target alpha.
            _zoomTargetAlpha = Mathf.Clamp(_zoomTargetAlpha + y / _zoomSteps, 0, 1);
        };
    }

    private void OnEnable()
    {
        _playerInput.Camera.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Camera.Disable();
    }

    void LateUpdate()
    {
        // Move the zoom alpha value smoothly towards the target value.
        _zoomAlpha = Mathf.SmoothDamp(_zoomAlpha, _zoomTargetAlpha, ref _zoomAlphaSpeed, 1.0f / _zoomSpeed);

        // Set the rotation of the camera according to the zoom alpha value and the angle profile curve.
        var angle = AngleCurve(_zoomAlpha);
        transform.rotation = Quaternion.Euler(angle, 0, 0);

        // Set the distance of the camera to the focal point according to the zoom alpha value and the distance profile curve.
        var distance = DistanceCurve(_zoomAlpha);
        transform.position = _focalPoint.position - distance * transform.forward;
    }

    float AngleCurve(float alpha)
    {
        return (_maxAngle - _minAngle) * _angleCurve(alpha) + _minAngle;
    }

    float DistanceCurve(float alpha)
    {
        return (_maxDistance - _minDistance) * _distanceCurve(alpha) + _minDistance;
    }
}

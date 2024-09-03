using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Angle")]
    [SerializeField]
    EasingFunction _angleProfile;

    public EasingFunction AngleProfile
    {
        get { return _angleProfile; }
        set
        {
            _angleProfile = value;
            UpdateAngleCurve();
        }
    }

    public float _MinAngle;

    public float _MaxAngle;

    [Header("Camera Distance")]
    [SerializeField]
    EasingFunction _distanceProfile;

    public EasingFunction DistanceProfile
    {
        get { return _distanceProfile; }
        set
        {
            _distanceProfile = value;
            UpdateDistanceCurve();
        }
    }

    public float _MinDistance;

    public float _MaxDistance;

    [Header("General")]
    [SerializeField]
    Transform _focalPoint;

    public int _NrOfZoomLevels;

    public float _ZoomSpeed;



    PlayerInput _playerInput;

    float _zoomAlpha = 1;

    float _zoomTargetAlpha = 1;

    float _zoomAlphaSpeed;



    public delegate float Function(float x);

    Function _angleCurve;

    Function _distanceCurve;



    private void OnValidate()
    {
        UpdateAngleCurve();
        UpdateDistanceCurve();
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
            _zoomTargetAlpha = Mathf.Clamp(_zoomTargetAlpha + y / (_NrOfZoomLevels - 1), 0, 1);
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
        _zoomAlpha = Mathf.SmoothDamp(_zoomAlpha, _zoomTargetAlpha, ref _zoomAlphaSpeed, 1.0f / _ZoomSpeed);

        // Set the rotation of the camera according to the zoom alpha value and the angle profile curve.
        var angle = AngleCurve(_zoomAlpha);
        transform.rotation = Quaternion.Euler(angle, 0, 0);

        // Set the distance of the camera to the focal point according to the zoom alpha value and the distance profile curve.
        var distance = DistanceCurve(_zoomAlpha);
        transform.position = _focalPoint.position - distance * transform.forward;
    }

    void UpdateAngleCurve()
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
    }

    void UpdateDistanceCurve()
    {
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

    float AngleCurve(float alpha)
    {
        return (_MaxAngle - _MinAngle) * _angleCurve(alpha) + _MinAngle;
    }

    float DistanceCurve(float alpha)
    {
        return (_MaxDistance - _MinDistance) * _distanceCurve(alpha) + _MinDistance;
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Controller References")]
    [SerializeField]
    PlayerMovementController _movementController;

    [SerializeField]
    PlayerAnimationController _animationController;

    [SerializeField]
    CameraController _cameraController;



    [Header("General UI Elements")]
    // Reset Button
    [SerializeField]
    Button _resetButton;



    // Movement Controller
    // - Top Speed
    [Header("Movement UI Elements")]
    [SerializeField]
    Slider _topSpeedSlider;

    [SerializeField]
    TextMeshProUGUI _topSpeedText;

    float _initialTopSpeed;

    // - Time To Reach Top Speed
    [SerializeField]
    Slider _accelerationDurationSlider;

    [SerializeField]
    TextMeshProUGUI _accelerationDurationText;

    float _initialAccelerationDuration;

    // - Acceleration Curve
    [SerializeField]
    TMP_Dropdown _accelerationCurveDropdown;

    EasingFunction _initialAccelerationCurve;

    // - Time To Stop Completely
    [SerializeField]
    Slider _decelerationDurationSlider;

    [SerializeField]
    TextMeshProUGUI _decelerationDurationText;

    float _initialDecelerationDuration;

    // - Deceleration Curve
    [SerializeField]
    TMP_Dropdown _decelerationCurveDropdown;

    EasingFunction _initialDecelerationCurve;



    // Obstacle Avoidance
    // - Obstacle Avoidance Enabled
    [SerializeField]
    Toggle _obstacleAvoidanceEnabledToggle;

    bool _initialObstacleAvoidanceEnabled;

    // - Minimum Surface Angle
    [SerializeField]
    Slider _minimumSurfaceAngleSlider;

    [SerializeField]
    TextMeshProUGUI _minimumSurfaceAngleText;

    float _initialMinimumSurfaceAngle;

    // - Allowed Steering Angle
    [SerializeField]
    Slider _allowedSteeringAngleSlider;

    [SerializeField]
    TextMeshProUGUI _allowedSteeringAngleText;

    float _initialAllowedSteeringAngle;

    // - Allowed Angle Error
    [SerializeField]
    Slider _allowedAngleErrorSlider;

    [SerializeField]
    TextMeshProUGUI _allowedAngleErrorText;

    float _initialAllowedAngleError;

    // - Detection Distance
    [SerializeField]
    Slider _detectionDistanceSlider;

    [SerializeField]
    TextMeshProUGUI _detectionDistanceText;

    float _initialDetectionDistance;



    // Animations
    [Header("Animation UI Elements")]
    [SerializeField]
    Slider _turningTimeSlider;

    [SerializeField]
    TextMeshProUGUI _turningTimeText;

    float _initialTurningTime;



    // Camera
    // - Number Of Zoom Levels
    [Header("Camera UI Elements")]
    [SerializeField]
    Slider _nrOfZoomLevelsSlider;

    [SerializeField]
    TextMeshProUGUI _nrOfZoomLevelsText;

    float _initialNrOfZoomLevels;

    // - Zoom Speed
    [SerializeField]
    Slider _zoomSpeedSlider;

    [SerializeField]
    TextMeshProUGUI _zoomSpeedText;

    float _initialZoomSpeed;

    // - Flattest Angle
    [SerializeField]
    Slider _flattestAngleSlider;

    [SerializeField]
    TextMeshProUGUI _flattestAngleText;

    float _initialFlattestAngle;

    // - Steepest Angle
    [SerializeField]
    Slider _steepestAngleSlider;

    [SerializeField]
    TextMeshProUGUI _steepestAngleText;

    float _initialSteepestAngle;

    // - Angle Curve
    [SerializeField]
    TMP_Dropdown _angleCurveDropdown;

    EasingFunction _initialAngleCurve;

    // - Closest Distance
    [SerializeField]
    Slider _closestDistanceSlider;

    [SerializeField]
    TextMeshProUGUI _closestDistanceText;

    float _initialClosestDistance;

    // - Farthest Distance
    [SerializeField]
    Slider _farthestDistanceSlider;

    [SerializeField]
    TextMeshProUGUI _farthestDistanceText;

    float _initialfarthestDistance;

    // - Distance Curve
    [SerializeField]
    TMP_Dropdown _distanceCurveDropdown;

    EasingFunction _initialDistanceCurve;



    private void Awake()
    {
        // Save the initial settings of the controllers.
        // - Movement
        _initialTopSpeed = _movementController._TopSpeed;

        _initialAccelerationDuration = _movementController._TimeToReachTopSpeed;
        _initialAccelerationCurve = _movementController.AccelerationProfile;

        _initialDecelerationDuration = _movementController._TimeToStop;
        _initialDecelerationCurve = _movementController.DecelerationProfile;

        _initialObstacleAvoidanceEnabled = _movementController._ObstacleAvoidanceEnabled;
        _initialMinimumSurfaceAngle = _movementController._MinimumSurfaceAngle;
        _initialAllowedSteeringAngle = _movementController._AllowedSteeringAngle;
        _initialAllowedAngleError = _movementController._AllowedAngleError;
        _initialDetectionDistance = _movementController._DetectionDistance;

        // - Animations
        _initialTurningTime = _animationController._TurningSpeed;

        // - Camera
        _initialNrOfZoomLevels = _cameraController._NrOfZoomLevels;

        _initialZoomSpeed = _cameraController._ZoomSpeed;

        _initialFlattestAngle = _cameraController._MinAngle;
        _initialSteepestAngle = _cameraController._MaxAngle;
        _initialAngleCurve = _cameraController.AngleProfile;

        _initialClosestDistance = _cameraController._MinDistance;
        _initialfarthestDistance = _cameraController._MaxDistance;
        _initialDistanceCurve = _cameraController.DistanceProfile;



        // Add listeners to UI elements.
        _resetButton.onClick.AddListener(delegate { OnResetButtonPressed(); });

        // - Movement controls
        _topSpeedSlider.onValueChanged.AddListener(delegate { OnTopSpeedSliderChange(); });

        _accelerationDurationSlider.onValueChanged.AddListener(delegate { OnAccelerationDurationSliderChange(); });
        _accelerationCurveDropdown.onValueChanged.AddListener(delegate { OnAccelerationCurveDropdownChange(); });

        _decelerationDurationSlider.onValueChanged.AddListener(delegate { OnDecelerationDurationSliderChange(); });
        _decelerationCurveDropdown.onValueChanged.AddListener(delegate { OnDecelerationCurveDropdownChange(); });

        _obstacleAvoidanceEnabledToggle.onValueChanged.AddListener(delegate { OnObstacleAvoidanceEnabledToggleChange(); });
        _minimumSurfaceAngleSlider.onValueChanged.AddListener(delegate { OnMinimumSurfaceAngleSliderChange(); });
        _allowedSteeringAngleSlider.onValueChanged.AddListener(delegate { OnAllowedSteeringAngleSliderChange(); });
        _allowedAngleErrorSlider.onValueChanged.AddListener(delegate { OnAllowedAngleErrorSliderChange(); });
        _detectionDistanceSlider.onValueChanged.AddListener(delegate { OnDetectionDistanceSliderChange(); });

        // - Animation controls
        _turningTimeSlider.onValueChanged.AddListener(delegate { OnTurningTimeSliderChange(); });

        // - Camera controls
        _nrOfZoomLevelsSlider.onValueChanged.AddListener(delegate { OnNrOfZoomLevelsSliderChange(); });

        _zoomSpeedSlider.onValueChanged.AddListener(delegate { OnZoomSpeedSliderChange(); });

        _flattestAngleSlider.onValueChanged.AddListener(delegate { OnFlattestAngleSliderChange(); });
        _steepestAngleSlider.onValueChanged.AddListener(delegate { OnSteepestAngleSliderChange(); });
        _angleCurveDropdown.onValueChanged.AddListener(delegate { OnAngleCurveDropdownChange(); });

        _closestDistanceSlider.onValueChanged.AddListener(delegate { OnClosestDistanceSliderChange(); });
        _farthestDistanceSlider.onValueChanged.AddListener(delegate { OnFarthestDistanceSliderChange(); });
        _distanceCurveDropdown.onValueChanged.AddListener(delegate { OnDistanceCurveDropdownChange(); });



        // Reset the UI
        OnResetButtonPressed();
    }

    void OnResetButtonPressed()
    {
        // The callbacks for the UI controls will change the associated settings in the controllers as well as the UI texts.
        // - Movement controls
        _topSpeedSlider.value = _initialTopSpeed;

        _accelerationDurationSlider.value = _initialAccelerationDuration;
        _accelerationCurveDropdown.value = (int)_initialAccelerationCurve;

        _decelerationDurationSlider.value = _initialDecelerationDuration;
        _decelerationCurveDropdown.value = (int)_initialDecelerationCurve;

        _obstacleAvoidanceEnabledToggle.isOn = _initialObstacleAvoidanceEnabled;
        _minimumSurfaceAngleSlider.value = _initialMinimumSurfaceAngle;

        _allowedSteeringAngleSlider.value = _initialAllowedSteeringAngle;
        _allowedAngleErrorSlider.value = _initialAllowedAngleError;
        _detectionDistanceSlider.value = _initialDetectionDistance;



        // - Animation controls
        _turningTimeSlider.value = _initialTurningTime;

        // - Camera controls
        _nrOfZoomLevelsSlider.value = _initialNrOfZoomLevels;

        _zoomSpeedSlider.value = _initialZoomSpeed;

        _flattestAngleSlider.value = _initialFlattestAngle;
        _steepestAngleSlider.value = _initialSteepestAngle;
        _angleCurveDropdown.value = (int)_initialAngleCurve;

        _closestDistanceSlider.value = _initialClosestDistance;
        _farthestDistanceSlider.value = _initialfarthestDistance;
        _distanceCurveDropdown.value = (int)_initialDistanceCurve;
    }



    // Movement controls
    void OnTopSpeedSliderChange()
    {
        var value = _topSpeedSlider.value;

        _topSpeedText.text = value.ToString();
        _movementController._TopSpeed = value;
    }

    void OnAccelerationDurationSliderChange()
    {
        var value = _accelerationDurationSlider.value;

        _accelerationDurationText.text = value.ToString();
        _movementController._TimeToReachTopSpeed = value;
    }

    void OnAccelerationCurveDropdownChange()
    {
        _movementController.AccelerationProfile = (EasingFunction) _accelerationCurveDropdown.value;
    }

    void OnDecelerationDurationSliderChange()
    {
        var value = _decelerationDurationSlider.value;

        _decelerationDurationText.text = value.ToString();
        _movementController._TimeToStop = value;
    }

    void OnDecelerationCurveDropdownChange()
    {
        _movementController.DecelerationProfile = (EasingFunction)_decelerationCurveDropdown.value;
    }

    void OnObstacleAvoidanceEnabledToggleChange()
    {
        _movementController._ObstacleAvoidanceEnabled = _obstacleAvoidanceEnabledToggle.isOn;
    }

    void OnMinimumSurfaceAngleSliderChange()
    {
        var value = _minimumSurfaceAngleSlider.value;

        _minimumSurfaceAngleText.text = value.ToString();
        _movementController._MinimumSurfaceAngle = value;
    }

    private void OnDetectionDistanceSliderChange()
    {
        var value = _detectionDistanceSlider.value;

        _detectionDistanceText.text = value.ToString();
        _movementController._DetectionDistance = value;
    }

    private void OnAllowedAngleErrorSliderChange()
    {
        var value = _allowedAngleErrorSlider.value;

        _allowedAngleErrorText.text = value.ToString();
        _movementController._AllowedAngleError = value;
    }

    private void OnAllowedSteeringAngleSliderChange()
    {
        var value = _allowedSteeringAngleSlider.value;

        _allowedSteeringAngleText.text = value.ToString();
        _movementController._AllowedSteeringAngle = value;
    }



    // Animation controls
    void OnTurningTimeSliderChange()
    {
        var value = _turningTimeSlider.value;

        _turningTimeText.text = value.ToString();
        _animationController._TurningSpeed = value;
    }



    // Camera controls
    void OnNrOfZoomLevelsSliderChange()
    {
        var value = _nrOfZoomLevelsSlider.value;

        _nrOfZoomLevelsText.text = value.ToString();
        _cameraController._NrOfZoomLevels = Mathf.RoundToInt(value);
    }

    void OnZoomSpeedSliderChange()
    {
        var value = _zoomSpeedSlider.value;

        _zoomSpeedText.text = value.ToString();
        _cameraController._ZoomSpeed = value;
    }

    void OnFlattestAngleSliderChange()
    {
        var value = _flattestAngleSlider.value;

        _flattestAngleText.text = value.ToString();
        _cameraController._MinAngle = value;
    }

    void OnSteepestAngleSliderChange()
    {
        var value = _steepestAngleSlider.value;

        _steepestAngleText.text = value.ToString();
        _cameraController._MaxAngle = value;
    }

    void OnAngleCurveDropdownChange()
    {
        _cameraController.AngleProfile = (EasingFunction)_angleCurveDropdown.value;
    }

    void OnClosestDistanceSliderChange()
    {
        var value = _closestDistanceSlider.value;

        _closestDistanceText.text = value.ToString();
        _cameraController._MinDistance = value;
    }

    void OnFarthestDistanceSliderChange()
    {
        var value = _farthestDistanceSlider.value;

        _farthestDistanceText.text = value.ToString();
        _cameraController._MaxDistance = value;
    }

    void OnDistanceCurveDropdownChange()
    {
        _cameraController.DistanceProfile = (EasingFunction)_distanceCurveDropdown.value;
    }
}

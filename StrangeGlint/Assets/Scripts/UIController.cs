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

    // - Wall Sliding Enabled
    [SerializeField]
    Toggle _wallSlidingEnabledToggle;

    bool _initialWallSlidingEnabled;

    // - Wall Sliding Angle Threshold
    [SerializeField]
    Slider _wallSlidingAngleSlider;

    [SerializeField]
    TextMeshProUGUI _wallSlidingAngleText;

    float _initialWallSlidingAngle;



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

        _initialAccelerationDuration = _movementController._AccelerationDuration;
        _initialAccelerationCurve = _movementController.AccelerationProfile;

        _initialDecelerationDuration = _movementController._DecelerationDuration;
        _initialDecelerationCurve = _movementController.DecelerationProfile;

        _initialWallSlidingEnabled = _movementController._ObstacleAvoidanceEnabled;
        _initialWallSlidingAngle = _movementController._AvoidanceAngleThreshold;

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

        _wallSlidingEnabledToggle.onValueChanged.AddListener(delegate { OnWallSlidingEnabledToggleChange(); });
        _wallSlidingAngleSlider.onValueChanged.AddListener(delegate { OnWallSlidingAngleSliderChange(); });

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

        _wallSlidingEnabledToggle.isOn = _initialWallSlidingEnabled;
        _wallSlidingAngleSlider.value = _initialWallSlidingAngle;

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
        _movementController._AccelerationDuration = value;
    }

    void OnAccelerationCurveDropdownChange()
    {
        _movementController.AccelerationProfile = (EasingFunction) _accelerationCurveDropdown.value;
    }

    void OnDecelerationDurationSliderChange()
    {
        var value = _decelerationDurationSlider.value;

        _decelerationDurationText.text = value.ToString();
        _movementController._DecelerationDuration = value;
    }

    void OnDecelerationCurveDropdownChange()
    {
        _movementController.DecelerationProfile = (EasingFunction)_decelerationCurveDropdown.value;
    }

    void OnWallSlidingEnabledToggleChange()
    {
        _movementController._ObstacleAvoidanceEnabled = _wallSlidingEnabledToggle.isOn;
    }

    void OnWallSlidingAngleSliderChange()
    {
        var value = _wallSlidingAngleSlider.value;

        _wallSlidingAngleText.text = value.ToString();
        _movementController._AvoidanceAngleThreshold = value;
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

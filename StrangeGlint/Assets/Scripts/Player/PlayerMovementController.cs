using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public float TopSpeed {
        get { return _topSpeed; }
        private set { _topSpeed = value; }
    }

    [SerializeField, Range(1f, 10f)]
    float _topSpeed;

    [Header("Acceleration")]
    [SerializeField, Range(0.1f, 1f), Tooltip("The time it takes the player from completely standing still to reaching top speed.")]
    float _accelerationDuration;

    [SerializeField, Tooltip("The curve that the player's speed follows when accelerating.")]
    EasingFunction _accelerationProfile;

    [Header("Deceleration")]
    [SerializeField, Range(0.1f, 1f), Tooltip("The time it takes the player from moving at top speed to completely standing still.")]
    float _decelerationDuration;

    [SerializeField, Tooltip("The curve that the player's speed follows when decelerating.")]
    EasingFunction _decelerationProfile;

    [Header("Obstacle Avoidance")]
    [SerializeField, Range(0, 90), Tooltip("If the player directs the character towards a wall at a greater angle than this, the player will move perpendicular to the wall.")]
    float _avoidanceAngleThreshold;

    Vector3 _mainDir = Vector3.forward;

    PlayerInput _playerInput;

    Rigidbody _rigidbody;

    CapsuleCollider _collider;


    // The following functions define the acceleration and deceleration of the player.
    // The x axis of these functions describe the time, and the y axis describe the speed of the player.
    public delegate float Function(float x);

    [SerializeField, HideInInspector]
    Function _accCurve;

    [SerializeField, HideInInspector]
    Function _accCurveInv;

    [SerializeField, HideInInspector]
    Function _decCurve;

    [SerializeField, HideInInspector]
    Function _decCurveInv;

    private void OnValidate()
    {
        switch(_accelerationProfile)
        {
            case EasingFunction.EaseInSine:
                _accCurve = EasingFunctions.EaseInSine;
                _accCurveInv = EasingFunctions.InverseEaseInSine;
                break;
            case EasingFunction.EaseOutSine:
                _accCurve = EasingFunctions.EaseOutSine;
                _accCurveInv = EasingFunctions.InverseEaseOutSine;
                break;
            case EasingFunction.EaseInCubic:
                _accCurve = EasingFunctions.EaseInCubic;
                _accCurveInv = EasingFunctions.InverseEaseInCubic;
                break;
            case EasingFunction.EaseOutCubic:
                _accCurve = EasingFunctions.EaseOutCubic;
                _accCurveInv = EasingFunctions.InverseEaseOutCubic;
                break;
            case EasingFunction.EaseInQuint:
                _accCurve = EasingFunctions.EaseInQuint;
                _accCurveInv = EasingFunctions.InverseEaseInQuint;
                break;
            case EasingFunction.EaseOutQuint:
                _accCurve = EasingFunctions.EaseOutQuint;
                _accCurveInv = EasingFunctions.InverseEaseOutQuint;
                break;
            case EasingFunction.EaseInCirc:
                _accCurve = EasingFunctions.EaseInCirc;
                _accCurveInv = EasingFunctions.InverseEaseInCirc;
                break;
            case EasingFunction.EaseOutCirc:
                _accCurve = EasingFunctions.EaseOutCirc;
                _accCurveInv = EasingFunctions.InverseEaseOutCirc;
                break;
            case EasingFunction.EaseInQuad:
                _accCurve = EasingFunctions.EaseInQuad;
                _accCurveInv = EasingFunctions.InverseEaseInQuad;
                break;
            case EasingFunction.EaseOutQuad:
                _accCurve = EasingFunctions.EaseOutQuad;
                _accCurveInv = EasingFunctions.InverseEaseOutQuad;
                break;
            case EasingFunction.EaseInQuart:
                _accCurve = EasingFunctions.EaseInQuart;
                _accCurveInv = EasingFunctions.InverseEaseInQuart;
                break;
            case EasingFunction.EaseOutQuart:
                _accCurve = EasingFunctions.EaseOutQuart;
                _accCurveInv = EasingFunctions.InverseEaseOutQuart;
                break;
            case EasingFunction.EaseInExpo:
                _accCurve = EasingFunctions.EaseInExpo;
                _accCurveInv = EasingFunctions.InverseEaseInExpo;
                break;
            case EasingFunction.EaseOutExpo:
                _accCurve = EasingFunctions.EaseOutExpo;
                _accCurveInv = EasingFunctions.InverseEaseOutExpo;
                break;
        }

        switch(_decelerationProfile)
        {
            case EasingFunction.EaseInSine:
                _decCurve = EasingFunctions.EaseInSine;
                _decCurveInv = EasingFunctions.InverseEaseInSine;
                break;
            case EasingFunction.EaseOutSine:
                _decCurve = EasingFunctions.EaseOutSine;
                _decCurveInv = EasingFunctions.InverseEaseOutSine;
                break;
            case EasingFunction.EaseInCubic:
                _decCurve = EasingFunctions.EaseInCubic;
                _decCurveInv = EasingFunctions.InverseEaseInCubic;
                break;
            case EasingFunction.EaseOutCubic:
                _decCurve = EasingFunctions.EaseOutCubic;
                _decCurveInv = EasingFunctions.InverseEaseOutCubic;
                break;
            case EasingFunction.EaseInQuint:
                _decCurve = EasingFunctions.EaseInQuint;
                _decCurveInv = EasingFunctions.InverseEaseInQuint;
                break;
            case EasingFunction.EaseOutQuint:
                _decCurve = EasingFunctions.EaseOutQuint;
                _decCurveInv = EasingFunctions.InverseEaseOutQuint;
                break;
            case EasingFunction.EaseInCirc:
                _decCurve = EasingFunctions.EaseInCirc;
                _decCurveInv = EasingFunctions.InverseEaseInCirc;
                break;
            case EasingFunction.EaseOutCirc:
                _decCurve = EasingFunctions.EaseOutCirc;
                _decCurveInv = EasingFunctions.InverseEaseOutCirc;
                break;
            case EasingFunction.EaseInQuad:
                _decCurve = EasingFunctions.EaseInQuad;
                _decCurveInv = EasingFunctions.InverseEaseInQuad;
                break;
            case EasingFunction.EaseOutQuad:
                _decCurve = EasingFunctions.EaseOutQuad;
                _decCurveInv = EasingFunctions.InverseEaseOutQuad;
                break;
            case EasingFunction.EaseInQuart:
                _decCurve = EasingFunctions.EaseInQuart;
                _decCurveInv = EasingFunctions.InverseEaseInQuart;
                break;
            case EasingFunction.EaseOutQuart:
                _decCurve = EasingFunctions.EaseOutQuart;
                _decCurveInv = EasingFunctions.InverseEaseOutQuart;
                break;
            case EasingFunction.EaseInExpo:
                _decCurve = EasingFunctions.EaseInExpo;
                _decCurveInv = EasingFunctions.InverseEaseInExpo;
                break;
            case EasingFunction.EaseOutExpo:
                _decCurve = EasingFunctions.EaseOutExpo;
                _decCurveInv = EasingFunctions.InverseEaseOutExpo;
                break;
        }
    }


    void Awake()
    {
        _playerInput = new();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
    }

    private void OnEnable()
    {
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Disable();
    }

    private void FixedUpdate()
    {
        // Calculate the velocity on the ground plane.
        var currVelFlattened = _rigidbody.velocity;
        currVelFlattened.y = 0;



        // Update the main direction.
        // - Construct the main direction from the player input. If the input is a 0 vector, then keep the previous main direction.
        var movementInput = _playerInput.Player.Move.ReadValue<Vector2>();
        var input = new Vector3(movementInput.x, 0, movementInput.y);
        _mainDir = input == Vector3.zero ? _mainDir : input.normalized;

        // - Check if the player would run against an obstacle.
        var point1 = _collider.center + transform.position + Vector3.up * _collider.height / 2;
        var point2 = _collider.center + transform.position + Vector3.down * _collider.height / 2;

        if (Physics.CapsuleCast(point1, point2, _collider.radius * 0.99f, _mainDir, out var raycastHit, _topSpeed * Time.fixedDeltaTime))
        {
            // - Check if the angle to that obstacle would exceed the avoidance angle threshold.
            var normalInverted = -raycastHit.normal;

            var angleToWall = Vector3.SignedAngle(_mainDir, normalInverted, Vector3.up);

            if (Mathf.Abs(angleToWall) > _avoidanceAngleThreshold)
            {
                // Adjust the main direction so the player walks around the obstacle.
                var normalInverted2D = new Vector2(normalInverted.x, normalInverted.z);

                var perpDirToWall2D = Vector2.Perpendicular(normalInverted2D);

                var perpDirToWall = new Vector3(perpDirToWall2D.x, 0, perpDirToWall2D.y).normalized;

                _mainDir = Mathf.Sign(angleToWall) * perpDirToWall;
            }
        }



        // Adjust the player's velocity within the main direction

        // - Calculate the speed with which the player is moving into the main direction (dot product).
        var mainDirSpeed = currVelFlattened.magnitude * Mathf.Cos(Vector3.Angle(_mainDir, currVelFlattened) * Mathf.Deg2Rad);

        // - Calculate the speed with which the player should be moving into the main direction.
        var mainDirTargetSpeed = input.magnitude * _topSpeed;

        // - Check if the player is moving towards the main direction, but below target speed.
        if (mainDirSpeed >= 0 && mainDirSpeed < mainDirTargetSpeed)
        {
            // Speed the player up according to the acceleration curve.

            // - First, determine where the player's speed is located on the acceleration curve.
            var x = AccelerationCurveInverse(mainDirSpeed);

            // - Next, progress along the acceleration curve.
            x += Time.fixedDeltaTime;

            // - Then calculate the new speed that the player should have in the main direction.
            var nextSpeed = AccelerationCurve(x);

            // - Make sure that the new speed does not exceed the target speed.
            nextSpeed = Mathf.Min(nextSpeed, mainDirTargetSpeed);

            // - Calculate the change in velocity necessary to achieve the new speed.
            var velChange = _mainDir * (nextSpeed - mainDirSpeed);

            // - And finally, apply the change.
            _rigidbody.velocity += velChange;

            Debug.DrawRay(transform.position, velChange, Color.blue, 0f, false);

            // - Check if the player is moving towards the main direction, but above target speed.
        } else if (mainDirSpeed >= 0 && mainDirSpeed > mainDirTargetSpeed)
        {
            // Slow the player down according to the deceleration curve.

            // - First, determine where the player's speed is located on the deceleration curve.
            var x = DecelerationCurveInverse(mainDirSpeed);

            // - Next, move back along the deceleration curve.
            x -= Time.fixedDeltaTime;

            // - Then calculate the new speed that the player should have in the main direction.
            var nextSpeed = DecelerationCurve(x);

            // - Make sure that the new speed does not go below the target speed.
            nextSpeed = Mathf.Max(nextSpeed, mainDirTargetSpeed);

            // - Calculate the change in velocity necessary to achieve the new speed.
            var velChange = _mainDir * (nextSpeed - mainDirSpeed);

            // - And finally, apply the change.
            _rigidbody.velocity += velChange;

            Debug.DrawRay(transform.position, velChange, Color.blue, 0f, false);

            // - Check if the player is moving away from the main direction.
        } else if (mainDirSpeed < 0)
        {
            // Decelerate the player in the main direction.

            // - First, determine where the player's speed is located on the deceleration curve.
            // - Since mainDirSpeed < 0, mainDirSpeed must be negated here to get the correct result.
            var x = DecelerationCurveInverse(-mainDirSpeed);

            // - Next, move back along the deceleration curve.
            x -= Time.fixedDeltaTime;

            // - Check if we reached the start of the deceleration curve.
            if (x < 0)
            {
                // The start of the deceleration curve has been reached, meaning the player will completely stop within the main direction.
                // If there is time left, then the player must be accelerated according to the acceleration curve.

                // - Calculate the time left after reaching the start of the deceleration curve.
                x = -x;

                // - Calculate the new speed that the player should have in the main direction.
                var nextSpeed = AccelerationCurve(x);

                // - Make sure that the new speed does not exceed the target speed.
                nextSpeed = Mathf.Min(nextSpeed, mainDirTargetSpeed);

                // - Calculate the change in velocity necessary to reach the new speed.
                var velChange = _mainDir * (nextSpeed - mainDirSpeed);

                // - Finally, apply the change in velocity.
                _rigidbody.velocity += velChange;

                Debug.DrawRay(transform.position, velChange, Color.blue, 0f, false);

            } else
            {
                // Decelerate the player according to the deceleration curve.

                // - Calculate the new speed that the player should have in the direction OPPOSITE to the main direction
                var nextSpeed = DecelerationCurve(x);

                // - Calculate the change in velocity necessary to reach the new speed.
                var velChange = _mainDir * (-mainDirSpeed - nextSpeed);

                // - And apply the velocity change.
                _rigidbody.velocity += velChange;

                Debug.DrawRay(transform.position, velChange, Color.blue, 0f, false);
            }
        }



        // Adjust the player's speed within the perpendicular direction to the main direction.

        // - Calculate the vector perpendicular to the main direction (anti-clockwise).
        var perpDir2D = Vector2.Perpendicular(new Vector2(_mainDir.x, _mainDir.z));
        var perpDir = new Vector3(perpDir2D.x, 0, perpDir2D.y).normalized;

        // - Calculate the speed of the player in the perpendicular direction (dot product).
        var perpDirSpeed = currVelFlattened.magnitude * Mathf.Cos(Vector3.Angle(perpDir, currVelFlattened) * Mathf.Deg2Rad);

        // - Calculate where the player's speed is located on the deceleration curve.
        // - The speed might be positive or negative, but we need a positive value for the function.
        var progress = DecelerationCurveInverse(Mathf.Abs(perpDirSpeed));

        // - Move back on the deceleration curve.
        progress -= Time.fixedDeltaTime;

        // - Calculate the new speed that the player should have.
        var newSpeed = DecelerationCurve(progress);

        // - Calculate the change in velocity necessary to achieve that new speed.
        var velocityChange = perpDir * Mathf.Sign(perpDirSpeed) * (newSpeed - Mathf.Abs(perpDirSpeed));

        // - Finally, apply the change in velocity.
        _rigidbody.velocity += velocityChange;

        Debug.DrawRay(transform.position, velocityChange, Color.red, 0f, false);
    }



    // The following functions perform necessary transformations on the easing functions. These include:
    // - Scaling the functions on the y axis by max Speed
    // - Scaling the functions on the x axis by the acceleration- and deceleration durations respectively
    // - Adding return statements for input values out of the domain of the functions

    float AccelerationCurve(float x)
    {
        if (x <= 0)
            return 0;

        if (x >= _accelerationDuration)
            return _topSpeed;

        return _topSpeed * _accCurve(x / _accelerationDuration);
    }

    float AccelerationCurveInverse(float x)
    {
        if (x <= 0)
            return 0;

        if (x >= _topSpeed)
            return _accelerationDuration;

        return _accelerationDuration * _accCurveInv(x / _topSpeed);
    }

    float DecelerationCurve(float x)
    {
        if (x <= 0)
            return 0;

        if (x >= _decelerationDuration)
            return _topSpeed;

        return _topSpeed * _decCurve(x / _decelerationDuration);
    }

    float DecelerationCurveInverse(float x)
    {
        if (x <= 0)
            return 0;

        if (x >= _topSpeed)
            return _decelerationDuration;

        return _decelerationDuration * _decCurveInv(x / _topSpeed);
    }
}
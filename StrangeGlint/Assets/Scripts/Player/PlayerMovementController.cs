using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField]
    public Transform _Test;

    [Header("General")]
    public float _TopSpeed;

    [SerializeField]
    LayerMask _groundLayer;

    [SerializeField]
    LayerMask _obstaclesLayer;

    [Header("Acceleration")]
    [Tooltip("The time it takes the player from completely standing still to reaching top speed.")]
    public float _TimeToReachTopSpeed;

    [SerializeField, Tooltip("The curve that the player's speed follows when accelerating.")]
    EasingFunction _accelerationProfile;

    public EasingFunction AccelerationProfile
    {
        get { return _accelerationProfile; }
        set
        {
            _accelerationProfile = value;
            UpdateAccelerationFunction();
        }
    }

    [Header("Deceleration")]
    [Tooltip("The time it takes the player from moving at top speed to completely standing still.")]
    public float _TimeToStop;

    [SerializeField, Tooltip("The curve that the player's speed follows when decelerating.")]
    EasingFunction _decelerationProfile;

    public EasingFunction DecelerationProfile
    {
        get { return _decelerationProfile; }
        set
        {
            _decelerationProfile = value;
            UpdateDecelerationFunction();
        }
    }

    [Header("Obstacle Avoidance")]
    [Tooltip("Enables obstacle avoidance.")]
    public bool _ObstacleAvoidanceEnabled;
    
    [Range(0, 90), Tooltip("The controller will only avoid an obstacle if the angle between its surface normal and the input direction is at least this high.")]
    public float _MinimumSurfaceAngle;

    [Tooltip("The highest angle that the controller is allowed to steer the player away from their intended movement direction.")]
    public float _AllowedSteeringAngle;

    [Tooltip("The controller may steer the player around obstacles by this additional angle. Higher values improve performance.")]
    public float _AllowedAngleError;

    [Tooltip("The maximum distance at which the controller will avoid an obstacle.")]
    public float _DetectionDistance;

    PlayerInput _playerInput;

    Rigidbody _rigidbody;

    CapsuleCollider _collider;


    // The following functions define the acceleration and deceleration of the player.
    // The x axis of these functions describe the time, and the y axis describe the speed of the player.
    public delegate float Function(float x);

    [SerializeField, HideInInspector]
    Function _accelerationFunction;

    [SerializeField, HideInInspector]
    Function _inverseAccelerationFunction;

    [SerializeField, HideInInspector]
    Function _decelerationFunction;

    [SerializeField, HideInInspector]
    Function _inverseDecelerationFunction;

    [SerializeField, HideInInspector]
    Function _integralDecelerationFunction;

    private void OnValidate()
    {
        UpdateAccelerationFunction();
        UpdateDecelerationFunction();
    }

    void Awake()
    {
        _playerInput = new();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();

        UpdateAccelerationFunction();
        UpdateDecelerationFunction();
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
        // Calculate the next velocity of the player.
        var movementInput = _playerInput.Player.Move.ReadValue<Vector2>();

        var newVelocity = CalculateNextVelocity(_rigidbody.velocity, movementInput);

        // Apply the obstacle avoidance if it is enabled.
        if (_ObstacleAvoidanceEnabled)
        {
            // Check if the player would run against an obstacle.

            // - Perform a cast in the movement direction of the player.
            // - - Calculate values that define the collider's capsule shape for the raycast.
            var capsulePoint1 = _collider.center + transform.position + Vector3.up * _collider.height / 2;
            var capsulePoint2 = _collider.center + transform.position + Vector3.down * _collider.height / 2;

            // - - The radius of the capsule for the raycast is chosen smaller than the collider's radius to allow the raycast to hit obstacles that touch the collider.
            var colliderRadius = _collider.radius * 0.99f;

            // - Calculate the distance for the raycast. The raycast should only check for obstacles that the player would definitely collide with plus the provided detection distance.
            // - The detection distance should be greater 0 because the calculated stopping position deviates somewhat from the actual position where the player stops.
            var stoppingPosition = CalculateStoppingPosition(transform.position, newVelocity);
            var raycastDistance = (transform.position - stoppingPosition).magnitude + _DetectionDistance;

            var raycastDirection = newVelocity;

            if (Physics.CapsuleCast(capsulePoint1, capsulePoint2, colliderRadius, raycastDirection, out var raycastHit, raycastDistance, _obstaclesLayer))
            {
                // There is an obstacle in the path of the player.
                // Check if the player intentionally moves towards the obstacle by checking the angle between the input and the hit normal.
                var normalInverted2D = new Vector2(-raycastHit.normal.x, -raycastHit.normal.z);

                var angleToWall = Vector2.SignedAngle(movementInput, normalInverted2D);

                if (Mathf.Abs(angleToWall) > _MinimumSurfaceAngle)
                {
                    // The player probably does not want to hit the obstacle, so engage the obstacle avoidance.

                    // - Capsule cast around the obstacle until there is no obstacle in the way or until avoiding the obstacle means deviating too much from the player's intended movement direction.
                    var deviationAngle = 0f;

                    var hitPoint = raycastHit.point;

                    while (true)
                    {
                        // Check if avoiding the obstacle means deviating too much from the player's intended movement direction.
                        // - Calculate the angle by which the velocity must be rotated to avoid the obstacle.
                        var hitpointDelta = transform.position - hitPoint;
                        hitpointDelta.y = 0;
                        var hitpointDistance = hitpointDelta.magnitude;

                        var angleIncrease = Mathf.Abs(Mathf.Atan(colliderRadius / hitpointDistance) + _AllowedAngleError);

                        // - Calculate the total angle of deviation.
                        deviationAngle += angleIncrease;

                        // - If the total angle of deviation is too large then we abort the obstacle avoidance. The player will move in the intended direction against the obstacle.
                        if (deviationAngle > _AllowedSteeringAngle)
                            break;

                        // The new angle does not deviate too much from the player's intended movement direction.
                        // Check if there is an obstacle in the new direction.

                        // - Calculate a new direction based on the angle increase.
                        raycastDirection = Quaternion.AngleAxis(Mathf.Sign(angleToWall) * angleIncrease, Vector3.up) * raycastDirection;

                        // - Calculate the new raycast distance
                        raycastDistance = hitpointDistance;

                        if (!Physics.CapsuleCast(capsulePoint1, capsulePoint2, colliderRadius, raycastDirection, out var hit, raycastDistance, _obstaclesLayer))
                        {
                            // The way is free and does not deviate too much from the intended direction.
                            // Therefore overwrite the movement input of the player with the new direction.

                            raycastDirection = raycastDirection.normalized;

                            newVelocity = CalculateNextVelocity(_rigidbody.velocity, new Vector2(raycastDirection.x, raycastDirection.z));
                            break;
                        }

                        // Update the hit point that should be avoided.
                        hitPoint = hit.point;
                    }
                }
            }
        }

        // Apply the new velocity that was calculated before.
        _rigidbody.velocity = newVelocity;


        SnapToGround();
    }


    void SnapToGround()
    {
        // Position the player on top of the ground.
        if (Physics.Raycast(transform.position, Vector3.down, out var groundHit, Mathf.Infinity, _groundLayer))
            transform.position = groundHit.point + Vector3.up * _collider.height / 2 - _collider.center;
    }


    Vector3 CalculateNextVelocity(Vector3 currentVelocity, Vector2 movementInput)
    {
        // Calculate the target velocity as a two-dimensional vector.
        var targetVelocity2D = movementInput * _TopSpeed;

        // The current velocity will transition to the target velocity in a straight line.
        var currentVelocity2D = new Vector2(currentVelocity.x, currentVelocity.z);
        var deltaVelocityDirection = (targetVelocity2D - currentVelocity2D).normalized;

        // The point on the line between current and target velocity that is closest to the origin is where the controller switches from decelerating to accelerating.
        var turningPoint = Vector2Utility.CalculateLinePointClosestToOrigin(currentVelocity2D, deltaVelocityDirection);
        var currentDistanceToTurningPoint = (turningPoint - currentVelocity2D).magnitude;

        var deltaTurningPoint = turningPoint - currentVelocity2D;

        // If the turning point is located in front of the target velocity then the controller must decelerate. Otherwise, it must accelerate.
        if (Vector2.Dot(deltaVelocityDirection, deltaTurningPoint) > 0)
        {
            // The turning point is located in front of the target velocity, so the controller must decelerate.
            // The current speed in the relevant direction is described by the distance to the turning point.
            var x = InverseDecelerationCurve(currentDistanceToTurningPoint);

            x -= Time.fixedDeltaTime;

            // If x < 0 then the start of the deceleration curve has been reached.
            // This means that the controller will reach the turning point and must accelerate for the remaining time.
            if (x < 0)
            {
                x = -x;

                var newVelocity2D = Vector2.MoveTowards(turningPoint, targetVelocity2D, AccelerationCurve(x));

                return new Vector3(newVelocity2D.x, 0, newVelocity2D.y);

            } else
            {
                // The new velocity will stay behind the turning point.
                var newSpeed = DecelerationCurve(x);

                var newVelocity2D = Vector2.MoveTowards(turningPoint, currentVelocity2D, newSpeed);

                return new Vector3(newVelocity2D.x, 0, newVelocity2D.y);
            }
        } else
        {
            // The controller must accelerate.

            var x = InverseAccelerationCurve(currentDistanceToTurningPoint);

            x += Time.fixedDeltaTime;

            var newSpeed = AccelerationCurve(x);

            var newVelocity2D = Vector2.MoveTowards(turningPoint, targetVelocity2D, newSpeed);

            return new Vector3(newVelocity2D.x, 0, newVelocity2D.y);
        }
    }



    Vector3 CalculateStoppingPosition(Vector3 position, Vector3 velocity)
    {
        // Initialize the result variable.
        velocity.y = 0;
        var currentSpeed = velocity.magnitude;

        // Calculate where the currentVelocity is on the deceleration curve
        var x = InverseDecelerationCurve(currentSpeed);

        // Calculate the distance traveled when stopping now
        var distance = IntegralDecelerationCurve(x);

        var stoppingPosition = position + velocity.normalized * distance;

        return stoppingPosition;
    }

    void UpdateAccelerationFunction()
    {
        switch (_accelerationProfile)
        {
            case EasingFunction.EaseInSine:
                _accelerationFunction = EasingFunctions.EaseInSine;
                _inverseAccelerationFunction = EasingFunctions.InverseEaseInSine;
                break;
            case EasingFunction.EaseOutSine:
                _accelerationFunction = EasingFunctions.EaseOutSine;
                _inverseAccelerationFunction = EasingFunctions.InverseEaseOutSine;
                break;
            case EasingFunction.EaseInCubic:
                _accelerationFunction = EasingFunctions.EaseInCubic;
                _inverseAccelerationFunction = EasingFunctions.InverseEaseInCubic;
                break;
            case EasingFunction.EaseOutCubic:
                _accelerationFunction = EasingFunctions.EaseOutCubic;
                _inverseAccelerationFunction = EasingFunctions.InverseEaseOutCubic;
                break;
            case EasingFunction.EaseInQuint:
                _accelerationFunction = EasingFunctions.EaseInQuint;
                _inverseAccelerationFunction = EasingFunctions.InverseEaseInQuint;
                break;
            case EasingFunction.EaseOutQuint:
                _accelerationFunction = EasingFunctions.EaseOutQuint;
                _inverseAccelerationFunction = EasingFunctions.InverseEaseOutQuint;
                break;
            case EasingFunction.EaseInCirc:
                _accelerationFunction = EasingFunctions.EaseInCirc;
                _inverseAccelerationFunction = EasingFunctions.InverseEaseInCirc;
                break;
            case EasingFunction.EaseOutCirc:
                _accelerationFunction = EasingFunctions.EaseOutCirc;
                _inverseAccelerationFunction = EasingFunctions.InverseEaseOutCirc;
                break;
            case EasingFunction.EaseInQuad:
                _accelerationFunction = EasingFunctions.EaseInQuad;
                _inverseAccelerationFunction = EasingFunctions.InverseEaseInQuad;
                break;
            case EasingFunction.EaseOutQuad:
                _accelerationFunction = EasingFunctions.EaseOutQuad;
                _inverseAccelerationFunction = EasingFunctions.InverseEaseOutQuad;
                break;
            case EasingFunction.EaseInQuart:
                _accelerationFunction = EasingFunctions.EaseInQuart;
                _inverseAccelerationFunction = EasingFunctions.InverseEaseInQuart;
                break;
            case EasingFunction.EaseOutQuart:
                _accelerationFunction = EasingFunctions.EaseOutQuart;
                _inverseAccelerationFunction = EasingFunctions.InverseEaseOutQuart;
                break;
            case EasingFunction.EaseInExpo:
                _accelerationFunction = EasingFunctions.EaseInExpo;
                _inverseAccelerationFunction = EasingFunctions.InverseEaseInExpo;
                break;
            case EasingFunction.EaseOutExpo:
                _accelerationFunction = EasingFunctions.EaseOutExpo;
                _inverseAccelerationFunction = EasingFunctions.InverseEaseOutExpo;
                break;
        }
    }

    

    void UpdateDecelerationFunction()
    {
        switch (_decelerationProfile)
        {
            case EasingFunction.EaseInSine:
                _decelerationFunction = EasingFunctions.EaseInSine;
                _inverseDecelerationFunction = EasingFunctions.InverseEaseInSine;
                _integralDecelerationFunction = EasingFunctions.IntegralEaseInSine;
                break;
            case EasingFunction.EaseOutSine:
                _decelerationFunction = EasingFunctions.EaseOutSine;
                _inverseDecelerationFunction = EasingFunctions.InverseEaseOutSine;
                _integralDecelerationFunction = EasingFunctions.IntegralEaseOutSine;
                break;
            case EasingFunction.EaseInCubic:
                _decelerationFunction = EasingFunctions.EaseInCubic;
                _inverseDecelerationFunction = EasingFunctions.InverseEaseInCubic;
                _integralDecelerationFunction = EasingFunctions.IntegralEaseInCubic;
                break;
            case EasingFunction.EaseOutCubic:
                _decelerationFunction = EasingFunctions.EaseOutCubic;
                _inverseDecelerationFunction = EasingFunctions.InverseEaseOutCubic;
                _integralDecelerationFunction = EasingFunctions.IntegralEaseOutCubic;
                break;
            case EasingFunction.EaseInQuint:
                _decelerationFunction = EasingFunctions.EaseInQuint;
                _inverseDecelerationFunction = EasingFunctions.InverseEaseInQuint;
                _integralDecelerationFunction = EasingFunctions.IntegralEaseInQuint;
                break;
            case EasingFunction.EaseOutQuint:
                _decelerationFunction = EasingFunctions.EaseOutQuint;
                _inverseDecelerationFunction = EasingFunctions.InverseEaseOutQuint;
                _integralDecelerationFunction = EasingFunctions.IntegralEaseOutQuint;
                break;
            case EasingFunction.EaseInCirc:
                _decelerationFunction = EasingFunctions.EaseInCirc;
                _inverseDecelerationFunction = EasingFunctions.InverseEaseInCirc;
                _integralDecelerationFunction = EasingFunctions.IntegralEaseInCirc;
                break;
            case EasingFunction.EaseOutCirc:
                _decelerationFunction = EasingFunctions.EaseOutCirc;
                _inverseDecelerationFunction = EasingFunctions.InverseEaseOutCirc;
                _integralDecelerationFunction = EasingFunctions.IntegralEaseOutCirc;
                break;
            case EasingFunction.EaseInQuad:
                _decelerationFunction = EasingFunctions.EaseInQuad;
                _inverseDecelerationFunction = EasingFunctions.InverseEaseInQuad;
                _integralDecelerationFunction = EasingFunctions.IntegralEaseInQuad;
                break;
            case EasingFunction.EaseOutQuad:
                _decelerationFunction = EasingFunctions.EaseOutQuad;
                _inverseDecelerationFunction = EasingFunctions.InverseEaseOutQuad;
                _integralDecelerationFunction = EasingFunctions.IntegralEaseOutQuad;
                break;
            case EasingFunction.EaseInQuart:
                _decelerationFunction = EasingFunctions.EaseInQuart;
                _inverseDecelerationFunction = EasingFunctions.InverseEaseInQuart;
                _integralDecelerationFunction = EasingFunctions.IntegralEaseInQuart;
                break;
            case EasingFunction.EaseOutQuart:
                _decelerationFunction = EasingFunctions.EaseOutQuart;
                _inverseDecelerationFunction = EasingFunctions.InverseEaseOutQuart;
                _integralDecelerationFunction = EasingFunctions.IntegralEaseOutQuart;
                break;
            case EasingFunction.EaseInExpo:
                _decelerationFunction = EasingFunctions.EaseInExpo;
                _inverseDecelerationFunction = EasingFunctions.InverseEaseInExpo;
                _integralDecelerationFunction = EasingFunctions.IntegralEaseInExpo;
                break;
            case EasingFunction.EaseOutExpo:
                _decelerationFunction = EasingFunctions.EaseOutExpo;
                _inverseDecelerationFunction = EasingFunctions.InverseEaseOutExpo;
                _integralDecelerationFunction = EasingFunctions.IntegralEaseOutExpo;
                break;
        }
    }



    // The following functions perform necessary transformations on the easing functions. These include:
    // - Scaling the functions on the y axis by max Speed
    // - Scaling the functions on the x axis by the acceleration- and deceleration durations respectively
    // - Adding return statements for input values out of the domain of the functions

    float AccelerationCurve(float x)
    {
        if (x <= 0)
            return 0;

        if (x >= _TimeToReachTopSpeed)
            return _TopSpeed;

        return _TopSpeed * _accelerationFunction(x / _TimeToReachTopSpeed);
    }

    float InverseAccelerationCurve(float x)
    {
        if (x <= 0)
            return 0;

        if (x >= _TopSpeed)
            return _TimeToReachTopSpeed;

        return _TimeToReachTopSpeed * _inverseAccelerationFunction(x / _TopSpeed);
    }

    float DecelerationCurve(float x)
    {
        if (x <= 0)
            return 0;

        if (x >= _TimeToStop)
            return _TopSpeed;

        return _TopSpeed * _decelerationFunction(x / _TimeToStop);
    }

    float InverseDecelerationCurve(float x)
    {
        if (x <= 0)
            return 0;

        if (x >= _TopSpeed)
            return _TimeToStop;

        return _TimeToStop * _inverseDecelerationFunction(x / _TopSpeed);
    }

    float IntegralDecelerationCurve(float x)
    {
        if (x <= 0)
            return 0;

        if (x >= _TimeToStop)
            return _TopSpeed * _TimeToStop;

        return _TopSpeed * _TimeToStop * _integralDecelerationFunction(x / _TimeToStop);
    } 
}
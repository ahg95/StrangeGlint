using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    Rigidbody _rigidbody;

    [SerializeField]
    PlayerMovementController _movementController;

    [Header("Properties")]
    [SerializeField, Range(0.01f, 1)]
    float _turningTime;

    [SerializeField]
    float _runningAnimationSpeed;

    Animator _animatorController;

    float _angularVelocity = 0;

    private void Awake()
    {
        _animatorController = GetComponent<Animator>();

        // Change the animator's animation speed based on the movement controller's top speed.
        // This prevents foot slipping when the top speed is changed.
        _animatorController.speed = _movementController.TopSpeed / _runningAnimationSpeed;
    }

    void Update()
    {
        // Rotate the player model towards the movement direction.
        var currentAngle = transform.eulerAngles.y;

        var currVelFlattened = _rigidbody.velocity;
        currVelFlattened.y = 0;

        if (currVelFlattened.magnitude > 0.1f)
        {
            var targetAngle = Vector3.SignedAngle(Vector3.back, currVelFlattened, Vector3.up) + 180;

            var nextAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref _angularVelocity, _turningTime);

            transform.eulerAngles = new Vector3(0, nextAngle, 0);
        }

        // Set the animation parameters.
        // - Calculate the movement speed in the forward direction.
        var forwardSpeed = currVelFlattened.magnitude * Mathf.Cos(Vector3.Angle(transform.forward, currVelFlattened) * Mathf.Deg2Rad);

        _animatorController.SetFloat("ForwardSpeed", forwardSpeed / _movementController.TopSpeed);


        // - Calculate the movement speed in the right direction.
        var sidewaysSpeed = currVelFlattened.magnitude * Mathf.Cos(Vector3.Angle(transform.right, currVelFlattened) * Mathf.Deg2Rad);

        _animatorController.SetFloat("SidewaysSpeed", sidewaysSpeed / _movementController.TopSpeed);


    }
}

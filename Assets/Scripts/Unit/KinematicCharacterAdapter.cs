using KinematicCharacterController;
using UnityEngine;

public class KinematicCharacterAdapter : MonoBehaviour, ICharacterController
{
    private KinematicCharacterMotor motor;
    private UnitController unit;
    private DeviceController device;
    private float gravity = 1f;
    private float movementFactor = 8f;

    private void Awake()
    {
        motor = GetComponent<KinematicCharacterMotor>();
        unit = GetComponent<UnitController>();
        motor.CharacterController = this;
    }

    void Start()
    {
        device = GetComponent<DeviceController>();
    }

    public bool IsColliderValidForCollisions(Collider collider)
    {
        var colliderUnit = collider.GetComponent<UnitController>();

        if (colliderUnit != null)
        {
            return unit.IsAlive() && colliderUnit.IsAlive();
        } else
        {
            var colliderItem = collider.GetComponent<ItemController>();

            if (colliderItem)
            {
                colliderItem.OnCollisionWithUnit(unit);
                return false;
            }
        }

        return true;
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        ApplyGravity(ref currentVelocity);

        var axis =
            unit.IsAlive() && IsCloseEnoughToMove()
                ? device.GetAxis()
                : DeviceController.frozenAxis;

        currentVelocity.x = axis.GetX() * movementFactor * unit.speed;
        currentVelocity.z = axis.GetY() * movementFactor * unit.speed;

        if (motor.GroundingStatus.IsStableOnGround && axis.GetButtonA() > 0)
        {
            currentVelocity.y = axis.GetButtonA() * 10f;
            motor.ForceUnground();
        }
    }
    
    public void SetPosition(Vector3 position)
    {
        motor.SetPosition(position);
    }
    
    public void SetRotation(Quaternion rotation)
    {
        motor.SetRotation(rotation);
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        if (!unit.IsAlive() || !IsCloseEnoughToMove())
        {
            return;
        }

        var axis = device.GetAxis();
        var rotateAt = new Vector3(axis.GetX(), 0, axis.GetY());

        if (rotateAt.magnitude > 0.15f)
        {
            currentRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotateAt), deltaTime * 10f);
        }
    }

    bool IsCloseEnoughToMove()
    {
        return transform.position.z > -50;
    }

    public void AfterCharacterUpdate(float deltaTime) { }

    public void BeforeCharacterUpdate(float deltaTime) { }

    public void OnDiscreteCollisionDetected(Collider hitCollider) { }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) {
    }

    public void PostGroundingUpdate(float deltaTime) { }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }

    void ApplyGravity(ref Vector3 currentVelocity)
    {
        currentVelocity.y -= gravity;

        if (motor.GroundingStatus.IsStableOnGround)
        {
            currentVelocity.y = 0;
        }
    }
}

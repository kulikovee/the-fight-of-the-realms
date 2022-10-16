using KinematicCharacterController;
using UnityEngine;

public class KinematicCharacterAdapter : MonoBehaviour, ICharacterController
{
    private KinematicCharacterMotor motor;
    private DeviceController device;
    private float gravity = 3f;
    private float movementSpeed = 8f;

    private void Awake()
    {
        motor = GetComponent<KinematicCharacterMotor>();
        motor.CharacterController = this;
    }

    private void Start()
    {
        device = GetComponent<DeviceController>();
    }

    public bool IsColliderValidForCollisions(Collider collider)
    {
        var unit = collider.GetComponent<UnitController>();
        return unit == null ? true : !unit.GetDevice().IsFrozen();
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        var axis = device.GetUpdatedAxis();

        currentVelocity.x = axis.GetX() * movementSpeed;
        currentVelocity.z = axis.GetY() * movementSpeed;

        if (motor.GroundingStatus.IsStableOnGround)
        {
            if (axis.GetAction() > 0)
            {
                currentVelocity.y = axis.GetAction() * 10f;
                motor.ForceUnground();
            } else if (currentVelocity.y <= 0)
            {
                currentVelocity.y = 0;
            }
        } else
        {
            currentVelocity.y -= gravity;
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
        var axis = device.GetUpdatedAxis();
        var rotateAt = new Vector3(axis.GetX(), 0, axis.GetY());

        if (rotateAt.magnitude > 0.15f)
        {
            currentRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotateAt), deltaTime * 10f);
        }
    }

    public void AfterCharacterUpdate(float deltaTime) { }

    public void BeforeCharacterUpdate(float deltaTime) { }

    public void OnDiscreteCollisionDetected(Collider hitCollider) { }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) {
    }

    public void PostGroundingUpdate(float deltaTime) { }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }
}

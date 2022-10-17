using KinematicCharacterController;
using UnityEngine;

public class KinematicCharacterAdapter : MonoBehaviour, ICharacterController
{
    private KinematicCharacterMotor motor;
    private UnitController unit;
    private DeviceController device;
    private float gravity = 1f;
    private float movementSpeed = 8f;

    private void Awake()
    {
        motor = GetComponent<KinematicCharacterMotor>();
        unit = GetComponent<UnitController>();
        motor.CharacterController = this;
    }

    private void Start()
    {
        device = GetComponent<DeviceController>();
    }

    public bool IsColliderValidForCollisions(Collider collider)
    {
        var colliderUnit = collider.GetComponent<UnitController>();
        return colliderUnit == null || (unit.IsAlive() && colliderUnit.IsAlive());
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        ApplyGravity(ref currentVelocity);

        var axis = 
            unit.IsAlive() 
                ? device.GetUpdatedAxis() 
                : DeviceController.frozenAxis;

        currentVelocity.x = axis.GetX() * movementSpeed;
        currentVelocity.z = axis.GetY() * movementSpeed;

        if (motor.GroundingStatus.IsStableOnGround && axis.GetAction() > 0)
        {
            currentVelocity.y = axis.GetAction() * 10f;
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
        if (!unit.IsAlive())
        {
            return;
        }

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

    void ApplyGravity(ref Vector3 currentVelocity)
    {
        currentVelocity.y -= gravity;

        if (motor.GroundingStatus.IsStableOnGround)
        {
            currentVelocity.y = 0;
        }
    }
}

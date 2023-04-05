using System.Collections;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [System.Serializable]
    public class Wheel
    {
        public WheelCollider collider;
        public Transform transform;
    }

    [System.Serializable]
    public class Axle
    {
        public Wheel leftWheel;
        public Wheel rightWheel;
        public bool isMotor;
        public bool isSteering;
    }

    public void UpdateWheelTransform(Wheel wheel)
    {
        wheel.collider.GetWorldPose(out Vector3 position, out Quaternion rotation);
        wheel.transform.position = position;
        wheel.transform.rotation = rotation;
    }

    [SerializeField] Axle[] axles;
    [SerializeField] float maxMotorTorque;
    [SerializeField] float maxSteeringAngle;

    // New reset variables
    public Transform resetPosition;
    public float resetDelay = 1f;
    private bool isResetting = false;

    public void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        foreach (Axle axle in axles)
        {
            if (axle.isSteering)
            {
                axle.leftWheel.collider.steerAngle = steering;
                axle.rightWheel.collider.steerAngle = steering;
            }
            if (axle.isMotor)
            {
                axle.leftWheel.collider.motorTorque = motor;
                axle.rightWheel.collider.motorTorque = motor;
            }
            UpdateWheelTransform(axle.leftWheel);
            UpdateWheelTransform(axle.rightWheel);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Entered");
        if (other.gameObject.CompareTag("ResetTrigger") && !isResetting)
        {
            StartCoroutine(ResetVehicleCoroutine());
        }
    }

    private IEnumerator ResetVehicleCoroutine()
    {
        isResetting = true;
        yield return new WaitForSeconds(resetDelay);
        transform.position = resetPosition.position;
        transform.rotation = resetPosition.rotation;
        isResetting = false;
    }
}

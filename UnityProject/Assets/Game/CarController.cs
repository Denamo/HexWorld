using UnityEngine;
using System.Collections;

public class CarController : MonoBehaviour {

	public Transform wheelTransformFL;
	public Transform wheelTransformFR;
	public Transform wheelTransformRL;
	public Transform wheelTransformRR;

	public WheelCollider wheelColliderFL;
	public WheelCollider wheelColliderFR;
	public WheelCollider wheelColliderRL;
	public WheelCollider wheelColliderRR;

	public float maxTorque = 100f;
	public float maxTurn = 20f;
	public Vector3 centerOfMass;

	void Start () {
		Rigidbody rb = GetComponent<Rigidbody>();
		if(rb){
			rb.centerOfMass = centerOfMass;
		}

		/*wheelColliderRL.extrmotorTorque = torque*0.5f;
		wheelColliderRR.motorTorque = torque*0.5f;
		wheelColliderFL.motorTorque = torque;
		wheelColliderFR.motorTorque = torque;*/


	}
	
	float rotationFL = 0f;
	float rotationFR = 0f;
	float rotationRL = 0f;
	float rotationRR = 0f;

	void FixedUpdate () {
		float torque = Input.GetAxis("Vertical") * maxTorque;
		float turn = Input.GetAxis("Horizontal") * maxTurn;

		//Debug.Log("Torque:" + torque + " Turn" + turn);

		wheelColliderRL.motorTorque = torque;
		wheelColliderRR.motorTorque = torque;

		//wheelColliderFL.motorTorque = torque*0.2f;
		//wheelColliderFR.motorTorque = torque*0.2f;

		wheelColliderFL.steerAngle = turn;
		wheelColliderFR.steerAngle = turn;

		float mul = Time.fixedDeltaTime*Mathf.PI;

		rotationFL += wheelColliderFL.rpm*mul;
		rotationFR += wheelColliderFR.rpm*mul;
		rotationRL += wheelColliderRL.rpm*mul;
		rotationRR += wheelColliderRR.rpm*mul;

	}

	void Update() {
		wheelTransformFL.position = getWheelSuspensionPosition( wheelColliderFL );
		wheelTransformFR.position = getWheelSuspensionPosition( wheelColliderFR );
		wheelTransformRL.position = getWheelSuspensionPosition( wheelColliderRL );
		wheelTransformRR.position = getWheelSuspensionPosition( wheelColliderRR );

		wheelTransformFL.rotation = transform.rotation * Quaternion.Euler(0,wheelColliderFL.steerAngle+90f,rotationFL);
		wheelTransformFR.rotation = transform.rotation * Quaternion.Euler(0,wheelColliderFR.steerAngle+90f,rotationFR);
		wheelTransformRL.rotation = transform.rotation * Quaternion.Euler(0,wheelColliderRL.steerAngle+90f,rotationRL);
		wheelTransformRR.rotation = transform.rotation * Quaternion.Euler(0,wheelColliderRR.steerAngle+90f,rotationRR);
	}

	Vector3 getWheelSuspensionPosition(WheelCollider collider)
	{
		Vector3 center = collider.transform.TransformPoint(collider.center);
		RaycastHit hit;

		if ( Physics.Raycast(center, -collider.transform.up, out hit, collider.suspensionDistance + collider.radius) ) {
			return hit.point + (collider.transform.up * collider.radius);
		} else {
			return center - (collider.transform.up * collider.suspensionDistance);
		}

	}



}

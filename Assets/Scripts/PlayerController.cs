using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

	Rigidbody myRigidbody;
	Vector3 velocity;

	void Start () {
		myRigidbody = GetComponent<Rigidbody>();
	}
		
	public void move(Vector3 _velocity){

		if (CheckOverGround ()) {
			velocity = _velocity;
		} else {
			velocity = -transform.position.normalized;
		}
	}

	 void FixedUpdate(){

			myRigidbody.MovePosition(myRigidbody.position + velocity * Time.deltaTime);

	}

	public void LookAt(Vector3 lookPoint){
		Vector3 heightCorrection = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
		transform.LookAt(heightCorrection);
	}

	public bool CheckOverGround(){

		RaycastHit outRay;
		 

		Physics.Raycast (this.transform.position, Vector3.down, out outRay);

		if (outRay.collider.tag == "EnvironmentGround") {
			return false;
		} else {
			return true; 
		}
		

	}
}


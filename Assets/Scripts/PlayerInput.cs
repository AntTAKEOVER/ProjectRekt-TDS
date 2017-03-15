using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class PlayerInput : LivingEntity {

	public float moveSpeed = 5f;
	public Vector3 moveVelocity;


	Camera viewCamera;
	PlayerController controller;
	GunController gunController;

	public CrossHairs crosshair;

	// Use this for initialization
	protected override void Start () {

		base.Start ();
		controller = GetComponent<PlayerController>();
		viewCamera = Camera.main;
		gunController = GetComponent<GunController>();


	}
	
	// Update is called onceper frame
	void Update () {


		//MoveInput
		Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		moveVelocity = moveInput.normalized * moveSpeed;
		controller.move(moveVelocity);


		//LookInput
		Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
		Plane groundPlane = new Plane(Vector3.up,Vector3.up * gunController.GunHeight);
		float rayDistance;

		if(groundPlane.Raycast(ray, out rayDistance)){
			Vector3 point = ray.GetPoint(rayDistance);
			//Debug.DrawLine(ray.origin, point, Color.red);
			controller.LookAt(point);
			crosshair.transform.position = point;
			crosshair.DetectTargets(ray);
		//	gunController.Aim (point);
			//Debug.Log((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).magnitude);
		}

		//WeaponInput


		if(Input.GetMouseButton(0)){

			gunController.OnTriggerHold();

		}

		if(Input.GetMouseButtonUp(0)){
			
			gunController.OntriggerRelease();
			
		}
	}
}


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
	}

	void Awake(){
		controller = GetComponent<PlayerController>();
		viewCamera = Camera.main;
		gunController = GetComponent<GunController>();
		FindObjectOfType<Spawner> ().OnNewWave += OnNewWave;
	}

	void OnNewWave(int waveNumber){
		health = startingHealth;
		gunController.EquipGun (waveNumber - 1);
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
	 		
			if((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 2.25f)
				gunController.Aim (point);
		}

		//WeaponInput


		if(Input.GetMouseButton(0)){

			gunController.OnTriggerHold();

		}

		if(Input.GetMouseButtonUp(0)){
			
			gunController.OntriggerRelease();
			
		}

		if (Input.GetKeyDown (KeyCode.R)) {
			gunController.Reload ();
		}
	}
}


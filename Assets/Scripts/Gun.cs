using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

	public enum FireMode{Auto, Burst, Single};
	public FireMode fireMode;


	public Transform[] muzzle;
	public Projectile projectile;
	public float msBetweenShots = 100;
	public float muzzleVelocity = 35;
	public int burstCount;
	public int projectilesPerMag;
	public float reloadTime = 0.3f;

	[Header("Recoil")]
	public Vector2 kickMinMax = new Vector2(0.2f, 0.5f);
	public Vector2 recoilAngleMinMax = new Vector2(3,6);
	public float recoilMoveReturnSpeed = 0.1f;
	public float recoilRotReturnSpeed = 0.1f;
	Vector3 recoilSmoothDamVelocity;
	float recoilRotSmoothDampVelocity;
	float recoilAngle;

	[Header("Effects")]
	public Transform shell;
	public Transform shellEjection;
	MuzzleFlash muzzleflash;
	float nextShotTime;
	bool isBurstTriggered = true;
	int shotsRemainingInBurst;
	int projectilesRemainingInMag;
	bool triggerReleased;
	bool isReloading;
	public AudioClip shootAudio;
	public AudioClip reloadAuido;


	void Start(){
		shotsRemainingInBurst = burstCount;
		muzzleflash = GetComponent<MuzzleFlash> ();
		projectilesRemainingInMag = projectilesPerMag;
	}

	void LateUpdate(){
		
		//return recoil
		transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDamVelocity, recoilMoveReturnSpeed);
		recoilAngle = Mathf.SmoothDamp (recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotReturnSpeed);
		transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

		if (!isReloading && projectilesRemainingInMag == 0) {
			Debug.Log (projectilesRemainingInMag);
			Reload ();
		}
	}



	void Shoot(){

		if(Time.time > nextShotTime && projectilesRemainingInMag > 0 && !isReloading){



			if(fireMode == FireMode.Burst){

				if (shotsRemainingInBurst == 0) {
					return;
				}
				shotsRemainingInBurst--;


			
			}
			else if(fireMode == FireMode.Single){
				if(!triggerReleased){
					return;
				}
			}

			//Shotgun 
			for (int i = 0; i < muzzle.Length; i++){
				if (projectilesRemainingInMag == 0) {
					break;
				}
				projectilesRemainingInMag--;
				nextShotTime = Time.time + msBetweenShots / 1000;
				Projectile newProjectile = Instantiate (projectile, muzzle [i].position, muzzle [i].rotation) as Projectile;
				newProjectile.setSpeed (muzzleVelocity);

			}

			transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
			recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
			recoilAngle = Mathf.Clamp (recoilAngle, 0, 30);
			muzzleflash.Activate ();
			Instantiate(shell, shellEjection.position, shellEjection.rotation);
			
			AudioManager.instance.PlaySouns (shootAudio, transform.position);
		}
	}


	public void Reload(){
		if (!isReloading && projectilesRemainingInMag != projectilesPerMag) 
			StartCoroutine (AnimateReload());
	}
		 
	IEnumerator AnimateReload(){
		isReloading = true;
		yield return new WaitForSeconds (0.2f);

		float reloadSpeed = 1 / reloadTime;
		float percent = 0;
		Vector3 initialRot = transform.localEulerAngles;
		float maxReloadAngle = 30;

		while (percent < 1) {
			percent += Time.deltaTime * reloadSpeed;
			float interpolation = (-Mathf.Pow (percent, 2) + percent) * 4;
			float reloadAngle = Mathf.Lerp (0, maxReloadAngle, interpolation);
			transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;

			yield return null;
		}

		isReloading = false;
		projectilesRemainingInMag = projectilesPerMag;
	}

	public void Aim(Vector3 aimPoint){
		if(!isReloading)
			transform.LookAt (aimPoint);
	}



	public void OnTriggerHold(){
		Shoot ();
		triggerReleased = false;
	}

	public void OnTriggerRelease(){
		triggerReleased = true;
		shotsRemainingInBurst = burstCount;

	}


}

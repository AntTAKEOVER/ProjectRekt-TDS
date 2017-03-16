using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

	public enum FireMode{Auto, Burst, Single};
	public FireMode fireMode;

	MuzzleFlash muzzleflash;
	public Transform[] muzzle;
	public Projectile projectile;
	public float msBetweenShots = 100;
	public float muzzleVelocity = 35;
	public int burstCount;
	int shotsRemainingInBurst;

	public Transform shell;
	public Transform shellEjection;
	public AudioClip shootAudio;
	public AudioClip reloadAuido;
	float nextShotTime;
	bool isBurstTriggered;

	bool triggerReleased;

	void Start(){
		shotsRemainingInBurst = burstCount;
		muzzleflash = GetComponent<MuzzleFlash> ();
	}



	void Shoot(){

		if(Time.time > nextShotTime){

			if(fireMode == FireMode.Burst){
				isBurstTriggered = true;
				if(shotsRemainingInBurst >= 0){
					isBurstTriggered = false;
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
				nextShotTime = Time.time + msBetweenShots / 1000;
				Projectile newProjectile = Instantiate(projectile, muzzle[i].position, muzzle[i].rotation) as Projectile;
				muzzleflash.Activate();
				newProjectile.setSpeed(muzzleVelocity);
			}

			Instantiate(shell, shellEjection.position, shellEjection.rotation);
			
			AudioManager.instance.PlaySouns (shootAudio, transform.position);
		}
	}

	public void Aim(Vector3 aimPoint){
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

	public void Update(){
		if (isBurstTriggered) {
			Shoot ();

		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	float masterVolumerPercent = 1;
	float sfxVolumePercent = 1;
	float musicVolumePercent = 1;

	AudioSource[] musicSources;
	int activeMusicSource;

	public static AudioManager instance;

	Transform audioListener;
	Transform playerT;

	void Awake(){

		instance = this;

		musicSources = new AudioSource[2];

		for (int i = 0; i < 2; i++) {

			GameObject newMusicSOurce = new GameObject ("Music Source " + (i+1));
			musicSources[i] = newMusicSOurce.AddComponent<AudioSource> ();
			newMusicSOurce.transform.parent = transform;


		}

		audioListener = FindObjectOfType<AudioListener> ().transform;
		playerT = FindObjectOfType<PlayerController> ().transform;
	}

	void Update(){
		if (playerT != null) {
			audioListener.position = playerT.position;
		}
	}

	public void PlayMusic(AudioClip clip, float fadeDuration = 1){
		activeMusicSource = 1 - activeMusicSource;
		musicSources [activeMusicSource].clip = clip;
		musicSources [activeMusicSource].Play ();

		StartCoroutine (AnimateMusicCrossFade (fadeDuration));

	}

	public void PlaySouns(AudioClip clip, Vector3 pos){
		if (clip != null) {
			AudioSource.PlayClipAtPoint (clip, pos, sfxVolumePercent * masterVolumerPercent);
		}
	
	}

	IEnumerator AnimateMusicCrossFade(float duration){
		float percent = 0;

		while (percent < 1) {
			percent += Time.deltaTime * 1 / duration;
			musicSources [activeMusicSource].volume = Mathf.Lerp (0, musicVolumePercent * masterVolumerPercent, percent);
			musicSources [1-activeMusicSource].volume = Mathf.Lerp (musicVolumePercent * masterVolumerPercent, 0, percent);
			yield return null;
		}
	}
}

﻿using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {
	
	public Wave[] waves;
	public Enemy enemy;
	
	LivingEntity playerEntity;
	Transform playerT;
	public bool devMode;
	
	Wave currentWave;
	int currentWaveNumber;
	
	int enemiesRemainingToSpawn;
	int enemiesRemainingAlive;
	float nextSpawnTime;
	
	MapGenerator map;
	
	float timeBetweenCampingChecks = 4;
	float campThresholdDistance = 1.5f;
	float nextCampCheckTime;
	Vector3 campPositionOld;
	bool isCamping;
	
	bool isDisabled;

	public event System.Action<int> OnNewWave;
	
	void Start() {
		playerEntity = FindObjectOfType<PlayerInput> ();
		playerT = playerEntity.transform;
		
		nextCampCheckTime = timeBetweenCampingChecks + Time.time;
		campPositionOld = playerT.position;
		playerEntity.OnDeath += OnPlayerDeath;
		
		map = FindObjectOfType<MapGenerator> ();
		NextWave ();
	}
	
	void Update() {


		if (!isDisabled) {

			if (Time.time > nextCampCheckTime) {
				nextCampCheckTime = Time.time + timeBetweenCampingChecks;
				
				isCamping = (Vector3.Distance (playerT.position, campPositionOld) < campThresholdDistance);
				campPositionOld = playerT.position;
			}
			
			if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime) {
				enemiesRemainingToSpawn--;
				nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
				
				StartCoroutine ("SpawnEnemy");
			}
		}

		if (devMode) {
			if(Input.GetKeyDown(KeyCode.Return)){
				StopCoroutine("SpawnEnemy");
				foreach(Enemy enemy in FindObjectsOfType<Enemy>()){
					GameObject.Destroy(enemy.gameObject);
				}
				NextWave();
			}
		}
	}
	
	IEnumerator SpawnEnemy() {
		float spawnDelay = 1;
		float tileFlashSpeed = 4;
		
		Transform spawnTile = map.GetRandomOpenCoord ();
		if (isCamping) {
			spawnTile = map.GetTileFromPosition(playerT.position);
		}
		Material tileMat = spawnTile.GetComponent<Renderer> ().material;
		Color initialColour = tileMat.color;
		Color flashColour = Color.red;
		float spawnTimer = 0;
		
		while (spawnTimer < spawnDelay) {
			
			tileMat.color = Color.Lerp(initialColour,flashColour, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
			
			spawnTimer += Time.deltaTime;
			yield return null;
		}

		spawnTile.GetComponent<Renderer> ().material.color = Color.white;
		Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
		spawnedEnemy.OnDeath += OnEnemyDeath;
		spawnedEnemy.SetCharacteristics (currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor);

	}
	
	void OnPlayerDeath() {
		isDisabled = true;
	}
	
	void OnEnemyDeath() {
		enemiesRemainingAlive --;
		
		if (enemiesRemainingAlive == 0) {
			NextWave();
		}
	}

	void resetPlayerPosition(){
		playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
	}
	
	void NextWave() {
		currentWaveNumber ++;

		if (currentWaveNumber - 1 < waves.Length) {
			currentWave = waves [currentWaveNumber - 1];
			
			enemiesRemainingToSpawn = currentWave.enemyCount;
			enemiesRemainingAlive = enemiesRemainingToSpawn;

			if(OnNewWave != null){
				OnNewWave(currentWaveNumber);
			}
			resetPlayerPosition();
		}
	}
	
	[System.Serializable]
	public class Wave {
		public bool infinite;
		public int enemyCount;
		public float timeBetweenSpawns;
		public float moveSpeed;
		public int hitsToKillPlayer;
		public float enemyHealth;
		public Color skinColor;


	}
	
}
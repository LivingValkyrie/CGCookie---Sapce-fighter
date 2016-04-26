using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Author: Matt Gipson
/// Contact: Deadwynn@gmail.com
/// Domain: www.livingvalkyrie.net
/// 
/// Description: GameManager 
/// </summary>
public class GameManager : MonoBehaviour {
	#region Fields

	public GameObject spaceShipPrefab;
	public GameObject startingRockPrefab;
	public GameObject saucerPrefab;

	public int playerLives = 3;
	public int score = 0;
	public int numStartingRocks = 4;
	public float saucerSpawnRate = 10;

	GameObject player;
	int rockSpawnRadius = 4;
	Vector3 screenSW;
	Vector3 screenNE;
	Vector3 screenSE;
	Vector3 screenNW;

	#endregion

	void Start() {
		player = Instantiate(spaceShipPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		player.GetComponent<SpaceShip>().SetGameManager(gameObject);

		screenSW = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.transform.localPosition.z));
		screenNE = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.localPosition.z));
		screenSE = new Vector3(screenSW.x, screenNE.y, 0);
		screenNW = new Vector3(screenNE.x, screenSW.y, 0); //pretty sure these are backwards yo. x is EW, y is NS. just saying.

		for (int i = 0; i < numStartingRocks; i++) {
			float spawnX = rockSpawnRadius * Mathf.Cos(Random.Range(0f, 260f));
			float spawnY = rockSpawnRadius * Mathf.Sin(Random.Range(0f, 260f));

			GameObject rockClone = Instantiate(startingRockPrefab, new Vector3(spawnX, spawnY, 0), Quaternion.identity) as GameObject;
			rockClone.GetComponent<Rock>().SetGameManager(gameObject);
		}

		StartCoroutine(SpawnSaucer());
	}

	void Update() {
		float translation = Input.GetAxis("Vertical");
		float rotation = Input.GetAxis("Horizontal");

		if (rotation > 0) {
			player.GetComponent<SpaceShip>().TurnRight(rotation);
		} else if (rotation < 0) {
			player.GetComponent<SpaceShip>().TurnLeft(rotation);
		}

		if (translation >= 0.5) {
			player.GetComponent<SpaceShip>().Move(translation);
		} else {
			player.GetComponent<SpaceShip>().Idle();
		}

		if (Input.GetButton("Jump")) {
			player.GetComponent<SpaceShip>().ShootBullet();
		}

		GameObject[] rocks = GameObject.FindGameObjectsWithTag("Rock");
		if (rocks.Length <= 0) {
			for (int i = 0; i < numStartingRocks; i++) {
				float spawnX = rockSpawnRadius * Mathf.Cos(Random.Range(0f, 260f));
				float spawnY = rockSpawnRadius * Mathf.Sin(Random.Range(0f, 260f));

				GameObject rockClone = Instantiate(startingRockPrefab, new Vector3(spawnX, spawnY, 0), Quaternion.identity) as GameObject;
				rockClone.GetComponent<Rock>().SetGameManager(gameObject);
			}
		}
	}

	public void ResetShip() {
		player.transform.localPosition = Vector3.zero;
	}

	public void UpdateScore(int scoreToAdd) {
		score += scoreToAdd;
	}

	public void UpdateLives(int livesLost = 1) {
		playerLives -= livesLost;
	}

	IEnumerator SpawnSaucer() {
		for (float timer = saucerSpawnRate; timer >= 0; timer -= Time.deltaTime) {
			yield return null;
		}

		int corner = Random.Range(0, 4);
		Vector3 spawnPos = Vector3.zero;

		if (corner == 0) {
			spawnPos = screenSW; //there is only so much of his code i can copy before i snap. or just completely refactor.
		} else if (corner == 1) {
			spawnPos = screenSE;
		} else if (corner == 2) {
			spawnPos = screenNE;
		} else if (corner == 3) {
			spawnPos = screenNW;
		}

		GameObject saucerClone = Instantiate(saucerPrefab, spawnPos, Quaternion.identity) as GameObject;
		saucerClone.GetComponent<Saucer>().SetGameManager(gameObject);

		if ( corner == 0 ) {
			saucerClone.transform.Rotate(Vector3.back * Random.Range(0, 90));
		} else if ( corner == 1 ) {
			saucerClone.transform.Rotate( Vector3.back * Random.Range( 90, 180 ) );
		} else if ( corner == 2 ) {
			saucerClone.transform.Rotate( Vector3.back * Random.Range( 180, 270 ) );
		} else if ( corner == 3 ) {
			saucerClone.transform.Rotate( Vector3.back * Random.Range( 270, 360 ) );
		}

		StartCoroutine(SpawnSaucer());
	}
}
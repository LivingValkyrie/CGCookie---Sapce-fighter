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

	public int playerLives = 3;
	public int score = 0;
	public int numStartingRocks = 4;

	GameObject player;
	int rockSpawnRadius = 4;

	#endregion

	void Start() {
		player = Instantiate(spaceShipPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		player.GetComponent<SpaceShip>().SetGameManager(gameObject);

		for ( int i = 0; i < numStartingRocks; i++ ) {
			float spawnX = rockSpawnRadius * Mathf.Cos( Random.Range( 0f, 260f ) );
			float spawnY = rockSpawnRadius * Mathf.Sin( Random.Range( 0f, 260f ) );

			GameObject rockClone = Instantiate( startingRockPrefab, new Vector3( spawnX, spawnY, 0 ), Quaternion.identity ) as GameObject;
			rockClone.GetComponent<Rock>().SetGameManager( gameObject );
		}
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
			for ( int i = 0; i < numStartingRocks; i++ ) {
				float spawnX = rockSpawnRadius * Mathf.Cos( Random.Range( 0f, 260f ) );
				float spawnY = rockSpawnRadius * Mathf.Sin( Random.Range( 0f, 260f ) );

				GameObject rockClone = Instantiate( startingRockPrefab, new Vector3(spawnX, spawnY, 0), Quaternion.identity ) as GameObject;
				rockClone.GetComponent<Rock>().SetGameManager( gameObject );
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
}
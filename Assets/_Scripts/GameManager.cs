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

	public int playerLives = 3;
	public int score = 0;

	GameObject player;

	#endregion

	void Start() {
		player = Instantiate(spaceShipPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		player.GetComponent<SpaceShip>().SetGameManager(gameObject);
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
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
	public GameObject gameUI;
	public GameObject scoreText;
	public GameObject livesText;
	public GameObject mainUI;
	public GameObject gameOverUI;
	public GameObject finalScore;

	public enum GameState {
		Main,
		Game,
		GameOver
	}

	public GameState state;

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
	int startingScore = 0;
	int startinglives = 3;

	#endregion

	void Start() {
		mainUI.SetActive(false);
		gameUI.SetActive(false);
		gameOverUI.SetActive(false);

		switch (state) {
			case GameState.Main:
				mainUI.SetActive(true);
				break;
			case GameState.Game:
				gameUI.SetActive(true);
				break;
			case GameState.GameOver:
				gameOverUI.SetActive(true);
				break;
		}

		score = startingScore;
		playerLives = startinglives;
		UpdateScore(0);
		UpdateLives(0);
	}

	void Update() {
		switch (state) {
			case GameState.Main:
				if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
					StartCoroutine(GameStart());
				}
				break;
			case GameState.Game:
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

				if (Input.GetButton("Fire1")) {
					player.GetComponent<SpaceShip>().Warp();
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
				break;
			case GameState.GameOver:
				if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
					GameObject[] rocksToDestroy = GameObject.FindGameObjectsWithTag("Rock");
					for (int i = 0; i < rocksToDestroy.Length; i++) {
						Destroy(rocksToDestroy[i]);
					}

					StartCoroutine(GameStart());
				}
				break;
		}
	}

	public void ResetShip() {
		player.transform.localPosition = Vector3.zero;
	}

	public void UpdateScore(int scoreToAdd) {
		score += scoreToAdd;
		scoreText.GetComponent<GUIText>().text = "Score " + score;
	}

	public void UpdateLives(int livesLost = 1) {
		playerLives -= livesLost;
		livesText.GetComponent<GUIText>().text = "Lives " + playerLives;

		if (playerLives < 1) {
			StartCoroutine(GameEnd());
		}
	}

	IEnumerator GameEnd() {
		mainUI.SetActive(false);
		gameOverUI.SetActive(true);
		gameUI.SetActive(false);
		state = GameState.GameOver;

		finalScore.GetComponent<GUIText>().text = "Final Score: " + score;

		Destroy(player);
		StopAllCoroutines();

		score = startingScore;
		playerLives = startinglives;

		yield return null;
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

		if (corner == 0) {
			saucerClone.transform.Rotate(Vector3.back * Random.Range(0, 90));
		} else if (corner == 1) {
			saucerClone.transform.Rotate(Vector3.back * Random.Range(90, 180));
		} else if (corner == 2) {
			saucerClone.transform.Rotate(Vector3.back * Random.Range(180, 270));
		} else if (corner == 3) {
			saucerClone.transform.Rotate(Vector3.back * Random.Range(270, 360));
		}

		StartCoroutine(SpawnSaucer());
	}

	/// <summary>
	/// no reason to be a coroutine
	/// </summary>
	/// <returns></returns>
	IEnumerator GameStart() {
		mainUI.SetActive( false );
		gameOverUI.SetActive( false );
		gameUI.SetActive( true );
		state = GameState.Game;

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

		//didnt like that things werent updating in his
		score = startingScore;
		playerLives = startinglives;
		UpdateScore( 0 );
		UpdateLives( 0 );

		yield return null;
	}
}
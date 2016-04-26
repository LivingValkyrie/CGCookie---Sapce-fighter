using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Author: Matt Gipson
/// Contact: Deadwynn@gmail.com
/// Domain: www.livingvalkyrie.net
/// 
/// Description: Rock 
/// </summary>
public class Rock : MonoBehaviour {
	#region Fields

	public GameObject childRockPrefab;
	public int numChildRocks = 1;
	public float speed = 1f;
	public int score;

	GameObject gameManager;
	Rigidbody2D rb2D;
	Vector3 screenSW;
	Vector3 screenNE;
	float wrapPadding = 1f;

	#endregion

	void Start() {
		rb2D = GetComponent<Rigidbody2D>();

		screenSW = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.transform.localPosition.z));
		screenNE = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.localPosition.z));

		transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
	}

	void Update() {
		rb2D.AddForce(transform.up * speed);

		Wrap();
	}

	void Wrap() {
		if (transform.localPosition.x < screenSW.x - wrapPadding) {
			transform.localPosition = new Vector3(screenNE.x, transform.localPosition.y, transform.localPosition.z);
		} else if (transform.localPosition.x > screenNE.x + wrapPadding) {
			transform.localPosition = new Vector3(screenSW.x, transform.localPosition.y, transform.localPosition.z);
		}

		if (transform.localPosition.y < screenSW.y - wrapPadding) {
			transform.localPosition = new Vector3(transform.localPosition.x, screenNE.y, transform.localPosition.z);
		} else if (transform.localPosition.y > screenNE.y + wrapPadding) {
			transform.localPosition = new Vector3(transform.localPosition.x, screenSW.y, transform.localPosition.z);
		}
	}

	public void SetGameManager(GameObject gameManagerObject) {
		gameManager = gameManagerObject;
	}

	public void RockHit() {
		StartCoroutine(DestroyRock());
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			StartCoroutine(DestroyRock());
		}
	}

	IEnumerator DestroyRock() {
		if (childRockPrefab != null) {
			for (int i = 0; i < numChildRocks; i++) {
				GameObject rockClone = Instantiate(childRockPrefab, transform.localPosition, Quaternion.identity) as GameObject;
				rockClone.GetComponent<Rock>().SetGameManager(gameManager);
			}
		}

		gameManager.GetComponent<GameManager>().UpdateScore(score);
		yield return new WaitForSeconds(0.2f);
		Destroy(gameObject);
	}
}
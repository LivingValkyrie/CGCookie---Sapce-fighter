using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Author: Matt Gipson
/// Contact: Deadwynn@gmail.com
/// Domain: www.livingvalkyrie.net
/// 
/// Description: Saucer 
/// </summary>
public class Saucer : MonoBehaviour {
	#region Fields

	public GameObject saucerBulletPrefab;
	public AudioClip bulletSFX;
	public AudioClip hitSFX;

	public float speed = 1f;
	public float maxFireWaitTime = 5f;
	public int score;

	Animator anim;
	Rigidbody2D rb2D;
	GameObject gameManager;
	Vector3 screenSW;
	Vector3 screenNE;
	float destroyPadding = 1f;
	AudioSource audioSource;

	#endregion

	void Start() {
		anim = GetComponent<Animator>();
		rb2D = GetComponent<Rigidbody2D>();
		audioSource = GetComponent<AudioSource>();

		screenSW = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.transform.localPosition.z));
		screenNE = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.localPosition.z));

		StartCoroutine(Attack());

		anim.SetInteger("State", 0);
	}

	void Update() {
		rb2D.AddForce(transform.up * speed);

		if (transform.localPosition.x < screenSW.x - destroyPadding ||
		    transform.localPosition.x > screenNE.x + destroyPadding ||
		    transform.localPosition.y < screenSW.y - destroyPadding ||
		    transform.localPosition.y > screenNE.y + destroyPadding) {
			Destroy(gameObject);
		}
	}

	public void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			StartCoroutine(Hit());
		}
	}

	public void SetGameManager(GameObject gameManagerObject) {
		gameManager = gameManagerObject;
	}

	public void SaucerHit() {
		StartCoroutine(Hit());
	}

	IEnumerator Hit() {
		audioSource.PlayOneShot( hitSFX );
		anim.SetInteger("State", 0);
		gameManager.GetComponent<GameManager>().UpdateScore(score);
		yield return new WaitForSeconds(0.3f);

		Destroy(gameObject);
	}

	IEnumerator Attack() {
		for (float timer = Random.Range(0, maxFireWaitTime); timer >= 0; timer -= Time.deltaTime) {
			yield return null;
		}

		Instantiate(saucerBulletPrefab, transform.localPosition, transform.localRotation);
		audioSource.PlayOneShot(bulletSFX);
	}

}
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// Author: Matt Gipson
/// Contact: Deadwynn@gmail.com
/// Domain: www.livingvalkyrie.net
/// 
/// Description: SpaceShip 
/// </summary>
public class SpaceShip : MonoBehaviour {
	#region Fields

	public GameObject bulletPrefab;

	public float speed = 1f;
	public float turnSpeed = 1f;
	public float fireRate = 0.5f;
	public float respawnRate = 1f;

	float accelRate = 0f;
	Animator anim;
	Rigidbody2D rb2D;
	Vector3 screenSW;
	Vector3 screenNE;
	float wrapPadding = 1f;
	bool hit = false;
	float nextFire;
	GameObject gameManager;

	#endregion

	void Start() {
		anim = GetComponent<Animator>();
		rb2D = GetComponent<Rigidbody2D>();

		screenSW = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.transform.localPosition.z));
		screenNE = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.localPosition.z));
	}

	void Update() {
		rb2D.AddForce(transform.up * (speed * accelRate));

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

	public void SpaceShipHit() {
		StartCoroutine(Hit());
	}

	public void OnTriggerEnter2D(Collider2D other) {
		if (hit) { //...really
			return;
		}

		if (other.tag == "Rock" || other.tag == "Saucer") {
			StartCoroutine(Hit());
		}
	}

	IEnumerator Hit() {
		hit = true;
		accelRate = 0;
		anim.SetInteger("State", 4);
		gameManager.GetComponent<GameManager>().UpdateLives(1);
		yield return new WaitForSeconds(0.1f);

		GetComponent<Renderer>().enabled = false;
		GetComponent<Collider2D>().enabled = false;
		yield return new WaitForSeconds(respawnRate);

		GetComponent<Renderer>().enabled = true;
		GetComponent<Collider2D>().enabled = true;
		hit = false;
		gameManager.GetComponent<GameManager>().ResetShip();
	}

	public void ShootBullet() {
		if (hit) { //im so glad this is my last cgcookie
			return;
		}

		if (Time.time > nextFire) {
			nextFire = Time.time + fireRate;

			Instantiate(bulletPrefab, transform.localPosition, transform.localRotation);
		}
	}

	public void Idle() {
		if (hit) { //grrr
			return;
		}

		accelRate = accelRate * 0.5f;

		anim.SetInteger("State", 0);
	}

	public void Move(float accel) {
		if (hit) { //SO FUCKING STUPID. JUST CHECK FOR HIT INSIDE OF THE DAMN UPDATE. 
			return;
		}

		accelRate = accel;

		anim.SetInteger("State", 1);
	}

	public void TurnRight(float rotation = 1) {
		if (hit) {
			return;
		}

		transform.Rotate(Vector3.back * (rotation * turnSpeed));
	}

	/// <summary>
	/// Useless method designed by somebody who doesnt know what they are doing and should not be creating instructional videos.
	/// </summary>
	/// <param name="rotation">The rotation.</param>
	public void TurnLeft(float rotation = 1) {
		if (hit) {
			return;
		}

		transform.Rotate(Vector3.forward * (rotation * -turnSpeed));
	}
}
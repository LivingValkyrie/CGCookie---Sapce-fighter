﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Author: Matt Gipson
/// Contact: Deadwynn@gmail.com
/// Domain: www.livingvalkyrie.net
/// 
/// Description: Bullet 
/// </summary>
public class Bullet : MonoBehaviour {
	#region Fields

	public float speed = 1f;
	public BulletType type;

	Rigidbody2D rb2D;
	Vector3 screenSW;
	Vector3 screenNE;
	float destroyPadding = 1f;

	#endregion

	void Start() {
		rb2D = GetComponent<Rigidbody2D>();

		screenSW = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.transform.localPosition.z));
		screenNE = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.localPosition.z));
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

	void OnTriggerEnter2D(Collider2D other) {
		switch (type) {
			case BulletType.SpaceShip:
				if (other.tag == "Rock") {
					other.GetComponent<Rock>().RockHit();
					Destroy(gameObject);
				} else if (other.tag == "Saucer") {
					other.GetComponent<Saucer>().SaucerHit();
					Destroy(gameObject);
				}
				break;
			case BulletType.Saucer:
				if (other.tag == "Player") {
					other.GetComponent<SpaceShip>().SpaceShipHit();
					Destroy(gameObject);
				}
				break;
		}
	}
}

public enum BulletType {
	SpaceShip,
	Saucer
}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public float maxAccel;
	public float jumpHeight;
	public float blowback;
	public float maxSpeed;
	public float freezingDuration;

	public GameObject bullet;
	public GameObject shotgunBullet;
	public GameObject deathEffects;
	public GameObject gameManager;
	public GameObject berserkParticles;
	public GameObject freezingParticles;

	public Slider rifleSlider;
	public Slider shotgunSlider;
	public Slider jumpSlider;
	public Slider healthbar;
	public Slider berserkSlider;
	public Text announcement;

	public float rifleCD;
	public float shotgunCD;
	public float jumpCD;
	public float berserkDuration;
	public float maxHealth;

	float currAccel;
	float currMaxSpeed;

	float x;
	float z;
	float health;

	public bool gotRifle;
	public bool gotShotgun;
	public bool gotJump;
	public bool gotBerserk;


	Animator anim;


	Ray ray;
	RaycastHit hit;
	Vector3 playerToMouse;
	int floorMask;

	Rigidbody rb;


	bool isJumping;
	bool isBerserk;
	bool isFreezing;

	float lastRifle;
	float lastShotgun;
	float lastJump;
	float lastBerserk;

	float lastFrozen;
	IEnumerator announcementCoroutune;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		floorMask = LayerMask.GetMask ("Floor");
		rb = GetComponent<Rigidbody> ();

		isJumping = false;
		lastRifle = 0f;
		lastShotgun = 0f;
		lastJump = 0f;
		health = maxHealth;
		currAccel = maxAccel;
		currMaxSpeed = maxSpeed;

		announcementCoroutune = HandlePickUps ();

		UnlockItem ();
	}
	
	// Update is called once per frame
	void Update () {
		LookAtMouse ();
		float time = Time.time;

		HandleInputs (time);
		UpdateUI (time);
		if (time - lastBerserk > berserkDuration) {
			isBerserk = false;
			berserkParticles.SetActive (false);
		}

		if (isFreezing && Time.time - lastFrozen > freezingDuration) {
			currAccel = maxAccel;
			currMaxSpeed = maxSpeed;
			isFreezing = false;
			freezingParticles.SetActive (false);
		}
	}

	void FixedUpdate()
	{
		Move ();
		Jump ();
	}

	void LookAtMouse()
	{
		ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		Physics.Raycast (ray, out hit, 50f, floorMask);
		playerToMouse = hit.point;
		playerToMouse.y = transform.position.y;
		transform.LookAt (playerToMouse);
	}

	void Move()
	{
		x = Input.GetAxisRaw ("Horizontal");
		z = Input.GetAxisRaw ("Vertical");

		if (x != 0 || z != 0) {
			anim.SetBool ("isMoving", true);
		} else {
			anim.SetBool ("isMoving", false);
		}
		//gameObject.transform.Translate (new Vector3 (x, 0f, z) * speed * Time.deltaTime, Space.World);
		if (!isJumping)
			{
				rb.AddForce(new Vector3(x,0f,z) * currAccel);
				if (Mathf.Sqrt((rb.velocity.x * rb.velocity.x) + (rb.velocity.z * rb.velocity.z))  > currMaxSpeed) 
				{
					rb.velocity = rb.velocity.normalized * currMaxSpeed;
				}
			}
	}

	void Jump()
	{
		if (Input.GetKey ("space") && !isJumping && Time.time - lastJump > jumpCD && gotJump) {
			rb.AddForce(new Vector3 (0f, jumpHeight, 0f));
			isJumping = true;
			lastJump = Time.time;
		}
	}

	void Shotgun()
	{
		rb.AddRelativeForce (new Vector3 (0f, 0f, -blowback));
		shotgunBullet.transform.rotation = transform.rotation;
		shotgunBullet.transform.position = transform.position;
		shotgunBullet.transform.LookAt (playerToMouse);
		shotgunBullet.transform.Translate (new Vector3 (0f, 2f, 2f));
		Instantiate (shotgunBullet);
		shotgunBullet.transform.eulerAngles = new Vector3 (0, shotgunBullet.transform.eulerAngles.y + 5f, 0);
		Instantiate (shotgunBullet);
		shotgunBullet.transform.eulerAngles = new Vector3 (0, shotgunBullet.transform.eulerAngles.y + 5f, 0);
		Instantiate (shotgunBullet);
		shotgunBullet.transform.eulerAngles = new Vector3 (0, shotgunBullet.transform.eulerAngles.y - 15f, 0);
		Instantiate (shotgunBullet);
		shotgunBullet.transform.eulerAngles = new Vector3 (0, shotgunBullet.transform.eulerAngles.y - 5f, 0);
		Instantiate (shotgunBullet);
		shotgunBullet.transform.eulerAngles = new Vector3 (0, shotgunBullet.transform.eulerAngles.y - 5f, 0);
		Instantiate (shotgunBullet);
		lastShotgun = Time.time;
	}

	void Rifle()
	{
		bullet.transform.rotation = transform.rotation;
		bullet.transform.position = transform.position;
		bullet.transform.LookAt (playerToMouse);
		bullet.transform.Translate (new Vector3 (0f, 2f, 2f));
		Instantiate (bullet);
		lastRifle = Time.time;
	}
		

	void OnCollisionEnter(Collision col)
	{
if (col.gameObject.CompareTag ("Floor")) {
			isJumping = false;
		} 
	}

	void OnCollisionStay(Collision col)
	{
		if (isBerserk && (col.gameObject.CompareTag("Zombie") ||col.gameObject.CompareTag("Big Zombie")) || col.gameObject.CompareTag("Freezing Zombie")) {
			col.gameObject.SendMessage ("TakeDamage");
		}

		if (col.gameObject.CompareTag ("Zombie")) {
			health -= 0.1f;
			if (health <= 0) {
				gameObject.SetActive (false);
				gameManager.SendMessage ("Death");
				deathEffects.transform.position = transform.position;
				deathEffects.SetActive (true);
			}
		} else if (col.gameObject.CompareTag ("Big Zombie")) {
			health -= 0.3f;
			if (health <= 0) {
				gameObject.SetActive (false);
				gameManager.SendMessage ("Death");
				deathEffects.transform.position = transform.position;
				deathEffects.SetActive (true);
			}
		} else if (col.gameObject.CompareTag ("Freezing Zombie")) {
			health -= 0.2f;
			if (health <= 0) {
				gameObject.SetActive (false);
				gameManager.SendMessage ("Death");
				deathEffects.transform.position = transform.position;
				deathEffects.SetActive (true);
			}
			isFreezing = true;
			lastFrozen = Time.time;
			currMaxSpeed = maxSpeed / 2;
			currAccel = maxAccel / 2;
			freezingParticles.SetActive (true);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ("Crate")) {
			Destroy (other.gameObject);
			health += 20;
			if (health > maxHealth) {
				health = maxHealth;
			}
		}

	}

	void PlayerShot()
	{
		health -= 10;
		if (health <= 0) {
			gameObject.SetActive(false);
			gameManager.SendMessage ("Death");
			deathEffects.transform.position = transform.position;
			deathEffects.SetActive (true);
		}
	}


	void HandleInputs(float time)
	{
		if (Input.GetMouseButton (0)) {
			if (time - lastRifle > rifleCD) {
				Rifle ();
			}
		}
		if (Input.GetMouseButton (1) && gotShotgun) {
			if (time - lastShotgun > shotgunCD) {
				Shotgun ();
			}
		}
		if (Input.GetKeyDown("q") && gotBerserk){
			if (!isBerserk) {
				lastBerserk = time;
				isBerserk = true;
				health = health * 0.75f;
				berserkParticles.SetActive (true);
			}
		}

	}
	void UpdateUI(float time)
	{
		float shotgunCDpercent = (time - lastShotgun) / shotgunCD;
		if (shotgunCDpercent > 1) {
			shotgunCDpercent = 1f;
		}
		shotgunSlider.value = shotgunCDpercent * 100;

		float rifleCDpercent = (time - lastRifle) / rifleCD;
		if (rifleCDpercent > 1) {
			rifleCDpercent = 1f;
		}
		rifleSlider.value = rifleCDpercent * 100;

		float jumpCDpercent = (time - lastJump) / jumpCD;
		if (jumpCDpercent > 1) {
			jumpCDpercent = 1f;
		}
		jumpSlider.value = jumpCDpercent * 100;

		float berserkCDpercent = (time - lastBerserk) / berserkDuration;
		if (berserkCDpercent > 1) {
			berserkCDpercent = 1f;
		}
		berserkSlider.value = berserkCDpercent * 100;
		healthbar.value = (health/maxHealth) * 100;
	}
		
	IEnumerator HandlePickUps()
	{
		if (!gotRifle) {
			gotRifle = true;
			announcement.text = "Unlocked Rifle \n Hold left mouse button to fire rifle";

		} else if (!gotShotgun) {
			gotShotgun = true;
			announcement.text = "Unlocked Shotgun \n Press right mouse button to fire shotgun";
			announcement.gameObject.SetActive (true);
			shotgunSlider.gameObject.SetActive (true);
		} else if (!gotJump) {
			gotJump = true;
			announcement.text = "Unlocked Boots \n Press Spacebar to jump";
			announcement.gameObject.SetActive (true);
			jumpSlider.gameObject.SetActive (true);
		} else if (!gotBerserk) {
			gotBerserk = true;
			announcement.text = "Unlocked Berserk Mode \n press q to activate and deal damage to nearby enemies \n Warning: berserk mode costs health";
			announcement.gameObject.SetActive (true);
			berserkSlider.gameObject.SetActive (true);
		} else {
			float rng = Random.value;
			if (rng < 0.25f) {
				announcement.text = "Upgraded Rifle \n Rifle fires faster";
				rifleCD = rifleCD * 0.8f;

			} else if (rng < 0.50f) {
				announcement.text = "Upgraded Shotgun \n Shotgun reloads faster";
				shotgunCD = shotgunCD * 0.8f;

			} else if (rng < 0.65f) {
				announcement.text = "Upgraded Berserk \n Berserk lasts longer";
				berserkDuration += 0.5f;
			} else if (rng < 0.8f) {
				announcement.text = "Upgraded Jump \n Jump more often";
				jumpCD = jumpCD * 0.8f;
			} else {
				announcement.text = "Upgraded Max Health";
				maxHealth += 20;
			}
		}
		announcement.gameObject.SetActive (true);
		yield return new WaitForSecondsRealtime (5f);
		announcement.gameObject.SetActive (false);
		StopCoroutine (announcementCoroutune);
		announcementCoroutune = HandlePickUps ();
	}

	void UnlockItem ()
	{
		StartCoroutine (announcementCoroutune);
	}
}

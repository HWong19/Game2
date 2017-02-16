using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public float speed;
	public float jumpHeight;
	public float blowback;
	public float maxSpeed;


	public GameObject bullet;
	public GameObject deathEffects;
	public GameObject gameManager;

	public Slider rifleSlider;
	public Slider shotgunSlider;
	public Slider jumpSlider;
	public Slider healthbar;
	public Text announcement;

	public float rifleCD;
	public float shotgunCD;
	public float jumpCD;

	float x;
	float z;
	float health;

	bool gotRifle;
	bool gotShotgun;
	bool gotJump;


	Animator anim;


	Ray ray;
	RaycastHit hit;
	Vector3 playerToMouse;
	int floorMask;

	Rigidbody rb;


	bool isJumping;

	float lastRifle;
	float lastShotgun;
	float lastJump;

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
		health = 100f;

		gotRifle = false;
		gotShotgun = false;
		gotJump = false;

		announcementCoroutune = HandlePickUps ();

		UnlockItem ();
	}
	
	// Update is called once per frame
	void Update () {
		LookAtMouse ();
		float time = Time.time;

		HandleMouseInputs (time);
		UpdateUI (time);

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
				rb.AddForce(new Vector3(x,0f,z) * speed);
				if (Mathf.Sqrt((rb.velocity.x * rb.velocity.x) + (rb.velocity.z * rb.velocity.z))  > maxSpeed) 
				{
					rb.velocity = rb.velocity.normalized * maxSpeed;
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
		bullet.transform.rotation = transform.rotation;
		bullet.transform.position = transform.position;
		bullet.transform.LookAt (playerToMouse);
		bullet.transform.Translate (new Vector3 (0f, 1f, 2f));
		Instantiate (bullet);
		bullet.transform.eulerAngles = new Vector3 (0, bullet.transform.eulerAngles.y + 5f, 0);
		Instantiate (bullet);
		bullet.transform.eulerAngles = new Vector3 (0, bullet.transform.eulerAngles.y + 5f, 0);
		Instantiate (bullet);
		bullet.transform.eulerAngles = new Vector3 (0, bullet.transform.eulerAngles.y - 15f, 0);
		Instantiate (bullet);
		bullet.transform.eulerAngles = new Vector3 (0, bullet.transform.eulerAngles.y - 5f, 0);
		Instantiate (bullet);
		bullet.transform.eulerAngles = new Vector3 (0, bullet.transform.eulerAngles.y - 5f, 0);
		Instantiate (bullet);
		lastShotgun = Time.time;
	}

	void Rifle()
	{
		bullet.transform.rotation = transform.rotation;
		bullet.transform.position = transform.position;
		bullet.transform.LookAt (playerToMouse);
		bullet.transform.Translate (new Vector3 (0f, 1f, 2f));
		Instantiate (bullet);
		lastRifle = Time.time;
	}
		

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.CompareTag ("Enemy")) {
			health -= 0.05f * col.relativeVelocity.magnitude;
			if (health <= 0) {
				gameObject.SetActive (false);
				gameManager.SendMessage ("Death");
				deathEffects.transform.position = transform.position;
				deathEffects.SetActive (true);
			}
		} else if (col.gameObject.CompareTag ("Floor")) {
			isJumping = false;
		} 
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ("Crate")) {
			Destroy (other.gameObject);
			health += 20;
			if (health > 100) {
				health = 100;
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


	void HandleMouseInputs(float time)
	{
		if (Input.GetMouseButton (0)) {
			if (time - lastRifle > rifleCD) {
				Rifle ();
			}
		}
		else if (Input.GetMouseButtonDown (1) && gotShotgun) {
			if (time - lastShotgun > shotgunCD) {
				Shotgun ();
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

		healthbar.value = health;
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
		} else {
			float rng = Random.value;
			if (rng < 0.33f) {
				announcement.text = "Upgraded Rifle \n Rifle fires faster";
				rifleCD = rifleCD * 0.8f;

			} else if (rng < 0.66f) {
				announcement.text = "Upgraded Shotgun \n Shotgun reloads faster";
				shotgunCD = shotgunCD * 0.8f;

			} else {
				announcement.text = "Upgraded Boots \n Player runs faster";
				maxSpeed = maxSpeed * 1.2f;
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

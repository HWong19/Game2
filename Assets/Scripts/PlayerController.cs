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


	public float rifleCD;
	public float shotgunCD;
	public float jumpCD;

	float x;
	float z;
	float health;

	Animator anim;


	Ray ray;
	RaycastHit hit;
	Vector3 playerToMouse;
	int floorMask;

	Rigidbody rb;

	float baseHeight;
	bool isJumping;

	float lastRifle;
	float lastShotgun;
	float lastJump;


	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		floorMask = LayerMask.GetMask ("Floor");
		rb = GetComponent<Rigidbody> ();
		baseHeight = transform.position.y - 0.2f;
		isJumping = false;
		lastRifle = 0f;
		lastShotgun = 0f;
		lastJump = 0f;
		health = 100f;
	}
	
	// Update is called once per frame
	void Update () {
		LookAtMouse ();
		float time = Time.time;

		if (transform.position.y <= baseHeight) {
			isJumping = false;
		}


		if (Input.GetMouseButtonDown (1)) {
			if (time - lastShotgun > shotgunCD) {
				Shotgun ();
			}
		} else if (Input.GetMouseButton (0)) {
			if (time - lastRifle > rifleCD) {
				lastRifle = time;
				Rifle ();
			}
		}

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

	void FixedUpdate()
	{
		Move ();
		if (Input.GetKeyDown ("space") && !isJumping && Time.time - lastJump > jumpCD) {
			rb.AddForce(new Vector3 (0f, jumpHeight, 0f));
			isJumping = true;
			lastJump = Time.time;
		}
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
	}
		

	void OnCollisionEnter(Collision col)
	{
		if(col.gameObject.CompareTag("Enemy"))
		{
			health -= 0.01f * col.relativeVelocity.magnitude ;
			if (health <= 0) {
				gameObject.SetActive(false);
				gameManager.SendMessage ("Death");
				deathEffects.transform.position = transform.position;
				deathEffects.SetActive (true);
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
		
}

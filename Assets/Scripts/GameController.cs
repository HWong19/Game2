using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {


	public Text scoreText;
	public Text gameOverText;
	public Button restartButton;
	public GameObject spawner;
	public GameObject player;
	public int scoreToUnlockShotgun;
	public int scoreToUnlockJump;
	public int scoreToUpgradeRifle;

	int score;



	void Awake()
	{
		Physics.gravity = new Vector3(0f, -39.2f, 0f);
	}
	
	// Use this for initialization
	void Start () {
		score = 0;

	}
	
	// Update is called once per frame
	void Update () {
		scoreText.text = "Score: " + score;

	}

	void IncScore()
	{
		score += 100;
		if (score == scoreToUnlockShotgun) {
			spawner.SendMessage ("IncreaseSpawnRate");
			player.SendMessage ("UnlockItem");
		} else if (score == scoreToUnlockJump) {
			spawner.SendMessage ("IncreaseSpawnRate");
			player.SendMessage ("UnlockItem");
		} else if (score == scoreToUpgradeRifle) {
			spawner.SendMessage ("IncreaseSpawnRate");
			player.SendMessage ("UnlockItem");
		}

	}

	void Death()
	{
		gameOverText.text = "You Died";
		gameOverText.gameObject.SetActive (true);
		restartButton.gameObject.SetActive (true);
	}
		
	void Restart()
	{
		SceneManager.LoadScene ("Start Menu");
	}
}

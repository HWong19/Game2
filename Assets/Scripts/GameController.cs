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
	public int scoreToUnlockBerserk;
	public float scaleFactor;

	int score;
	int scoreTillNextUpgrade;


	void Awake()
	{
		Physics.gravity = new Vector3(0f, -39.2f, 0f);
	}
	
	// Use this for initialization
	void Start () {
		score = 0;
		scoreTillNextUpgrade = scoreToUnlockJump;
	}
	
	// Update is called once per frame
	void Update () {
		scoreText.text = "Score: " + score;

	}

	void IncScore()
	{
		score += 1;
		if (score == scoreToUnlockShotgun) {
			spawner.SendMessage ("IncreaseSpawnRate");
			player.SendMessage ("UnlockItem");
		} else if (score == scoreToUnlockJump) {
			spawner.SendMessage ("IncreaseSpawnRate");
			player.SendMessage ("UnlockItem");
		} else if (score == scoreToUnlockBerserk) {
			spawner.SendMessage ("IncreaseSpawnRate");
			player.SendMessage ("UnlockItem");
		} else if (score >  scoreTillNextUpgrade * scaleFactor) {
			scoreTillNextUpgrade = score; 
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

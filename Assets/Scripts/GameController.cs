using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {


	public Text scoretext;
	public Text gameOverText;
	public Button restartButton;
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
		scoretext.text = "Score: " + score;
	}

	void IncScore()
	{
		score += 100;
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

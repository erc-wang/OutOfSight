using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLevel0 : Player
{
    #region variables
    private GameObject[] enemies;
    private Vector3[] enemiesStartPositions;
    #endregion

    // Use this for initialization
	protected override void Start () {
        this.gameObject.SetActive(true); //added this line, but not sure if it's necessary
        playerRB = GetComponent<Rigidbody2D>();
        flashlight = GameObject.FindWithTag("Flashlight");
        flashlightStatus = true;
        hasTreasure = false;
        animator = GetComponent<Animator>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        submarine = GameObject.FindWithTag("GoalLocation");
        //Debug.Log(enemies.Length);
        //Debug.Log(enemies[0].transform.position);
        enemiesStartPositions = new Vector3[enemies.Length];
        for (int i = 0; i < enemies.Length; i++){
            enemiesStartPositions[i] = enemies[i].transform.position;
        }
        // initalize the UI
        isTutorial = true;
        keyText = GameObject.FindWithTag("KeyText").GetComponent<TextMeshProUGUI>();
        keyIcon = GameObject.FindWithTag("KeyIcon").GetComponent<Image>();
        UpdateKeyText();
        UpdateKeyIcon();
        StartCoroutine(SubmarineFall());
	}


    #region Interaction_functions
    protected override void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            //goes back to starting position
            GameObject.FindWithTag("Player").transform.position = new Vector2(GameObject.FindWithTag("GoalLocation").transform.position.x + 5, GameObject.FindWithTag("GoalLocation").transform.position.y);
            for (int i = 0; i < enemies.Length; i++){
                enemies[i].transform.position = enemiesStartPositions[i];
            }
        }
        if (other.gameObject.tag == "Ground")
        {
            isJumping = false;
        }
    }
    #endregion
}

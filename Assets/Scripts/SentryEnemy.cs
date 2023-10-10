using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryEnemy : MonoBehaviour
{
    public Rigidbody2D enemyRB;
    private GameObject player;

    #region Movement_variables
    private Vector3 direction;
    public float moveSpeed;
    public float sentryMoveSpeed;
    private float moveSpeedNorm;
    private bool observed;
    public float sentryMoveRange;
    private Vector3 initPos;
    private float sentryMoveDir; 
    #endregion
    
    #region Animation_variables
    public Animator animator;
    #endregion


private void Start() {
    enemyRB = GetComponent<Rigidbody2D>();
    animator = GetComponent<Animator>();
    player = GameObject.FindWithTag("Player");
    moveSpeedNorm = 0.001f;
    sentryMoveDir = 1f;
    initPos = transform.position;
}
 
private void Update () {
    if (observed) {
        // follow the player
        direction = player.transform.position - transform.position;
        direction = direction.normalized;
        transform.Translate(direction * (moveSpeed * moveSpeedNorm), Space.World);
    }  else {
        // // move back and forth
        // if ((transform.position.x <= (initPos.x-(sentryMoveRange/2))) || transform.position.x >= (initPos.x+(sentryMoveRange/2))) {
        //     sentryMoveDir = sentryMoveDir*(-1f);
        // }
        // enemyRB.velocity = new Vector2(sentryMoveDir * sentryMoveSpeed, enemyRB.velocity.y);
        // Animate();
    }
}

private void FixedUpdate() {
      if (!observed) {
        if ((transform.position.x <= (initPos.x-(sentryMoveRange/2))) || transform.position.x >= (initPos.x+(sentryMoveRange/2))) {
            sentryMoveDir = sentryMoveDir*(-1f);
        }
        enemyRB.velocity = new Vector2(sentryMoveDir * sentryMoveSpeed, enemyRB.velocity.y);
        Animate();
      }
      
      
  }

#region Animation_functions
    private void Animate() {
        if (observed){
            animator.SetFloat ("dirX", direction.x);
        } else {
            animator.SetFloat ("dirX", sentryMoveDir);
        }
    }
   #endregion

private void OnCollisionEnter2D(Collision2D other) {
    sentryMoveDir = sentryMoveDir*(-1f);
    }
   
private void OnTriggerEnter2D(Collider2D other) {
    Debug.Log("triggerEnter");
    if (other.gameObject.tag == "Flashlight") {
        observed = true;
    }
}
private void OnTriggerExit2D(Collider2D other) {
    Debug.Log("triggerExit");
    if (other.gameObject.tag == "Flashlight") {
        observed = false;
        initPos = transform.position; // reset the initial position to wherever you are now
    }
}
}

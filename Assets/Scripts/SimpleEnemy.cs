using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    public Rigidbody2D enemyRB;
    private GameObject player;
    private Vector3 direction;
    public float moveSpeed;
    private float moveSpeedNorm;
    private bool observed;
    #region Animation_variables
    public Animator animator;
    #endregion


private void Start() {
    enemyRB = GetComponent<Rigidbody2D>();
    animator = GetComponent<Animator>();
    player = GameObject.FindWithTag("Player");
    moveSpeedNorm = 0.001f;
}
 
private void Update () {
    if (observed) {
        direction = player.transform.position - transform.position;
        direction = direction.normalized;
        transform.Translate(direction * (moveSpeed * moveSpeedNorm), Space.World);
    }  
    Animate();
}

#region Animation_functions
private void Animate() {
    if (observed){
        animator.SetBool("isMoving", true);
        animator.SetFloat ("dirX", direction.x);
    } else {
        animator.SetBool("isMoving", false);
    }
}
#endregion
   
private void OnTriggerEnter2D(Collider2D other) {
    // Debug.Log("triggerEnter");
    if (other.gameObject.tag == "Flashlight") {
        observed = true;
    }
}
private void OnTriggerExit2D(Collider2D other) {
    // Debug.Log("triggerExit");
    if (other.gameObject.tag == "Flashlight") {
        observed = false;
    }
}
}

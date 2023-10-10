using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordEnemy : MonoBehaviour
{
    public Rigidbody2D enemyRB;
    private GameObject player;
    private Vector3 direction;
    public float moveSpeed;
    public float swordMoveSpeed;
    private bool observed;
    public float swordMoveRange;
    private Vector3 initPos;
    private float swordMoveDir; 
    
    #region Animation_variables
    public Animator animator;
    #endregion

    bool moving;


    private void Start() {
        enemyRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        swordMoveDir = 1f;
        initPos = transform.position;
    moving = false;
    }
 
    private void Update () {
        if (observed) {
            //if (moving == false)
            //{
            //    Vector3 pos = player.GetComponent<Transform>().position;
             //   StartCoroutine(MoveCoroutine(pos));
            //}
            // follow the player
            direction = player.transform.position - transform.position;
            direction = direction.normalized;
        }  else {
        // move back and forth
        //if ((transform.position.x <= (initPos.x-(swordMoveRange/2))) || transform.position.x >= (initPos.x+(swordMoveRange/2))) {
        //    swordMoveDir = swordMoveDir*(-1f);
        //}
    }
        Animate();
        if (observed)
        {
            if (moving == false)
            {
                Vector3 pos = player.GetComponent<Transform>().position;
                StartCoroutine(MoveCoroutine(pos));
            }
        }
    }

#region Animation_functions
    private void Animate() {
        if (observed){
            animator.SetFloat ("dirX", direction.x);
        } else {
            animator.SetFloat ("dirX", swordMoveDir);
        }
    }
   #endregion

private void OnCollisionEnter2D(Collision2D other) {
    swordMoveDir = swordMoveDir*(-1f);
    }
   
private void OnTriggerEnter2D(Collider2D other) {
    Debug.Log("triggerEnter");
        if (other.gameObject.tag == "Flashlight") {
      observed = true;
    }
}
IEnumerator MoveCoroutine(Vector3 pos)
    {
        moving = true;
        float elapsedTime = 0;
        yield return new WaitForSeconds((float) 0.6);
        while (moving == true)
        {
            Vector3 curr = this.GetComponent<Transform>().position;
            this.GetComponent<Transform>().position = Vector3.Lerp(curr, pos, elapsedTime / 12);
            elapsedTime += Time.deltaTime;
            yield return null;

            if (this.GetComponent<Transform>().position == pos)
            {
                moving = false;
            }
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

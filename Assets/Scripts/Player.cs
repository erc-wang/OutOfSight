using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour {
    public Rigidbody2D playerRB;

    # region Movement_variables 
    private float moveVertical;
    private float moveHorizontal;
    # endregion

    #region Animation_variables
    public Animator animator;
    private bool isMoving;
    [SerializeField]
    private float jumpHeight;

    protected bool isJumping = false;
    private float fallSpeed = -2f;

    [SerializeField]
    public float movementSpeed;

    #endregion

    #region Submarine_variables
    [SerializeField]
    public Vector2 groundPos;

    protected GameObject submarine;
    protected bool followPlayer;
    #endregion

    #region Flashlight_variables
    protected GameObject flashlight;
    protected bool flashlightStatus;
    #endregion

    #region Treasure_variables
    [HideInInspector]
    public bool hasTreasure;
    #endregion

    #region UI_variables
    [HideInInspector]
    public TextMeshProUGUI keyText;
    [HideInInspector]
    public Image keyIcon;
    [HideInInspector]
    public bool isTutorial;
    private bool firstCollision = true;
    #endregion

    #region Sound_variables
    public AudioClip keySound;
    public AudioClip flipOn;
    public AudioClip flipOff;
    public AudioClip swimSound;
    #endregion

    // Use this for initialization
    protected virtual void Start () {
        this.gameObject.SetActive(true); //added this line, but not sure if it's necessary
        playerRB = GetComponent<Rigidbody2D>();
        flashlight = GameObject.FindWithTag("Flashlight");
        flashlightStatus = true;
        submarine = GameObject.FindWithTag("GoalLocation");
        hasTreasure = false;
        animator = GetComponent<Animator>();
        // initalize the UI
        isTutorial = false;
        keyText = GameObject.FindWithTag("KeyText").GetComponent<TextMeshProUGUI>();
        keyIcon = GameObject.FindWithTag("KeyIcon").GetComponent<Image>();
        UpdateKeyText();
        UpdateKeyIcon();
        StartCoroutine(SubmarineFall());
	}
	
	// Update is called once per frame
	private void Update () {
        if (!isJumping && Input.GetKeyDown(KeyCode.UpArrow) && !followPlayer) {
            //if (Input.GetKeyDown(KeyCode.UpArrow)) {
            AudioSource.PlayClipAtPoint(swimSound, this.transform.position);
            isJumping = true;
            Debug.Log("is Jumping is" + isJumping);
            playerRB.velocity = new Vector2(playerRB.velocity.x, jumpHeight) * movementSpeed;
        } else {
            isJumping = false;
        }
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");
        Move();
        Animate();
        if (Input.GetKeyDown(KeyCode.Space) && !followPlayer){            
            FlashlightSwitch();
        }
        if (playerRB.velocity.y < fallSpeed)
        {
            playerRB.velocity = new Vector2(playerRB.velocity.x, fallSpeed);
        }
    }

    protected IEnumerator SubmarineFall() {
        followPlayer = true;
        while (followPlayer) {
            if (playerRB.position.y >= groundPos.y) {
                submarine.GetComponent<Rigidbody2D>().position = new Vector2(submarine.GetComponent<Rigidbody2D>().position.x, (float)(playerRB.position.y+0.45));
                submarine.GetComponent<Rigidbody2D>().velocity = new Vector2(0, playerRB.velocity.y);
                submarine.GetComponent<Rigidbody2D>().gravityScale = playerRB.gravityScale;
                playerRB.velocity = new Vector2(0, playerRB.velocity.y);
                // fix animation
                animator.SetBool("isMoving", false);
                animator.SetFloat ("dirX", 1f); 
            } else {
                submarine.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
                submarine.GetComponent<Rigidbody2D>().gravityScale = 0;
                playerRB.velocity = new Vector2(playerRB.velocity.x, playerRB.velocity.y);
                followPlayer = false;
                Debug.Log("it's false");
            }
            yield return null;
        }
    }

    #region Movement_functions
    // Move according to key presses
    private void Move() {
        if (Input.GetKey(KeyCode.LeftArrow)) {
            playerRB.velocity = new Vector2(-movementSpeed, playerRB.velocity.y);
        }
        else
        {
            if(Input.GetKey(KeyCode.RightArrow))
            {
                playerRB.velocity = new Vector2(movementSpeed, playerRB.velocity.y);
            }
            else
            {
                playerRB.velocity = new Vector2(0, playerRB.velocity.y);
            }
        }
    }
    #endregion

    #region Flashlight_functions
    private void FlashlightSwitch(){
        flashlightStatus = !flashlightStatus;
        AudioSource.PlayClipAtPoint(flipOff, this.transform.position);
        flashlight.gameObject.SetActive(flashlightStatus);
    }
    #endregion

    #region Interaction_functions
    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Die();
            // find GameManager and lose game
            GameObject gm = GameObject.FindWithTag("GameController");
            gm.GetComponent<GameManager>().LoseGame();
        }
        if (other.gameObject.tag == "Ground")
        {
            isJumping = false;
        }
    }
    #endregion

    #region Animation_functions
    private void Animate() {
        isMoving = (moveVertical != 0)||(moveHorizontal !=0);
        animator.SetBool("isMoving", isMoving);
        if (moveHorizontal != 0) {
           animator.SetFloat ("dirX", moveHorizontal); 
        } 
    }
   #endregion

   #region Treasure_functions
   private void OnTriggerEnter2D(Collider2D other) {
    // don't stop moving and activate the win screen yet, wait til player reaches destination
    if (other.gameObject.tag == "Treasure") {
        //playerRB.bodyType = RigidbodyType2D.Static;  
        //Die();
        AudioSource.PlayClipAtPoint(keySound, this.transform.position);
        hasTreasure = true;
        //triggers treasure UI stuff
        other.gameObject.SetActive(false); //treasure in the environment set to inactive
        UpdateKeyIcon();  // updates the treasure icon
        UpdateKeyText(); // updates the treasure text
        StartCoroutine(ShowKeyText()); // show the treasure text for a bit
    }
    else if (other.gameObject.tag == "GoalLocation") {
        // check if has treasure 
        if (hasTreasure) {
            // find GameManager and win game
            hasTreasure = false;
            Die();
            GameObject gm = GameObject.FindWithTag("GameController");
            if (!isTutorial) {
                gm.GetComponent<GameManager>().WinGame();
            } else {
                gm.GetComponent<GameManager>().EndTutorial();
            }
        } else {
            if (!firstCollision) {  // don't show text at the very beginning
                StartCoroutine(ShowKeyText()); // show the treasure text for a bit
            } else {
                firstCollision = false;
            }
        }
    }
}
   #endregion

    
    #region Death_functions
    // destroys player object and triggers end scene
    private void Die() {
        // destroy this object
        this.gameObject.SetActive(false); //instead of destroying the game object, temporarily disables it to prevent errors
    }
   #endregion

   #region UI_functions
   public void UpdateKeyIcon() {
    if (hasTreasure) {
        keyIcon.gameObject.transform.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f); 
    } else {
        keyIcon.gameObject.transform.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.35f); 
    }
   }

   public void UpdateKeyText() {
        if (hasTreasure) {
            keyText.text = "Great! Now go back to the submarine";
        } else {
            keyText.text = "You haven't found the key yet!";
            keyText.gameObject.SetActive(false);
        }
   }

   IEnumerator ShowKeyText() 
    {   if ((!isTutorial) | (hasTreasure)) {
        keyText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        yield return new WaitForEndOfFrame();
        keyText.gameObject.SetActive(false);
        }  
    }
   #endregion

}
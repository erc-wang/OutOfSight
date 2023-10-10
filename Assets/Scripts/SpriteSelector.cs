using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpriteSelector : MonoBehaviour
{
    public Sprite[] spriteArray;
    private SpriteRenderer spriteR;
    private Scene scene;
    private int prefixLen = 5; // how many letters in the word "Level"?
    private int n;

    // Start is called before the first frame update
    void Start()
    {
        // Return the current Active Scene in order to get the current Scene name.
        scene = SceneManager.GetActiveScene();
        n = int.Parse(scene.name.Substring(prefixLen));

        // Change the sprite
        spriteR = gameObject.GetComponent<SpriteRenderer>();
        spriteR.sprite = spriteArray[n];
    }
}

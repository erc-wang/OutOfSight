using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ImageSelector : MonoBehaviour
{
    public Sprite[] spriteArray;
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
        gameObject.GetComponent<Image>().sprite = spriteArray[n];
    }
}

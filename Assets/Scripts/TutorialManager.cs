using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    #region variables

    public GameObject[] popUps; //displays intructions
    private int popUpIndex;
    private bool conditionMet;
    protected PlayerLevel0 playerScript;


    #endregion

    // Start is called before the first frame update
    void Start()
    {
        popUpIndex = 0;
        conditionMet = false;
        playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerLevel0>();
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < popUps.Length; i++) {
            if (i == popUpIndex) {
                popUps[i].SetActive(true);
            } else {
                popUps[i].SetActive(false);
            }
        }   
        if (popUpIndex == 0) {
            if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)) && !conditionMet) {
                StartCoroutine(IncIndex());
            }
        } else if (popUpIndex == 1) {
            if (Input.GetKeyDown(KeyCode.UpArrow) && !conditionMet) {
                StartCoroutine(IncIndex());
            }
        } else if (popUpIndex == 2) {
            //check if at the position after an obstacle
            if (GameObject.FindWithTag("Player").transform.position.x >= 0 && !conditionMet) {
                StartCoroutine(IncIndex());
            }
        } else if (popUpIndex == 3) {
            //flashlight
            if (Input.GetKeyDown(KeyCode.Space) && !conditionMet) {
                StartCoroutine(IncIndex());
            }
        } else if (popUpIndex == 4) {
            //check if at a position near treasure
            if (GameObject.FindWithTag("Player").transform.position.x >= 18 && !conditionMet) {
                StartCoroutine(IncIndex());
            }
        } else if (popUpIndex == 5) {
            //treasure
            if (playerScript.hasTreasure && !conditionMet) {
                StartCoroutine(IncIndex());
            }
        } 
    }

    #region misc functions
    IEnumerator IncIndex() 
    {   
        conditionMet = true;
        //Debug.Log("gonna wait for 2 seconds");
        yield return new WaitForSeconds(1);
        //Debug.Log("2 seconds has passed");
        popUpIndex++; 
        yield return new WaitForEndOfFrame();
        conditionMet = false;
    }
    #endregion
}

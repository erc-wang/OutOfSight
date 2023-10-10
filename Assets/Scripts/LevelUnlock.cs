using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUnlock : MonoBehaviour
{
    public Button[] levelButtons;
    public Image[] treasureLocks;
    private int maxReachedLevel;
    private int maxUnlockedLevel;
    private int maxLevel;
    private GameObject gm; 

    #region Unity_functions
    private void Start() {
        gm = GameObject.FindWithTag("GameController");
        maxReachedLevel = gm.GetComponent<GameManager>().maxReachedLevel;
        maxLevel = gm.GetComponent<GameManager>().maxLevel;
        maxUnlockedLevel = gm.GetComponent<GameManager>().maxUnlockedLevel;
        Debug.Log($"from level unlock start: maxLevel = {maxLevel}, maxReachedLevel = {maxReachedLevel}, maxUnlockedLevel = {maxUnlockedLevel}");
        for (int i = 0; i < levelButtons.Length; i++) {
            //bool thisSetting = (i + 1) <= maxReachedLevel;
            bool thisSetting = (((i + 1) <= maxReachedLevel) | (i == 0));
            levelButtons[i].interactable = thisSetting;
            GameObject thisLock = levelButtons[i].transform.GetChild(1).gameObject;
            thisLock.SetActive(!thisSetting);
        } 
        for (int i = 0; i < treasureLocks.Length; i++) {
            bool thisSetting = (i + 1) <= maxUnlockedLevel;
            treasureLocks[i].enabled = !thisSetting;
        } 
    }
    #endregion
}
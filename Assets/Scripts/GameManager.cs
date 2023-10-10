using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.CloudSave;

public class GameManager : MonoBehaviour {
    public static GameManager Instance = null;
    private TextMeshProUGUI chosenLevel;
    public AudioClip select;
    private int currLevel;
    [HideInInspector] public int maxReachedLevel;
    [HideInInspector] public int maxLevel;
    [HideInInspector] public int maxUnlockedLevel;
    //dioSource selectAudio;
    bool newUser = true;

    #region Unity_functions
    private async void Awake() {
        //lectAudio = GetComponent<AudioSource>();
        //bug.Log(selectAudio == null);
        if (Instance == null) {
            // first time loading the game manager
            Instance = this;
            await InitCloudSave();
        } else if (Instance != this) {
            // a game manager already exists
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    async void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        await InitPlayerData();
    }

    private void OnDestroy () {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    internal async Task InitCloudSave() {
        // activate Cloud Save
        await UnityServices.InitializeAsync();
        // sign in anonymously
        await SignInAnonymously();
        // initialize player data
        await InitPlayerData();
    }

    private async Task SignInAnonymously() {
        AuthenticationService.Instance.SignedIn += () =>
        {
            var playerId = AuthenticationService.Instance.PlayerId;
            Debug.Log("Signed in as: " + playerId);
        };
        AuthenticationService.Instance.SignInFailed += s =>
        {
            Debug.Log(s);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async Task InitPlayerData() {
        // set levels or get them from stored data
        // check if needed variables exist
        await CheckExisting();
        if (newUser) {
            var userData = new Dictionary<string, object>{
                {"currLevel", 1}, 
                {"maxReachedLevel", 1}, 
                {"maxLevel", 3}, 
                {"maxUnlockedLevel", 0}
                };
            await CloudSaveService.Instance.Data.ForceSaveAsync(userData);
        } else {
            var currLevelObj = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> {"currLevel"});
            currLevel = int.Parse(currLevelObj["currLevel"]);
            var maxReachedLevelObj = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> {"maxReachedLevel"});
            maxReachedLevel = int.Parse(maxReachedLevelObj["maxReachedLevel"]);
            //var maxLevelObj = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> {"maxLevel"});
            //maxLevel = int.Parse(maxLevelObj["maxLevel"]);
            maxLevel = 3;
            var maxUnlockedLevelObj = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> {"maxUnlockedLevel"});
            maxUnlockedLevel = int.Parse(maxUnlockedLevelObj["maxUnlockedLevel"]);
        }
    }

    private async Task CheckExisting() {
    // check if information is stored
        List<string> keys = await CloudSaveService.Instance.Data.RetrieveAllKeysAsync();
        newUser = !keys.Contains("maxUnlockedLevel"); 
        Debug.Log($"new user? {newUser}");
    }

    private async Task SaveData() {
        var userData = new Dictionary<string, object>{
                {"currLevel", currLevel}, 
                {"maxReachedLevel", maxReachedLevel}, 
                {"maxLevel", maxLevel}, 
                {"maxUnlockedLevel", maxUnlockedLevel}
                };
        await CloudSaveService.Instance.Data.ForceSaveAsync(userData);
    }
    #endregion

    
    #region Scene_transitions
    public async void StartGame() {
        //AudioSource.PlayClipAtPoint(select, this.transform.position, 2f);
        //await Task.Delay((int)(select.length * 1500f));
        var currLevelObj = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> {"currLevel"});
        currLevel = int.Parse(currLevelObj["currLevel"]);
        SceneManager.LoadScene($"Level{currLevel}");
    }

    public async void Continue() {
        //AudioSource.PlayClipAtPoint(select, this.transform.position, 2f);
        //await Task.Delay((int)(select.length * 1500f));
        var currLevelObj = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> {"currLevel"});
        currLevel = int.Parse(currLevelObj["currLevel"]) + 1;
        var maxReachedLevelObj = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> {"maxReachedLevel"});
        maxReachedLevel = int.Parse(maxReachedLevelObj["maxReachedLevel"]);
        var maxUnlockedLevelObj = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> {"maxUnlockedLevel"});
        maxUnlockedLevel = int.Parse(maxUnlockedLevelObj["maxUnlockedLevel"]);
        await SaveData();
        StartGame();
    }

    public void LevelSelection() {;
        //Debug.Log("Level Selection");
        //VoidSelect("LevelSelectionScene");
        Debug.Log(select.length);
        AudioSource.PlayClipAtPoint(select, this.transform.position, 10f);
        Invoke("LevelSelect", select.length * 2);
    }
    public void LevelSelect()
    {
        SceneManager.LoadScene("LevelSelectionScene", LoadSceneMode.Single);
    }
    //public async void GoToLevel(TextMeshProUGUI chosenLevel) {
    public async void GoToLevel(int chosenLevel) {
        //AudioSource.PlayClipAtPoint(select, this.transform.position, 2f);
        //await Task.Delay((int) (select.length * 1500f));
        //currLevel = int.Parse(chosenLevel.text);
        currLevel = chosenLevel;
        var maxReachedLevelObj = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> {"maxReachedLevel"});
        maxReachedLevel = int.Parse(maxReachedLevelObj["maxReachedLevel"]);
        var maxUnlockedLevelObj = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> {"maxUnlockedLevel"});
        maxUnlockedLevel = int.Parse(maxUnlockedLevelObj["maxUnlockedLevel"]);
        await SaveData();
        SceneManager.LoadScene("Level" + currLevel);
    }

    public void LoseGame() {
        SceneManager.LoadScene("LoseScene",LoadSceneMode.Single);
    }

    public async void WinGame() {
        // update the max reached level if you are trying the hardest level yet
        if ((currLevel == maxReachedLevel) && (maxReachedLevel < maxLevel)) {
            maxReachedLevel += 1;
            maxUnlockedLevel += 1;
        }
        await SaveData();
        // call the final scene if you completed all levels
        if (currLevel == maxLevel) {
            SceneManager.LoadScene("FinalScene",LoadSceneMode.Single);
        } else {
            SceneManager.LoadScene("WinScene",LoadSceneMode.Single);
        }
    }

    public async void EndTutorial() {
        // update the max reached level if you are trying the hardest level yet
        await SaveData();
        // call the final scene if you completed all levels
        SceneManager.LoadScene("LevelSelectionScene",LoadSceneMode.Single);
    }

    public void MainMenu() {
        AudioSource.PlayClipAtPoint(select, this.transform.position, 2f);
        Invoke("MainMenuSelect", select.length * 2);
        //SceneManager.LoadScene("MainMenu",LoadSceneMode.Single);
    }

    public void MainMenuSelect()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }


    #endregion
}

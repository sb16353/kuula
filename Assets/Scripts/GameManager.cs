using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Linq;
public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject gameOverPrefab;  
    [SerializeField]
    GameObject eventSystemPrefab;
    [SerializeField]
    GameObject backgroundPrefab;


    private static GameManager _instance;

    public static GameManager Instance
    {
        get {
            /*
            if (_instance == null)
                return new GameObject("GameManager").AddComponent<GameManager>();
            */
            return _instance;
        }
    }

    bool enableTimer = true;

    public bool TimerEnabled
        => enableTimer;

    public void ToggleTimer(UnityEngine.UI.Toggle _toggle)
        => enableTimer = _toggle.isOn;

    public static readonly WaitForFixedUpdate FixedUpdateDelay = new();

    public LevelSetData levelSetData;

    GameObject gameOver;
    GameObject eventSystem;

    public readonly List<string> levelCompletionTimes = new ();


    public void GameOver(){
        gameOver = Instantiate(gameOverPrefab);
        //Destroy(_player.gameObject);
    }

    void Awake(){
        if(_instance != null && _instance != this){
            Destroy(this);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(gameObject);


        

        SceneManager.activeSceneChanged += delegate(Scene _, Scene loadedScene) {
            StopAllCoroutines();

            bool inMainMenu = loadedScene.buildIndex <= 1;

            if(inMainMenu){
                _currentLevelSetName = string.Empty;
                levelCompletionTimes.Clear();
            }


            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if(playerGO != null){
                _player = playerGO.GetComponent<Player>();
                var camera = Camera.main;
                if(camera.gameObject.GetComponent<MoveCamera>() == null)
                    camera.gameObject.AddComponent<MoveCamera>();            
            }

            if(!eventSystem) {
                var eS = FindAnyObjectByType<EventSystem>();
                if(eS != null)
                    eventSystem = eS.gameObject;
                else
                    eventSystem = Instantiate(eventSystemPrefab);  
                DontDestroyOnLoad(eventSystem);
            }
            else {
                var eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None).Select(x => x.gameObject).ToArray();
                if(eventSystems.Length > 1)
                {
                    for(int i = eventSystems.Length - 1; i >= 1; i--)
                        Destroy(eventSystems[i]);
                }
                eventSystem = eventSystems[0];
            }

            // var bg = Instantiate(backgroundPrefab);

            // Color color;
            // do {
            //     color = new Color(Random.value, Random.value, Random.value);
            // }
            // while(IsTooGreenOrDark(color));

            // var bgRenderer = bg.GetComponent<Renderer>();
            // bgRenderer.material.color = color;

            // if(inMainMenu)
            //     StartCoroutine(MainMenuColorLoop(bgRenderer));
        };
    }
    // private IEnumerator MainMenuColorLoop(Renderer _bgRenderer){
    //     Color color1 = _bgRenderer.material.color;
    //     Color color2;
    //     while(true) {
    //         do {
    //             color2 = new Color(Random.value, Random.value, Random.value);
    //         }
    //         while(IsTooGreenOrDark(color2));

    //         float startTime = Time.time;
    //         while(Time.time - startTime <= 2.0f) {

    //             if(_bgRenderer == null)
    //                 yield break;

    //             _bgRenderer.material.color = Color.Lerp(color1, color2, (Time.time - startTime) / 2.0f);

    //             yield return null;
    //         }

    //         color1 = color2;
    //         _bgRenderer.material.color = color1;
    //     }            
    // }   

    //  bool IsTooGreenOrDark(Color color)
    // {
    //     // Too green: green component much higher than red/blue
    //     if (color.g > Fractions.ThreeFifths && color.g > color.r + Fractions.OneFifth && color.g > color.b + Fractions.OneFifth)
    //         return true;

    //     double luminance = (0.2126 * color.r) + (0.7152 * color.g) + (0.0722 * color.b);
    //     if (luminance < Fractions.OneHalf)
    //         return true;

    //     return false;
    // }

    public Player Player {
        get => _player;
    }

    Player _player;

    public void DisplayLoading(){
        if(gameOver != null) {
            Destroy(gameOver);
        }

        Instantiate(loadingPrefab);
    }

    public void LoadLevel(string _name)
    {
        DisplayLoading();
        SceneManager.LoadSceneAsync(_name);
    }
    public void LoadMainMenu()
        => LoadLevel("main");
    public void ReloadCurrentLevel()
        => LoadLevel(SceneManager.GetActiveScene().name);

    string _currentLevelSetName;
    public string CurrentLevelSetName 
    {
        get => _currentLevelSetName;
    }

    [SerializeField]
    private GameObject loadingPrefab;

    public void StopTimer(bool addRecord = true) {
        var timer = FindAnyObjectByType<GameTimer>();
        if(timer != null)
            timer.StopTimer(addRecord);
    }
    
    public void LoadNextLevel()
    {
        DisplayLoading();

        string currentScene = SceneManager.GetActiveScene().name;

        foreach (var set in levelSetData.levelSets) {
            int index = set.levelSceneNames.IndexOf(currentScene);
            if (index != -1) {
                _currentLevelSetName = set.name;
                int nextIndex = index + 1;
                string nextScene = nextIndex >= set.levelSceneNames.Count ? "victory" : set.levelSceneNames[nextIndex % set.levelSceneNames.Count];
                SceneManager.LoadSceneAsync(nextScene);
                return;
            }
        }

        Debug.LogWarning("Current scene not found in any level set.");
    }

    const float DEATH_Y_THRESHOLD = -20.0f;

    private void LateUpdate(){
        if(Player != null && !Player.isDead && Player.position.y < DEATH_Y_THRESHOLD)
            Player.Die();
    }
}

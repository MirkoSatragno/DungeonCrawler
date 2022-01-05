using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
    }

    [HideInInspector]
    public const string LEVEL_NAME_MAIN_MENU = "MainMenu";
    [HideInInspector]
    public const string LEVEL_NAME_LEVEL_1 = "Level1";
    [HideInInspector]
    public const string LEVEL_NAME_LEVEL_2 = "Level2";

    [HideInInspector]
    public const string TAG_PLAYER = "Player";
    [HideInInspector]
    public const string TAG_VISUAL_EFFECT = "VisualEffect";

    private bool _isGamePaused;
    public bool IsGamePaused
    {
        get { return _isGamePaused; }
        set { _isGamePaused = value; }
    }

    private void Awake()
    {
        if (!Instance)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        IsGamePaused = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGamePaused(bool setPaused)
    {
        if (setPaused)
        {
            Time.timeScale = 0;
            IsGamePaused = true;
        }
        else
        {
            Time.timeScale = 1;
            IsGamePaused = false;
        }
    }
}

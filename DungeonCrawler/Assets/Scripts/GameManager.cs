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

    [SerializeReference]
    private Texture2D defaultMouseIcon;
    [SerializeReference]
    private Texture2D selectableMouseIcon;
    [SerializeReference]
    private Texture2D infoMouseIcon;

    [HideInInspector]
    public const string LEVEL_NAME_MAIN_MENU = "MainMenu";
    [HideInInspector]
    public const string LEVEL_NAME_LEVEL_1 = "Level1";
    [HideInInspector]
    public const string LEVEL_NAME_LEVEL_2 = "Level2";

    [HideInInspector]
    public const string TAG_PLAYER = "Player";
    [HideInInspector]
    public const string TAG_ENEMY = "Enemy";
    [HideInInspector]
    public const string TAG_ENEMY_SPAWNER = "EnemySpawner";
    [HideInInspector]
    public const string TAG_ITEM = "Item";
    [HideInInspector]
    public const string TAG_OBSTACLE = "Obstacle";

    [HideInInspector]
    public const string AXIS_NAME_HORIZONTAL = "Horizontal";
    [HideInInspector]
    public const string AXIS_NAME_VERTICAL = "Vertical";


    [HideInInspector]
    public const string LAYER_NAME_SPAWN_COLLIDER_BOUNDARY = "SpawnColliderBoundary";

    public enum MouseIcon
    {
        Default,
        Selectable,
        Info
    }


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
        Debug.Assert(defaultMouseIcon && selectableMouseIcon && infoMouseIcon, "GameManager: mouseIcon reference not found");

        setMouseIcon(MouseIcon.Default);
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


    public void setMouseIcon(MouseIcon icon)
    {
        Texture2D newIcon = default(Texture2D);
        
        switch (icon)
        {
            case MouseIcon.Default:
                newIcon = defaultMouseIcon;
                break;
            case MouseIcon.Selectable:
                newIcon = selectableMouseIcon;
                break;
            case MouseIcon.Info:
                newIcon = infoMouseIcon;
                break;
        }

        Cursor.SetCursor(newIcon, Vector2.zero, CursorMode.Auto);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("World")]
    [SerializeField] string _stringSeed = "Sisyphus";
    [SerializeField] List<ScriptableWorld> _worlds;

    public Unlocks Unlocks;

    int _worldIndex = 0;
    float _seed = 1234f;
    public float Seed { get { return _seed; } }

    public ScriptableWorld CurrentWorld { get { return _worlds[_worldIndex % _worlds.Count];  } }

    public GameObject _unlockMessagePrefab;
    public Transform _unlockMessageContainer;

    public static GameManager Instance;
    public void Awake()
    {
        if(Instance != null)
        {
            Restart();
            Destroy(gameObject);
            return;
        }
 
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Unlocks = Data.LoadUnlocks();

        Instance = this;

        Restart();

    }


    void Restart()
    {
        _stringSeed = GetRandomSeed(6);
        _seed = StringToFloat(_stringSeed);
        _seedText.text = _stringSeed;

        _distanceText.text = Mathf.FloorToInt(_distanceReached) + "m";
        _difficultyTime = 0f;
        _distanceReached = 0f;
    }

    float _difficultyTime = 0f;
    float _distanceReached = 0f;
    public float DistanceReached { get { return _distanceReached; } }
    public float Difficulty
    {
        get
        {
            return 
                1f
                + (_difficultyTime / 60) * (_difficultyTime / 60) / 100 // doubles every 10mn
            ;
        }
    }

    [Header("Difficulty")]
    [SerializeField] TextMeshProUGUI _distanceText;
    [SerializeField] TextMeshProUGUI _difficultyText;
    [SerializeField] TextMeshProUGUI _seedText;

    [Header("Player Health")]
    [SerializeField] Heart _heartPrefab;
    [SerializeField] Transform _heartContainerTransform;
    [SerializeField] List<Heart> _hearts = new List<Heart>();
    
    public void UpdateHealthContainers()
    {
        foreach (Transform t in _heartContainerTransform) Destroy(t.gameObject);
        _hearts.Clear();
        for(int i = 0; i < Player.Instance.MaxHealth; i++)
        {
            _hearts.Add(Instantiate(_heartPrefab, _heartContainerTransform));
        }
    }

    public void UpdatePlayerHealth()
    {
        int i = 0;
        foreach(Heart heart in _hearts)
        {
            i++;
            heart.SetActive(i <= Player.Instance.CurrentHealth);
        }
    }

    [Header("Player Coins")]
    [SerializeField] TextMeshProUGUI _coinsText;
    public void UpdatePlayerCoins()
    {
        _coinsText.text = Player.Instance.Coins.ToString();
    }


    void Update()
    {
        if (InputHelper.GetMenuButtonDown()) SettingsPanelUI.Instance.Toggle();
        _difficultyTime += Time.deltaTime;
        if (Player.Instance.transform.position.x > _distanceReached)
        {
            _distanceReached = Player.Instance.transform.position.x;
            _distanceText.text = Mathf.FloorToInt(_distanceReached / 3) + "m";
        }
        _difficultyText.text = 
            "Difficulty : " + Mathf.FloorToInt(Difficulty * 10f) / 10f
            + "\r\nEntityManager Credits : " + Mathf.FloorToInt(EntityManager.Instance.Money)
            + "\r\nStructures : " + TerrainManager.Instance.LandPartCount
        ;
    }

    public void SetTimeScale(float value, float lerp=1f)
    {
        if (_paused) return;
        Time.timeScale = Mathf.Lerp(Time.timeScale, value, lerp);
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    string GetRandomSeed(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string str = "";
        for (int i = 0; i < length; i++) str += chars[Mathf.FloorToInt(Random.Range(0, chars.Length))];
        return str;
    }
    float StringToFloat(string s)
    {
        float res = 0;
        foreach (char c in s)
        {
            res += (int)c;
        }
        return res;
    }

    bool _paused = false;
    public bool IsPaused { get { return _paused; } }
    public void Pause()
    {
        _paused = true;
        Time.timeScale = 0f;
    }
    public void Play()
    {
        _paused = false;
        Time.timeScale = 1f;
    }

    public void NextWorld()
    {
        _worldIndex++;
    }

    public void DisplayUnlockMessage()
    {
        Instantiate(_unlockMessagePrefab, _unlockMessageContainer);
    }
}

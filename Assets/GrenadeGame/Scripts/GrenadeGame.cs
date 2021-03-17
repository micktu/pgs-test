using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;


public enum GameState
{
    Init,
    Playing,
    Finished
}


public class GrenadeGame : MonoBehaviour
{
    public DamageLabel DamageLabelPrefab;

    public CharacterCamera CharacterCamera;


    public GameConfig Config { get; private set; }

    
    public event Action<Grenade> GrenadePickedUp;

    public event Action<Vector3, int, Vector3, float> GrenadeThrown;

    public event Action<Enemy, int> DamageDealt;

    public event Action<GameState> StateChanged;


    private GameManager _gameManager;

    private GrenadeCharacter _character;
    
    private GameUI _hud;

    private GrenadeManager _grenadeManager;

    private EnemyManager _enemyManager;

    private int[] _grenadeStock;

    private int _grenadeSelected;

    private GameState _state;


    public IEnumerator Init(GameManager gameManager)
    {
        _gameManager = gameManager;

        // Load a chain of dependencies in order - @micktu
        var configHandle = Addressables.LoadAssetAsync<GameConfig>("ConfigDefault");
        yield return configHandle;
        
        Config = configHandle.Result;

        int numGrenadeTypes = Config.GrenadeTypes.Length;
        _grenadeStock = new int[numGrenadeTypes];
        for (int i = 0; i < numGrenadeTypes; i++)
        {
            Config.GrenadeTypes[i].Index = i;
        }

        // Character

        var handle = Addressables.InstantiateAsync("CharacterDefault", Vector3.zero, Quaternion.identity);
        yield return handle;

        _character = handle.Result.GetComponent<GrenadeCharacter>();
        _character.Init(this);
        CharacterCamera.Init(_character);
        _character.Camera = CharacterCamera;

        // Grenade
        
        handle = Addressables.LoadAssetAsync<GameObject>("GrenadeDefault");
        yield return handle;

        var grenadePrefab = handle.Result.GetComponent<Grenade>();

        handle = Addressables.LoadAssetAsync<GameObject>("ExplosionDefault");
        yield return handle;

        var explosionPrefab = handle.Result.GetComponent<Explosion>();

        _grenadeManager = new GrenadeManager();
        _grenadeManager.Init(this, grenadePrefab, explosionPrefab);

        // Enemy

        handle = Addressables.LoadAssetAsync<GameObject>("EnemyDefault");
        yield return handle;

        _enemyManager = new EnemyManager();
        _enemyManager.Init(this, handle.Result.GetComponent<Enemy>());

        // HUD

        handle = Addressables.InstantiateAsync("GameUI");
        yield return handle;

        _hud = handle.Result.GetComponent<GameUI>();
        _hud.Init(this);
        _hud.transform.SetParent(_gameManager.Canvas.transform, false);

        _hud.ClearGrenadeList();
        foreach (GrenadeType type in Config.GrenadeTypes)
        {
            _hud.AddGrenadeItem(type.Color);
        }

        _hud.SelectGrenadeType(0);

        // Everything is set up

        EnterState(GameState.Playing);
    }

    void Update()
    {
        switch (_state)
        {
            case GameState.Init:
                // Wait until loaded - @micktu
                break;
            case GameState.Playing:
                _grenadeManager.Tick(Time.deltaTime);
                _enemyManager.Tick(Time.deltaTime);

                break;
            case GameState.Finished:
                // We have no win condition - @micktu
                break;
        }
    }

    public void EnterState(GameState state)
    {
        _state = state;
        StateChanged?.Invoke(state);
    }

    // Global events

    public void PickupGrenade(Grenade grenade)
    {
        _grenadeStock[grenade.TypeIndex]++;

        GrenadePickedUp?.Invoke(grenade);
    }

    public void ThrowGrenade(Vector3 position, Vector3 velocity, float gravity)
    {
        int type = _grenadeSelected;

        if (_grenadeStock[type] < 1) return;
        _grenadeStock[type]--;
        GrenadeThrown?.Invoke(position, type, velocity, gravity);
    }

    public void DealDamage(Enemy enemy, int amount)
    {
        DamageLabel label = Instantiate(DamageLabelPrefab);
        label.Init(enemy.transform, amount, CharacterCamera.transform);
        
        enemy.TakeDamage(amount);

        DamageDealt?.Invoke(enemy,amount);
    }

    public void SelectGrenade(int index)
    {
        _grenadeSelected = index;
        _hud.SelectGrenadeType(index);
    }

    public void SelectGrenadeNext()
    {
        int index = _grenadeSelected + 1;
        if (index >= Config.GrenadeTypes.Length) index = 0;
        SelectGrenade(index);
    }

    public void SelectGrenadePrevious()
    {
        int index = _grenadeSelected - 1;
        if (index < 0) index = Config.GrenadeTypes.Length - 1;
        SelectGrenade(index);
    }

}

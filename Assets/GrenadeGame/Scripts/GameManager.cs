using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public LoaderUI Hud;

    public Camera LoaderCamera;

    public Canvas Canvas;

    // This is concrete, but can easily be refactored to be abstract - @micktu
    public GrenadeGame Game;
    

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        Hud.SetLoading(true);

        Addressables.LoadSceneAsync("GameDefault", LoadSceneMode.Additive).Completed += (scene) =>
        {
            Game = FindObjectOfType<GrenadeGame>();
            Game.StateChanged += OnGameStateChanged;
            IEnumerator init = Game.Init(this);
            StartCoroutine(init);
        };
    }

    public void OnGameStateChanged(GameState state)
    {
        Hud.SetLoading(false);
        Destroy(LoaderCamera.gameObject);
    }
}

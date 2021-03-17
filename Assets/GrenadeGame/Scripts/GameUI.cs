using System.Collections.Generic;
using UnityEngine;


public class GameUI : MonoBehaviour
{
    public GrenadeGame Game;

    public GrenadeCounter GrenadeItemPrefab;

    public GameObject GrenadeList;


    private List<GrenadeCounter> _counters = new List<GrenadeCounter>();


    public void Init(GrenadeGame game)
    {
        Game = game;
        Game.GrenadePickedUp += OnGrenadePickedUp;
        Game.GrenadeThrown += OnGrenadeThrown;
    }

    public void AddGrenadeItem(Color color)
    {
        GrenadeCounter counter = Instantiate(GrenadeItemPrefab, GrenadeList.transform, false);
        counter.SetColor(color);
        _counters.Add(counter);
    }
    public void ClearGrenadeList()
    {
        foreach (Transform t in GrenadeList.transform)
        {
            Destroy(t.gameObject);
        }
    }

    public int GetCounter(int index)
    {
        return _counters[index].GetCount();
    }

    public void SetCounter(int index, int value)
    {
        _counters[index].SetCount(value);
    }

    public void SelectGrenadeType(int type)
    {
        int i = 0;

        foreach (GrenadeCounter counter in _counters)
        {
            counter.SetSelected(i == type);
            i++;
        }
    }

    public void OnGrenadePickedUp(Grenade grenade)
    {
        SetCounter(grenade.TypeIndex, GetCounter(grenade.TypeIndex) + 1);
    }

    public void OnGrenadeThrown(Vector3 position, int type, Vector3 velocity, float gravity)
    {
        int numGrenades = GetCounter(type);
        if (numGrenades < 1) return;
        SetCounter(type, numGrenades - 1);
    }
}

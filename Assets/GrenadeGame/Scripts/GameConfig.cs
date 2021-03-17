using System;
using UnityEngine;

[Serializable]
public struct GrenadeType
{
    public int Index;
    public Color Color;
}

[CreateAssetMenu(fileName = "GameConfig", menuName = "Config/GameConfig", order = 1)]
public class GameConfig : ScriptableObject
{
    public GrenadeType[] GrenadeTypes;

    public int MaxGrenades = 100;
    public float GrenadeRespawnTime = 5.0f;

    public float GrenadePickupDistance = 1.0f;
    public float GrenadeExplosionDistance = 1.0f;

    public float GrenadeLateral = 1.0f;
    public float GrenadeArc = 1.0f;
    public float GrenadeSpeed = 10.0f;
    public float GrenadeFuseTime = 0.5f;

    public int GrenadeMaxDamage = 40;
    public float GrenadeBlastRadius = 2.0f;

    public int MaxEnemies = 50;
    public int EnemyMaxHealth = 100;
    public float EnemyRespawnTime = 10.0f;

    public int MaxTrajectoryPoints = 100;
}


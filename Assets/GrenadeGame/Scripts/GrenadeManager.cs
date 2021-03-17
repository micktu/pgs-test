using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;


public class GrenadeManager
{
    public Grenade GrenadePrefab;

    public Explosion ExplosionPrefab;
    public int NumGrenadeTypes { get; private set; }

    public GrenadeGame Game { get; private set; }


    private List<Grenade> _grenades = new List<Grenade>();


    public void Init(GrenadeGame game, Grenade grenadePrefab, Explosion explosionPrefab)
    {
        Game = game;
        GrenadePrefab = grenadePrefab;
        ExplosionPrefab = explosionPrefab;

        int maxGrenades = Game.Config.MaxGrenades;
        int numGrenades = 0;

        NumGrenadeTypes = Game.Config.GrenadeTypes.Length;

        float radius = Game.Config.GrenadePickupDistance;
        int layerMask = 1 << LayerMask.NameToLayer("Pickups");

        RaycastHit hit;

        // Spawn grenades randomly - @micktu
        while (numGrenades < maxGrenades)
        {
            Vector3 position = new Vector3(Random.Range(-50.0f, 50.0f), 1.0f, Random.Range(-50.0f, 50.0f));

            // Probably the most efficient way to check for overlapping - @micktu
            bool isTooClose = Physics.SphereCast(position, radius, Vector3.one, out hit, 0.0f, layerMask);
            if (isTooClose) continue;

            SpawnGrenade(position);
            numGrenades++;
        }

        Game.GrenadeThrown += OnGrenadeThrown;
        Game.GrenadePickedUp += OnGrenadePickedUp;
    }

    public void Tick(float deltaTime)
    {
        foreach (Grenade grenade in _grenades)
        {
            grenade.Tick(deltaTime);
        }
    }

    public Grenade SpawnGrenade(Vector3 position, int type = -1)
    {
        Grenade newGrenade = Object.Instantiate(GrenadePrefab, position, Quaternion.identity);

        // FIXME Can be replaced with a common GrenadeInfo struct - @micktu
        newGrenade.PickupRadius = Game.Config.GrenadePickupDistance;
        newGrenade.ExplosionRadius = Game.Config.GrenadeExplosionDistance;
        newGrenade.Speed = Game.Config.GrenadeSpeed;
        newGrenade.FuseTime = Game.Config.GrenadeFuseTime;
        newGrenade.RespawnTime = Game.Config.GrenadeRespawnTime;
        newGrenade.Init(this, Random.value);
        newGrenade.SetType(type >= 0 ? type : Random.Range(0, NumGrenadeTypes - 1));

        newGrenade.Index = _grenades.Count;
        _grenades.Add(newGrenade);
        
        return newGrenade;
    }

    public void OnGrenadePickedUp(Grenade grenade)
    {
        _grenades[grenade.Index].EnterState(GrenadeState.PickedUp);
    }

    public void OnGrenadeThrown(Vector3 position, int type, Vector3 velocity, float gravity)
    {
        Grenade grenade = SpawnGrenade(position, type);

        grenade.Velocity = velocity;
        grenade.Gravity = gravity;
        
        grenade.EnterState(GrenadeState.Thrown);
    }

    public void ExplodeGrenade(Grenade grenade)
    {
        _grenades.Remove(grenade);

        Vector3 position = grenade.transform.position;

        Explosion explosion = Object.Instantiate(ExplosionPrefab, position, Quaternion.identity);
        explosion.SetColor(grenade.GetColor());

        float radius = Game.Config.GrenadeBlastRadius;

        int layerMask = 1 << LayerMask.NameToLayer("Enemies"); // This should be cached - @micktu
        RaycastHit[] hits = Physics.SphereCastAll(position, radius, grenade.Velocity.normalized, 0.0f, layerMask);

        int maxDamage = Game.Config.GrenadeMaxDamage;

        foreach (RaycastHit hit in hits)
        {
            var enemy = hit.collider.GetComponent<Enemy>();
            if (enemy == null) continue;

            float distance = Vector3.Distance(grenade.transform.position, enemy.transform.position);
            int damage = Mathf.CeilToInt(maxDamage * (1.0f - distance / radius));

            Game.DealDamage(enemy, damage);
        }

        Object.Destroy(grenade.gameObject);
    }
}

using System.Collections.Generic;
using UnityEngine;


public class EnemyManager
{
    public Enemy EnemyPrefab;

    public GrenadeGame Game;


    private List<Enemy> _enemies = new List<Enemy>();


    public void Init(GrenadeGame game, Enemy enemyPrefab)
    {
        Game = game;
        EnemyPrefab = enemyPrefab;

        int maxEnemies = Game.Config.MaxEnemies;
        int numEnemies = 0;

        float radius = Game.Config.GrenadePickupDistance;
        int layerMask = 1 << LayerMask.NameToLayer("Pickups") & 1 << LayerMask.NameToLayer("_enemies");

        RaycastHit hit;

        while (numEnemies < maxEnemies)
        {
            Vector3 position = new Vector3(Random.Range(-50.0f, 50.0f), 1.0f, Random.Range(-50.0f, 50.0f));

            // Probably the most efficient way to check for overlapping - @micktu
            bool isTooClose = Physics.SphereCast(position, radius, Vector3.one, out hit, 0.0f, layerMask);
            if (isTooClose) continue;

            var enemy = SpawnEnemy(position);
            enemy.Index = _enemies.Count;
            _enemies.Add(enemy);
            numEnemies++;
        }
    }

    public void Tick(float deltaTime)
    {
        foreach (Enemy enemy in _enemies)
        {
            enemy.Tick(deltaTime);
        }
    }

    public Enemy SpawnEnemy(Vector3 position)
    {
        Enemy newEnemy = Object.Instantiate(EnemyPrefab, position, Quaternion.identity);

        newEnemy.MaxHealth = Game.Config.EnemyMaxHealth;
        newEnemy.RespawnTime = Game.Config.EnemyRespawnTime;
        newEnemy.Init(this);

        return newEnemy;
    }

    public void KillEnemy(int index)
    {
        var enemy = _enemies[index];
        enemy.EnterState(EnemyState.Dead);
    }
}

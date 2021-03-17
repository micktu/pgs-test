using UnityEngine;


public enum EnemyState
{
    Idle,
    Dead
}


public class Enemy : MonoBehaviour
{
    public EnemyManager Manager;

    public int Index;

    public int MaxHealth;
    public int CurrentHealth;

    public float RespawnTime;


    private EnemyState _state;

    private float _stateTime;


    public void Init(EnemyManager manager)
    {
        Manager = manager;

        EnterState(EnemyState.Idle);
    }

    public void Tick(float deltaTime)
    {
        switch (_state)
        {
            case EnemyState.Idle:
                break;

            case EnemyState.Dead:
                if (_stateTime >= RespawnTime)
                {
                    EnterState(EnemyState.Idle);
                }
                break;
        }

        _stateTime += deltaTime;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            Manager.KillEnemy(Index);
        }
    }

    public void EnterState(EnemyState newState)
    {
        switch (newState)
        {
            case EnemyState.Idle:
                CurrentHealth = MaxHealth;
                gameObject.SetActive(true);
                break;

            case EnemyState.Dead:
                gameObject.SetActive(false);
                break;
        }

        _state = newState;
        _stateTime = 0.0f;
    }
}

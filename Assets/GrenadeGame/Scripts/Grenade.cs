using UnityEngine;


public enum GrenadeState
{
    Idle,
    PickedUp,
    Thrown
}


public class Grenade : MonoBehaviour
{
    public MeshRenderer[] Meshes;

    public Color Color;

    public int Index;

    public int TypeIndex;

    public float PickupRadius;

    public float ExplosionRadius;

    public float Speed;

    public float FuseTime;

    public Vector3 Velocity;

    public float Gravity;

    public float RespawnTime;

    private GrenadeManager _manager;

    private float _stateTime;
    
    private GrenadeState _state;

    private SphereCollider _collider;


    public void Init(GrenadeManager manager, float phase)
    {
        _manager = manager;
        _stateTime = phase;

        _collider = GetComponent<SphereCollider>();

        EnterState(GrenadeState.Idle);
    }

    public void Tick(float deltaTime)
    {
        switch (_state)
        {
            case GrenadeState.Idle:
                Vector3 position = transform.position;

                // FIXME Cosmetics are hardcoded - @micktu
                position.y = 1.0f + 0.25f * Mathf.Sin(2.0f * _stateTime);
                transform.position = position;

                transform.Rotate(Vector3.up, 30.0f * deltaTime);
                break;

            case GrenadeState.PickedUp:
                if (_stateTime >= RespawnTime)
                {
                    SetType(Random.Range(0, _manager.NumGrenadeTypes - 1));
                    EnterState(GrenadeState.Idle);
                }
                break;
            case GrenadeState.Thrown:
                // Protect grenade from detonating early because I'm to lazy for an extra collision layer - @micktu
                if (_stateTime > FuseTime)
                {
                    _collider.enabled = true;
                }

                Velocity.y -= Gravity * deltaTime * Speed;
                transform.position += Velocity * deltaTime * Speed;
                transform.Rotate(new Vector3(1.0f, 0.0f, 1.0f), 270.0f * deltaTime);
                break;
        }

        _stateTime += deltaTime;
    }

    public void SetType(int type)
    {
        TypeIndex = type;
        SetColor(_manager.Game.Config.GrenadeTypes[type].Color); // FIXME Bad, we don't need to access the config - @micktu
    }

    public void SetColor(Color color)
    {
        Color = color;

        foreach (MeshRenderer mesh in Meshes)
        {
            mesh.material.SetColor("_Color", color);
        }
    }

    public Color GetColor()
    {
        return Color;
    }

    public void SetCollisionDistance(float distance)
    {
        _collider.radius = distance;
    }

    public void EnterState(GrenadeState newState)
    {
        switch (newState)
        {
            case GrenadeState.Idle:
                gameObject.SetActive(true);
                _collider.isTrigger = true;
                _collider.enabled = true;
                SetCollisionDistance(PickupRadius);
                break;

            case GrenadeState.PickedUp:
                gameObject.SetActive(false);
                break;

            case GrenadeState.Thrown:
                _collider.isTrigger = false;
                _collider.enabled = false;
                SetCollisionDistance(ExplosionRadius);
                break;
        }

        _state = newState;
        _stateTime = 0.0f;
    }

    void OnCollisionEnter(Collision collision)
    {
        _manager.ExplodeGrenade(this);
    }
}

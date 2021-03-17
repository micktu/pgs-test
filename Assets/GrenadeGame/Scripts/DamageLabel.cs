using TMPro;
using UnityEngine;


public class DamageLabel : MonoBehaviour
{
    public TextMeshPro Text;

    public Transform LookAtTransform;

    public float InitialDistance = 0.5f;

    public float MaxDistance = 1.0f;

    public float Period = 2.0f;

    private Vector3 _targetPosition;

    private float _distance;

    private float _time;


    public void Init(Transform target, int amount, Transform lookAtTransform)
    {
        _targetPosition = target.position;

        LookAtTransform = lookAtTransform;
        SetDamage(amount);

        UpdateTransform();
    }

    public void Update()
    {
        if (_time >= Period)
        {
            Destroy(gameObject);
        }

        float alpha = _time / Period;
        _distance = InitialDistance + alpha * (MaxDistance - InitialDistance);

        UpdateTransform();

        Color color = Text.color;
        color.a = 1.0f - alpha;
        Text.color = color;

        _time += Time.deltaTime;
    }

    public void SetDamage(int damage)
    {
        Text.text = $"{damage}";
    }

    public void UpdateTransform()
    {
        transform.position = new Vector3(_targetPosition.x, _targetPosition.y + _distance, _targetPosition.z);
        transform.rotation = LookAtTransform.rotation;

    }
}

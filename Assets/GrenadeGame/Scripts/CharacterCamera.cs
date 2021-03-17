using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class CharacterCamera : MonoBehaviour
{
    private GrenadeCharacter _character;

    public float Distance = 10.0f;

    public Vector3 ExpectedPosition { get; private set; }
    public Quaternion ExpectedRotation { get; private set; }

    private Quaternion _lastRotation;


    public void Init(GrenadeCharacter character)
    {
        _character = character;
        _lastRotation = transform.rotation;
    }

    void Update()
    {
        if (_character == null) return;

        float x = CrossPlatformInputManager.GetAxis("Mouse X");
        float y = CrossPlatformInputManager.GetAxis("Mouse Y");

        float delta = Time.deltaTime * 100.0f;

        if (_character.State == CharacterState.Idle)
        {
            ExpectedRotation = _lastRotation;
        }

        // Not entirely correct because it's non-linear, but good enough.
        // Rotate by Euler angles because quaternion interpolation is relentless. - @micktu
        Vector3 angles = ExpectedRotation.eulerAngles;
        angles = new Vector3(angles.x - y * delta, angles.y + x * delta, 0.0f);
        ExpectedRotation = Quaternion.Euler(angles);
        ExpectedPosition = _character.transform.position + new Vector3(0.0f, 1.0f, 0.0f) - ExpectedRotation * Vector3.forward * Distance;

        if (_character.State == CharacterState.Idle)
        {
            transform.SetPositionAndRotation(ExpectedPosition, ExpectedRotation);
            _lastRotation = ExpectedRotation;
        }
    }
}

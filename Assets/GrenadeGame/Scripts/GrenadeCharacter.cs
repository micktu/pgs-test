using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;


public enum CharacterState
{
    Idle,
    Aiming
}


public class GrenadeCharacter : MonoBehaviour
{
    public CharacterCamera Camera;

    public AimingLine AimingLine;

    public Transform CameraBoom;
    
    public Transform ThrowMuzzle;
    
    public GrenadeGame Game;

    public Transform CameraTransform;
    
    public CharacterState State { get; private set; }


    private ThirdPersonCharacter _character;

    private float _stateTime;

    
    public void Init(GrenadeGame game)
    {
        Game = game;
        CameraTransform = game.CharacterCamera.transform;
        _character = GetComponent<ThirdPersonCharacter>();

        AimingLine.Init(this);

        EnterState(CharacterState.Idle);
    }

    private void Update()
    {
        if (CameraTransform == null) return;

        Vector3 cameraForward = CameraTransform.forward;

        switch (State)
        {
            case CharacterState.Idle:
                if (CrossPlatformInputManager.GetButtonDown("Next"))
                {
                    Game.SelectGrenadeNext();
                }
                else if (CrossPlatformInputManager.GetButtonDown("Previous"))
                {
                    Game.SelectGrenadePrevious();
                }

                if (CrossPlatformInputManager.GetButton("Fire1"))
                {
                    EnterState(CharacterState.Aiming);
                    break;
                }

                float h = CrossPlatformInputManager.GetAxis("Horizontal");
                float v = CrossPlatformInputManager.GetAxis("Vertical");

                Vector3 moveDirection = v * cameraForward + h * CameraTransform.right;

                bool isJumping = CrossPlatformInputManager.GetButtonDown("Jump");
                _character.Move(moveDirection, false, isJumping);

                break;

            case CharacterState.Aiming:
                _character.Move(Vector3.zero, false, false);

                AimingLine.Tick(Time.deltaTime);

                if (!CrossPlatformInputManager.GetButton("Fire1"))
                {
                    Game.ThrowGrenade(ThrowMuzzle.position, AimingLine.FireVelocity, AimingLine.Gravity);

                    EnterState(CharacterState.Idle);
                }

                break;
        }

        _stateTime += Time.deltaTime;
    }

    void OnTriggerEnter(Collider collider)
    {
        Grenade grenade = collider.GetComponent<Grenade>();
        if (grenade == null) return;

        Game.PickupGrenade(grenade);
    }

    public void EnterState(CharacterState newState)
    {
        switch (newState)
        {
            case CharacterState.Idle:
                AimingLine.gameObject.SetActive(false);
                break;
            case CharacterState.Aiming:
                AimingLine.gameObject.SetActive(true);
                break;
        }

        State = newState;
        _stateTime = 0.0f;
    }
}

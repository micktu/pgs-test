using UnityEngine;


public class AimingLine : MonoBehaviour
{
    public LineRenderer LineRenderer;

    public Transform ThrowMuzzle;

    public Vector3 FireVelocity;

    public float Gravity;

    public Vector3 ImpactPoint;


    private GrenadeCharacter _character;

    private Vector3[] _points;

    private Vector3 _lastDestination;


    public void Init(GrenadeCharacter grenadeCharacter)
    {
        GameConfig config = grenadeCharacter.Game.Config;

        _points = new Vector3[config.MaxTrajectoryPoints];
        _character = grenadeCharacter;
    }
    
    public void Tick(float deltaTime)
    {
        Vector3 cameraPosition = _character.Camera.ExpectedPosition;
        Vector3 cameraForward = _character.Camera.ExpectedRotation * Vector3.forward;

        // Calculate floor intersection point at y=0 - @micktu
        Vector3 destination = cameraPosition + cameraForward * (-cameraPosition.y / cameraForward.y);

        // Don't recalculate if the position is the same - @micktu
        if (Vector3.Distance(destination, _lastDestination) < 1e-5) return;
        _lastDestination = destination;

        GameConfig config = _character.Game.Config;

        // Calculate initial velocity and gravity - @micktu
        Utils.SolveBallisticArcLateral(ThrowMuzzle.position, config.GrenadeLateral, destination, Vector3.zero, config.GrenadeArc, out FireVelocity, out Gravity, out ImpactPoint);

        Vector3 velocity = FireVelocity;
        Vector3 lastPoint = ThrowMuzzle.position;

        // Calculate trajectory points - @micktu
        int numPoints = 0;
        while (numPoints < _points.Length)
        {
            // Integrate the next position - @micktu
            velocity.y -= Gravity;
            Vector3 newPoint = lastPoint + velocity;

            _points[numPoints] = newPoint;

            lastPoint = newPoint;
            numPoints++;

            // Break on intersection against XZ plane - @micktu
            if (lastPoint.y <= 0) break;
        }

        LineRenderer.positionCount = numPoints;
        LineRenderer.SetPositions(_points);
    }
}

using UnityEngine;


public static class Utils
{
    // https://www.forrestthewoods.com/blog/solving_ballistic_trajectories/
    public static bool SolveBallisticArcLateral(Vector3 position, float lateralSpeed, Vector3 target, Vector3 targetVelocity, float maxHeightOffset, out Vector3 velocity, out float gravity, out Vector3 impactPoint)
    {
        velocity = Vector3.zero;
        gravity = 0.0f;
        impactPoint = Vector3.zero;

        Vector3 targetVelocityXZ = new Vector3(targetVelocity.x, 0f, targetVelocity.z);
        Vector3 deltaXZ = target - position;
        deltaXZ.y = 0;

        float c0 = Vector3.Dot(targetVelocityXZ, targetVelocityXZ) - lateralSpeed * lateralSpeed;
        float c1 = 2.0f * Vector3.Dot(deltaXZ, targetVelocityXZ);
        float c2 = Vector3.Dot(deltaXZ, deltaXZ);
        int n = SolveQuadric(c0, c1, c2, out var t0, out var t1);

        bool isValid1 = n > 0 && t0 > 0;
        bool isValid2 = n > 1 && t1 > 0;

        float t;
        if (!isValid1 && !isValid2) return false;
        if (isValid1 && isValid2) t = Mathf.Min(t0, t1);
        else t = isValid1 ? t0 : t1;

        impactPoint = target + targetVelocity * t;

        Vector3 direction = impactPoint - position;
        velocity = new Vector3(direction.x, 0f, direction.z).normalized * lateralSpeed;

        float a = position.y;
        float b = Mathf.Max(position.y, impactPoint.y) + maxHeightOffset;
        float c = impactPoint.y;

        gravity = -4 * (a - 2 * b + c) / (t * t);
        velocity.y = -(3 * a - 4 * b + c) / t;

        return true;
    }

    // https://github.com/erich666/GraphicsGems/blob/240a34f2ad3fa577ef57be74920db6c4b00605e4/gems/Roots3And4.c
    public static int SolveQuadric(float c0, float c1, float c2, out float s0, out float s1)
    {
        s0 = float.NaN;
        s1 = float.NaN;

        float p, q, D;

        p = c1 / (2 * c0);
        q = c2 / c0;

        D = p * p - q;

        if (Mathf.Approximately(D, 1e-9f))
        {
            s0 = -p;
            return 1;
        }
        
        if (D < 0)
        {
            return 0;
        }

        float sqrt_D = Mathf.Sqrt(D);

        s0 = sqrt_D - p;
        s1 = -sqrt_D - p;
        return 2;
    }
}

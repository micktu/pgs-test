using UnityEngine;


public class Explosion : MonoBehaviour
{  
    public Light Light;

    public ParticleSystem LongestParticleSystem;
    
    public ParticleSystem ColorParticleSystem;
    

    public void SetColor(Color color)
    {
        Light.color = color;
        ColorParticleSystem.startColor = color;
    }

    public void Update()
    {
        if (!LongestParticleSystem.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}


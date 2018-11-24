using System.Collections.Generic;
using UnityEngine;

public class PaintImpactParticle : MonoBehaviour
{

    private ParticleSystem[] particles;
    private List<ParticleSystem.MainModule> particleModules = new List<ParticleSystem.MainModule>();

    private void Awake()
    {
        particles = GetComponentsInChildren<ParticleSystem>();

        for (int i = 0; i < particles.Length; i++)
        {
            particleModules.Add(particles[i].main);
        }
    }

    public void Initialise(Color color)
    {
        for (int i = 0; i < particleModules.Count; i++)
        {
            ParticleSystem.MainModule moduleCopy = particleModules[i];
            moduleCopy.startColor = color;
            particleModules[i] = moduleCopy;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public bool playAura = true; //파티클 제어 bool
    public ParticleSystem particleObject; //파티클시스템

    void Start()
    {
        playAura = true;
        particleObject.Play();
    }


    void Update()
    {
        if (playAura)
            particleObject.Play();
        else if (!playAura)
            particleObject.Stop();
    }
}

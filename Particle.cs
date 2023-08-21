using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public bool playAura = true; //��ƼŬ ���� bool
    public ParticleSystem particleObject; //��ƼŬ�ý���

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

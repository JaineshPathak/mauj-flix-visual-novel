using UnityEngine;

public class UIHandIconEvent : MonoBehaviour
{
    public ParticleSystem clickParticles;

    public void PlayClickEffect()
    {
        if (clickParticles == null)
            return;

        clickParticles.Play(true);
    }
}

using UnityEngine;

public class ShotEffectsManager : MonoBehaviour
{
    [SerializeField]
    GameObject muzzlePrefab;
    [SerializeField]
    GameObject impactPrefab;
    
    // [SerializeField]
    // AudioSource gunAudio

    ParticleSystem muzzleFlash;
    ParticleSystem impactEffect;

    //Create the impact effect for our shots
    public void Initialize()
    {
        // muzzleFlash = Instantiate(muzzlePrefab).GetComponent<ParticleSystem>();
        // impactEffect = Instantiate(impactPrefab).GetComponent<ParticleSystem>();
        muzzlePrefab.SetActive(true);
        muzzleFlash = muzzlePrefab.GetComponent<ParticleSystem>();

        impactPrefab.SetActive(true);
        impactEffect = impactPrefab.GetComponent<ParticleSystem>();
        
    }

    //Play muzzle flash and audio
    public void PlayShotEffects()
    {
            muzzleFlash.Stop(true);
            muzzleFlash.Play(true);
        // gunAudio.Stop();
        // gunAudio.Play();
    }

    //Play impact effect and target position
    public void PlayImpactEffect(Vector3 impactPosition)
    {
        impactEffect.transform.position = impactPosition;
        impactEffect.Stop();
        impactEffect.Play();

        if (impactEffect.isPlaying)
            Debug.Log("Playing == false");

    }
}
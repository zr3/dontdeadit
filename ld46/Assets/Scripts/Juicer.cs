using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class Juicer : MonoBehaviour
{
    [Header("Configuration")]
    public NoiseSettings CameraShakeProfile;
    public GameObject[] Fx;
    public AudioClip[] SelectSFX;
    public AudioClip[] NavSFX;
    public AudioClip[] EatSFX;
    public AudioClip[] PickSFX;

    [Header("References")]
    public CinemachineCameraShake Camera;
    public AudioMixerGroup uiSfxMixerGroup;

    private static Juicer _instance;
    public static Juicer Instance => _instance;

    // camera shake
    private NoiseSettings initialProfile;
    private float initialAmplitudeGain;
    private float initialFrequencyGain;
    private Coroutine shakeCoroutine;

    // sound
    private AudioSource audioSource;

    private void Awake()
    {
        _instance = this.CheckSingleton(_instance);
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = uiSfxMixerGroup;
    }

    public static void ShakeCamera(float intensity = 1f)
    {
        _instance.NonStaticShakeCamera(intensity);
    }

    private void NonStaticShakeCamera(float intensity)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(ShakeCoroutine());

        IEnumerator ShakeCoroutine()
        {
            for (float i = 0.5f; i > 0; i -= Time.deltaTime)
            {
                Camera.Blend = i * intensity * 2f;
                yield return null;
            }
            Camera.Blend = 0;
        }
    }

    public static void CreateFx(int fxIndex, Vector3 position, Quaternion rotation)
    {
        _instance.NonStaticCreateFx(fxIndex, position, rotation);
    }

    public static void CreateFx(int fxIndex, Vector3 position)
    {
        _instance.NonStaticCreateFx(fxIndex, position, Quaternion.identity);
    }

    private void NonStaticCreateFx(int fxIndex, Vector3 position, Quaternion rotation)
    {
        var gob = GobPool.Instantiate(Fx[fxIndex]);
        gob.transform.position = position;
        gob.transform.rotation = rotation;
    }

    public void PlaySelectSFX()
    {
        PlayNaturalClip(SelectSFX[Random.Range(0, SelectSFX.Length)]);
    }

    public void PlayNavSFX()
    {
        PlayNaturalClip(NavSFX[Random.Range(0, NavSFX.Length)]);
    }

    public void PlayEatSFX()
    {
        PlayNaturalClip(EatSFX[Random.Range(0, EatSFX.Length)]);
    }

    public void PlayPickSFX()
    {
        PlayNaturalClip(PickSFX[Random.Range(0, PickSFX.Length)]);
    }

    public void PlayNaturalClip(AudioClip clip)
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(clip, Random.Range(0.9f, 1.1f));
    }
}
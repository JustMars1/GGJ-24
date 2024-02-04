using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class AudioPool : MonoBehaviour
{
    public IObjectPool<AudioSource> pool { get; private set; }

    AudioSource _fadingInSource;
    AudioSource _fadingOutSource;

    float _fadeTimer;
    float _fadeTimerMax;

    void Awake()
    {
        pool = new ObjectPool<AudioSource>(CreateAudio, OnGetAudio, OnReleaseAudio, OnDestroyAudio);
    }

    public void ReleaseAfter(AudioSource audioSource, float delay)
    {
        StartCoroutine(ReleaseAfterCo(audioSource, delay));
    }

    public void Release(AudioSource audioSource)
    {
        if (audioSource.gameObject.activeInHierarchy)
        {
            pool.Release(audioSource);
        }
    }

    IEnumerator ReleaseAfterCo(AudioSource audioSource, float delay)
    {
        if (audioSource.ignoreListenerPause)
        {
            yield return new WaitForSecondsRealtime(delay);
        }
        else
        {
            yield return new WaitForSeconds(delay);
        }

        yield return new WaitWhile(() => audioSource.isPlaying);
        Release(audioSource);
    }

    AudioSource CreateAudio()
    {
        var source = new GameObject().AddComponent<AudioSource>();
        source.playOnAwake = false;
        return source;
    }

    void OnGetAudio(AudioSource audioSource)
    {
        audioSource.gameObject.SetActive(true);
    }

    void OnReleaseAudio(AudioSource audioSource)
    {
        audioSource.Stop();
        audioSource.gameObject.SetActive(false);
        audioSource.name = "Released";
    }

    void OnDestroyAudio(AudioSource audioSource)
    {
        Destroy(audioSource.gameObject);
    }
}
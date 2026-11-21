
using System.Collections;
using UnityEngine;

public class MusicaMenu : MonoBehaviour
{
    public AudioSource audioSource;
    public float duracaoFade = 2f;

    private Coroutine fadeAtual;

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource da música do menu não encontrado!");
            return;
        }

        audioSource.loop = true;
        audioSource.volume = 0f;
        audioSource.Play();

        FadeIn();
    }

    public void FadeIn()
    {
        if (audioSource == null)
            return;

        if (!audioSource.isPlaying)
            audioSource.Play();

        if (fadeAtual != null)
            StopCoroutine(fadeAtual);

        fadeAtual = StartCoroutine(
            AlterarVolume(1f)
        );
    }

    public void FadeOut()
    {
        if (audioSource == null)
            return;

        if (fadeAtual != null)
            StopCoroutine(fadeAtual);

        fadeAtual = StartCoroutine(
            AlterarVolume(0f)
        );
    }

    private IEnumerator AlterarVolume(float volumeDestino)
    {
        float volumeInicial = audioSource.volume;
        float tempo = 0f;

        while (tempo < duracaoFade)
        {
            tempo += Time.unscaledDeltaTime;

            audioSource.volume = Mathf.Lerp(
                volumeInicial,
                volumeDestino,
                tempo / duracaoFade
            );

            yield return null;
        }

        audioSource.volume = volumeDestino;

        if (volumeDestino == 0f)
            audioSource.Stop();

        fadeAtual = null;
    }
}

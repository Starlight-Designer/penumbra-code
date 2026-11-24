
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicaMenu : MonoBehaviour
{
    public AudioSource audioSource;
    public float duracaoFade = 2f;

    [Range(0f, 1f)]
    public float volumeMaximo = 1f;

    [Header("Controle de reprodução")]
    public bool iniciarAutomaticamente = true;

    private Coroutine fadeAtual;

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource não encontrado!");
            return;
        }

        audioSource.loop = true;

        // Permite que o GerenciadorPuzzlePeixes
        // controle a música na cena da peixaria.
        if (!iniciarAutomaticamente)
        {
            Debug.Log("MusicaMenu: reprodução automática desativada.");
            return;
        }

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

        fadeAtual = StartCoroutine(AlterarVolume(volumeMaximo));
    }

    public void FadeOut()
    {
        if (audioSource == null)
            return;

        if (fadeAtual != null)
            StopCoroutine(fadeAtual);

        fadeAtual = StartCoroutine(AlterarVolume(0f));
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

    public IEnumerator FadeOutECarregarCena(string nomeCena)
    {
        FadeOut();

        yield return new WaitForSecondsRealtime(duracaoFade);

        SceneManager.LoadScene(nomeCena);
    }
}

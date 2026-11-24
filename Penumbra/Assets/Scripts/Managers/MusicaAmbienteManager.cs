
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicaAmbienteManager : MonoBehaviour
{
    public static MusicaAmbienteManager Instance;

    [Header("Música ambiente")]
    public AudioSource musicaAmbiente;

    [Header("Fade")]
    public float duracaoFade = 1.5f;

    [Range(0f, 1f)]
    public float volumeMaximo = 1f;

    private Coroutine fadeAtual;
    private bool primeiraCena = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicaAmbiente == null)
            musicaAmbiente = GetComponent<AudioSource>();

        if (musicaAmbiente == null)
        {
            Debug.LogError("MusicaAmbienteManager: AudioSource não foi conectado!");
            return;
        }

        musicaAmbiente.loop = true;
        musicaAmbiente.playOnAwake = false;

        SceneManager.sceneLoaded += AoCarregarCena;
    }

    private void Start()
    {
        if (musicaAmbiente == null)
            return;

        musicaAmbiente.volume = 0f;

        if (!musicaAmbiente.isPlaying)
            musicaAmbiente.Play();

        IniciarFadeIn();
    }

    private void AoCarregarCena(Scene cena, LoadSceneMode modo)
    {
        if (primeiraCena)
        {
            primeiraCena = false;
            return;
        }

        IniciarFadeIn();
    }

    public IEnumerator FadeOutParaTroca()
    {
        if (musicaAmbiente == null || !musicaAmbiente.isPlaying)
            yield break;

        PararFadeAtual();

        yield return AlterarVolume(0f);

        // Pausa sem perder a posição atual da música.
        musicaAmbiente.Pause();
    }

    public void IniciarFadeIn()
    {
        if (musicaAmbiente == null)
            return;

        PararFadeAtual();

        musicaAmbiente.volume = 0f;

        if (musicaAmbiente.timeSamples == 0 && !musicaAmbiente.isPlaying)
            musicaAmbiente.Play();
        else
            musicaAmbiente.UnPause();

        fadeAtual = StartCoroutine(FadeInRotina());
    }

    private IEnumerator FadeInRotina()
    {
        yield return AlterarVolume(volumeMaximo);
        fadeAtual = null;
    }

    private IEnumerator AlterarVolume(float destino)
    {
        float volumeInicial = musicaAmbiente.volume;
        float tempo = 0f;

        if (duracaoFade <= 0f)
        {
            musicaAmbiente.volume = destino;
            yield break;
        }

        while (tempo < duracaoFade)
        {
            tempo += Time.unscaledDeltaTime;

            float progresso = Mathf.Clamp01(tempo / duracaoFade);

            musicaAmbiente.volume = Mathf.Lerp(
                volumeInicial,
                destino,
                progresso
            );

            yield return null;
        }

        musicaAmbiente.volume = destino;
    }

    private void PararFadeAtual()
    {
        if (fadeAtual != null)
        {
            StopCoroutine(fadeAtual);
            fadeAtual = null;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= AoCarregarCena;
            Instance = null;
        }
    }
}

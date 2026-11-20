
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class FadePreto : MonoBehaviour
{
    [Header("Tempos (segundos)")]
    [SerializeField] private float duracaoEscurecer = 2f;
    [SerializeField] private float tempoNoPreto = 0.5f;
    [SerializeField] private float duracaoClarear = 2.5f;

    [Header("Cena da cutscene (modo B)")]
    [SerializeField] private bool clarearAoIniciar = false;

    [Header("Áudio durante a transição")]
    public AudioSource audioSource;
    public AudioClip audioVidroQuebrando;
    public float duracaoFadeAudio = 1.5f;
    public float atrasoAudioAposFade = 0f;

    [Header("Eventos")]
    [SerializeField] private UnityEvent aoFicarPreto;
    [SerializeField] private UnityEvent aoComecarCutscene;

    private CanvasGroup grupo;
    private bool ocupado;

    public static bool TransicaoAtiva { get; private set; }

    private void Awake()
    {
        grupo = GetComponent<CanvasGroup>();
        grupo.alpha = clarearAoIniciar ? 1f : 0f;
        grupo.blocksRaycasts = clarearAoIniciar;

        if (clarearAoIniciar)
            TransicaoAtiva = true;
    }

    private void Start()
    {
        if (clarearAoIniciar)
            StartCoroutine(ClarearInicial());
    }

    public void Transicao()
    {
        if (ocupado) return;
        StartCoroutine(RotinaTransicao());
    }

    public void EscurecerECarregar(string nomeCena)
    {
        if (ocupado) return;
        StartCoroutine(RotinaCarregar(nomeCena));
    }

    // Método para iniciar a fuga após o diálogo com o pastor.
    public void IrParaIgrejaFuga()
    {
        EscurecerECarregar("Igreja_Fuga");
    }

    private IEnumerator RotinaTransicao()
    {
        ocupado = true;
        TransicaoAtiva = true;
        grupo.blocksRaycasts = true;

        yield return Fade(0f, 1f, duracaoEscurecer);
        aoFicarPreto?.Invoke();

        yield return new WaitForSecondsRealtime(tempoNoPreto);

        aoComecarCutscene?.Invoke();

        yield return Fade(1f, 0f, duracaoClarear);

        grupo.blocksRaycasts = false;
        ocupado = false;
        TransicaoAtiva = false;
    }

    private IEnumerator RotinaCarregar(string nomeCena)
    {
        ocupado = true;
        TransicaoAtiva = true;
        grupo.blocksRaycasts = true;

        // Primeiro, escurece a tela completamente.
        yield return Fade(0f, 1f, duracaoEscurecer);

        // Espera o tempo escolhido antes de tocar o vidro.
        if (atrasoAudioAposFade > 0f)
        {
            yield return new WaitForSecondsRealtime(
                atrasoAudioAposFade
            );
        }

        if (audioSource != null && audioVidroQuebrando != null)
        {
            float volumeInicial = audioSource.volume;

            audioSource.clip = audioVidroQuebrando;
            audioSource.volume = volumeInicial;
            audioSource.Play();

            float tempoFade = Mathf.Min(
                duracaoFadeAudio,
                audioVidroQuebrando.length
            );

            float tempoAguardar = audioVidroQuebrando.length - tempoFade;

            if (tempoAguardar > 0f)
                yield return new WaitForSecondsRealtime(tempoAguardar);

            yield return FadeAudio(
                audioSource,
                volumeInicial,
                0f,
                tempoFade
            );

            audioSource.Stop();
            audioSource.volume = volumeInicial;
        }
        else
        {
            yield return new WaitForSecondsRealtime(tempoNoPreto);
        }

        SceneManager.LoadScene(nomeCena);
    }

    private IEnumerator ClarearInicial()
    {
        ocupado = true;
        TransicaoAtiva = true;

        yield return new WaitForSecondsRealtime(tempoNoPreto);

        aoComecarCutscene?.Invoke();

        yield return Fade(1f, 0f, duracaoClarear);

        grupo.blocksRaycasts = false;
        ocupado = false;
        TransicaoAtiva = false;
    }

    private IEnumerator Fade(float de, float para, float duracao)
    {
        float t = 0f;

        if (duracao <= 0f)
        {
            grupo.alpha = para;
            yield break;
        }

        while (t < duracao)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duracao);
            float suave = p * p * (3f - 2f * p);

            grupo.alpha = Mathf.Lerp(de, para, suave);

            yield return null;
        }

        grupo.alpha = para;
    }

    private IEnumerator FadeAudio(
        AudioSource source,
        float volumeInicial,
        float volumeFinal,
        float duracao)
    {
        float t = 0f;

        if (duracao <= 0f)
        {
            source.volume = volumeFinal;
            yield break;
        }

        while (t < duracao)
        {
            t += Time.unscaledDeltaTime;

            float progresso = Mathf.Clamp01(t / duracao);

            source.volume = Mathf.Lerp(
                volumeInicial,
                volumeFinal,
                progresso
            );

            yield return null;
        }

        source.volume = volumeFinal;
    }
}

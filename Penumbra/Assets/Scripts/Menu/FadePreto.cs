
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

    [Header("Áudio padrão durante a transição")]
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

    // Transição SEM áudio.
    public void EscurecerECarregar(string nomeCena)
    {
        if (ocupado) return;

        StartCoroutine(
            RotinaCarregar(
                nomeCena,
                null,
                null,
                0f,
                0f
            )
        );
    }

    // Transição usando explicitamente o áudio padrão.
    // Utilizada pela Igreja_Porta.
    public void EscurecerECarregarComAudioPadrao(string nomeCena)
    {
        EscurecerECarregarComAudio(
            nomeCena,
            audioVidroQuebrando,
            audioSource,
            atrasoAudioAposFade,
            duracaoFadeAudio
        );
    }

    // Transição com áudio escolhido individualmente pelo ponto de saída.
    public void EscurecerECarregarComAudio(
        string nomeCena,
        AudioClip audio,
        AudioSource fonteAudio,
        float atraso,
        float duracaoFade
    )
    {
        if (ocupado) return;

        StartCoroutine(
            RotinaCarregar(
                nomeCena,
                audio,
                fonteAudio,
                atraso,
                duracaoFade
            )
        );
    }

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

    private IEnumerator RotinaCarregar(
        string nomeCena,
        AudioClip audio,
        AudioSource fonteAudio,
        float atraso,
        float duracaoFade
    )
    {
        ocupado = true;
        TransicaoAtiva = true;
        grupo.blocksRaycasts = true;

        // Escurece a tela.
        yield return Fade(0f, 1f, duracaoEscurecer);

        // Aguarda o atraso configurado.
        if (atraso > 0f)
            yield return new WaitForSecondsRealtime(atraso);

        // Só reproduz áudio quando um clip foi informado.
        if (audio != null)
        {
            if (fonteAudio != null)
            {
                float volumeInicial = fonteAudio.volume;

                fonteAudio.PlayOneShot(audio);

                float tempoFade = Mathf.Min(
                    duracaoFade,
                    audio.length
                );

                float tempoAguardar = audio.length - tempoFade;

                if (tempoAguardar > 0f)
                {
                    yield return new WaitForSecondsRealtime(
                        tempoAguardar
                    );
                }

                yield return FadeAudio(
                    fonteAudio,
                    volumeInicial,
                    0f,
                    tempoFade
                );

                fonteAudio.Stop();
                fonteAudio.volume = volumeInicial;
            }
            else
            {
                AudioSource.PlayClipAtPoint(
                    audio,
                    Camera.main != null
                        ? Camera.main.transform.position
                        : Vector3.zero
                );

                yield return new WaitForSecondsRealtime(audio.length);
            }
        }
        else
        {
            // Sem áudio, utiliza a espera normal.
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
        float duracao
    )
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


using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutsceneInicialManager : MonoBehaviour
{
    [Header("Vídeo")]
    public VideoPlayer reprodutorVideo;

    [Header("Texto para pular")]
    public GameObject textoPularCutscene;
    public CanvasGroup grupoTextoPular;
    public float duracaoFadeTextoPular = 1f;
    public float duracaoFadeOutTextoPular = 0.5f;

    [Header("Painel de pausa")]
    public GameObject painelPausaCutscene;

    [Header("Tela preta de transição")]
    public CanvasGroup telaPreta;
    public float duracaoFadePreto = 1.5f;

    [Header("Destino")]
    public string nomeCenaDestino = "Cena_Teste";

    private bool cutsceneFinalizada = false;
    private bool cutscenePausada = false;

    private void Start()
    {
        if (reprodutorVideo == null)
        {
            Debug.LogError("CutsceneInicialManager: VideoPlayer não atribuído!");
            return;
        }

        if (textoPularCutscene != null)
            textoPularCutscene.SetActive(false);

        if (painelPausaCutscene != null)
            painelPausaCutscene.SetActive(false);

        if (grupoTextoPular != null)
            grupoTextoPular.alpha = 0f;

        if (telaPreta != null)
        {
            telaPreta.alpha = 0f;
            telaPreta.gameObject.SetActive(false);
        }

        // Evita que o vídeo comece antes de estar preparado.
        reprodutorVideo.playOnAwake = false;
        reprodutorVideo.waitForFirstFrame = true;

        reprodutorVideo.loopPointReached += FinalizarCutscene;

        StartCoroutine(PrepararVideo());
    }

    private IEnumerator PrepararVideo()
    {
        reprodutorVideo.Prepare();

        // Aguarda o Unity terminar de preparar o vídeo.
        while (!reprodutorVideo.isPrepared && !cutsceneFinalizada)
        {
            yield return null;
        }

        if (cutsceneFinalizada)
            yield break;

        Debug.Log("Vídeo preparado. Iniciando cutscene.");

        reprodutorVideo.Play();

        StartCoroutine(MostrarTextoPular());
    }

    private void Update()
    {
        if (cutsceneFinalizada || reprodutorVideo == null)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!cutscenePausada)
                PausarCutscene();
            else
                ContinuarCutscene();
        }

        if (!cutscenePausada && Input.GetKeyDown(KeyCode.E))
        {
            PularCutscene();
        }
    }

    private void PausarCutscene()
    {
        cutscenePausada = true;

        reprodutorVideo.Pause();

        if (painelPausaCutscene != null)
            painelPausaCutscene.SetActive(true);
    }

    public void ContinuarCutscene()
    {
        if (cutsceneFinalizada || reprodutorVideo == null)
            return;

        cutscenePausada = false;

        if (painelPausaCutscene != null)
            painelPausaCutscene.SetActive(false);

        reprodutorVideo.Play();
    }

    public void PularCutscene()
    {
        FinalizarAbertura();
    }

    private void FinalizarCutscene(VideoPlayer video = null)
    {
        FinalizarAbertura();
    }

    private void FinalizarAbertura()
    {
        if (cutsceneFinalizada)
            return;

        cutsceneFinalizada = true;

        StopAllCoroutines();

        StartCoroutine(EncerrarCutscene());
    }

    private IEnumerator EncerrarCutscene()
    {
        if (textoPularCutscene != null &&
            textoPularCutscene.activeSelf &&
            grupoTextoPular != null)
        {
            yield return StartCoroutine(
                FadeCanvas(
                    grupoTextoPular,
                    grupoTextoPular.alpha,
                    0f,
                    duracaoFadeOutTextoPular
                )
            );
        }

        if (textoPularCutscene != null)
            textoPularCutscene.SetActive(false);

        if (painelPausaCutscene != null)
            painelPausaCutscene.SetActive(false);

        if (reprodutorVideo != null)
            reprodutorVideo.Stop();

        PlayerPrefs.SetInt("CutsceneInicialAssistida", 1);
        PlayerPrefs.Save();

        if (telaPreta != null)
        {
            telaPreta.gameObject.SetActive(true);
            telaPreta.alpha = 0f;

            yield return StartCoroutine(
                FadeCanvas(
                    telaPreta,
                    0f,
                    1f,
                    duracaoFadePreto
                )
            );
        }

        Time.timeScale = 1f;

        SceneManager.LoadScene(nomeCenaDestino);
    }

    private IEnumerator FadeCanvas(
        CanvasGroup grupo,
        float inicio,
        float fim,
        float duracao
    )
    {
        if (grupo == null)
            yield break;

        grupo.alpha = inicio;

        if (duracao <= 0f)
        {
            grupo.alpha = fim;
            yield break;
        }

        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.unscaledDeltaTime;

            grupo.alpha = Mathf.Lerp(
                inicio,
                fim,
                Mathf.Clamp01(tempo / duracao)
            );

            yield return null;
        }

        grupo.alpha = fim;
    }

    private IEnumerator MostrarTextoPular()
    {
        yield return new WaitForSecondsRealtime(3f);

        if (cutsceneFinalizada)
            yield break;

        if (textoPularCutscene == null ||
            grupoTextoPular == null)
            yield break;

        textoPularCutscene.SetActive(true);
        grupoTextoPular.alpha = 0f;

        float tempo = 0f;

        while (tempo < duracaoFadeTextoPular)
        {
            if (cutsceneFinalizada)
                yield break;

            tempo += Time.unscaledDeltaTime;

            grupoTextoPular.alpha = Mathf.Lerp(
                0f,
                1f,
                Mathf.Clamp01(tempo / duracaoFadeTextoPular)
            );

            yield return null;
        }

        grupoTextoPular.alpha = 1f;
    }

    private void OnDestroy()
    {
        if (reprodutorVideo != null)
        {
            reprodutorVideo.loopPointReached -= FinalizarCutscene;
        }
    }
}

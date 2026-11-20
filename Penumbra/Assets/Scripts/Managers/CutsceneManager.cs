
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using TMPro;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [Header("Cutscene")]
    public VideoPlayer reprodutorVideo;
    public GameObject textoPularCutscene;
    public GameObject painelPausaCutscene;

    [Header("Fade do texto Pular Cutscene")]
    public CanvasGroup grupoTextoPular;
    public float duracaoFadeTextoPular = 1f;
    public float duracaoFadeOutTextoPular = 0.5f;

    [Header("Tela preta final")]
    public CanvasGroup telaPretaFinal;
    public float duracaoFadePreto = 1.5f;
    public float esperaAntesTexto = 2f;

    [Header("Mensagem final")]
    public TMP_Text textoFinal;
    public CanvasGroup grupoTextoFinal;
    public float duracaoFadeTexto = 1f;
    public float velocidadeLetras = 0.06f;
    public float tempoTextoNaTela = 5f;

    [Header("Destino")]
    public string nomeCenaDestino = "Menu_Inicial";

    private bool cutsceneFinalizada = false;
    private bool cutscenePausada = false;

    void Start()
    {
        textoPularCutscene.SetActive(false);
        painelPausaCutscene.SetActive(false);

        grupoTextoPular.alpha = 0f;

        telaPretaFinal.alpha = 0f;
        telaPretaFinal.gameObject.SetActive(false);

        grupoTextoFinal.alpha = 0f;
        grupoTextoFinal.gameObject.SetActive(false);

        textoFinal.maxVisibleCharacters = 0;

        reprodutorVideo.loopPointReached += FinalizarCutscene;

        StartCoroutine(MostrarTextoPular());
    }

    void Update()
    {
        if (cutsceneFinalizada)
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

    void PausarCutscene()
    {
        cutscenePausada = true;
        reprodutorVideo.Pause();
        painelPausaCutscene.SetActive(true);
    }

    public void ContinuarCutscene()
    {
        if (cutsceneFinalizada)
            return;

        cutscenePausada = false;
        painelPausaCutscene.SetActive(false);
        reprodutorVideo.Play();
    }

    public void PularCutscene()
    {
        IniciarFinal();
    }

    void FinalizarCutscene(VideoPlayer video = null)
    {
        IniciarFinal();
    }

    void IniciarFinal()
    {
        if (cutsceneFinalizada)
            return;

        cutsceneFinalizada = true;

        StopCoroutine("MostrarTextoPular");

        StartCoroutine(FinalizarComFadeTexto());
    }

    IEnumerator FinalizarComFadeTexto()
    {
        // Faz o fade out do texto, caso esteja visível.
        if (textoPularCutscene.activeSelf)
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

        textoPularCutscene.SetActive(false);

        // Depois começa a sequência final.
        StartCoroutine(SequenciaFinal());
    }

    IEnumerator SequenciaFinal()
    {
        // Interrompe o vídeo.
        reprodutorVideo.Stop();

        textoPularCutscene.SetActive(false);
        painelPausaCutscene.SetActive(false);

        PlayerPrefs.SetInt("CutsceneInicialAssistida", 1);
        PlayerPrefs.Save();

        DialogueManager.DialogoGlobalAtivo = false;

        // Fade In da tela preta.
        telaPretaFinal.gameObject.SetActive(true);
        telaPretaFinal.alpha = 0f;

        yield return StartCoroutine(
            FadeCanvas(telaPretaFinal, 0f, 1f, duracaoFadePreto)
        );

        // Espera dois segundos no escuro.
        yield return new WaitForSecondsRealtime(esperaAntesTexto);

        // Prepara a mensagem.
        grupoTextoFinal.gameObject.SetActive(true);
        grupoTextoFinal.alpha = 0f;

        textoFinal.ForceMeshUpdate();
        textoFinal.maxVisibleCharacters = 0;

        // Fade In do texto.
        yield return StartCoroutine(
            FadeCanvas(grupoTextoFinal, 0f, 1f, duracaoFadeTexto)
        );

        // Letras aparecem uma por uma.
        int quantidadeLetras = textoFinal.textInfo.characterCount;

        for (int i = 0; i <= quantidadeLetras; i++)
        {
            textoFinal.maxVisibleCharacters = i;

            yield return new WaitForSecondsRealtime(
                velocidadeLetras
            );
        }

        // Mantém o texto visível por cinco segundos.
        yield return new WaitForSecondsRealtime(tempoTextoNaTela);

        // Fade Out do texto final.
        yield return StartCoroutine(
            FadeCanvas(grupoTextoFinal, 1f, 0f, duracaoFadeTexto)
        );

        // Carrega o menu inicial.
        SceneManager.LoadScene(nomeCenaDestino);
    }

    IEnumerator FadeCanvas(
        CanvasGroup grupo,
        float inicio,
        float fim,
        float duracao)
    {
        float tempo = 0f;

        grupo.alpha = inicio;

        if (duracao <= 0f)
        {
            grupo.alpha = fim;
            yield break;
        }

        while (tempo < duracao)
        {
            tempo += Time.unscaledDeltaTime;

            float progresso = Mathf.Clamp01(tempo / duracao);

            grupo.alpha = Mathf.Lerp(
                inicio,
                fim,
                progresso
            );

            yield return null;
        }

        grupo.alpha = fim;
    }

    IEnumerator MostrarTextoPular()
    {
        // Aguarda três segundos.
        yield return new WaitForSecondsRealtime(3f);

        if (cutsceneFinalizada)
            yield break;

        textoPularCutscene.SetActive(true);
        grupoTextoPular.alpha = 0f;

        // Fade In do texto.
        float tempo = 0f;

        while (tempo < duracaoFadeTextoPular)
        {
            if (cutsceneFinalizada)
                yield break;

            tempo += Time.unscaledDeltaTime;

            float progresso = Mathf.Clamp01(
                tempo / duracaoFadeTextoPular
            );

            grupoTextoPular.alpha = progresso;

            yield return null;
        }

        grupoTextoPular.alpha = 1f;
    }

    void OnDestroy()
    {
        if (reprodutorVideo != null)
        {
            reprodutorVideo.loopPointReached -= FinalizarCutscene;
        }
    }
}

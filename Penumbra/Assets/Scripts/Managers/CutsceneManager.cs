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

    [Header("Áudio após o vídeo")]
    public AudioSource audioAmbienteNoite;

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

    [Header("Pausa entre as frases")]
    public float esperaEntreFrases = 2f;

    [Header("Destino")]
    public string nomeCenaDestino = "Menu_Inicial";

    [Header("Marque SOMENTE na Cutscene_Fuga (fim do jogo)")]
    public bool ehCutsceneFinal = false;

    private bool cutsceneFinalizada = false;
    private bool cutscenePausada = false;

    private const string CHAVE_REINICIAR_APOS_FINAL =
        "ReiniciarAoVoltarMenu";

    private const string CHAVE_RETORNAR_FLORESTA =
        "RetornarFlorestaAposDerrota";

    private const string CHAVE_CUTSCENE_PENDENTE =
        "CutscenePendenteMenu";

    void Start()
    {
        textoPularCutscene.SetActive(false);
        painelPausaCutscene.SetActive(false);

        grupoTextoPular.alpha = 0f;

        telaPretaFinal.alpha = 0f;
        telaPretaFinal.gameObject.SetActive(false);

        if (grupoTextoFinal != null)
        {
            grupoTextoFinal.alpha = 0f;
            grupoTextoFinal.gameObject.SetActive(false);
        }

        if (textoFinal != null)
            textoFinal.maxVisibleCharacters = 0;

        // Garante que o áudio ambiente só comece após o vídeo.
        if (audioAmbienteNoite != null)
        {
            audioAmbienteNoite.playOnAwake = false;
            audioAmbienteNoite.Stop();
        }

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

        // Inicia o áudio ambiente após o vídeo terminar ou ser pulado.
        if (audioAmbienteNoite != null)
        {
            audioAmbienteNoite.Play();
        }

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

    // Marca que o jogo deve ser reiniciado quando o menu abrir.
    // O MenuManager apaga o progresso no Start() ao ver esta chave.
    void MarcarReinicioDoJogo()
    {
        // Só a cutscene final reinicia o jogo.
        if (!ehCutsceneFinal)
            return;

        PlayerPrefs.SetInt(CHAVE_REINICIAR_APOS_FINAL, 1);

        PlayerPrefs.DeleteKey(CHAVE_RETORNAR_FLORESTA);
        PlayerPrefs.DeleteKey(CHAVE_CUTSCENE_PENDENTE);

        PlayerPrefs.Save();
    }

    IEnumerator SequenciaFinal()
    {
        // Interrompe o vídeo.
        reprodutorVideo.Stop();

        textoPularCutscene.SetActive(false);
        painelPausaCutscene.SetActive(false);

        // A cutscene final não altera o registro
        // de que a cutscene inicial foi assistida.

        DialogueManager.DialogoGlobalAtivo = false;

        // Fim do jogo: o progresso será apagado ao abrir o menu.
        // Marcado aqui no início para valer mesmo que o jogador
        // feche o jogo durante a sequência final.
        MarcarReinicioDoJogo();

        // Fade In da tela preta.
        telaPretaFinal.gameObject.SetActive(true);
        telaPretaFinal.alpha = 0f;

        yield return StartCoroutine(
            FadeCanvas(telaPretaFinal, 0f, 1f, duracaoFadePreto)
        );

        // Espera antes de começar o texto.
        yield return new WaitForSecondsRealtime(esperaAntesTexto);

        // A mensagem final só aparece se os campos de texto estiverem preenchidos.
        if (grupoTextoFinal != null && textoFinal != null)
        {
            // Prepara o painel da mensagem.
            grupoTextoFinal.gameObject.SetActive(true);
            grupoTextoFinal.alpha = 0f;

            // Fade In do painel.
            yield return StartCoroutine(
                FadeCanvas(grupoTextoFinal, 0f, 1f, duracaoFadeTexto)
            );

            // PRIMEIRA FRASE.
            textoFinal.text = "Nem todo mundo tem finais felizes...";

            yield return StartCoroutine(MostrarLetras());

            // Espera dois segundos antes da segunda frase.
            yield return new WaitForSecondsRealtime(esperaEntreFrases);

            // SEGUNDA FRASE.
            textoFinal.text = "E pelo visto você teve o mesmo destino que eu... Alice.";

            yield return StartCoroutine(MostrarLetras());

            // Mantém o texto completo visível.
            yield return new WaitForSecondsRealtime(tempoTextoNaTela);

            // Fade Out do texto final.
            yield return StartCoroutine(
                FadeCanvas(grupoTextoFinal, 1f, 0f, duracaoFadeTexto)
            );
        }

        // Garante a marcação de reinício antes de ir ao menu.
        MarcarReinicioDoJogo();

        Time.timeScale = 1f;

        // Carrega o menu inicial.
        SceneManager.LoadScene(nomeCenaDestino);
    }

    IEnumerator MostrarLetras()
    {
        textoFinal.ForceMeshUpdate();
        textoFinal.maxVisibleCharacters = 0;

        int quantidadeLetras = textoFinal.textInfo.characterCount;

        for (int i = 0; i <= quantidadeLetras; i++)
        {
            textoFinal.maxVisibleCharacters = i;

            yield return new WaitForSecondsRealtime(velocidadeLetras);
        }
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
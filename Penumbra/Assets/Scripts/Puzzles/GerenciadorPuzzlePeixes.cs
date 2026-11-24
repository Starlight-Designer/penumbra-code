
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GerenciadorPuzzlePeixes : MonoBehaviour
{
    [Header("Painel do puzzle")]
    public GameObject painelPuzzlePeixes;

    [Header("Popup do puzzle")]
    public GameObject painelPopupPeixes;

    [Header("Bloqueio dos peixes")]
    public CanvasGroup grupoInteracaoPeixes;

    [Header("Som do popup")]
    public AudioSource audioPopup;

    [Header("Músicas")]
    public AudioSource musicaAmbiente;
    public AudioSource florestaAmbiente;
    public AudioSource musicaPuzzle;

    [Header("Animação do popup")]
    public float velocidadeFadeIn = 0.15f;
    public float velocidadeFadeOut = 0.15f;

    [Header("Popups extras")]
    public int quantidadePopups = 40;
    public float intervaloEntrePopups = 0.06f;
    public float proximidadeCursor = 40f;

    [Header("Cutscene")]
    public CutscenePeixesManager cutscenePeixesManager;

    private const string CHAVE_PUZZLE_ATIVO = "PuzzlePeixesEmAndamento";
    private const string CHAVE_QUANTIDADE = "PeixesOrganizados";
    private const string CHAVE_POPUP10 = "PopupPeixes10Mostrado";
    private const string CHAVE_POPUP15 = "PopupPeixes15Mostrado";
    private const string CHAVE_POPUP_ABERTO = "PopupPeixesAberto";
    private const string CHAVE_POPUPS_EXTRAS = "PopupsPeixesExtrasIniciados";
    private const string CHAVE_POPUPS_CRIADOS = "QuantidadePopupsPeixesCriados";

    private int peixesOrganizados = 0;

    private CanvasGroup grupoPopup;
    private Coroutine animacaoAtual;
    private Coroutine rotinaPopupsExtras;

    private bool popupsJaApareceram = false;
    private bool musicaPuzzleIniciada = false;
    private bool painelPuzzleJaAtivado = false;

    private bool popup10Mostrado = false;
    private bool popup15Mostrado = false;
    private bool sequenciaPopupsExtrasEmAndamento = false;

    private int popupsExtrasCriados = 0;

    private List<AudioSource> sonsPopupsExtras =
        new List<AudioSource>();

    private void Awake()
    {
        if (painelPopupPeixes != null)
        {
            grupoPopup =
                painelPopupPeixes.GetComponent<CanvasGroup>();

            if (grupoPopup == null)
                Debug.LogError(
                    "Adicione um CanvasGroup ao PainelPopupPeixes!"
                );
        }

        if (grupoInteracaoPeixes == null)
        {
            Debug.LogError(
                "Conecte o Grupo Interacao Peixes no Inspector!"
            );
        }

        if (musicaPuzzle != null)
            musicaPuzzle.Stop();
    }

    private void Start()
    {
        CarregarProgresso();

        // Músicas ambientes.
        if (musicaAmbiente != null)
        {
            musicaAmbiente.Stop();
            musicaAmbiente.Play();
        }

        if (florestaAmbiente != null)
        {
            florestaAmbiente.Stop();
            florestaAmbiente.Play();
        }

        if (musicaPuzzle != null)
            musicaPuzzle.Stop();

        musicaPuzzleIniciada = false;

        // Se havia um puzzle em andamento, abre novamente o painel.
        if (PlayerPrefs.GetInt(CHAVE_PUZZLE_ATIVO, 0) == 1)
        {
            if (painelPuzzlePeixes != null)
                painelPuzzlePeixes.SetActive(true);

            Debug.Log("Retomando o puzzle dos peixes!");
        }

        // Restaura a música de acordo com o progresso.
        if (painelPuzzlePeixes != null &&
            painelPuzzlePeixes.activeInHierarchy)
        {
            painelPuzzleJaAtivado = true;

            if (peixesOrganizados < 10)
                IniciarMusicaPuzzle();
            else
                PararMusicaPuzzle();
        }

        // Se o jogador saiu enquanto um popup estava aberto,
        // mostra novamente o popup.
        if (PlayerPrefs.GetInt(CHAVE_POPUP_ABERTO, 0) == 1)
        {
            MostrarPopup();
        }

        // Continua a sequência de popups extras, se necessário.
        if (sequenciaPopupsExtrasEmAndamento)
        {
            rotinaPopupsExtras =
                StartCoroutine(AparecerPopupsExtras());
        }

        Debug.Log(
            "Progresso carregado. Peixes organizados: " +
            peixesOrganizados
        );
    }

    private void CarregarProgresso()
    {
        peixesOrganizados =
            PlayerPrefs.GetInt(CHAVE_QUANTIDADE, 0);

        popup10Mostrado =
            PlayerPrefs.GetInt(CHAVE_POPUP10, 0) == 1;

        popup15Mostrado =
            PlayerPrefs.GetInt(CHAVE_POPUP15, 0) == 1;

        popupsJaApareceram =
            PlayerPrefs.GetInt(CHAVE_POPUPS_EXTRAS, 0) == 1;

        sequenciaPopupsExtrasEmAndamento =
            PlayerPrefs.GetInt("SequenciaPopupsPeixesEmAndamento", 0) == 1;

        popupsExtrasCriados =
            PlayerPrefs.GetInt(CHAVE_POPUPS_CRIADOS, 0);
    }

    private void SalvarProgresso()
    {
        PlayerPrefs.SetInt(CHAVE_PUZZLE_ATIVO, 1);

        PlayerPrefs.SetInt(
            CHAVE_QUANTIDADE,
            peixesOrganizados
        );

        PlayerPrefs.SetInt(
            CHAVE_POPUP10,
            popup10Mostrado ? 1 : 0
        );

        PlayerPrefs.SetInt(
            CHAVE_POPUP15,
            popup15Mostrado ? 1 : 0
        );

        PlayerPrefs.SetInt(
            CHAVE_POPUP_ABERTO,
            painelPopupPeixes != null &&
            painelPopupPeixes.activeSelf ? 1 : 0
        );

        PlayerPrefs.SetInt(
            CHAVE_POPUPS_EXTRAS,
            popupsJaApareceram ? 1 : 0
        );

        PlayerPrefs.SetInt(
            "SequenciaPopupsPeixesEmAndamento",
            sequenciaPopupsExtrasEmAndamento ? 1 : 0
        );

        PlayerPrefs.SetInt(
            CHAVE_POPUPS_CRIADOS,
            popupsExtrasCriados
        );

        PlayerPrefs.Save();
    }

    private void Update()
    {
        // Registra que o puzzle foi iniciado.
        if (painelPuzzlePeixes != null &&
            painelPuzzlePeixes.activeInHierarchy)
        {
            if (PlayerPrefs.GetInt(CHAVE_PUZZLE_ATIVO, 0) == 0)
            {
                PlayerPrefs.SetInt(CHAVE_PUZZLE_ATIVO, 1);
                PlayerPrefs.Save();
            }

            if (!painelPuzzleJaAtivado)
            {
                painelPuzzleJaAtivado = true;
                IniciarMusicaPuzzle();
            }
        }

        if (painelPopupPeixes == null ||
            peixesOrganizados < 15 ||
            !painelPopupPeixes.activeInHierarchy ||
            popupsJaApareceram ||
            sequenciaPopupsExtrasEmAndamento)
            return;

        RectTransform rectPopup =
            painelPopupPeixes.GetComponent<RectTransform>();

        Canvas canvas =
            painelPopupPeixes.GetComponentInParent<Canvas>();

        if (rectPopup == null || canvas == null)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectPopup,
            Input.mousePosition,
            canvas.worldCamera,
            out Vector2 posicaoLocal
        );

        Rect areaPopup = rectPopup.rect;

        areaPopup.xMin -= proximidadeCursor;
        areaPopup.xMax += proximidadeCursor;
        areaPopup.yMin -= proximidadeCursor;
        areaPopup.yMax += proximidadeCursor;

        if (areaPopup.Contains(posicaoLocal))
        {
            popupsJaApareceram = true;
            sequenciaPopupsExtrasEmAndamento = true;

            SalvarProgresso();

            rotinaPopupsExtras =
                StartCoroutine(AparecerPopupsExtras());
        }
    }

    public void RegistrarPeixe()
    {
        peixesOrganizados++;

        Debug.Log("Peixes organizados: " + peixesOrganizados);

        if (peixesOrganizados == 10)
        {
            PararMusicaPuzzle();
        }

        if (peixesOrganizados == 10 && !popup10Mostrado)
        {
            popup10Mostrado = true;

            Debug.Log("BIANCA NÃO ESTÁ AQUI.");

            MostrarPopup();
        }

        if (peixesOrganizados == 15 && !popup15Mostrado)
        {
            popup15Mostrado = true;

            Debug.Log("BIANCA NÃO ESTÁ AQUI.");

            MostrarPopup();
        }

        SalvarProgresso();
    }

    private void IniciarMusicaPuzzle()
    {
        if (musicaPuzzleIniciada)
            return;

        musicaPuzzleIniciada = true;

        if (musicaAmbiente != null)
        {
            musicaAmbiente.Stop();
            Debug.Log("Música ambiente interrompida!");
        }

        if (musicaPuzzle != null)
        {
            musicaPuzzle.Play();
            Debug.Log("Música do puzzle iniciada!");
        }
        else
        {
            Debug.LogWarning(
                "MusicaPuzzle não foi conectada no Inspector!"
            );
        }
    }

    private void PararMusicaPuzzle()
    {
        if (musicaPuzzle != null && musicaPuzzle.isPlaying)
        {
            musicaPuzzle.Stop();

            Debug.Log("Música do puzzle interrompida!");
        }
    }

    private void BloquearInteracaoPeixes()
    {
        if (grupoInteracaoPeixes == null)
            return;

        grupoInteracaoPeixes.interactable = false;
        grupoInteracaoPeixes.blocksRaycasts = false;

        Debug.Log("Interação com os peixes bloqueada!");
    }

    private void LiberarInteracaoPeixes()
    {
        if (grupoInteracaoPeixes == null)
            return;

        grupoInteracaoPeixes.interactable = true;
        grupoInteracaoPeixes.blocksRaycasts = true;

        Debug.Log("Interação com os peixes liberada!");
    }

    private void MostrarPopup()
    {
        if (painelPopupPeixes == null)
            return;

        BloquearInteracaoPeixes();

        painelPopupPeixes.SetActive(true);

        PlayerPrefs.SetInt(CHAVE_POPUP_ABERTO, 1);
        PlayerPrefs.Save();

        if (audioPopup != null)
        {
            audioPopup.time = 0.3f;
            audioPopup.Play();
        }

        if (grupoPopup != null)
        {
            grupoPopup.alpha = 0f;
            grupoPopup.interactable = true;
            grupoPopup.blocksRaycasts = true;

            if (animacaoAtual != null)
                StopCoroutine(animacaoAtual);

            animacaoAtual = StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeIn()
    {
        while (grupoPopup.alpha < 1f)
        {
            grupoPopup.alpha +=
                Time.deltaTime / velocidadeFadeIn;

            yield return null;
        }

        grupoPopup.alpha = 1f;
        animacaoAtual = null;
    }

    public void FecharPopup()
    {
        if (painelPopupPeixes != null && grupoPopup != null)
        {
            if (animacaoAtual != null)
                StopCoroutine(animacaoAtual);

            animacaoAtual = StartCoroutine(FadeOut());
        }
        else if (painelPopupPeixes != null)
        {
            painelPopupPeixes.SetActive(false);

            PlayerPrefs.SetInt(CHAVE_POPUP_ABERTO, 0);
            PlayerPrefs.Save();

            LiberarInteracaoPeixes();
        }
    }

    private IEnumerator FadeOut()
    {
        grupoPopup.interactable = false;
        grupoPopup.blocksRaycasts = false;

        while (grupoPopup.alpha > 0f)
        {
            grupoPopup.alpha -=
                Time.deltaTime / velocidadeFadeOut;

            yield return null;
        }

        grupoPopup.alpha = 0f;
        painelPopupPeixes.SetActive(false);
        animacaoAtual = null;

        PlayerPrefs.SetInt(CHAVE_POPUP_ABERTO, 0);
        PlayerPrefs.Save();

        LiberarInteracaoPeixes();
    }

    private IEnumerator AparecerPopupsExtras()
    {
        RectTransform areaCanvas =
            painelPopupPeixes.transform.parent as RectTransform;

        if (areaCanvas == null)
            yield break;

        for (int i = popupsExtrasCriados;
             i < quantidadePopups;
             i++)
        {
            GameObject novoPopup = Instantiate(
                painelPopupPeixes,
                painelPopupPeixes.transform.parent
            );

            novoPopup.SetActive(true);

            RectTransform rectNovo =
                novoPopup.GetComponent<RectTransform>();

            rectNovo.anchorMin = new Vector2(0.5f, 0.5f);
            rectNovo.anchorMax = new Vector2(0.5f, 0.5f);

            float limiteX = Mathf.Max(
                0f,
                areaCanvas.rect.width / 2f -
                rectNovo.rect.width / 2f
            );

            float limiteY = Mathf.Max(
                0f,
                areaCanvas.rect.height / 2f -
                rectNovo.rect.height / 2f
            );

            rectNovo.anchoredPosition = new Vector2(
                Random.Range(-limiteX, limiteX),
                Random.Range(-limiteY, limiteY)
            );

            rectNovo.localRotation = Quaternion.Euler(
                0f,
                0f,
                Random.Range(-12f, 12f)
            );

            CanvasGroup grupoNovo =
                novoPopup.GetComponent<CanvasGroup>();

            if (grupoNovo != null)
            {
                grupoNovo.alpha = 1f;
                grupoNovo.interactable = false;
                grupoNovo.blocksRaycasts = false;
            }

            AudioSource somNovoPopup =
                novoPopup.GetComponent<AudioSource>();

            if (somNovoPopup != null)
            {
                somNovoPopup.time = 0.3f;
                somNovoPopup.Play();

                sonsPopupsExtras.Add(somNovoPopup);
            }

            popupsExtrasCriados++;
            SalvarProgresso();

            yield return new WaitForSeconds(
                intervaloEntrePopups
            );
        }

        sequenciaPopupsExtrasEmAndamento = false;
        SalvarProgresso();

        if (cutscenePeixesManager != null)
        {
            Debug.Log("Iniciando cutscene dos peixes!");

            PararSonsDosPopups();

            LimparProgressoPuzzle();

            cutscenePeixesManager.IniciarCutscene();
        }
        else
        {
            Debug.LogError(
                "CutscenePeixesManager não foi conectado no Inspector!"
            );
        }

        rotinaPopupsExtras = null;
    }

    private void PararSonsDosPopups()
    {
        if (audioPopup != null)
            audioPopup.Stop();

        foreach (AudioSource som in sonsPopupsExtras)
        {
            if (som != null)
                som.Stop();
        }

        sonsPopupsExtras.Clear();
    }

    private void LimparProgressoPuzzle()
    {
        PlayerPrefs.DeleteKey(CHAVE_PUZZLE_ATIVO);
        PlayerPrefs.DeleteKey(CHAVE_QUANTIDADE);
        PlayerPrefs.DeleteKey(CHAVE_POPUP10);
        PlayerPrefs.DeleteKey(CHAVE_POPUP15);
        PlayerPrefs.DeleteKey(CHAVE_POPUP_ABERTO);
        PlayerPrefs.DeleteKey(CHAVE_POPUPS_EXTRAS);
        PlayerPrefs.DeleteKey(CHAVE_POPUPS_CRIADOS);
        PlayerPrefs.DeleteKey("SequenciaPopupsPeixesEmAndamento");

        PixeArrastavel[] peixes =
            FindObjectsByType<PixeArrastavel>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (PixeArrastavel peixe in peixes)
        {
            if (peixe != null)
                peixe.LimparEstadoSalvo();
        }

        PlayerPrefs.Save();
    }
}


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

    private int peixesOrganizados = 0;

    private CanvasGroup grupoPopup;
    private Coroutine animacaoAtual;

    private bool popupsJaApareceram = false;
    private bool musicaPuzzleIniciada = false;
    private bool painelPuzzleJaAtivado = false;

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
        // Ao entrar ou retornar à peixaria,
        // as duas músicas ambientes começam juntas.

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

        Debug.Log("Músicas ambientes da peixaria iniciadas!");
    }

    private void Update()
    {
        // Começa a música do puzzle quando o painel aparece.
        if (painelPuzzlePeixes != null &&
            painelPuzzlePeixes.activeInHierarchy &&
            !painelPuzzleJaAtivado)
        {
            painelPuzzleJaAtivado = true;
            IniciarMusicaPuzzle();
        }

        // Verifica a proximidade do cursor para os pop-ups extras.
        if (painelPopupPeixes == null ||
            peixesOrganizados < 15 ||
            !painelPopupPeixes.activeInHierarchy ||
            popupsJaApareceram)
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
            StartCoroutine(AparecerPopupsExtras());
        }
    }

    public void RegistrarPeixe()
    {
        peixesOrganizados++;

        Debug.Log("Peixes organizados: " + peixesOrganizados);

        // No décimo peixe, aparece o primeiro popup.
        // A música do puzzle para e a floresta continua.

        if (peixesOrganizados == 10)
        {
            PararMusicaPuzzle();
        }

        if (peixesOrganizados == 10 || peixesOrganizados == 15)
        {
            Debug.Log("BIANCA NÃO ESTÁ AQUI.");

            MostrarPopup();
        }
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

        // Bloqueia os peixes enquanto o popup estiver aberto.
        BloquearInteracaoPeixes();

        painelPopupPeixes.SetActive(true);

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

        LiberarInteracaoPeixes();
    }

    private IEnumerator AparecerPopupsExtras()
    {
        RectTransform areaCanvas =
            painelPopupPeixes.transform.parent as RectTransform;

        if (areaCanvas == null)
            yield break;

        for (int i = 0; i < quantidadePopups; i++)
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

            yield return new WaitForSeconds(
                intervaloEntrePopups
            );
        }

        if (cutscenePeixesManager != null)
        {
            Debug.Log("Iniciando cutscene dos peixes!");

            PararSonsDosPopups();

            cutscenePeixesManager.IniciarCutscene();
        }
        else
        {
            Debug.LogError(
                "CutscenePeixesManager não foi conectado no Inspector!"
            );
        }
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
}

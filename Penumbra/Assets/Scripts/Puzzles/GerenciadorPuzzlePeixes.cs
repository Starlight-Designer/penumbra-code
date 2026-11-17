
using UnityEngine;
using System.Collections;

public class GerenciadorPuzzlePeixes : MonoBehaviour
{
    [Header("Popup do puzzle")]
    public GameObject painelPopupPeixes;

    [Header("Som do popup")]
    public AudioSource audioPopup;

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

    private void Awake()
    {
        if (painelPopupPeixes != null)
        {
            grupoPopup = painelPopupPeixes.GetComponent<CanvasGroup>();

            if (grupoPopup == null)
                Debug.LogError("Adicione um CanvasGroup ao PainelPopupPeixes!");
        }
    }

    private void Update()
    {
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

        if (peixesOrganizados == 10 || peixesOrganizados == 15)
        {
            Debug.Log("BIANCA NÃO ESTÁ AQUI.");

            MostrarPopup();
        }
    }

    private void MostrarPopup()
    {
        if (painelPopupPeixes == null)
            return;

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
            grupoPopup.alpha += Time.deltaTime / velocidadeFadeIn;
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
        }
    }

    private IEnumerator FadeOut()
    {
        grupoPopup.interactable = false;
        grupoPopup.blocksRaycasts = false;

        while (grupoPopup.alpha > 0f)
        {
            grupoPopup.alpha -= Time.deltaTime / velocidadeFadeOut;
            yield return null;
        }

        grupoPopup.alpha = 0f;
        painelPopupPeixes.SetActive(false);
        animacaoAtual = null;
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
                areaCanvas.rect.width / 2f - rectNovo.rect.width / 2f
            );

            float limiteY = Mathf.Max(
                0f,
                areaCanvas.rect.height / 2f - rectNovo.rect.height / 2f
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

            // Reproduz o áudio em cada popup extra.
            AudioSource somNovoPopup =
                novoPopup.GetComponent<AudioSource>();

            if (somNovoPopup != null)
            {
                somNovoPopup.time = 0.3f;
                somNovoPopup.Play();
            }

            yield return new WaitForSeconds(intervaloEntrePopups);
        }

        // Inicia a cutscene depois que todos os popups aparecerem.
        if (cutscenePeixesManager != null)
        {
            Debug.Log("Iniciando cutscene dos peixes!");

            cutscenePeixesManager.IniciarCutscene();
        }
        else
        {
            Debug.LogError(
                "CutscenePeixesManager não foi conectado no Inspector!"
            );
        }
    }
}

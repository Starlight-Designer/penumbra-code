
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

    private int peixesOrganizados = 0;
    private CanvasGroup grupoPopup;
    private Coroutine animacaoAtual;

    private void Awake()
    {
        if (painelPopupPeixes != null)
        {
            grupoPopup = painelPopupPeixes.GetComponent<CanvasGroup>();

            if (grupoPopup == null)
                Debug.LogError("Adicione um CanvasGroup ao PainelPopupPeixes!");
        }
    }

    public void RegistrarPeixe()
    {
        peixesOrganizados++;

        Debug.Log("Peixes organizados: " + peixesOrganizados);

        if (peixesOrganizados == 10)
        {
            Debug.Log("BIANCA NÃO ESTÁ AQUI.");

            if (painelPopupPeixes != null)
            {
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
}

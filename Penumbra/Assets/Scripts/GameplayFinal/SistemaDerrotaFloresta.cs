
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SistemaDerrotaFloresta : MonoBehaviour
{
    [Header("Painel de derrota")]
    public GameObject painelDerrota;
    public CanvasGroup grupoPainel;

    [Header("Texto")]
    public TMP_Text textoDerrota;
    public float velocidadeLetras = 0.05f;

    [Header("Botões")]
    public CanvasGroup grupoBotaoSim;
    public CanvasGroup grupoBotaoNao;

    [Header("Animações")]
    public float duracaoFadeIn = 1f;
    public float duracaoFadeTexto = 0.5f;
    public float duracaoFadeBotoes = 0.7f;
    public float duracaoFadeOut = 0.5f;

    [Header("Cenas")]
    public string cenaRecomecar = "Floresta_Fuga";
    public string cenaMenu = "Menu_Inicial";

    private bool derrotado = false;
    private bool saindo = false;

    private void Start()
    {
        if (painelDerrota == null)
        {
            Debug.LogError("Painel de derrota não foi atribuído!");
            return;
        }

        if (grupoPainel == null)
            grupoPainel = painelDerrota.GetComponent<CanvasGroup>();

        if (grupoPainel == null)
            grupoPainel = painelDerrota.AddComponent<CanvasGroup>();

        grupoPainel.alpha = 0f;
        grupoPainel.interactable = false;
        grupoPainel.blocksRaycasts = false;

        if (textoDerrota != null)
            textoDerrota.maxVisibleCharacters = 0;

        PrepararBotao(grupoBotaoSim);
        PrepararBotao(grupoBotaoNao);

        painelDerrota.SetActive(false);
    }

    private void PrepararBotao(CanvasGroup grupo)
    {
        if (grupo == null)
            return;

        grupo.alpha = 0f;
        grupo.interactable = false;
        grupo.blocksRaycasts = false;

        HoverBotao hover = grupo.GetComponent<HoverBotao>();

        if (hover != null)
            hover.Bloquear();
    }

    public void MostrarDerrota()
    {
        if (derrotado || painelDerrota == null)
            return;

        derrotado = true;

        Time.timeScale = 0f;

        painelDerrota.SetActive(true);

        StartCoroutine(SequenciaDerrota());
    }

    private IEnumerator SequenciaDerrota()
    {
        // 1. Fade In do painel
        yield return StartCoroutine(
            FadeGrupo(grupoPainel, 0f, 1f, duracaoFadeIn)
        );

        // 2. Texto aparecendo letra por letra
        if (textoDerrota != null)
        {
            textoDerrota.maxVisibleCharacters = 0;
            textoDerrota.ForceMeshUpdate();

            int quantidadeLetras = textoDerrota.textInfo.characterCount;

            for (int i = 0; i < quantidadeLetras; i++)
            {
                textoDerrota.maxVisibleCharacters = i + 1;

                yield return new WaitForSecondsRealtime(velocidadeLetras);
            }

            textoDerrota.maxVisibleCharacters = quantidadeLetras;
        }

        // 3. Fade In do botão SIM
        if (grupoBotaoSim != null)
        {
            grupoBotaoSim.blocksRaycasts = true;

            yield return StartCoroutine(
                FadeGrupo(
                    grupoBotaoSim,
                    0f,
                    1f,
                    duracaoFadeBotoes
                )
            );
        }

        // 4. Fade In do botão NÃO
        if (grupoBotaoNao != null)
        {
            grupoBotaoNao.blocksRaycasts = true;

            yield return StartCoroutine(
                FadeGrupo(
                    grupoBotaoNao,
                    0f,
                    1f,
                    duracaoFadeBotoes
                )
            );
        }

        // 5. Liberar interação depois de toda a sequência
        grupoPainel.interactable = true;
        grupoPainel.blocksRaycasts = true;

        if (grupoBotaoSim != null)
        {
            grupoBotaoSim.interactable = true;

            HoverBotao hoverSim =
                grupoBotaoSim.GetComponent<HoverBotao>();

            if (hoverSim != null)
                hoverSim.Liberar();
        }

        if (grupoBotaoNao != null)
        {
            grupoBotaoNao.interactable = true;

            HoverBotao hoverNao =
                grupoBotaoNao.GetComponent<HoverBotao>();

            if (hoverNao != null)
                hoverNao.Liberar();
        }
    }

    private IEnumerator FadeGrupo(
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

    public void Recomecar()
    {
        if (saindo || !derrotado)
            return;

        StartCoroutine(
            FadeOutECarregarCena(cenaRecomecar)
        );
    }

    public void VoltarAoMenu()
    {
        if (saindo || !derrotado)
            return;

        StartCoroutine(
            FadeOutECarregarCena(cenaMenu)
        );
    }

    private IEnumerator FadeOutECarregarCena(string nomeCena)
    {
        saindo = true;

        grupoPainel.interactable = false;
        grupoPainel.blocksRaycasts = true;

        yield return StartCoroutine(
            FadeGrupo(
                grupoPainel,
                grupoPainel.alpha,
                0f,
                duracaoFadeOut
            )
        );

        Time.timeScale = 1f;

        SceneManager.LoadScene(nomeCena);
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SistemaDerrotaFloresta : MonoBehaviour
{
    [Header("Painel de derrota")]
    public GameObject painelDerrota;
    public CanvasGroup grupoPainel;

    [Header("Áudios")]
    public AudioSource audioNoite;
    public AudioSource musicaPerseguicao;

    [Header("Fade da música de perseguição")]
    public float duracaoFadeOutPerseguicao = 2f;

    [Header("Fade do áudio da noite")]
    public float duracaoFadeInNoite = 2f;
    public float duracaoFadeOutNoite = 1.5f;

    private float volumeOriginalNoite;
    private float volumeOriginalPerseguicao;

    [Header("Texto")]
    public TMP_Text textoDerrota;
    public float velocidadeLetras = 0.05f;

    [Header("Pausa entre as frases")]
    public float esperaEntreFrases = 2f;

    [Header("Mensagens da derrota")]
    public string primeiraFrase = "Alice, você falhou...";
    public string segundaFrase = "Mas eu te dou mais uma chance de sobreviver, você quer ser diferente?";

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

    private const string CHAVE_REINICIAR_APOS_FINAL =
        "ReiniciarAoVoltarMenu";

    private const string CHAVE_RETORNAR_FLORESTA =
        "RetornarFlorestaAposDerrota";

    private const string CHAVE_CUTSCENE_PENDENTE =
        "CutscenePendenteMenu";

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

        // Guarda o volume original do áudio noturno.
        if (audioNoite != null)
        {
            volumeOriginalNoite = audioNoite.volume;
            audioNoite.playOnAwake = false;
            audioNoite.Stop();
        }

        // Guarda o volume original da perseguição.
        if (musicaPerseguicao != null)
        {
            volumeOriginalPerseguicao = musicaPerseguicao.volume;
        }

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

        // Inicia as transições de áudio.
        StartCoroutine(TransicaoAudioDerrota());

        StartCoroutine(SequenciaDerrota());
    }

    private IEnumerator TransicaoAudioDerrota()
    {
        // FADE OUT DA MÚSICA DE PERSEGUIÇÃO.
        if (musicaPerseguicao != null && musicaPerseguicao.isPlaying)
        {
            float volumeInicial = musicaPerseguicao.volume;
            float tempo = 0f;

            while (tempo < duracaoFadeOutPerseguicao)
            {
                tempo += Time.unscaledDeltaTime;

                float progresso = Mathf.Clamp01(
                    tempo / duracaoFadeOutPerseguicao
                );

                musicaPerseguicao.volume = Mathf.Lerp(
                    volumeInicial,
                    0f,
                    progresso
                );

                yield return null;
            }

            musicaPerseguicao.volume = 0f;
            musicaPerseguicao.Stop();

            // Restaura o volume para uma próxima reprodução.
            musicaPerseguicao.volume = volumeOriginalPerseguicao;
        }

        // FADE IN DO ÁUDIO NOTURNO.
        if (audioNoite != null)
        {
            audioNoite.Stop();
            audioNoite.volume = 0f;
            audioNoite.Play();

            yield return StartCoroutine(
                FadeAudio(
                    audioNoite,
                    0f,
                    volumeOriginalNoite,
                    duracaoFadeInNoite
                )
            );
        }
    }

    private IEnumerator SequenciaDerrota()
    {
        // 1. Fade In do painel.
        yield return StartCoroutine(
            FadeGrupo(grupoPainel, 0f, 1f, duracaoFadeIn)
        );

        // 2. Primeira frase.
        if (textoDerrota != null)
        {
            textoDerrota.text = primeiraFrase;

            yield return StartCoroutine(MostrarLetras());

            // 3. Espera entre as frases.
            yield return new WaitForSecondsRealtime(esperaEntreFrases);

            // 4. Segunda frase.
            textoDerrota.text = segundaFrase;

            yield return StartCoroutine(MostrarLetras());
        }

        // 5. Fade In do botão SIM.
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

        // 6. Fade In do botão NÃO.
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

        // 7. Libera as interações.
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

    private IEnumerator MostrarLetras()
    {
        textoDerrota.ForceMeshUpdate();
        textoDerrota.maxVisibleCharacters = 0;

        int quantidadeLetras = textoDerrota.textInfo.characterCount;

        for (int i = 0; i < quantidadeLetras; i++)
        {
            textoDerrota.maxVisibleCharacters = i + 1;

            yield return new WaitForSecondsRealtime(velocidadeLetras);
        }

        textoDerrota.maxVisibleCharacters = quantidadeLetras;
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

    private IEnumerator FadeAudio(
        AudioSource audio,
        float inicio,
        float fim,
        float duracao
    )
    {
        if (audio == null)
            yield break;

        audio.volume = inicio;

        if (duracao <= 0f)
        {
            audio.volume = fim;
            yield break;
        }

        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.unscaledDeltaTime;

            float progresso = Mathf.Clamp01(tempo / duracao);

            audio.volume = Mathf.Lerp(
                inicio,
                fim,
                progresso
            );

            yield return null;
        }

        audio.volume = fim;
    }

    public void Recomecar()
    {
        if (saindo || !derrotado)
            return;

        // Remove qualquer indicação antiga de retorno à floresta
        // e qualquer pedido de reinício.
        PlayerPrefs.DeleteKey(CHAVE_RETORNAR_FLORESTA);
        PlayerPrefs.DeleteKey(CHAVE_REINICIAR_APOS_FINAL);
        PlayerPrefs.Save();

        StartCoroutine(
            FadeOutECarregarCena(cenaRecomecar)
        );
    }

    public void VoltarAoMenu()
    {
        if (saindo || !derrotado)
            return;

        // NÃO reinicia o jogo: o progresso é mantido.
        PlayerPrefs.DeleteKey(CHAVE_REINICIAR_APOS_FINAL);

        // Ao apertar Jogar no menu, volta para a Floresta_Fuga.
        PlayerPrefs.SetInt(CHAVE_RETORNAR_FLORESTA, 1);

        // Evita que uma cutscene antiga salva tenha prioridade.
        PlayerPrefs.DeleteKey(CHAVE_CUTSCENE_PENDENTE);

        // Evita carregar uma posição salva de outra cena.
        PlayerPrefs.SetInt("CarregarPosicaoSalva", 0);

        PlayerPrefs.Save();

        StartCoroutine(
            FadeOutECarregarCena(cenaMenu)
        );
    }

    private IEnumerator FadeOutECarregarCena(string nomeCena)
    {
        saindo = true;

        grupoPainel.interactable = false;
        grupoPainel.blocksRaycasts = true;

        // FADE OUT DO PAINEL.
        yield return StartCoroutine(
            FadeGrupo(
                grupoPainel,
                grupoPainel.alpha,
                0f,
                duracaoFadeOut
            )
        );

        // FADE OUT DO ÁUDIO NOTURNO.
        if (audioNoite != null && audioNoite.isPlaying)
        {
            yield return StartCoroutine(
                FadeAudio(
                    audioNoite,
                    audioNoite.volume,
                    0f,
                    duracaoFadeOutNoite
                )
            );

            audioNoite.Stop();

            // Restaura o volume original.
            audioNoite.volume = volumeOriginalNoite;
        }

        Time.timeScale = 1f;

        SceneManager.LoadScene(nomeCena);
    }
}
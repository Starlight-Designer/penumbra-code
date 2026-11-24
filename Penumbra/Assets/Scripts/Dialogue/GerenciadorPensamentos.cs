
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class GerenciadorPensamentos : MonoBehaviour
{
    [Header("Painel de pensamentos")]
    public GameObject painelPensamentos;
    public TextMeshProUGUI nomePensamento;
    public TextMeshProUGUI textoPensamento;

    [Header("Imagem da Alice")]
    public Image imagemAlice;
    public Sprite aliceBocaFechada;
    public Sprite aliceBocaAberta;

    [Header("Animações")]
    public float velocidadeTexto = 0.03f;
    public float intervaloAnimacaoBoca = 0.2f;

    [Header("Áudio da Alice")]
    public AudioSource audioVozAlice;
    public AudioClip vozAlice;
    [Range(0f, 1f)]
    public float volumeVoz = 1f;

    [Header("Sprites - Pensamento 1 da segunda fita")]
    public Sprite fita2Pensamento1Fechada;
    public Sprite fita2Pensamento1Aberta;

    [Header("Sprites - Pensamento 2 da segunda fita")]
    public Sprite fita2Pensamento2Fechada;
    public Sprite fita2Pensamento2Aberta;

    [Header("Sprites - Pensamento 3 da segunda fita")]
    public Sprite fita2Pensamento3Fechada;
    public Sprite fita2Pensamento3Aberta;

    [Header("Sprites - Pensamento 4 da segunda fita")]
    public Sprite fita2Pensamento4Fechada;
    public Sprite fita2Pensamento4Aberta;

    [Header("Sprites - Pensamento 5 da segunda fita")]
    public Sprite fita2Pensamento5Fechada;
    public Sprite fita2Pensamento5Aberta;

    [Header("Sprites - Pensamento 1 da terceira fita")]
    public Sprite fita3Pensamento1Fechada;
    public Sprite fita3Pensamento1Aberta;

    [Header("Sprites - Pensamento 2 da terceira fita")]
    public Sprite fita3Pensamento2Fechada;
    public Sprite fita3Pensamento2Aberta;

    [Header("Sprites - Pensamento 3 da terceira fita")]
    public Sprite fita3Pensamento3Fechada;
    public Sprite fita3Pensamento3Aberta;

    [Header("Sprites - Pensamento 4 da terceira fita")]
    public Sprite fita3Pensamento4Fechada;
    public Sprite fita3Pensamento4Aberta;

    [Header("Sprites - Pensamento 5 da terceira fita")]
    public Sprite fita3Pensamento5Fechada;
    public Sprite fita3Pensamento5Aberta;

    [Header("Sprites - Pensamento 6 da terceira fita")]
    public Sprite fita3Pensamento6Fechada;
    public Sprite fita3Pensamento6Aberta;

    private bool pensamentoAtivo = false;
    private bool textoDigitando = false;

    private Coroutine digitacaoTexto;
    private Coroutine animacaoBoca;

    private Action acaoAoFechar;

    private Sprite bocaFechadaAtual;
    private Sprite bocaAbertaAtual;

    private const string CHAVE_INICIAR_PENSAMENTOS_FITA1 =
        "IniciarPensamentosPosFita1";

    private const string CHAVE_BLOQUEIO_FITA1 =
        "BloquearAlicePosFita1";

    private const string CHAVE_INICIAR_PENSAMENTOS_FITA2 =
        "IniciarPensamentosPosFita2";

    private const string CHAVE_BLOQUEIO_FITA2 =
        "BloquearAlicePosFita2";

    private const string CHAVE_INICIAR_PENSAMENTOS_FITA3 =
        "IniciarPensamentosPosFita3";

    private const string CHAVE_BLOQUEIO_FITA3 =
        "BloquearAlicePosFita3";

    void Start()
    {
        painelPensamentos.SetActive(false);

        PararAudioVoz();

        bocaFechadaAtual = aliceBocaFechada;
        bocaAbertaAtual = aliceBocaAberta;

        bool bloqueioFita1 =
            PlayerPrefs.GetInt(CHAVE_BLOQUEIO_FITA1, 0) == 1;

        bool bloqueioFita2 =
            PlayerPrefs.GetInt(CHAVE_BLOQUEIO_FITA2, 0) == 1;

        bool bloqueioFita3 =
            PlayerPrefs.GetInt(CHAVE_BLOQUEIO_FITA3, 0) == 1;

        if (bloqueioFita1 || bloqueioFita2 || bloqueioFita3)
        {
            DialogueManager.DialogoGlobalAtivo = true;
        }

        if (PlayerPrefs.GetInt(CHAVE_INICIAR_PENSAMENTOS_FITA1, 0) == 1)
        {
            PlayerPrefs.SetInt(CHAVE_INICIAR_PENSAMENTOS_FITA1, 0);
            PlayerPrefs.Save();

            StartCoroutine(IniciarPensamentosAutomaticamente(1));
        }
        else if (PlayerPrefs.GetInt(CHAVE_INICIAR_PENSAMENTOS_FITA2, 0) == 1)
        {
            PlayerPrefs.SetInt(CHAVE_INICIAR_PENSAMENTOS_FITA2, 0);
            PlayerPrefs.Save();

            StartCoroutine(IniciarPensamentosAutomaticamente(2));
        }
        else if (PlayerPrefs.GetInt(CHAVE_INICIAR_PENSAMENTOS_FITA3, 0) == 1)
        {
            PlayerPrefs.SetInt(CHAVE_INICIAR_PENSAMENTOS_FITA3, 0);
            PlayerPrefs.Save();

            StartCoroutine(IniciarPensamentosAutomaticamente(3));
        }
    }

    private IEnumerator IniciarPensamentosAutomaticamente(int numeroFita)
    {
        yield return null;

        DialogueManager.DialogoGlobalAtivo = true;

        if (numeroFita == 1)
            IniciarPensamentosPosFita1();
        else if (numeroFita == 2)
            IniciarPensamentosPosFita2();
        else if (numeroFita == 3)
            IniciarPensamentosPosFita3();
    }

    void Update()
    {
        if (!pensamentoAtivo || !Input.GetKeyDown(KeyCode.F))
            return;

        if (textoDigitando)
        {
            CompletarTexto();
        }
        else
        {
            FecharPensamento();
        }
    }

    // =========================================================
    // ÁUDIO DA ALICE
    // =========================================================

    private void IniciarAudioVoz()
    {
        PararAudioVoz();

        if (audioVozAlice == null || vozAlice == null)
            return;

        audioVozAlice.clip = vozAlice;
        audioVozAlice.volume = volumeVoz;
        audioVozAlice.loop = true;
        audioVozAlice.Play();
    }

    private void PararAudioVoz()
    {
        if (audioVozAlice != null && audioVozAlice.isPlaying)
            audioVozAlice.Stop();
    }

    // =========================================================
    // MOSTRAR PENSAMENTOS
    // =========================================================

    public void MostrarPensamento(string mensagem)
    {
        MostrarPensamento(mensagem, null);
    }

    public void MostrarPensamento(string mensagem, Action aoFechar)
    {
        MostrarPensamento(
            mensagem,
            aoFechar,
            aliceBocaFechada,
            aliceBocaAberta
        );
    }

    public void MostrarPensamento(
        string mensagem,
        Action aoFechar,
        Sprite rostoFechado,
        Sprite rostoAberto
    )
    {
        if (pensamentoAtivo)
            return;

        pensamentoAtivo = true;
        acaoAoFechar = aoFechar;

        bocaFechadaAtual = rostoFechado != null
            ? rostoFechado
            : aliceBocaFechada;

        bocaAbertaAtual = rostoAberto != null
            ? rostoAberto
            : aliceBocaAberta;

        DialogueManager.DialogoGlobalAtivo = true;

        painelPensamentos.SetActive(true);

        nomePensamento.text = "Alice";
        textoPensamento.text = mensagem;

        if (imagemAlice != null)
            imagemAlice.sprite = bocaFechadaAtual;

        IniciarDigitacao();
        IniciarAnimacaoBoca();
    }

    // =========================================================
    // PRIMEIRA FITA VHS
    // =========================================================

    private void IniciarPensamentosPosFita1()
    {
        MostrarPensamento(
            "Nossa...",
            MostrarSegundoPensamento
        );
    }

    private void MostrarSegundoPensamento()
    {
        MostrarPensamento(
            "Eu preciso descobrir mais sobre esse caso.",
            MostrarTerceiroPensamento
        );
    }

    private void MostrarTerceiroPensamento()
    {
        MostrarPensamento(
            "Vou à peixaria conversar com o pescador. Talvez ele saiba algo sobre este caso.",
            FinalizarPensamentosPosFita1
        );
    }

    private void FinalizarPensamentosPosFita1()
    {
        PlayerPrefs.SetInt(CHAVE_BLOQUEIO_FITA1, 0);
        PlayerPrefs.Save();

        DialogueManager.DialogoGlobalAtivo = false;

        Debug.Log("Pensamentos da primeira fita finalizados.");
    }

    // =========================================================
    // SEGUNDA FITA VHS
    // =========================================================

    private void IniciarPensamentosPosFita2()
    {
        MostrarPensamento(
            "Esse cara...",
            MostrarSegundoPensamentoFita2,
            fita2Pensamento1Fechada,
            fita2Pensamento1Aberta
        );
    }

    private void MostrarSegundoPensamentoFita2()
    {
        MostrarPensamento(
            "Será que?...",
            MostrarTerceiroPensamentoFita2,
            fita2Pensamento2Fechada,
            fita2Pensamento2Aberta
        );
    }

    private void MostrarTerceiroPensamentoFita2()
    {
        MostrarPensamento(
            "Eu preciso ser enxerida mesmo que eu seja presa.",
            MostrarQuartoPensamentoFita2,
            fita2Pensamento3Fechada,
            fita2Pensamento3Aberta
        );
    }

    private void MostrarQuartoPensamentoFita2()
    {
        MostrarPensamento(
            "Bianca, eu vou descobrir o que aconteceu com você, custe o que custar.",
            MostrarQuintoPensamentoFita2,
            fita2Pensamento4Fechada,
            fita2Pensamento4Aberta
        );
    }

    private void MostrarQuintoPensamentoFita2()
    {
        MostrarPensamento(
            "Obrigada, Dona Cida. Eu prometo que vou descobrir o que aconteceu com a sua filha.",
            FinalizarPensamentosPosFita2,
            fita2Pensamento5Fechada,
            fita2Pensamento5Aberta
        );
    }

    private void FinalizarPensamentosPosFita2()
    {
        PlayerPrefs.SetInt(CHAVE_BLOQUEIO_FITA2, 0);
        PlayerPrefs.Save();

        DialogueManager.DialogoGlobalAtivo = false;

        Debug.Log("Pensamentos da segunda fita finalizados. Alice pode se movimentar.");
    }

    // =========================================================
    // TERCEIRA FITA VHS
    // =========================================================

    private void IniciarPensamentosPosFita3()
    {
        MostrarPensamento(
            "...",
            MostrarSegundoPensamentoFita3,
            fita3Pensamento1Fechada,
            fita3Pensamento1Aberta
        );
    }

    private void MostrarSegundoPensamentoFita3()
    {
        MostrarPensamento(
            "...",
            MostrarTerceiroPensamentoFita3,
            fita3Pensamento2Fechada,
            fita3Pensamento2Aberta
        );
    }

    private void MostrarTerceiroPensamentoFita3()
    {
        MostrarPensamento(
            "...",
            MostrarQuartoPensamentoFita3,
            fita3Pensamento3Fechada,
            fita3Pensamento3Aberta
        );
    }

    private void MostrarQuartoPensamentoFita3()
    {
        MostrarPensamento(
            "Foi...ele...",
            MostrarQuintoPensamentoFita3,
            fita3Pensamento4Fechada,
            fita3Pensamento4Aberta
        );
    }

    private void MostrarQuintoPensamentoFita3()
    {
        MostrarPensamento(
            "Ele que matou a Bianca....",
            MostrarSextoPensamentoFita3,
            fita3Pensamento5Fechada,
            fita3Pensamento5Aberta
        );
    }

    private void MostrarSextoPensamentoFita3()
    {
        MostrarPensamento(
            "EU PRECISO MOSTRAR ISSO PRO MEU TIO",
            FinalizarPensamentosPosFita3,
            fita3Pensamento6Fechada,
            fita3Pensamento6Aberta
        );
    }

    private void FinalizarPensamentosPosFita3()
    {
        PlayerPrefs.SetInt(CHAVE_BLOQUEIO_FITA3, 0);
        PlayerPrefs.Save();

        DialogueManager.DialogoGlobalAtivo = false;

        Debug.Log("Pensamentos da terceira fita finalizados. Indo para a Igreja.");

        // Procura o FadePreto na cena do Almoxarifado.
        FadePreto fade = FindObjectOfType<FadePreto>();

        if (fade != null)
        {
            // Faz a transição com o áudio padrão da porta.
            fade.EscurecerECarregarComAudioPadrao("Igreja_Porta");
        }
        else
        {
            Debug.LogWarning("FadePreto não encontrado! Carregando a cena sem áudio.");
            SceneManager.LoadScene("Igreja_Porta");
        }
    }

    // =========================================================
    // DIGITAÇÃO DO TEXTO
    // =========================================================

    void IniciarDigitacao()
    {
        if (digitacaoTexto != null)
            StopCoroutine(digitacaoTexto);

        textoPensamento.maxVisibleCharacters = 0;
        textoPensamento.ForceMeshUpdate();

        textoDigitando = true;

        IniciarAudioVoz();

        digitacaoTexto = StartCoroutine(AnimarTexto());
    }

    IEnumerator AnimarTexto()
    {
        int totalCaracteres = textoPensamento.textInfo.characterCount;

        for (int i = 0; i <= totalCaracteres; i++)
        {
            textoPensamento.maxVisibleCharacters = i;

            yield return new WaitForSeconds(velocidadeTexto);
        }

        textoPensamento.maxVisibleCharacters = int.MaxValue;
        textoDigitando = false;
        digitacaoTexto = null;

        PararAudioVoz();
    }

    void CompletarTexto()
    {
        if (digitacaoTexto != null)
        {
            StopCoroutine(digitacaoTexto);
            digitacaoTexto = null;
        }

        textoPensamento.maxVisibleCharacters = int.MaxValue;
        textoDigitando = false;

        PararAudioVoz();
    }

    // =========================================================
    // ANIMAÇÃO DA BOCA
    // =========================================================

    void IniciarAnimacaoBoca()
    {
        if (imagemAlice != null && bocaAbertaAtual != null)
            animacaoBoca = StartCoroutine(AnimarBoca());
    }

    IEnumerator AnimarBoca()
    {
        bool bocaAberta = false;

        while (pensamentoAtivo)
        {
            bocaAberta = !bocaAberta;

            imagemAlice.sprite = bocaAberta
                ? bocaAbertaAtual
                : bocaFechadaAtual;

            yield return new WaitForSeconds(intervaloAnimacaoBoca);
        }
    }

    // =========================================================
    // FECHAR PENSAMENTO
    // =========================================================

    void FecharPensamento()
    {
        PararAudioVoz();

        if (digitacaoTexto != null)
            StopCoroutine(digitacaoTexto);

        if (animacaoBoca != null)
            StopCoroutine(animacaoBoca);

        digitacaoTexto = null;
        animacaoBoca = null;

        textoDigitando = false;
        pensamentoAtivo = false;

        textoPensamento.maxVisibleCharacters = int.MaxValue;

        bocaFechadaAtual = aliceBocaFechada;
        bocaAbertaAtual = aliceBocaAberta;

        if (imagemAlice != null)
            imagemAlice.sprite = aliceBocaFechada;

        painelPensamentos.SetActive(false);

        Action acao = acaoAoFechar;
        acaoAoFechar = null;

        if (acao != null)
        {
            acao.Invoke();
        }
        else
        {
            DialogueManager.DialogoGlobalAtivo = false;
        }
    }

    private void OnDisable()
    {
        PararAudioVoz();
    }
}

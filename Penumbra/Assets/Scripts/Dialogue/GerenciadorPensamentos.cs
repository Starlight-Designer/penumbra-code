
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;

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

    private bool pensamentoAtivo = false;
    private bool textoDigitando = false;

    private Coroutine digitacaoTexto;
    private Coroutine animacaoBoca;

    private Action acaoAoFechar;

    private Sprite bocaFechadaAtual;
    private Sprite bocaAbertaAtual;

    private const string CHAVE_INICIAR_PENSAMENTOS =
        "IniciarPensamentosPosFita1";

    private const string CHAVE_BLOQUEIO_FITA1 =
        "BloquearAlicePosFita1";

    void Start()
    {
        painelPensamentos.SetActive(false);

        PararAudioVoz();

        bocaFechadaAtual = aliceBocaFechada;
        bocaAbertaAtual = aliceBocaAberta;

        // Mantém Alice bloqueada ao retornar da cutscene.
        if (PlayerPrefs.GetInt(CHAVE_BLOQUEIO_FITA1, 0) == 1)
        {
            DialogueManager.DialogoGlobalAtivo = true;
        }

        if (PlayerPrefs.GetInt(CHAVE_INICIAR_PENSAMENTOS, 0) == 1)
        {
            PlayerPrefs.SetInt(CHAVE_INICIAR_PENSAMENTOS, 0);
            PlayerPrefs.Save();

            StartCoroutine(IniciarPensamentosAutomaticamente());
        }
    }

    private IEnumerator IniciarPensamentosAutomaticamente()
    {
        yield return null;

        DialogueManager.DialogoGlobalAtivo = true;

        IniciarPensamentosPosFita1();
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

    // Mantém compatibilidade com os pensamentos já existentes.
    public void MostrarPensamento(string mensagem)
    {
        MostrarPensamento(mensagem, null);
    }

    // Pensamento com ação ao fechar.
    public void MostrarPensamento(string mensagem, Action aoFechar)
    {
        MostrarPensamento(
            mensagem,
            aoFechar,
            aliceBocaFechada,
            aliceBocaAberta
        );
    }

    // Pensamento com rosto personalizado.
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
    // SEQUÊNCIA DE PENSAMENTOS APÓS A PRIMEIRA FITA VHS
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

    // Só libera Alice depois que o terceiro pensamento for fechado.
    private void FinalizarPensamentosPosFita1()
    {
        PlayerPrefs.SetInt(CHAVE_BLOQUEIO_FITA1, 0);
        PlayerPrefs.Save();

        DialogueManager.DialogoGlobalAtivo = false;

        Debug.Log("Pensamentos finalizados. Alice pode se movimentar.");
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

        // Se existe outro pensamento na sequência,
        // mantém o bloqueio ativo.
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

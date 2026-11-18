
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

    private bool pensamentoAtivo = false;
    private bool textoDigitando = false;

    private Coroutine digitacaoTexto;
    private Coroutine animacaoBoca;

    private Action acaoAoFechar;

    private Sprite bocaFechadaAtual;
    private Sprite bocaAbertaAtual;

    void Start()
    {
        painelPensamentos.SetActive(false);

        bocaFechadaAtual = aliceBocaFechada;
        bocaAbertaAtual = aliceBocaAberta;
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

    // Mantém compatibilidade com os pensamentos já existentes
    public void MostrarPensamento(string mensagem)
    {
        MostrarPensamento(mensagem, null);
    }

    // Pensamento com ação ao fechar
    public void MostrarPensamento(string mensagem, Action aoFechar)
    {
        MostrarPensamento(
            mensagem,
            aoFechar,
            aliceBocaFechada,
            aliceBocaAberta
        );
    }

    // Pensamento com rosto personalizado
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

    void IniciarDigitacao()
    {
        if (digitacaoTexto != null)
            StopCoroutine(digitacaoTexto);

        textoPensamento.maxVisibleCharacters = 0;
        textoPensamento.ForceMeshUpdate();

        textoDigitando = true;
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
    }

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

    void FecharPensamento()
    {
        if (digitacaoTexto != null)
            StopCoroutine(digitacaoTexto);

        if (animacaoBoca != null)
            StopCoroutine(animacaoBoca);

        digitacaoTexto = null;
        animacaoBoca = null;

        textoDigitando = false;
        pensamentoAtivo = false;

        DialogueManager.DialogoGlobalAtivo = false;

        textoPensamento.maxVisibleCharacters = int.MaxValue;

        // Restaura o rosto original da Alice
        bocaFechadaAtual = aliceBocaFechada;
        bocaAbertaAtual = aliceBocaAberta;

        if (imagemAlice != null)
            imagemAlice.sprite = aliceBocaFechada;

        painelPensamentos.SetActive(false);

        // Executa a ação somente depois que o painel for fechado
        Action acao = acaoAoFechar;
        acaoAoFechar = null;

        if (acao != null)
            acao.Invoke();
    }
}

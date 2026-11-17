
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

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

    void Start()
    {
        painelPensamentos.SetActive(false);
    }

    void Update()
    {
        if (!pensamentoAtivo || !Input.GetKeyDown(KeyCode.E))
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

    public void MostrarPensamento(string mensagem)
    {
        if (pensamentoAtivo)
            return;

        pensamentoAtivo = true;

        // Bloqueia o movimento da Alice
        DialogueManager.DialogoGlobalAtivo = true;

        painelPensamentos.SetActive(true);

        nomePensamento.text = "Alice";
        textoPensamento.text = mensagem;

        if (imagemAlice != null)
            imagemAlice.sprite = aliceBocaFechada;

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
        if (imagemAlice != null && aliceBocaAberta != null)
            animacaoBoca = StartCoroutine(AnimarBoca());
    }

    IEnumerator AnimarBoca()
    {
        bool bocaAberta = false;

        while (pensamentoAtivo)
        {
            bocaAberta = !bocaAberta;

            imagemAlice.sprite = bocaAberta
                ? aliceBocaAberta
                : aliceBocaFechada;

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

        // Libera o movimento da Alice
        DialogueManager.DialogoGlobalAtivo = false;

        textoPensamento.maxVisibleCharacters = int.MaxValue;

        if (imagemAlice != null)
            imagemAlice.sprite = aliceBocaFechada;

        painelPensamentos.SetActive(false);
    }
}

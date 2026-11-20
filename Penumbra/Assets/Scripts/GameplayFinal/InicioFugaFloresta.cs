
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InicioFugaFloresta : MonoBehaviour
{
    [Header("Alice")]
    public Transform alice;
    public float posicaoInicialX = -8f;
    public float velocidadeEntrada = 3f;

    [Header("Componentes da Alice")]
    public PlayerMovement playerMovement;
    public Rigidbody2D rb;
    public Animator animator;
    public Collider2D colisor;

    [Header("Texto de instrucao")]
    public TMP_Text textoInstrucao;
    public float duracaoFadeInTexto = 1f;
    public float duracaoFadeOutTexto = 1f;

    [Header("Gerenciador de obstáculos")]
    public GerenciadorObstaculos gerenciadorObstaculos;

    private Vector3 destinoFinal;

    private void Start()
    {
        if (alice == null || playerMovement == null || rb == null)
        {
            Debug.LogError("Conecte os componentes da Alice!");
            return;
        }

        if (colisor == null)
            colisor = alice.GetComponent<Collider2D>();

        destinoFinal = alice.position;

        playerMovement.enabled = false;
        rb.linearVelocity = Vector2.zero;

        if (colisor != null)
            colisor.enabled = false;

        alice.position = new Vector3(
            posicaoInicialX,
            destinoFinal.y,
            destinoFinal.z
        );

        if (textoInstrucao != null)
        {
            textoInstrucao.alpha = 0f;
            textoInstrucao.gameObject.SetActive(false);
        }

        StartCoroutine(SequenciaInicial());
    }

    private IEnumerator SequenciaInicial()
    {
        while (FadePreto.TransicaoAtiva)
            yield return null;

        if (animator != null)
        {
            animator.SetBool("Andando", true);
            animator.SetFloat("MoveX", 1f);
            animator.SetFloat("MoveY", 0f);
        }

        while (Vector2.Distance(rb.position, destinoFinal) > 0.05f)
        {
            Vector2 proximaPosicao = Vector2.MoveTowards(
                rb.position,
                new Vector2(destinoFinal.x, destinoFinal.y),
                velocidadeEntrada * Time.fixedDeltaTime
            );

            rb.MovePosition(proximaPosicao);

            yield return new WaitForFixedUpdate();
        }

        rb.position = new Vector2(destinoFinal.x, destinoFinal.y);
        rb.linearVelocity = Vector2.zero;

        if (animator != null)
            animator.SetBool("Andando", false);

        // Mostra o texto com Fade In.
        if (textoInstrucao != null)
        {
            textoInstrucao.gameObject.SetActive(true);
            yield return StartCoroutine(
                FadeTexto(0f, 1f, duracaoFadeInTexto)
            );
        }

        // Aguarda o jogador apertar WASD.
        while (!TeclaDeMovimentoPressionada())
            yield return null;

        // Esconde o texto com Fade Out.
        if (textoInstrucao != null)
        {
            yield return StartCoroutine(
                FadeTexto(1f, 0f, duracaoFadeOutTexto)
            );

            textoInstrucao.gameObject.SetActive(false);
        }

        // Reativa as colisões e libera a Alice.
        if (colisor != null)
            colisor.enabled = true;

        playerMovement.enabled = true;

        // Inicia o aparecimento dos obstáculos.
        if (gerenciadorObstaculos != null)
        {
            gerenciadorObstaculos.IniciarObstaculos();
        }
        else
        {
            Debug.LogError(
                "GerenciadorObstaculos não foi conectado no Inspector!"
            );
        }

        Debug.Log("Gameplay da floresta iniciada!");
    }

    private IEnumerator FadeTexto(float alphaInicial, float alphaFinal, float duracao)
    {
        float tempo = 0f;

        if (duracao <= 0f)
        {
            textoInstrucao.alpha = alphaFinal;
            yield break;
        }

        textoInstrucao.alpha = alphaInicial;

        while (tempo < duracao)
        {
            tempo += Time.unscaledDeltaTime;

            float progresso = Mathf.Clamp01(tempo / duracao);

            textoInstrucao.alpha = Mathf.Lerp(
                alphaInicial,
                alphaFinal,
                progresso
            );

            yield return null;
        }

        textoInstrucao.alpha = alphaFinal;
    }

    private bool TeclaDeMovimentoPressionada()
    {
        if (Keyboard.current == null)
            return false;

        return Keyboard.current.wKey.wasPressedThisFrame ||
               Keyboard.current.aKey.wasPressedThisFrame ||
               Keyboard.current.sKey.wasPressedThisFrame ||
               Keyboard.current.dKey.wasPressedThisFrame;
    }
}

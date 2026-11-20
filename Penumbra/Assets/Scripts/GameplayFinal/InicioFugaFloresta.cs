
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

    [Header("Pastor")]
    public Transform pastor;
    public float posicaoInicialXPastor = -10f;
    public float velocidadeEntradaPastor = 3.5f;

    [Header("Componentes do Pastor")]
    public Rigidbody2D rbPastor;
    public Animator animatorPastor;

    [Header("Texto de instrucao")]
    public TMP_Text textoInstrucao;
    public float duracaoFadeInTexto = 1f;
    public float duracaoFadeOutTexto = 1f;

    [Header("Gerenciador de obstáculos")]
    public GerenciadorObstaculos gerenciadorObstaculos;

    private Vector3 destinoFinal;
    private Vector2 destinoFinalPastor;

    private void Start()
    {
        if (alice == null || playerMovement == null || rb == null)
        {
            Debug.LogError("Conecte os componentes da Alice!");
            return;
        }

        if (pastor == null)
        {
            Debug.LogError("Conecte o Pastor no Inspector!");
            return;
        }

        if (rbPastor == null)
            rbPastor = pastor.GetComponent<Rigidbody2D>();

        if (rbPastor == null)
        {
            Debug.LogError("O Pastor precisa de um Rigidbody2D!");
            return;
        }

        if (animatorPastor == null)
            animatorPastor = pastor.GetComponent<Animator>();

        if (colisor == null)
            colisor = alice.GetComponent<Collider2D>();

        // Guarda as posições finais dos dois personagens.
        destinoFinal = alice.position;
        destinoFinalPastor = rbPastor.position;

        // Bloqueia o controle da Alice.
        playerMovement.enabled = false;
        rb.linearVelocity = Vector2.zero;

        if (colisor != null)
            colisor.enabled = false;

        // Posiciona Alice no início da entrada.
        alice.position = new Vector3(
            posicaoInicialX,
            destinoFinal.y,
            destinoFinal.z
        );

        rb.position = new Vector2(
            posicaoInicialX,
            destinoFinal.y
        );

        // Posiciona o Pastor no início da entrada.
        rbPastor.linearVelocity = Vector2.zero;

        pastor.position = new Vector3(
            posicaoInicialXPastor,
            destinoFinalPastor.y,
            pastor.position.z
        );

        rbPastor.position = new Vector2(
            posicaoInicialXPastor,
            destinoFinalPastor.y
        );

        if (animator != null)
            animator.SetBool("Andando", false);

        if (animatorPastor != null)
            animatorPastor.SetBool("Andando", false);

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

        // Inicia a animação de corrida dos dois.
        if (animator != null)
        {
            animator.SetBool("Andando", true);
            animator.SetFloat("MoveX", 1f);
            animator.SetFloat("MoveY", 0f);
        }

        if (animatorPastor != null)
        {
            animatorPastor.SetBool("Andando", true);
            animatorPastor.SetFloat("MoveX", 1f);
            animatorPastor.SetFloat("MoveY", 0f);
        }

        // Alice e Pastor caminham simultaneamente.
        while (
            Vector2.Distance(rb.position, destinoFinal) > 0.05f ||
            Vector2.Distance(rbPastor.position, destinoFinalPastor) > 0.05f
        )
        {
            Vector2 proximaPosicaoAlice = Vector2.MoveTowards(
                rb.position,
                new Vector2(destinoFinal.x, destinoFinal.y),
                velocidadeEntrada * Time.fixedDeltaTime
            );

            Vector2 proximaPosicaoPastor = Vector2.MoveTowards(
                rbPastor.position,
                destinoFinalPastor,
                velocidadeEntradaPastor * Time.fixedDeltaTime
            );

            rb.MovePosition(proximaPosicaoAlice);
            rbPastor.MovePosition(proximaPosicaoPastor);

            yield return new WaitForFixedUpdate();
        }

        // Garante que os dois estejam exatamente nos destinos.
        rb.position = new Vector2(destinoFinal.x, destinoFinal.y);
        rb.linearVelocity = Vector2.zero;

        rbPastor.position = destinoFinalPastor;
        rbPastor.linearVelocity = Vector2.zero;

        if (animator != null)
            animator.SetBool("Andando", false);

        if (animatorPastor != null)
            animatorPastor.SetBool("Andando", false);

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

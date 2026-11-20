
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class PastorPerseguicao : MonoBehaviour
{
    [Header("Referências")]
    public Transform alice;
    public Animator animator;

    [Header("Obstáculos")]
    public string tagObstaculo = "Obstaculo";
    public float forcaArremesso = 8f;
    public float impulsoVertical = 7f;
    public float forcaRotacao = 180f;
    public float gravidadeObstaculo = 2f;
    public float tempoParaDestruir = 5f;

    [Header("Ataque")]
    public float velocidadeAtaque = 8f;
    public float distanciaCaptura = 0.5f;

    [Header("Evento de captura")]
    public UnityEvent aoCapturar;

    private Rigidbody2D rb;
    private Rigidbody2D rbAlice;

    private bool atacando = false;
    private bool capturou = false;

    private int contadorDiagnostico = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (alice == null)
        {
            GameObject objetoAlice = GameObject.FindGameObjectWithTag("Player");

            if (objetoAlice != null)
                alice = objetoAlice.transform;
        }

        if (alice != null)
            rbAlice = alice.GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (capturou)
            return;

        if (alice == null)
        {
            Debug.LogError("Pastor: referência da Alice não encontrada!");
            return;
        }

        if (!atacando)
            return;

        MoverAteAlice();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ArremessarObstaculo(collision.collider);
    }

    private void ArremessarObstaculo(Collider2D obstaculo)
    {
        if (!obstaculo.CompareTag(tagObstaculo) &&
            !obstaculo.transform.root.CompareTag(tagObstaculo))
            return;

        Rigidbody2D rbObstaculo = obstaculo.attachedRigidbody;

        if (rbObstaculo == null)
        {
            Debug.LogWarning(
                "O obstáculo precisa ter um Rigidbody2D: " +
                obstaculo.name
            );
            return;
        }

        if (rbObstaculo.bodyType == RigidbodyType2D.Dynamic)
            return;

        MovimentoObstaculo movimento =
            rbObstaculo.GetComponent<MovimentoObstaculo>();

        if (movimento != null)
            movimento.Arremessar();

        Collider2D[] colisores =
            rbObstaculo.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D colisor in colisores)
            colisor.enabled = false;

        rbObstaculo.bodyType = RigidbodyType2D.Dynamic;
        rbObstaculo.gravityScale = gravidadeObstaculo;
        rbObstaculo.constraints = RigidbodyConstraints2D.None;

        rbObstaculo.linearVelocity = Vector2.zero;
        rbObstaculo.angularVelocity = 0f;

        Vector2 direcao = new Vector2(1f, 1f).normalized;

        rbObstaculo.AddForce(
            direcao * forcaArremesso +
            Vector2.up * impulsoVertical,
            ForceMode2D.Impulse
        );

        float sentido = Random.value > 0.5f ? 1f : -1f;

        rbObstaculo.AddTorque(
            forcaRotacao * sentido,
            ForceMode2D.Impulse
        );

        Destroy(rbObstaculo.gameObject, tempoParaDestruir);

        Debug.Log(
            "Pastor arremessou e removeu a colisão de: " +
            obstaculo.name
        );
    }

    private void MoverAteAlice()
    {
        Vector2 destino = rbAlice != null
            ? rbAlice.position
            : (Vector2)alice.position;

        float distanciaAtual = Vector2.Distance(rb.position, destino);

        Vector2 novaPosicao = Vector2.MoveTowards(
            rb.position,
            destino,
            velocidadeAtaque * Time.fixedDeltaTime
        );

        rb.MovePosition(novaPosicao);

        if (distanciaAtual <= distanciaCaptura)
        {
            CapturarAlice();
            return;
        }

        // Mostra a distância periodicamente para facilitar o diagnóstico.
        contadorDiagnostico++;

        if (contadorDiagnostico >= 50)
        {
            contadorDiagnostico = 0;

            Debug.Log(
                "Pastor perseguindo Alice. Distância: " +
                distanciaAtual.ToString("F2") +
                " | Posição Pastor: " + rb.position +
                " | Posição Alice: " + destino
            );
        }
    }

    private void CapturarAlice()
    {
        if (capturou)
            return;

        capturou = true;
        atacando = false;

        rb.linearVelocity = Vector2.zero;

        if (animator != null)
            animator.SetBool("Andando", false);

        Debug.Log("Alice foi capturada pelo pastor!");

        aoCapturar?.Invoke();
    }

    public void IniciarAtaque()
    {
        if (atacando || capturou)
            return;

        if (alice == null)
        {
            Debug.LogError(
                "Não é possível iniciar a perseguição: Alice não foi encontrada!"
            );
            return;
        }

        if (rbAlice == null)
            rbAlice = alice.GetComponent<Rigidbody2D>();

        atacando = true;

        if (animator != null)
            animator.SetBool("Andando", true);

        Debug.Log("O pastor começou a perseguir Alice!");
    }

    public void EncerrarAtaque()
    {
        atacando = false;

        if (animator != null)
            animator.SetBool("Andando", false);
    }
}

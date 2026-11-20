
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

    private bool atacando = false;
    private bool capturou = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (alice == null || capturou)
            return;

        if (atacando)
        {
            MoverAteAlice();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ArremessarObstaculo(collision.collider);
    }

    private void ArremessarObstaculo(Collider2D obstaculo)
    {
        if (!obstaculo.CompareTag(tagObstaculo))
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

        // Impede que o mesmo obstáculo seja arremessado novamente.
        if (rbObstaculo.bodyType == RigidbodyType2D.Dynamic)
            return;

        // Para o movimento automático do obstáculo.
        MovimentoObstaculo movimento =
            obstaculo.GetComponent<MovimentoObstaculo>();

        if (movimento != null)
            movimento.Arremessar();

        // Remove todos os colisores do obstáculo.
        Collider2D[] colisores =
            rbObstaculo.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D colisor in colisores)
        {
            colisor.enabled = false;
        }

        // Ativa a física.
        rbObstaculo.bodyType = RigidbodyType2D.Dynamic;
        rbObstaculo.gravityScale = gravidadeObstaculo;
        rbObstaculo.constraints = RigidbodyConstraints2D.None;

        rbObstaculo.linearVelocity = Vector2.zero;
        rbObstaculo.angularVelocity = 0f;

        // Lança o obstáculo para cima e para a direita.
        Vector2 direcao = new Vector2(1f, 1f).normalized;

        rbObstaculo.AddForce(
            direcao * forcaArremesso +
            Vector2.up * impulsoVertical,
            ForceMode2D.Impulse
        );

        // Faz o obstáculo girar no ar.
        float sentido = Random.value > 0.5f ? 1f : -1f;

        rbObstaculo.AddTorque(
            forcaRotacao * sentido,
            ForceMode2D.Impulse
        );

        // Destrói o obstáculo depois do tempo configurado.
        Destroy(rbObstaculo.gameObject, tempoParaDestruir);

        Debug.Log(
            "Pastor arremessou e removeu a colisão de: " +
            obstaculo.name
        );
    }

    private void MoverAteAlice()
    {
        Vector2 destino = alice.position;

        Vector2 novaPosicao = Vector2.MoveTowards(
            rb.position,
            destino,
            velocidadeAtaque * Time.fixedDeltaTime
        );

        rb.MovePosition(novaPosicao);

        if (Vector2.Distance(novaPosicao, destino) <= distanciaCaptura)
        {
            capturou = true;
            rb.linearVelocity = Vector2.zero;

            if (animator != null)
                animator.SetBool("Andando", false);

            Debug.Log("Alice foi capturada pelo pastor!");

            aoCapturar?.Invoke();
        }
    }

    public void IniciarAtaque()
    {
        if (atacando || capturou)
            return;

        atacando = true;

        Debug.Log("O pastor começou a perseguir Alice!");
    }

    public void EncerrarAtaque()
    {
        atacando = false;
    }
}

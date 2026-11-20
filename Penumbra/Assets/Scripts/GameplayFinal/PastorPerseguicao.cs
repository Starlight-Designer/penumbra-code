
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class PastorPerseguicao : MonoBehaviour
{
    [Header("Referências")]
    public Transform alice;
    public Animator animator;

    [Header("Movimento normal")]
    public float posicaoXFixa;
    public float velocidadeVertical = 2f;
    public float alturaMinima = -2.67f;
    public float alturaMaxima = 0.01f;

    [Header("Desvio de obstáculos")]
    public float distanciaDeteccao = 7f;
    public float distanciaSeguranca = 0.1f;
    public float velocidadeDesvio = 4f;
    public string tagObstaculo = "Obstaculo";

    [Header("Ataque")]
    public float velocidadeAtaque = 8f;
    public float distanciaCaptura = 0.5f;

    [Header("Evento de captura")]
    public UnityEvent aoCapturar;

    private Rigidbody2D rb;
    private Collider2D colisorPastor;

    private bool atacando = false;
    private bool capturou = false;
    private bool desviando = false;

    private float alturaInicial;
    private float alturaAlvo;

    private Collider2D obstaculoAtual;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        colisorPastor = GetComponent<Collider2D>();

        posicaoXFixa = rb.position.x;

        alturaInicial = rb.position.y;
        alturaAlvo = alturaInicial;

        if (colisorPastor == null)
            Debug.LogError("O pastor precisa de um Collider2D!");
    }

    private void FixedUpdate()
    {
        if (alice == null || capturou)
            return;

        if (atacando)
        {
            MoverAteAlice();
            return;
        }

        if (desviando)
        {
            VerificarFimDesvio();
        }
        else
        {
            ProcurarObstaculo();
        }

        Vector2 destino = new Vector2(
            posicaoXFixa,
            Mathf.Clamp(alturaAlvo, alturaMinima, alturaMaxima)
        );

        rb.MovePosition(Vector2.MoveTowards(
            rb.position,
            destino,
            velocidadeDesvio * Time.fixedDeltaTime
        ));
    }

    private void ProcurarObstaculo()
    {
        Collider2D[] obstaculos = Physics2D.OverlapBoxAll(
            new Vector2(
                rb.position.x + distanciaDeteccao / 2f,
                (alturaMinima + alturaMaxima) / 2f
            ),
            new Vector2(
                distanciaDeteccao,
                alturaMaxima - alturaMinima + 2f
            ),
            0f
        );

        Collider2D maisProximo = null;
        float menorDistancia = float.MaxValue;

        foreach (Collider2D obstaculo in obstaculos)
        {
            if (obstaculo == null ||
                !obstaculo.CompareTag(tagObstaculo))
                continue;

            float distancia =
                obstaculo.bounds.min.x - colisorPastor.bounds.max.x;

            if (distancia >= 0f && distancia < menorDistancia)
            {
                menorDistancia = distancia;
                maisProximo = obstaculo;
            }
        }

        if (maisProximo == null)
            return;

        EscolherDesvio(maisProximo);
    }

    private void EscolherDesvio(Collider2D obstaculo)
    {
        Bounds limitesPastor = colisorPastor.bounds;
        Bounds limitesObstaculo = obstaculo.bounds;

        float deslocamentoCollider =
            limitesPastor.center.y - rb.position.y;

        float meiaAlturaPastor = limitesPastor.extents.y;

        float alturaAcima =
            limitesObstaculo.max.y +
            distanciaSeguranca -
            deslocamentoCollider +
            meiaAlturaPastor;

        float alturaAbaixo =
            limitesObstaculo.min.y -
            distanciaSeguranca -
            deslocamentoCollider -
            meiaAlturaPastor;

        bool podeSubir = alturaAcima <= alturaMaxima;
        bool podeDescer = alturaAbaixo >= alturaMinima;

        if (!podeSubir && !podeDescer)
        {
            Debug.LogWarning(
                "Não existe espaço suficiente para desviar de " +
                obstaculo.name
            );
            return;
        }

        if (podeSubir && podeDescer)
        {
            float distanciaSubir =
                Mathf.Abs(alturaAcima - rb.position.y);

            float distanciaDescer =
                Mathf.Abs(alturaAbaixo - rb.position.y);

            alturaAlvo = distanciaSubir <= distanciaDescer
                ? alturaAcima
                : alturaAbaixo;
        }
        else
        {
            alturaAlvo = podeSubir ? alturaAcima : alturaAbaixo;
        }

        obstaculoAtual = obstaculo;
        desviando = true;

        Debug.Log(
            "Pastor iniciou desvio de " + obstaculo.name +
            " | Altura escolhida: " + alturaAlvo
        );
    }

    private void VerificarFimDesvio()
    {
        if (obstaculoAtual == null)
        {
            FinalizarDesvio();
            return;
        }

        float distanciaHorizontal =
            obstaculoAtual.bounds.min.x -
            colisorPastor.bounds.max.x;

        if (distanciaHorizontal < -distanciaSeguranca)
        {
            Debug.Log(
                "Pastor terminou de passar por " +
                obstaculoAtual.name
            );

            FinalizarDesvio();
        }
    }

    private void FinalizarDesvio()
    {
        desviando = false;
        obstaculoAtual = null;

        // Mantém a altura alcançada.
        // Não retorna automaticamente ao centro.
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
        desviando = false;
        obstaculoAtual = null;

        Debug.Log("O pastor começou a perseguir Alice!");
    }

    public void EncerrarAtaque()
    {
        atacando = false;
        alturaAlvo = alturaInicial;
        desviando = false;
        obstaculoAtual = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(
            new Vector3(
                transform.position.x + distanciaDeteccao / 2f,
                (alturaMinima + alturaMaxima) / 2f,
                0f
            ),
            new Vector3(
                distanciaDeteccao,
                alturaMaxima - alturaMinima + 2f,
                0f
            )
        );
    }
}

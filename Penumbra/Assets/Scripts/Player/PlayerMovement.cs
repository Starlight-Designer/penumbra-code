
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    [Header("Perseguição do Pastor")]
    [SerializeField] private PastorPerseguicao pastor;
    [SerializeField] private string tagObstaculo = "Obstaculo";

    [Header("Áudio dos Passos")]
    [SerializeField] private AudioClip somPassos;
    [SerializeField] private float intervaloPassos = 0.4f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sr;
    private AudioSource audioPassos;
    private Vector2 movement;

    private bool movimentoBloqueado = false;
    private bool estavaBloqueado = false;

    private Coroutine rotinaPassos;

    private ObjectiveManager objectiveManager;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        audioPassos = GetComponent<AudioSource>();

        objectiveManager = FindFirstObjectByType<ObjectiveManager>();

        audioPassos.playOnAwake = false;
        audioPassos.loop = false;
    }

    void Update()
    {
        bool dialogoAtivo = DialogueManager.DialogoGlobalAtivo;
        bool inventarioAberto = InventoryManager.InventarioAberto;
        bool inventarioBloqueado = InventoryManager.InventarioBloqueado;
        bool transicaoAtiva = FadePreto.TransicaoAtiva;

        bool bloqueado =
            dialogoAtivo ||
            inventarioAberto ||
            inventarioBloqueado ||
            transicaoAtiva ||
            movimentoBloqueado;

        // Diagnóstico: mostra o motivo do bloqueio apenas quando muda.
        if (bloqueado != estavaBloqueado)
        {
            if (bloqueado)
            {
                Debug.Log(
                    "MOVIMENTO DA ALICE BLOQUEADO! " +
                    "Diálogo: " + dialogoAtivo +
                    " | Inventário aberto: " + inventarioAberto +
                    " | Inventário bloqueado: " + inventarioBloqueado +
                    " | Fade: " + transicaoAtiva +
                    " | Bloqueio manual: " + movimentoBloqueado
                );
            }
            else
            {
                Debug.Log("MOVIMENTO DA ALICE LIBERADO!");
            }

            estavaBloqueado = bloqueado;
        }

        if (bloqueado)
        {
            movement = Vector2.zero;
            rb.linearVelocity = Vector2.zero;

            AtualizarAnimacao();
            AtualizarAudioPassos();

            return;
        }

        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) input.y += 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) input.y -= 1;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input.x -= 1;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input.x += 1;
        }

        movement = input.normalized;

        // Avisa o ObjectiveManager quando Alice começa a andar.
        if (movement != Vector2.zero)
        {
            if (objectiveManager == null)
                objectiveManager = FindFirstObjectByType<ObjectiveManager>();

            if (objectiveManager != null)
                objectiveManager.AoJogadorComecarAMover();
        }

        AtualizarAnimacao();
        AtualizarAudioPassos();
    }

    void FixedUpdate()
    {
        if (DialogueManager.DialogoGlobalAtivo ||
            InventoryManager.InventarioAberto ||
            InventoryManager.InventarioBloqueado ||
            FadePreto.TransicaoAtiva ||
            movimentoBloqueado)
        {
            movement = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = movement * speed;
    }

    private void AtualizarAudioPassos()
    {
        bool andando = movement != Vector2.zero;

        if (andando && somPassos != null)
        {
            if (rotinaPassos == null)
            {
                rotinaPassos = StartCoroutine(TocarPassos());
            }
        }
        else
        {
            PararPassos();
        }
    }

    private IEnumerator TocarPassos()
    {
        while (movement != Vector2.zero && somPassos != null)
        {
            audioPassos.PlayOneShot(somPassos);

            yield return new WaitForSeconds(intervaloPassos);
        }

        rotinaPassos = null;
    }

    private void PararPassos()
    {
        if (rotinaPassos != null)
        {
            StopCoroutine(rotinaPassos);
            rotinaPassos = null;
        }

        if (audioPassos != null)
        {
            audioPassos.Stop();
        }
    }

    public void BloquearMovimento(bool bloquear)
    {
        movimentoBloqueado = bloquear;

        movement = Vector2.zero;
        rb.linearVelocity = Vector2.zero;

        PararPassos();

        if (sr != null)
            sr.enabled = !bloquear;

        if (animator != null)
            animator.SetBool("Andando", false);
    }

    private void OnDisable()
    {
        PararPassos();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        VerificarObstaculo(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        VerificarObstaculo(other);
    }

    private void VerificarObstaculo(Collider2D obstaculo)
    {
        if (SceneManager.GetActiveScene().name != "Floresta_Fuga")
            return;

        bool temTagCorreta =
            obstaculo.CompareTag(tagObstaculo) ||
            obstaculo.transform.root.CompareTag(tagObstaculo);

        if (!temTagCorreta)
            return;

        Debug.Log("Alice detectou o obstáculo: " + obstaculo.name);

        if (pastor == null)
        {
            Debug.LogError("Pastor não foi atribuído no PlayerMovement!");
            return;
        }

        pastor.IniciarAtaque();

        Debug.Log("Alice encostou em um obstáculo! O Pastor iniciou a perseguição!");
    }

    private void AtualizarAnimacao()
    {
        if (animator == null) return;

        bool andando = movement != Vector2.zero;
        animator.SetBool("Andando", andando);

        if (andando)
        {
            animator.SetFloat("MoveX", Mathf.Abs(movement.x));
            animator.SetFloat("MoveY", movement.y);

            if (sr != null)
            {
                if (SceneManager.GetActiveScene().name == "Floresta_Fuga")
                {
                    sr.flipX = false;
                }
                else if (movement.x != 0)
                {
                    sr.flipX = movement.x > 0;
                }
            }
        }
    }
}

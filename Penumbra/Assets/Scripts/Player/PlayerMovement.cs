
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    [Header("Perseguição do Pastor")]
    [SerializeField] private PastorPerseguicao pastor;
    [SerializeField] private string tagObstaculo = "Obstaculo";

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sr;
    private Vector2 movement;

    private bool movimentoBloqueado = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (DialogueManager.DialogoGlobalAtivo ||
            InventoryManager.InventarioAberto ||
            InventoryManager.InventarioBloqueado ||
            FadePreto.TransicaoAtiva ||
            movimentoBloqueado)
        {
            movement = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
            AtualizarAnimacao();
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

        AtualizarAnimacao();
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

    public void BloquearMovimento(bool bloquear)
    {
        movimentoBloqueado = bloquear;

        movement = Vector2.zero;
        rb.linearVelocity = Vector2.zero;

        if (sr != null)
            sr.enabled = !bloquear;

        if (animator != null)
            animator.SetBool("Andando", false);
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

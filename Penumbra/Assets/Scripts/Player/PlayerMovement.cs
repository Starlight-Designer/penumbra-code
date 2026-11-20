
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

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

    private void AtualizarAnimacao()
    {
        if (animator == null) return;

        bool andando = movement != Vector2.zero;
        animator.SetBool("Andando", andando);

        if (andando)
        {
            animator.SetFloat("MoveX", Mathf.Abs(movement.x));
            animator.SetFloat("MoveY", movement.y);

            if (movement.x != 0 && sr != null)
                sr.flipX = movement.x > 0;
        }
    }
}

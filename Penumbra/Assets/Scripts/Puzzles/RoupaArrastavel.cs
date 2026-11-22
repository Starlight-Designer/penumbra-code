using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class RoupaArrastavel : MonoBehaviour
{
    [Header("Armário")]
    public PuzzleArmario armario;

    [Header("Destaque")]
    public DestaqueSprite2D destaque;

    [Header("Áudio")]
    public AudioSource fonteAudio;
    public AudioClip somPegarItem;

    private Camera cameraPrincipal;
    private Vector3 posicaoOriginal;
    private Vector3 deslocamento;
    private int ordemOriginal;

    private SpriteRenderer spriteRenderer;
    private bool arrastando;
    private bool encaixado;
    private bool mouseEmCima;

    void Awake()
    {
        cameraPrincipal = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (fonteAudio == null)
            fonteAudio = GetComponent<AudioSource>();

        if (destaque == null)
            destaque = GetComponent<DestaqueSprite2D>();
    }

    void OnMouseEnter()
    {
        mouseEmCima = true;
        AtualizarDestaque();
    }

    void OnMouseExit()
    {
        mouseEmCima = false;
        AtualizarDestaque();
    }

    void OnMouseDown()
    {
        if (encaixado || armario == null || armario.EstaCompleto())
            return;

        arrastando = true;
        TocarSom(somPegarItem);

        posicaoOriginal = transform.position;
        ordemOriginal = spriteRenderer.sortingOrder;

        Vector3 mouse = ObterPosicaoMouse();
        deslocamento = transform.position - mouse;

        spriteRenderer.sortingOrder = ordemOriginal + 100;
        AtualizarDestaque();
    }

    void OnMouseDrag()
    {
        if (!arrastando)
            return;

        Vector3 mouse = ObterPosicaoMouse();
        transform.position = mouse + deslocamento;

        Collider2D colisor = armario.GetComponent<Collider2D>();
        bool sobreArmario = colisor != null && colisor.OverlapPoint(mouse);

        armario.Destacar(sobreArmario);
    }

    void OnMouseUp()
    {
        if (!arrastando)
            return;

        arrastando = false;

        Vector2 posicaoMouse = ObterPosicaoMouse();
        bool aceito = armario != null && armario.TentarGuardar(this, posicaoMouse);

        if (!aceito)
        {
            transform.position = posicaoOriginal;
            spriteRenderer.sortingOrder = ordemOriginal;
        }

        if (armario != null)
            armario.Destacar(false);

        AtualizarDestaque();
    }

    Vector3 ObterPosicaoMouse()
    {
        float distancia = Mathf.Abs(
            cameraPrincipal.transform.position.z - transform.position.z
        );

        Vector3 posicaoTela = Input.mousePosition;
        posicaoTela.z = distancia;

        return cameraPrincipal.ScreenToWorldPoint(posicaoTela);
    }

    void AtualizarDestaque()
    {
        if (destaque != null)
            destaque.Destacar(mouseEmCima || arrastando);
    }

    void TocarSom(AudioClip som)
    {
        if (som == null)
            return;

        if (fonteAudio != null)
            fonteAudio.PlayOneShot(som);
        else
            AudioSource.PlayClipAtPoint(som, transform.position);
    }

    public void ConfirmarEncaixe()
    {
        encaixado = true;
        arrastando = false;

        if (destaque != null)
            destaque.Destacar(false);

        gameObject.SetActive(false);
    }
}

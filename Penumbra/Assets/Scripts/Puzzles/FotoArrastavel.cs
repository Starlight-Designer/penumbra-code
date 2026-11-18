
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class FotoArrastavel : MonoBehaviour
{
    [Header("Fio")]
    public PuzzleFotosFio fio;

    [Header("Identificação da foto")]
    [Range(0, 3)]
    public int indiceFoto;

    [Header("Destaque")]
    public DestaqueSprite2D destaque;

    private Camera cameraPrincipal;
    private Vector3 posicaoOriginal;
    private Vector3 deslocamento;
    private int ordemOriginal;

    private SpriteRenderer spriteRenderer;
    private bool arrastando;
    private bool encaixada;
    private bool mouseEmCima;

    void Awake()
    {
        cameraPrincipal = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();

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
        if (encaixada || fio == null)
            return;

        arrastando = true;

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

        Collider2D colisor = fio.GetComponent<Collider2D>();

        bool sobreFio = colisor != null &&
            colisor.OverlapPoint(mouse);

        fio.Destacar(sobreFio);
    }

    void OnMouseUp()
    {
        if (!arrastando)
            return;

        arrastando = false;

        Vector2 posicaoMouse = ObterPosicaoMouse();

        bool aceito = fio != null &&
            fio.TentarPendurar(this, posicaoMouse);

        if (!aceito)
        {
            transform.position = posicaoOriginal;
            spriteRenderer.sortingOrder = ordemOriginal;
        }

        if (fio != null)
            fio.Destacar(false);

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

    public int ObterIndice()
    {
        return indiceFoto;
    }

    public void ConfirmarEncaixe(Transform posicao)
    {
        encaixada = true;
        arrastando = false;

        transform.position = posicao.position;
        transform.rotation = posicao.rotation;

        if (destaque != null)
            destaque.Destacar(false);

        transform.SetParent(posicao);

        spriteRenderer.sortingOrder = ordemOriginal;
    }
}

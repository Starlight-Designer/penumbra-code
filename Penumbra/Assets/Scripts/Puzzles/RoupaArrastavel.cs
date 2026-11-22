
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

        bool sobreArmario =
            colisor != null && colisor.OverlapPoint(mouse);

        armario.Destacar(sobreArmario);
    }

    void OnMouseUp()
    {
        if (!arrastando)
            return;

        arrastando = false;

        Vector2 posicaoMouse = ObterPosicaoMouse();

        bool aceito = armario != null &&
                      armario.TentarGuardar(this, posicaoMouse);

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
        if (cameraPrincipal == null)
            cameraPrincipal = Camera.main;

        if (cameraPrincipal == null)
            return transform.position;

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
        {
            Debug.LogWarning(
                "Som de pegar roupa não foi configurado em: " + gameObject.name
            );
            return;
        }

        GameObject objetoAudio = new GameObject("AudioTemporarioRoupa");

        if (Camera.main != null)
            objetoAudio.transform.position = Camera.main.transform.position;
        else
            objetoAudio.transform.position = transform.position;

        AudioSource audio = objetoAudio.AddComponent<AudioSource>();

        audio.clip = som;
        audio.playOnAwake = false;
        audio.spatialBlend = 0f;
        audio.volume = 1f;

        audio.Play();

        Destroy(objetoAudio, som.length + 0.2f);
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

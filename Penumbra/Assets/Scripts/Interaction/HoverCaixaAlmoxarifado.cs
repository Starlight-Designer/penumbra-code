
using UnityEngine;

// Efeito de hover e sons para as caixas do Almoxarifado.
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
public class HoverCaixaAlmoxarifado : MonoBehaviour
{
    [Header("Aumento")]
    [SerializeField] private float escalaHover = 1.1f;
    [SerializeField] private float velocidade = 15f;

    [Header("Borda")]
    [SerializeField] private Color corBorda = Color.white;
    [SerializeField] private float tamanhoBorda = 1.08f;

    [Header("Sons")]
    [SerializeField] private AudioClip somHover;
    [SerializeField] private AudioClip somSelecao;

    [Range(0f, 1f)]
    [SerializeField] private float volumeHover = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float volumeSelecao = 1f;

    private SpriteRenderer spriteRenderer;
    private SpriteRenderer bordaRenderer;
    private AudioSource audioSource;

    private Vector3 escalaBase;
    private bool mouseEmCima;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;

        escalaBase = transform.localScale;

        CriarBorda();
    }

    private void CriarBorda()
    {
        GameObject borda = new GameObject("Borda_Hover");
        borda.transform.SetParent(transform);
        borda.transform.localPosition = Vector3.zero;
        borda.transform.localRotation = Quaternion.identity;
        borda.transform.localScale = Vector3.one * tamanhoBorda;

        bordaRenderer = borda.AddComponent<SpriteRenderer>();

        bordaRenderer.sprite = spriteRenderer.sprite;
        bordaRenderer.color = corBorda;
        bordaRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        bordaRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;

        bordaRenderer.enabled = false;
    }

    private void OnMouseEnter()
    {
        mouseEmCima = true;

        bordaRenderer.enabled = true;

        if (somHover != null)
        {
            audioSource.PlayOneShot(somHover, volumeHover);
        }
    }

    private void OnMouseExit()
    {
        mouseEmCima = false;
        bordaRenderer.enabled = false;
    }

    private void OnMouseDown()
    {
        if (somSelecao != null)
        {
            audioSource.PlayOneShot(somSelecao, volumeSelecao);
        }
    }

    private void Update()
    {
        Vector3 escalaAlvo =
            escalaBase * (mouseEmCima ? escalaHover : 1f);

        float suavizacao =
            1f - Mathf.Exp(-velocidade * Time.unscaledDeltaTime);

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            escalaAlvo,
            suavizacao
        );

        if (bordaRenderer != null)
        {
            bordaRenderer.sprite = spriteRenderer.sprite;
            bordaRenderer.color = corBorda;
            bordaRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
            bordaRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
            bordaRenderer.flipX = spriteRenderer.flipX;
            bordaRenderer.flipY = spriteRenderer.flipY;
        }
    }

    private void OnDisable()
    {
        mouseEmCima = false;

        if (bordaRenderer != null)
            bordaRenderer.enabled = false;

        transform.localScale = escalaBase;
    }
}


using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DestaqueSprite2D : MonoBehaviour
{
    [Header("Aumento")]
    [SerializeField] private bool aumentarNoHover = true;
    [SerializeField] private float escalaHover = 1.1f;
    [SerializeField] private float velocidade = 15f;

    [Header("Borda")]
    [SerializeField] private Color corBorda = Color.white;
    [SerializeField] private float espessuraBorda = 1.08f;

    private SpriteRenderer spriteOriginal;
    private SpriteRenderer spriteBorda;
    private Vector3 escalaBase;
    private bool destacado;

    void Awake()
    {
        spriteOriginal = GetComponent<SpriteRenderer>();
        escalaBase = transform.localScale;

        GameObject objetoBorda = new GameObject("BordaHover");
        objetoBorda.transform.SetParent(transform);
        objetoBorda.transform.localPosition = Vector3.zero;
        objetoBorda.transform.localRotation = Quaternion.identity;
        objetoBorda.transform.localScale = Vector3.one * espessuraBorda;

        spriteBorda = objetoBorda.AddComponent<SpriteRenderer>();
        spriteBorda.sprite = spriteOriginal.sprite;
        spriteBorda.color = corBorda;
        spriteBorda.sortingLayerID = spriteOriginal.sortingLayerID;
        spriteBorda.sortingOrder = spriteOriginal.sortingOrder - 1;

        spriteBorda.enabled = false;
    }

    void LateUpdate()
    {
        if (spriteOriginal.sprite != spriteBorda.sprite)
            spriteBorda.sprite = spriteOriginal.sprite;

        spriteBorda.sortingLayerID = spriteOriginal.sortingLayerID;
        spriteBorda.sortingOrder = spriteOriginal.sortingOrder - 1;

        spriteBorda.enabled = destacado;

        Vector3 alvo = escalaBase;

        if (aumentarNoHover && destacado)
            alvo = escalaBase * escalaHover;

        float suavizacao = 1f -
            Mathf.Exp(-velocidade * Time.unscaledDeltaTime);

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            alvo,
            suavizacao
        );
    }

    public void Destacar(bool valor)
    {
        destacado = valor;
    }

    void OnDisable()
    {
        destacado = false;

        if (spriteBorda != null)
            spriteBorda.enabled = false;

        transform.localScale = escalaBase;
    }
}

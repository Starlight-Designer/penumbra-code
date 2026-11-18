
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class HoverPorta : MonoBehaviour
{
    [Header("Aumento")]
    [SerializeField] private float escalaHover = 1.1f;
    [SerializeField] private float velocidade = 15f;

    [Header("Contorno")]
    [SerializeField] private Color corBorda = Color.white;
    [SerializeField] private float espessuraBorda = 0.04f;

    private Vector3 escalaBase;
    private bool mouseEmCima;

    private SpriteRenderer spriteOriginal;
    private SpriteRenderer spriteContorno;
    private GameObject objetoContorno;

    private void Awake()
    {
        escalaBase = transform.localScale;
        spriteOriginal = GetComponent<SpriteRenderer>();

        CriarContorno();
    }

    private void CriarContorno()
    {
        objetoContorno = new GameObject("Contorno_Hover");

        objetoContorno.transform.SetParent(transform);
        objetoContorno.transform.localPosition = Vector3.zero;
        objetoContorno.transform.localRotation = Quaternion.identity;
        objetoContorno.transform.localScale = Vector3.one;

        spriteContorno = objetoContorno.AddComponent<SpriteRenderer>();

        spriteContorno.sprite = spriteOriginal.sprite;
        spriteContorno.color = corBorda;
        spriteContorno.sortingLayerID = spriteOriginal.sortingLayerID;
        spriteContorno.sortingOrder = spriteOriginal.sortingOrder - 1;

        objetoContorno.transform.localScale = new Vector3(
            1f + espessuraBorda,
            1f + espessuraBorda,
            1f
        );

        objetoContorno.SetActive(false);
    }

    private void OnMouseEnter()
    {
        mouseEmCima = true;
        spriteContorno.color = corBorda;
        objetoContorno.SetActive(true);
    }

    private void OnMouseExit()
    {
        mouseEmCima = false;
        objetoContorno.SetActive(false);
    }

    private void Update()
    {
        Vector3 alvo = escalaBase * (mouseEmCima ? escalaHover : 1f);

        float suavizacao = 1f - Mathf.Exp(
            -velocidade * Time.unscaledDeltaTime
        );

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            alvo,
            suavizacao
        );
    }
}

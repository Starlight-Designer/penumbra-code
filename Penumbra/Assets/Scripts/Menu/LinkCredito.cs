using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Coloque em cada nome do painel de créditos (objeto com TextMeshPro - Text (UI)).
// Hover: o nome cresce, ganha sublinhado e uma borda. Clique: abre o link no navegador do player.
[RequireComponent(typeof(TMP_Text))]
public class LinkCredito : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Link")]
    [SerializeField] private string url = "https://www.instagram.com/";

    [Header("Hover")]
    [SerializeField] private float escalaHover = 1.1f;   // 1.1 = 10% maior
    [SerializeField] private float velocidade = 15f;     // quanto maior, mais rápido
    [SerializeField] private bool sublinharNoHover = true;

    [Header("Borda do texto")]
    [SerializeField] private bool usarBorda = true;
    [SerializeField] private Color corBorda = Color.white;
    [SerializeField, Range(0f, 1f)] private float larguraBorda = 0.2f;

    private TMP_Text texto;
    private Vector3 escalaBase = Vector3.one;
    private FontStyles estiloOriginal;
    private bool mouseEmCima;

    private void Awake()
    {
        texto = GetComponent<TMP_Text>();
        estiloOriginal = texto.fontStyle;

        if (transform.localScale.sqrMagnitude > 0.0001f)
            escalaBase = transform.localScale;

        if (usarBorda)
        {
            texto.outlineColor = (Color32)corBorda;
            texto.outlineWidth = 0f;
        }
    }

    private void OnDisable()
    {
        // volta ao normal se o painel fechar com o mouse em cima
        mouseEmCima = false;
        AplicarEstilo(false);
        transform.localScale = escalaBase;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseEmCima = true;
        AplicarEstilo(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseEmCima = false;
        AplicarEstilo(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (string.IsNullOrWhiteSpace(url)) return;
        Application.OpenURL(url);
    }

    private void Update()
    {
        Vector3 alvo = escalaBase * (mouseEmCima ? escalaHover : 1f);
        float suavizacao = 1f - Mathf.Exp(-velocidade * Time.unscaledDeltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, alvo, suavizacao);
    }

    private void AplicarEstilo(bool hover)
    {
        if (sublinharNoHover)
            texto.fontStyle = hover ? (estiloOriginal | FontStyles.Underline) : estiloOriginal;

        if (usarBorda)
            texto.outlineWidth = hover ? larguraBorda : 0f;
    }
}
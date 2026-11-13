using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Coloque em cada botão do menu (o objeto que tem o Image do botão).
// Ao passar o mouse: aparece uma borda branca e o botão cresce um pouco.
[RequireComponent(typeof(Outline))]
public class HoverBotao : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Aumento")]
    [SerializeField] private float escalaHover = 1.1f;  // 1.1 = 10% maior
    [SerializeField] private float velocidade = 15f;    // quanto maior, mais rápido

    [Header("Borda")]
    [SerializeField] private Color corBorda = Color.white;
    [SerializeField] private float espessuraBorda = 4f;

    private Outline borda;
    private bool liberado = true;   // só reage ao mouse depois que a animação de abertura termina
    private bool mouseEmCima;
    private Vector3 escalaBase = Vector3.one; // o tamanho que o objeto tem no editor

    // pega o Outline sob demanda (evita erro de ordem entre Awake e OnEnable)
    private Outline Borda
    {
        get
        {
            if (borda == null) borda = GetComponent<Outline>();
            return borda;
        }
    }

    private void Awake()
    {
        // guarda o tamanho original (ex.: um handle de slider com Scale diferente de 1)
        if (transform.localScale.sqrMagnitude > 0.0001f)
            escalaBase = transform.localScale;

        Borda.effectColor = corBorda;
        Borda.effectDistance = new Vector2(espessuraBorda, espessuraBorda);
        Borda.enabled = false;
    }

    private void OnDisable()
    {
        mouseEmCima = false;
        Borda.enabled = false;
    }

    // chamado pelo AnimacaoMenu
    public void Bloquear()
    {
        liberado = false;
        mouseEmCima = false;
        Borda.enabled = false;
    }

    public void Liberar()
    {
        liberado = true;
    }

    public void OnPointerEnter(PointerEventData eventData) => mouseEmCima = true;
    public void OnPointerExit(PointerEventData eventData) => mouseEmCima = false;

    private void Update()
    {
        if (!liberado) return;

        Borda.enabled = mouseEmCima;

        Vector3 alvo = escalaBase * (mouseEmCima ? escalaHover : 1f);
        float suavizacao = 1f - Mathf.Exp(-velocidade * Time.unscaledDeltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, alvo, suavizacao);
    }
}
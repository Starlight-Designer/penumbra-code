using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Coloque este script no painel de reiniciar (o objeto que o MenuManager ativa).
// ENTRADA: toda vez que o painel é ativado, ele começa pequeno e transparente, como se estivesse
//          lá no fundo da tela, e vai crescendo até aparecer no tamanho normal.
// SAÍDA:   chame Fechar() (por exemplo no OnClick dos botões Confirmar e Cancelar) e o painel
//          encolhe de volta para o fundo, some e só então é desligado.
public class AnimacaoZoom : MonoBehaviour
{
    [Header("Zoom")]
    [SerializeField, Range(0f, 1f)] private float escalaInicial = 0.2f; // 0.2 = começa/termina com 20% do tamanho
    [SerializeField] private float duracao = 0.6f;                      // entrada: quanto maior, mais devagar
    [SerializeField] private float duracaoFade = 0.3f;                  // tempo do fade (entrada e saída)

    [Header("Saída")]
    [SerializeField] private float duracaoSaida = 0.4f;
    [SerializeField] private UnityEvent aoFechar;                       // opcional: algo para fazer depois que a saída termina

    [Header("Cliques")]
    [SerializeField] private bool bloquearCliquesDuranteAnimacao = true;

    private CanvasGroup grupo;
    private Vector3 escalaBase = Vector3.one; // o tamanho que o painel tem no editor
    private bool fechando;

    private void Awake()
    {
        grupo = GetComponent<CanvasGroup>();
        if (grupo == null) grupo = gameObject.AddComponent<CanvasGroup>();

        if (transform.localScale.sqrMagnitude > 0.0001f)
            escalaBase = transform.localScale;
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        fechando = false;

        transform.localScale = escalaBase * escalaInicial;
        grupo.alpha = 0f;
        if (bloquearCliquesDuranteAnimacao) grupo.blocksRaycasts = false;

        StartCoroutine(Entrar());
    }

    private void OnDisable()
    {
        grupo.blocksRaycasts = true;
    }

    // Ligue no OnClick dos botões que fecham o painel
    public void Fechar()
    {
        if (fechando || !gameObject.activeInHierarchy) return;
        StopAllCoroutines();
        StartCoroutine(Sair());
    }

    private IEnumerator Entrar()
    {
        Vector3 inicio = escalaBase * escalaInicial;

        float t = 0f;
        while (t < duracao)
        {
            t += Time.unscaledDeltaTime; // funciona mesmo com o jogo pausado
            float p = Mathf.Clamp01(t / duracao);
            float e = 1f - Mathf.Pow(1f - p, 3f); // começa rápido e desacelera até parar

            transform.localScale = Vector3.Lerp(inicio, escalaBase, e);
            grupo.alpha = duracaoFade > 0f ? Mathf.Clamp01(t / duracaoFade) : 1f;
            yield return null;
        }

        transform.localScale = escalaBase;
        grupo.alpha = 1f;
        grupo.blocksRaycasts = true;
    }

    private IEnumerator Sair()
    {
        fechando = true;
        grupo.blocksRaycasts = false; // evita cliques duplos enquanto sai

        Vector3 inicio = transform.localScale;
        Vector3 fim = escalaBase * escalaInicial;
        float alphaInicial = grupo.alpha;

        float t = 0f;
        while (t < duracaoSaida)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duracaoSaida);
            float e = p * p * p; // começa devagar e acelera, o inverso da entrada

            transform.localScale = Vector3.Lerp(inicio, fim, e);

            // o fade acontece no final da saída
            float restante = duracaoSaida - t;
            float fade = duracaoFade > 0f ? Mathf.Clamp01(restante / duracaoFade) : 1f;
            grupo.alpha = Mathf.Min(alphaInicial, fade);
            yield return null;
        }

        transform.localScale = fim;
        grupo.alpha = 0f;

        // primeiro avisa, depois desliga o painel
        aoFechar?.Invoke();
        gameObject.SetActive(false);
    }
}
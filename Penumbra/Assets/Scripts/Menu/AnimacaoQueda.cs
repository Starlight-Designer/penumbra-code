using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Coloque este script no painel de configurações (o objeto que o MenuManager ativa).
// ENTRADA: toda vez que o painel é ativado, ele começa acima da tela e cai até a posição que você deixou no editor.
// SAÍDA: chame Fechar() (por exemplo no OnClick do botão Voltar) e o painel sai da tela antes de desligar.
public class AnimacaoQueda : MonoBehaviour
{
    public enum Estilo
    {
        Suave,   // começa e termina devagar, parece flutuar
        Quicar   // bate no "chão" e quica algumas vezes
    }

    public enum DirecaoSaida
    {
        Subir,   // volta para cima, o caminho inverso da entrada
        Descer   // continua caindo para baixo da tela
    }

    [Header("Entrada (queda)")]
    [SerializeField] private float duracao = 1.5f;     // quanto maior, mais devagar
    [SerializeField] private Estilo estilo = Estilo.Suave;
    [SerializeField] private float distancia = 0f;     // 0 = usa a altura do Canvas

    [Header("Saída")]
    [SerializeField] private float duracaoSaida = 0.8f;
    [SerializeField] private DirecaoSaida direcaoSaida = DirecaoSaida.Subir;

    [Header("Depois que a saída termina")]
    [SerializeField] private UnityEvent aoFechar;      // ligue aqui o que o botão Voltar já fazia (ex.: mostrar o menu principal)

    private RectTransform rect;
    private Vector2 posicaoFinal;
    private bool fechando;

    private void Awake()
    {
        rect = (RectTransform)transform;
        posicaoFinal = rect.anchoredPosition; // a posição que você deixou no editor
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        fechando = false;

        float d = DistanciaDeQueda();
        Vector2 inicio = posicaoFinal + Vector2.up * d;

        rect.anchoredPosition = inicio;
        StartCoroutine(Cair(inicio));
    }

    // Ligue no OnClick do botão Voltar
    public void Fechar()
    {
        if (fechando || !gameObject.activeInHierarchy) return;
        StopAllCoroutines();
        StartCoroutine(Sair());
    }

    private IEnumerator Cair(Vector2 inicio)
    {
        float t = 0f;
        while (t < duracao)
        {
            t += Time.unscaledDeltaTime; // funciona mesmo com o jogo pausado
            float p = Mathf.Clamp01(t / duracao);
            float e = estilo == Estilo.Quicar ? EaseOutBounce(p) : Suavizar(p);

            rect.anchoredPosition = Vector2.LerpUnclamped(inicio, posicaoFinal, e);
            yield return null;
        }
        rect.anchoredPosition = posicaoFinal;
    }

    private IEnumerator Sair()
    {
        fechando = true;

        Vector2 inicio = rect.anchoredPosition;
        Vector2 sentido = direcaoSaida == DirecaoSaida.Subir ? Vector2.up : Vector2.down;
        Vector2 destino = posicaoFinal + sentido * DistanciaDeQueda();

        float t = 0f;
        while (t < duracaoSaida)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duracaoSaida);
            float e = p * p * p; // começa devagar e acelera, como se fosse puxado para fora

            rect.anchoredPosition = Vector2.LerpUnclamped(inicio, destino, e);
            yield return null;
        }
        rect.anchoredPosition = destino;

        // primeiro avisa (ex.: reabrir o menu principal), depois desliga o painel
        aoFechar?.Invoke();
        gameObject.SetActive(false);
    }

    private float DistanciaDeQueda()
    {
        return distancia > 0f ? distancia : AlturaDoCanvas();
    }

    private float AlturaDoCanvas()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null && canvas.rootCanvas.transform is RectTransform raiz)
            return raiz.rect.height;
        return 1200f;
    }

    // devagar no começo, devagar no fim
    private float Suavizar(float x)
    {
        return x * x * (3f - 2f * x);
    }

    private float EaseOutBounce(float x)
    {
        const float n1 = 7.5625f;
        const float d1 = 2.75f;

        if (x < 1f / d1) return n1 * x * x;
        if (x < 2f / d1) { x -= 1.5f / d1; return n1 * x * x + 0.75f; }
        if (x < 2.5f / d1) { x -= 2.25f / d1; return n1 * x * x + 0.9375f; }
        x -= 2.625f / d1;
        return n1 * x * x + 0.984375f;
    }
}
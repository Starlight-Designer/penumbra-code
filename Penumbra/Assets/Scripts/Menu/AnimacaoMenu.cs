using System.Collections;
using UnityEngine;

// Coloque este script no painel do menu (o objeto que você ativa/desativa).
// Toda vez que o painel for ativado, os itens aparecem com efeito "pop", um após o outro.
public class AnimacaoMenu : MonoBehaviour
{
    [Header("Itens (na ordem em que aparecem)")]
    [SerializeField] private RectTransform[] itens;   // título e botões

    [Header("Tempos")]
    [SerializeField] private float duracao = 0.8f;    // tempo que cada item leva para aparecer
    [SerializeField] private float intervalo = 0.2f;  // atraso entre um item e o próximo
    [SerializeField] private float duracaoFade = 0.2f; // fade do painel

    private CanvasGroup grupo;

    private void Awake()
    {
        grupo = GetComponent<CanvasGroup>();
        if (grupo == null) grupo = gameObject.AddComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        StopAllCoroutines();

        // estado inicial: painel invisível e itens com escala zero
        grupo.alpha = 0f;
        foreach (var item in itens)
        {
            item.localScale = Vector3.zero;

            // enquanto anima, o hover fica bloqueado
            if (item.TryGetComponent(out HoverBotao hover))
                hover.Bloquear();
        }

        StartCoroutine(Animar());
    }

    private IEnumerator Animar()
    {
        // inicia cada item com um atraso crescente
        for (int i = 0; i < itens.Length; i++)
            StartCoroutine(AnimarItem(itens[i], i * intervalo));

        // fade do painel
        float t = 0f;
        while (t < duracaoFade)
        {
            t += Time.unscaledDeltaTime;
            grupo.alpha = Mathf.Clamp01(t / duracaoFade);
            yield return null;
        }
        grupo.alpha = 1f;
    }

    private IEnumerator AnimarItem(RectTransform item, float atraso)
    {
        // unscaled: funciona mesmo com o jogo pausado (Time.timeScale = 0)
        yield return new WaitForSecondsRealtime(atraso);

        float t = 0f;
        while (t < duracao)
        {
            t += Time.unscaledDeltaTime;
            float progresso = Mathf.Clamp01(t / duracao);
            item.localScale = Vector3.one * EaseOutBack(progresso);
            yield return null;
        }
        item.localScale = Vector3.one;

        // terminou de aparecer: agora o botão pode reagir ao mouse
        if (item.TryGetComponent(out HoverBotao hover))
            hover.Liberar();
    }

    // Curva que passa um pouco de 1 e volta (efeito "pop").
    // Para uma versão mais suave, troque o retorno por: 1f - Mathf.Pow(1f - x, 3f)
    private float EaseOutBack(float x)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }
}
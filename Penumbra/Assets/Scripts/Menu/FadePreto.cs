using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// Coloque em uma Image preta que cobre a tela inteira (filha direta do Canvas, por cima de tudo).
//
// MODO A, mesma cena:  chame Transicao()
//   escurece -> fica preto (aoFicarPreto: troque o menu pela cutscene) -> clareia aos poucos (aoComecarCutscene)
//
// MODO B, outra cena:  chame EscurecerECarregar("NomeDaCena") no menu
//   e, na cena da cutscene, tenha um FadePreto com "Clarear Ao Iniciar" marcado:
//   ele começa preto e clareia sozinho, disparando aoComecarCutscene.
[RequireComponent(typeof(CanvasGroup))]
public class FadePreto : MonoBehaviour
{
    [Header("Tempos (segundos)")]
    [SerializeField] private float duracaoEscurecer = 2f;
    [SerializeField] private float tempoNoPreto = 0.5f;
    [SerializeField] private float duracaoClarear = 2.5f;

    [Header("Cena da cutscene (modo B)")]
    [SerializeField] private bool clarearAoIniciar = false;

    [Header("Eventos")]
    [SerializeField] private UnityEvent aoFicarPreto;       // a tela está 100% preta (ex.: desligar o menu, ligar a cutscene)
    [SerializeField] private UnityEvent aoComecarCutscene;  // o preto começou a sumir (ex.: dar Play na cutscene)

    private CanvasGroup grupo;
    private bool ocupado;

    private void Awake()
    {
        grupo = GetComponent<CanvasGroup>();
        grupo.alpha = clarearAoIniciar ? 1f : 0f;
        grupo.blocksRaycasts = clarearAoIniciar;
    }

    private void Start()
    {
        if (clarearAoIniciar)
            StartCoroutine(ClarearInicial());
    }

    // Modo A: ligue no OnClick do botão Jogar / Confirmar reinício
    public void Transicao()
    {
        if (ocupado) return;
        StartCoroutine(RotinaTransicao());
    }

    // Modo B: ligue no OnClick, escrevendo o nome da cena da cutscene
    public void EscurecerECarregar(string nomeCena)
    {
        if (ocupado) return;
        StartCoroutine(RotinaCarregar(nomeCena));
    }

    private IEnumerator RotinaTransicao()
    {
        ocupado = true;
        grupo.blocksRaycasts = true; // bloqueia cliques durante a transição

        yield return Fade(0f, 1f, duracaoEscurecer);

        aoFicarPreto?.Invoke();
        yield return new WaitForSecondsRealtime(tempoNoPreto);

        aoComecarCutscene?.Invoke();
        yield return Fade(1f, 0f, duracaoClarear);

        grupo.blocksRaycasts = false;
        ocupado = false;
    }

    private IEnumerator RotinaCarregar(string nomeCena)
    {
        ocupado = true;
        grupo.blocksRaycasts = true;

        yield return Fade(0f, 1f, duracaoEscurecer);
        yield return new WaitForSecondsRealtime(tempoNoPreto);

        SceneManager.LoadScene(nomeCena);
    }

    private IEnumerator ClarearInicial()
    {
        ocupado = true;

        yield return new WaitForSecondsRealtime(tempoNoPreto);

        aoComecarCutscene?.Invoke();
        yield return Fade(1f, 0f, duracaoClarear);

        grupo.blocksRaycasts = false;
        ocupado = false;
    }

    private IEnumerator Fade(float de, float para, float duracao)
    {
        float t = 0f;
        while (t < duracao)
        {
            t += Time.unscaledDeltaTime; // funciona mesmo com o jogo pausado
            float p = Mathf.Clamp01(t / duracao);
            float suave = p * p * (3f - 2f * p); // começa e termina devagar
            grupo.alpha = Mathf.Lerp(de, para, suave);
            yield return null;
        }
        grupo.alpha = para;
    }
}
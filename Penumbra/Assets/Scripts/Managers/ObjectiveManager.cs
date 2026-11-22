
using System.Collections;
using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    public TextMeshProUGUI textoObjetivo1;
    public TextMeshProUGUI textoObjetivo2;

    [Header("Painel de objetivos")]
    public GameObject painelObjetivos;
    public float tempoNaTela = 3f;

    private AnimacaoZoom animacaoZoom;
    private Coroutine rotinaObjetivos;

    private int objetivoAtual = 0;
    private bool notificacaoEmAndamento = false;

    private const string CHAVE_NPC_LIBERADO = "NPCOpcionalLiberado";
    private const string CHAVE_NOTIFICACAO_PENDENTE = "NotificacaoObjetivoPendente";

    private string[] objetivos =
    {
        "Vá até a Delegacia.",
        "Fale com seu tio.",
        "Pegue a fita VHS.",
        "Vá até a Peixaria e converse com o pescador.",
        "Vá até a casa da mãe da Bianca.",
        "Converse com Dona Cida.",
        "Entre no quarto da Bianca e resolva o puzzle.",
        "Volte à sala e converse com Dona Cida.",
        "Receba a fita VHS de Dona Cida.",
        "Explore a floresta e siga em direção à igreja.",
        "Resolva o último puzzle para entrar na igreja.",
        "Investigue o Almoxarifado."
    };

    void Start()
    {
        objetivoAtual = PlayerPrefs.GetInt("ObjetivoAtual", 0);

        if (painelObjetivos != null)
        {
            animacaoZoom = painelObjetivos.GetComponent<AnimacaoZoom>();

            if (animacaoZoom == null)
                Debug.LogError("Adicione AnimacaoZoom ao Painel_Objetivos!");

            painelObjetivos.SetActive(false);
        }
        else
        {
            Debug.LogError("Conecte Painel_Objetivos no Inspector!");
        }

        AtualizarObjetivos();
    }

    public void ConcluirObjetivo()
    {
        objetivoAtual = PlayerPrefs.GetInt("ObjetivoAtual", 0);

        if (objetivoAtual >= objetivos.Length)
            return;

        if (objetivoAtual == 3)
        {
            PlayerPrefs.SetInt(CHAVE_NPC_LIBERADO, 1);
            Debug.Log("NPC OPCIONAL LIBERADO NA CENA_Teste!");
        }

        objetivoAtual++;

        PlayerPrefs.SetInt("ObjetivoAtual", objetivoAtual);
        PlayerPrefs.SetInt(CHAVE_NOTIFICACAO_PENDENTE, 1);

        PlayerPrefs.Save();

        Debug.Log("OBJETIVO CONCLUÍDO! NOVO VALOR: " + objetivoAtual);

        AtualizarObjetivos();
    }

    void AtualizarObjetivos()
    {
        objetivoAtual = Mathf.Clamp(objetivoAtual, 0, objetivos.Length);

        if (textoObjetivo1 == null || textoObjetivo2 == null)
        {
            Debug.LogError("Conecte os dois textos de objetivos no Inspector!");
            return;
        }

        if (objetivoAtual == 0)
        {
            textoObjetivo1.text = objetivos[0];
            textoObjetivo1.fontStyle = FontStyles.Normal;
            textoObjetivo1.color = Color.black;

            textoObjetivo2.text = objetivos[1];
            textoObjetivo2.fontStyle = FontStyles.Normal;
            textoObjetivo2.color = Color.black;
        }
        else
        {
            textoObjetivo1.text = objetivos[objetivoAtual - 1];
            textoObjetivo1.fontStyle = FontStyles.Strikethrough;
            textoObjetivo1.color = new Color(0f, 0f, 0f, 0.4f);

            if (objetivoAtual < objetivos.Length)
            {
                textoObjetivo2.text = objetivos[objetivoAtual];
                textoObjetivo2.fontStyle = FontStyles.Normal;
                textoObjetivo2.color = Color.black;
            }
            else
            {
                textoObjetivo2.text = "";
            }
        }
    }

    // Chamado pelo PlayerMovement quando Alice começa a andar.
    public void AoJogadorComecarAMover()
    {
        if (PlayerPrefs.GetInt(CHAVE_NOTIFICACAO_PENDENTE, 0) != 1)
            return;

        if (notificacaoEmAndamento)
            return;

        if (Time.timeScale == 0f || FadePreto.TransicaoAtiva)
            return;

        MostrarObjetivosAutomaticamente();
    }

    void MostrarObjetivosAutomaticamente()
    {
        if (painelObjetivos == null || animacaoZoom == null)
        {
            Debug.LogError("Painel ou AnimacaoZoom não encontrado!");
            return;
        }

        PlayerPrefs.SetInt(CHAVE_NOTIFICACAO_PENDENTE, 0);
        PlayerPrefs.Save();

        notificacaoEmAndamento = true;

        painelObjetivos.SetActive(false);
        painelObjetivos.SetActive(true);

        Debug.Log("ANIMAÇÃO DE CONCLUSÃO DO OBJETIVO INICIADA!");

        if (rotinaObjetivos != null)
            StopCoroutine(rotinaObjetivos);

        rotinaObjetivos = StartCoroutine(ExibirObjetivos());
    }

    IEnumerator ExibirObjetivos()
    {
        yield return new WaitForSecondsRealtime(
            Mathf.Max(0f, tempoNaTela)
        );

        if (painelObjetivos != null &&
            painelObjetivos.activeSelf &&
            notificacaoEmAndamento)
        {
            animacaoZoom.Fechar();

            yield return new WaitUntil(() =>
                painelObjetivos == null ||
                !painelObjetivos.activeSelf
            );
        }

        notificacaoEmAndamento = false;
        rotinaObjetivos = null;
    }

    public void CancelarNotificacaoParaPausa()
    {
        if (!notificacaoEmAndamento)
            return;

        if (rotinaObjetivos != null)
        {
            StopCoroutine(rotinaObjetivos);
            rotinaObjetivos = null;
        }

        notificacaoEmAndamento = false;

        if (painelObjetivos != null)
            painelObjetivos.SetActive(false);

        Debug.Log("Notificação interrompida para abrir a pausa.");
    }
}


using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static bool DialogoGlobalAtivo = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetarDialogoGlobal()
    {
        DialogoGlobalAtivo = false;
    }

    public GameObject painelDialogo;
    public TextMeshProUGUI textoDialogo;
    public TextMeshProUGUI nomeNPC;
    public ObjectiveManager objectiveManager;

    [Header("Imagem dos Personagens")]
    public Image imagemPersonagem;
    public Sprite imagemDoNPC;
    public Sprite imagemDaAlice;
    public Sprite imagemDaAliceBocaAberta;
    public Sprite imagemNPCBocaAberta;

    [Header("Expressões das respostas da Alice")]
    public Sprite[] aliceBocaFechadaPorResposta;
    public Sprite[] aliceBocaAbertaPorResposta;

    [Header("Expressões do NPC por fala")]
    public Sprite[] npcBocaFechadaPorFala;
    public Sprite[] npcBocaAbertaPorFala;

    [Header("Índice da resposta da Alice por fala do NPC")]
    public int[] indiceRespostaPorFala;

    [Header("Expressão de fala especial da Alice")]
    public Sprite aliceEspecialBocaFechada;
    public Sprite aliceEspecialBocaAberta;

    [Header("Expressão especial do pescador - VAI EMBORA")]
    public Sprite pescadorVaiEmboraBocaFechada;
    public Sprite pescadorVaiEmboraBocaAberta;

    [Header("Animação da fala")]
    public float intervaloAnimacaoBoca = 0.2f;

    private Coroutine animacaoBoca;

    private Sprite spriteBocaFechadaAtual;
    private Sprite spriteBocaAbertaAtual;

    [Header("Animação do texto")]
    public float velocidadeTexto = 0.03f;

    private Coroutine digitacaoTexto;
    private bool textoDigitando = false;

    [Header("Puzzle dos Peixes")]
    public GameObject painelPuzzlePeixes;

    [Header("Indicação de Interação")]
    public GameObject indicadorInteracao;

    [Header("Configuração do NPC")]
    public string nomeDoPersonagem = "Delegado";
    public string nomeProtagonista = "Alice";

    [Header("Falas do NPC")]
    [TextArea(2, 4)]
    public string[] falas;

    [Header("Respostas da Alice")]
    [TextArea(2, 4)]
    public string[] respostasAlice;

    [Header("Falas do pescador após o puzzle")]
    [TextArea(2, 4)]
    public string[] falasDepoisPuzzle;

    [Header("Respostas da Alice após o puzzle")]
    [TextArea(2, 4)]
    public string[] respostasAliceDepoisPuzzle;

    public int objetivoParaConcluir = 3;

    [Header("Item liberado após o diálogo")]
    public GameObject itemParaRevelar;

    private bool jogadorPerto = false;
    private bool dialogoAtivo = false;
    private bool mostrandoResposta = false;
    private bool falaEspecial = false;
    private bool dialogoFinalPescador = false;

    private bool pescadorInteracaoBloqueada = false;

    private int falaAtual = 0;
    private int respostaAtual = -1;

    private string[] falasOriginais;
    private string[] respostasOriginais;

    void Start()
    {
        DialogoGlobalAtivo = false;

        painelDialogo.SetActive(false);

        if (indicadorInteracao != null)
            indicadorInteracao.SetActive(false);

        if (nomeDoPersonagem == "Pescador")
        {
            falasOriginais = falas;
            respostasOriginais = respostasAlice;

            pescadorInteracaoBloqueada =
                PlayerPrefs.GetInt("PescadorPrimeiraConversa", 0) == 1 &&
                PlayerPrefs.GetInt("DialogoFinalPescadorConcluido", 0) == 0;
        }

        if (itemParaRevelar != null)
        {
            bool conversou = PlayerPrefs.GetInt("DelegadoConversou", 0) == 1;
            bool fitaColetada = PlayerPrefs.GetInt("FitaVHS01Coletada", 0) == 1;

            itemParaRevelar.SetActive(conversou && !fitaColetada);
        }
    }

    void Update()
    {
        // Bloqueia novas interações com o pescador,
        // mas permite avançar um diálogo automático já iniciado.
        if (nomeDoPersonagem == "Pescador" &&
            pescadorInteracaoBloqueada &&
            !dialogoAtivo)
        {
            if (indicadorInteracao != null)
                indicadorInteracao.SetActive(false);

            return;
        }

        if (indicadorInteracao != null)
        {
            indicadorInteracao.SetActive(
                jogadorPerto && !dialogoAtivo &&
                !pescadorInteracaoBloqueada
            );
        }

        // F inicia e avança os diálogos.
        if (jogadorPerto && Input.GetKeyDown(KeyCode.F))
        {
            if (!dialogoAtivo)
            {
                IniciarDialogo();
            }
            else if (textoDigitando)
            {
                CompletarTexto();
            }
            else
            {
                AvancarDialogo();
            }
        }
    }

    public void IniciarDialogoAutomatico()
    {
        if (dialogoAtivo)
            return;

        jogadorPerto = true;

        Debug.Log("=== DIÁLOGO AUTOMÁTICO ===");
        Debug.Log("Personagem: " + nomeDoPersonagem);
        Debug.Log("Puzzle concluído: " + PlayerPrefs.GetInt("PescadorPuzzleConcluido", 0));
        Debug.Log("Falas finais cadastradas: " + (falasDepoisPuzzle != null ? falasDepoisPuzzle.Length : 0));

        // Força a seleção das falas finais do pescador.
        if (nomeDoPersonagem == "Pescador")
        {
            dialogoFinalPescador = true;
            falas = falasDepoisPuzzle;
            respostasAlice = respostasAliceDepoisPuzzle;

            IniciarDialogo(true);
        }
        else
        {
            IniciarDialogo();
        }
    }

    void IniciarDialogo(bool forcarDialogoFinalPescador = false)
    {
        // Depois do diálogo final, o pescador só responde VAI EMBORA.
        if (nomeDoPersonagem == "Pescador" &&
            PlayerPrefs.GetInt("DialogoFinalPescadorConcluido", 0) == 1)
        {
            MostrarFalaEspecial("VAI EMBORA.", nomeDoPersonagem);
            return;
        }

        bool conversou = PlayerPrefs.GetInt("DelegadoConversou", 0) == 1;
        bool fitaColetada = PlayerPrefs.GetInt("FitaVHS01Coletada", 0) == 1;

        if (nomeDoPersonagem == "Delegado" && fitaColetada)
        {
            MostrarFalaEspecial("Acho melhor não perturbar mais ele.", nomeProtagonista);
            return;
        }

        if (nomeDoPersonagem == "Delegado" && conversou)
        {
            MostrarFalaEspecial("Pode pegar a fita.", nomeDoPersonagem);
            return;
        }

        // Seleciona a conversa correta do pescador.
        if (nomeDoPersonagem == "Pescador")
        {
            dialogoFinalPescador =
                forcarDialogoFinalPescador ||
                PlayerPrefs.GetInt("PescadorPuzzleConcluido", 0) == 1;

            if (dialogoFinalPescador)
            {
                Debug.Log("=== SELECIONANDO DIÁLOGO FINAL ===");

                falas = falasDepoisPuzzle;
                respostasAlice = respostasAliceDepoisPuzzle;
            }
            else
            {
                falas = falasOriginais;
                respostasAlice = respostasOriginais;
            }
        }

        if (falas == null || falas.Length == 0)
        {
            Debug.LogError("Não há falas configuradas para " + nomeDoPersonagem);
            return;
        }

        dialogoAtivo = true;
        DialogoGlobalAtivo = true;

        mostrandoResposta = false;
        falaEspecial = false;
        falaAtual = 0;
        respostaAtual = -1;

        painelDialogo.SetActive(true);
        MostrarFala();
    }

    void MostrarFalaEspecial(string mensagem, string personagem)
    {
        dialogoAtivo = true;
        DialogoGlobalAtivo = true;
        falaEspecial = true;

        painelDialogo.SetActive(true);
        nomeNPC.text = personagem;
        textoDialogo.text = mensagem;

        IniciarDigitacao();

        if (personagem == nomeProtagonista)
        {
            ConfigurarAnimacaoAlice(
                aliceEspecialBocaFechada,
                aliceEspecialBocaAberta
            );
        }
        else if (nomeDoPersonagem == "Pescador" &&
                 mensagem == "VAI EMBORA.")
        {
            // Usa os sprites exclusivos do pescador.
            PararAnimacaoBoca();

            spriteBocaFechadaAtual =
                pescadorVaiEmboraBocaFechada != null
                ? pescadorVaiEmboraBocaFechada
                : imagemDoNPC;

            spriteBocaAbertaAtual =
                pescadorVaiEmboraBocaAberta != null
                ? pescadorVaiEmboraBocaAberta
                : imagemNPCBocaAberta;

            if (imagemPersonagem != null)
                imagemPersonagem.sprite = spriteBocaFechadaAtual;

            IniciarAnimacaoBoca();
        }
        else
        {
            ConfigurarAnimacaoNPC();
        }
    }

    void AvancarDialogo()
    {
        if (falaEspecial)
        {
            FinalizarDialogo();
            return;
        }

        if (!mostrandoResposta)
        {
            int indiceResposta = falaAtual;

            if (indiceRespostaPorFala != null &&
                falaAtual < indiceRespostaPorFala.Length)
            {
                indiceResposta = indiceRespostaPorFala[falaAtual];
            }

            if (respostasAlice != null &&
                indiceResposta >= 0 &&
                indiceResposta < respostasAlice.Length &&
                !string.IsNullOrWhiteSpace(respostasAlice[indiceResposta]))
            {
                respostaAtual = indiceResposta;
                mostrandoResposta = true;

                MostrarFala();
                return;
            }

            falaAtual++;
        }
        else
        {
            mostrandoResposta = false;
            respostaAtual = -1;
            falaAtual++;
        }

        if (falaAtual < falas.Length)
        {
            MostrarFala();
        }
        else
        {
            FinalizarDialogo();
        }
    }

    void MostrarFala()
    {
        if (mostrandoResposta)
        {
            nomeNPC.text = nomeProtagonista;
            textoDialogo.text = respostasAlice[respostaAtual];

            Sprite fechada = imagemDaAlice;
            Sprite aberta = null;

            if (aliceBocaFechadaPorResposta != null &&
                respostaAtual < aliceBocaFechadaPorResposta.Length &&
                aliceBocaFechadaPorResposta[respostaAtual] != null)
            {
                fechada = aliceBocaFechadaPorResposta[respostaAtual];
            }

            if (aliceBocaAbertaPorResposta != null &&
                respostaAtual < aliceBocaAbertaPorResposta.Length)
            {
                aberta = aliceBocaAbertaPorResposta[respostaAtual];
            }

            ConfigurarAnimacaoAlice(fechada, aberta);
        }
        else
        {
            nomeNPC.text = nomeDoPersonagem;
            textoDialogo.text = falas[falaAtual];

            ConfigurarAnimacaoNPC(falaAtual);
        }

        IniciarDigitacao();
    }

    void ConfigurarAnimacaoAlice(Sprite fechada, Sprite aberta)
    {
        PararAnimacaoBoca();

        spriteBocaFechadaAtual = fechada != null
            ? fechada
            : imagemDaAlice;

        spriteBocaAbertaAtual = aberta != null
            ? aberta
            : imagemDaAliceBocaAberta;

        if (imagemPersonagem != null)
            imagemPersonagem.sprite = spriteBocaFechadaAtual;

        IniciarAnimacaoBoca();
    }

    void ConfigurarAnimacaoNPC(int indiceFala = -1)
    {
        PararAnimacaoBoca();

        spriteBocaFechadaAtual = imagemDoNPC;
        spriteBocaAbertaAtual = imagemNPCBocaAberta;

        if (indiceFala >= 0 &&
            npcBocaFechadaPorFala != null &&
            indiceFala < npcBocaFechadaPorFala.Length &&
            npcBocaFechadaPorFala[indiceFala] != null)
        {
            spriteBocaFechadaAtual =
                npcBocaFechadaPorFala[indiceFala];
        }

        if (indiceFala >= 0 &&
            npcBocaAbertaPorFala != null &&
            indiceFala < npcBocaAbertaPorFala.Length &&
            npcBocaAbertaPorFala[indiceFala] != null)
        {
            spriteBocaAbertaAtual =
                npcBocaAbertaPorFala[indiceFala];
        }

        if (imagemPersonagem != null)
            imagemPersonagem.sprite = spriteBocaFechadaAtual;

        IniciarAnimacaoBoca();
    }

    void IniciarAnimacaoBoca()
    {
        if (imagemPersonagem != null &&
            spriteBocaAbertaAtual != null)
        {
            animacaoBoca = StartCoroutine(AnimarBoca());
        }
    }

    IEnumerator AnimarBoca()
    {
        bool bocaAberta = false;

        while (true)
        {
            bocaAberta = !bocaAberta;

            imagemPersonagem.sprite = bocaAberta
                ? spriteBocaAbertaAtual
                : spriteBocaFechadaAtual;

            yield return new WaitForSeconds(intervaloAnimacaoBoca);
        }
    }

    void PararAnimacaoBoca()
    {
        if (animacaoBoca != null)
        {
            StopCoroutine(animacaoBoca);
            animacaoBoca = null;
        }
    }

    void IniciarDigitacao()
    {
        if (digitacaoTexto != null)
        {
            StopCoroutine(digitacaoTexto);
            digitacaoTexto = null;
        }

        textoDialogo.maxVisibleCharacters = 0;
        textoDialogo.ForceMeshUpdate();

        textoDigitando = true;
        digitacaoTexto = StartCoroutine(AnimarTexto());
    }

    IEnumerator AnimarTexto()
    {
        int totalCaracteres = textoDialogo.textInfo.characterCount;

        for (int i = 0; i <= totalCaracteres; i++)
        {
            textoDialogo.maxVisibleCharacters = i;

            yield return new WaitForSeconds(velocidadeTexto);
        }

        textoDialogo.maxVisibleCharacters = int.MaxValue;

        textoDigitando = false;
        digitacaoTexto = null;
    }

    void CompletarTexto()
    {
        if (digitacaoTexto != null)
        {
            StopCoroutine(digitacaoTexto);
            digitacaoTexto = null;
        }

        textoDialogo.maxVisibleCharacters = int.MaxValue;
        textoDigitando = false;
    }

    void FinalizarDialogo()
    {
        if (digitacaoTexto != null)
        {
            StopCoroutine(digitacaoTexto);
            digitacaoTexto = null;
        }

        textoDigitando = false;
        textoDialogo.maxVisibleCharacters = int.MaxValue;

        PararAnimacaoBoca();

        dialogoAtivo = false;
        DialogoGlobalAtivo = false;
        falaEspecial = false;

        painelDialogo.SetActive(false);

        // Ações específicas do delegado.
        if (nomeDoPersonagem == "Delegado" &&
            PlayerPrefs.GetInt("DelegadoConversou", 0) == 0)
        {
            PlayerPrefs.SetInt("DelegadoConversou", 1);
            PlayerPrefs.Save();

            if (itemParaRevelar != null)
                itemParaRevelar.SetActive(true);
        }

        // Ações específicas do pescador.
        if (nomeDoPersonagem == "Pescador")
        {
            bool puzzleConcluido =
                PlayerPrefs.GetInt("PescadorPuzzleConcluido", 0) == 1;

            if (!puzzleConcluido)
            {
                PlayerPrefs.SetInt("PescadorPrimeiraConversa", 1);
                PlayerPrefs.Save();

                pescadorInteracaoBloqueada = true;

                if (indicadorInteracao != null)
                    indicadorInteracao.SetActive(false);

                if (painelPuzzlePeixes != null)
                    painelPuzzlePeixes.SetActive(true);

                return;
            }

            // Conclui o objetivo somente após a conversa final.
            if (dialogoFinalPescador &&
                PlayerPrefs.GetInt("DialogoFinalPescadorConcluido", 0) == 0)
            {
                int objetivoAtual = PlayerPrefs.GetInt("ObjetivoAtual", 0);

                if (objectiveManager == null)
                {
                    Debug.LogError("Objective Manager não foi conectado no Pescador!");
                    return;
                }

                if (objetivoAtual != objetivoParaConcluir)
                {
                    Debug.LogWarning(
                        "Objetivo do pescador não concluído. Objetivo atual: " +
                        objetivoAtual + " | Objetivo esperado: " + objetivoParaConcluir
                    );
                    return;
                }

                objectiveManager.ConcluirObjetivo();

                PlayerPrefs.SetInt("DialogoFinalPescadorConcluido", 1);
                PlayerPrefs.Save();

                pescadorInteracaoBloqueada = false;

                Debug.Log("Objetivo do pescador concluído! Casa da Bianca liberada.");
            }

            return;
        }

        // Conclusão do objetivo configurado para cada outro NPC.
        if (objectiveManager != null &&
            PlayerPrefs.GetInt("ObjetivoAtual", 0) == objetivoParaConcluir)
        {
            objectiveManager.ConcluirObjetivo();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            jogadorPerto = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            jogadorPerto = false;
    }
}


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

    [Header("Sprites da Dona Cida - pós-puzzle do quarto")]
    public Sprite[] donaCidaBocaFechadaPorFalaDepoisPuzzle;
    public Sprite[] donaCidaBocaAbertaPorFalaDepoisPuzzle;

    [Header("Sprites da Alice - respostas pós-puzzle do quarto")]
    public Sprite[] aliceBocaFechadaPorRespostaDepoisPuzzle;
    public Sprite[] aliceBocaAbertaPorRespostaDepoisPuzzle;

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

    [Header("Falas da Dona Cida após o puzzle do quarto")]
    [TextArea(2, 4)]
    public string[] falasDepoisPuzzleDonaCida;

    [Header("Respostas da Alice após o puzzle do quarto")]
    [TextArea(2, 4)]
    public string[] respostasAliceDepoisPuzzleDonaCida;

    [Header("Dona Cida após receber a segunda fita VHS")]
    [TextArea(2, 4)]
    public string falaDonaCidaDepoisDaFita = "...";
    public Sprite spriteDonaCidaDepoisDaFita;

    public int objetivoParaConcluir = 3;

    [Header("Item liberado após o diálogo")]
    public GameObject itemParaRevelar;

    private bool jogadorPerto = false;
    private bool dialogoAtivo = false;
    private bool mostrandoResposta = false;
    private bool falaEspecial = false;
    private bool dialogoFinalPescador = false;

    private bool pescadorInteracaoBloqueada = false;
    private bool donaCidaDialogoPosPuzzle = false;

    private int falaAtual = 0;
    private int respostaAtual = -1;

    private string[] falasOriginais;
    private string[] respostasOriginais;

    // Controle de continuidade do diálogo final do pescador.
    private const string CHAVE_DIALOGO_PESCADOR_INICIADO =
        "DialogoFinalPescadorIniciado";

    private const string CHAVE_FALA_PESCADOR =
        "DialogoFinalPescadorFala";

    private const string CHAVE_RESPOSTA_PESCADOR =
        "DialogoFinalPescadorResposta";

    private const string CHAVE_MOSTRANDO_RESPOSTA_PESCADOR =
        "DialogoFinalPescadorMostrandoResposta";

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

            // Verifica se existe um diálogo final interrompido.
            if (PlayerPrefs.GetInt("PescadorPuzzleConcluido", 0) == 1 &&
                PlayerPrefs.GetInt("DialogoFinalPescadorConcluido", 0) == 0 &&
                PlayerPrefs.GetInt(CHAVE_DIALOGO_PESCADOR_INICIADO, 0) == 1)
            {
                StartCoroutine(RetomarDialogoFinalPescador());
            }
        }

        // Configuração do item do Delegado.
        if (nomeDoPersonagem == "Delegado" && itemParaRevelar != null)
        {
            bool conversou =
                PlayerPrefs.GetInt("DelegadoConversou", 0) == 1;

            bool fitaColetada =
                PlayerPrefs.GetInt("FitaVHS01Coletada", 0) == 1;

            itemParaRevelar.SetActive(conversou && !fitaColetada);
        }

        // Configuração da segunda fita da Dona Cida.
        if (nomeDoPersonagem == "Dona Cida" && itemParaRevelar != null)
        {
            bool fitaLiberada =
                PlayerPrefs.GetInt("DonaCidaFitaLiberada", 0) == 1;

            itemParaRevelar.SetActive(fitaLiberada);
        }

        // Verifica se Alice chegou após concluir o puzzle do quarto.
        if (nomeDoPersonagem == "Dona Cida" &&
            PlayerPrefs.GetInt("IniciarDialogoDonaCida", 0) == 1)
        {
            PlayerPrefs.SetInt("IniciarDialogoDonaCida", 0);
            PlayerPrefs.SetInt("DonaCidaDialogoDoQuarto", 1);
            PlayerPrefs.Save();

            donaCidaDialogoPosPuzzle = true;
            StartCoroutine(IniciarDialogoDonaCida());
        }
    }

    IEnumerator IniciarDialogoDonaCida()
    {
        yield return null;

        Debug.Log("Iniciando diálogo automático com Dona Cida!");

        IniciarDialogoAutomatico();
    }

    // Retoma o diálogo final do pescador de onde parou.
    IEnumerator RetomarDialogoFinalPescador()
    {
        yield return null;

        if (dialogoAtivo)
            yield break;

        Debug.Log("Retomando diálogo final do pescador!");

        dialogoFinalPescador = true;

        falas = falasDepoisPuzzle;
        respostasAlice = respostasAliceDepoisPuzzle;

        falaAtual = PlayerPrefs.GetInt(CHAVE_FALA_PESCADOR, 0);
        respostaAtual = PlayerPrefs.GetInt(CHAVE_RESPOSTA_PESCADOR, -1);

        mostrandoResposta =
            PlayerPrefs.GetInt(CHAVE_MOSTRANDO_RESPOSTA_PESCADOR, 0) == 1;

        falaEspecial = false;

        if (falas == null || falas.Length == 0)
        {
            Debug.LogError("Não há falas pós-puzzle configuradas para o pescador!");
            yield break;
        }

        if (falaAtual < 0 || falaAtual >= falas.Length)
            falaAtual = 0;

        if (mostrandoResposta &&
            (respostasAlice == null ||
             respostaAtual < 0 ||
             respostaAtual >= respostasAlice.Length))
        {
            mostrandoResposta = false;
            respostaAtual = -1;
        }

        dialogoAtivo = true;
        DialogoGlobalAtivo = true;
        jogadorPerto = true;

        painelDialogo.SetActive(true);

        MostrarFala();
    }

    // Salva a posição atual do diálogo final do pescador.
    void SalvarProgressoDialogoPescador()
    {
        if (nomeDoPersonagem != "Pescador" ||
            !dialogoFinalPescador ||
            !dialogoAtivo)
            return;

        PlayerPrefs.SetInt(CHAVE_DIALOGO_PESCADOR_INICIADO, 1);
        PlayerPrefs.SetInt(CHAVE_FALA_PESCADOR, falaAtual);
        PlayerPrefs.SetInt(CHAVE_RESPOSTA_PESCADOR, respostaAtual);

        PlayerPrefs.SetInt(
            CHAVE_MOSTRANDO_RESPOSTA_PESCADOR,
            mostrandoResposta ? 1 : 0
        );

        PlayerPrefs.Save();
    }

    void Update()
    {
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
        Debug.Log("Puzzle do quarto: " +
            PlayerPrefs.GetInt("PuzzleQuartoConcluido", 0));

        if (nomeDoPersonagem == "Pescador")
        {
            dialogoFinalPescador = true;
            falas = falasDepoisPuzzle;
            respostasAlice = respostasAliceDepoisPuzzle;

            IniciarDialogo(true);
        }
        else if (nomeDoPersonagem == "Dona Cida" &&
                 donaCidaDialogoPosPuzzle)
        {
            falas = falasDepoisPuzzleDonaCida;
            respostasAlice = respostasAliceDepoisPuzzleDonaCida;

            IniciarDialogo();
        }
        else
        {
            IniciarDialogo();
        }
    }

    void IniciarDialogo(bool forcarDialogoFinalPescador = false)
    {
        bool segundaFitaColetada =
            InventoryManager.instance != null &&
            InventoryManager.instance.itens.Contains("Fita VHS 2");

        if (nomeDoPersonagem == "Dona Cida" && segundaFitaColetada)
        {
            MostrarFalaEspecial(falaDonaCidaDepoisDaFita, nomeDoPersonagem);

            PararAnimacaoBoca();

            if (imagemPersonagem != null && spriteDonaCidaDepoisDaFita != null)
                imagemPersonagem.sprite = spriteDonaCidaDepoisDaFita;

            return;
        }

        if (nomeDoPersonagem == "Dona Cida" &&
            PlayerPrefs.GetInt("DonaCidaFitaLiberada", 0) == 1 &&
            !segundaFitaColetada)
        {
            MostrarFalaEspecial("Pode pegar a fita, minha filha.", nomeDoPersonagem);
            return;
        }

        if (nomeDoPersonagem == "Dona Cida" &&
            PlayerPrefs.GetInt("DonaCidaPrimeiraConversaConcluida", 0) == 1 &&
            PlayerPrefs.GetInt("PuzzleQuartoConcluido", 0) == 0)
        {
            MostrarFalaEspecial(
                "O quarto da Bianca é o primeiro da esquerda para a direita, pode entrar lá, a porta tá aberta.",
                nomeDoPersonagem
            );
            return;
        }

        if (nomeDoPersonagem == "Pescador" &&
            PlayerPrefs.GetInt("DialogoFinalPescadorConcluido", 0) == 1)
        {
            MostrarFalaEspecial("VAI EMBORA.", nomeDoPersonagem);
            return;
        }

        bool conversou =
            PlayerPrefs.GetInt("DelegadoConversou", 0) == 1;

        bool fitaColetada =
            PlayerPrefs.GetInt("FitaVHS01Coletada", 0) == 1;

        if (nomeDoPersonagem == "Delegado" && fitaColetada)
        {
            MostrarFalaEspecial(
                "Acho melhor não perturbar mais ele.",
                nomeProtagonista
            );
            return;
        }

        if (nomeDoPersonagem == "Delegado" && conversou)
        {
            MostrarFalaEspecial("Pode pegar a fita.", nomeDoPersonagem);
            return;
        }

        if (nomeDoPersonagem == "Pescador")
        {
            dialogoFinalPescador =
                forcarDialogoFinalPescador ||
                PlayerPrefs.GetInt("PescadorPuzzleConcluido", 0) == 1;

            if (dialogoFinalPescador)
            {
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
            MostrarFala();
        else
            FinalizarDialogo();
    }

    void MostrarFala()
    {
        if (mostrandoResposta)
        {
            nomeNPC.text = nomeProtagonista;
            textoDialogo.text = respostasAlice[respostaAtual];

            Sprite fechada = imagemDaAlice;
            Sprite aberta = null;

            Sprite[] spritesFechadosAlice =
                nomeDoPersonagem == "Dona Cida" && donaCidaDialogoPosPuzzle
                ? aliceBocaFechadaPorRespostaDepoisPuzzle
                : aliceBocaFechadaPorResposta;

            Sprite[] spritesAbertosAlice =
                nomeDoPersonagem == "Dona Cida" && donaCidaDialogoPosPuzzle
                ? aliceBocaAbertaPorRespostaDepoisPuzzle
                : aliceBocaAbertaPorResposta;

            if (spritesFechadosAlice != null &&
                respostaAtual < spritesFechadosAlice.Length &&
                spritesFechadosAlice[respostaAtual] != null)
            {
                fechada = spritesFechadosAlice[respostaAtual];
            }

            if (spritesAbertosAlice != null &&
                respostaAtual < spritesAbertosAlice.Length &&
                spritesAbertosAlice[respostaAtual] != null)
            {
                aberta = spritesAbertosAlice[respostaAtual];
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

        // Salva a fala ou resposta atual do pescador.
        SalvarProgressoDialogoPescador();
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

        Sprite[] spritesFechadosNPC =
            nomeDoPersonagem == "Dona Cida" && donaCidaDialogoPosPuzzle
            ? donaCidaBocaFechadaPorFalaDepoisPuzzle
            : npcBocaFechadaPorFala;

        Sprite[] spritesAbertosNPC =
            nomeDoPersonagem == "Dona Cida" && donaCidaDialogoPosPuzzle
            ? donaCidaBocaAbertaPorFalaDepoisPuzzle
            : npcBocaAbertaPorFala;

        if (indiceFala >= 0 &&
            spritesFechadosNPC != null &&
            indiceFala < spritesFechadosNPC.Length &&
            spritesFechadosNPC[indiceFala] != null)
        {
            spriteBocaFechadaAtual =
                spritesFechadosNPC[indiceFala];
        }

        if (indiceFala >= 0 &&
            spritesAbertosNPC != null &&
            indiceFala < spritesAbertosNPC.Length &&
            spritesAbertosNPC[indiceFala] != null)
        {
            spriteBocaAbertaAtual =
                spritesAbertosNPC[indiceFala];
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

        // Ações específicas do Delegado.
        if (nomeDoPersonagem == "Delegado" &&
            PlayerPrefs.GetInt("DelegadoConversou", 0) == 0)
        {
            PlayerPrefs.SetInt("DelegadoConversou", 1);
            PlayerPrefs.Save();

            if (itemParaRevelar != null)
                itemParaRevelar.SetActive(true);
        }

        // Ações específicas do Pescador.
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

                // Apaga o progresso salvo porque o diálogo foi concluído.
                PlayerPrefs.DeleteKey(CHAVE_DIALOGO_PESCADOR_INICIADO);
                PlayerPrefs.DeleteKey(CHAVE_FALA_PESCADOR);
                PlayerPrefs.DeleteKey(CHAVE_RESPOSTA_PESCADOR);
                PlayerPrefs.DeleteKey(CHAVE_MOSTRANDO_RESPOSTA_PESCADOR);

                PlayerPrefs.Save();

                pescadorInteracaoBloqueada = false;

                Debug.Log("Objetivo do pescador concluído! Casa da Bianca liberada.");
            }

            return;
        }

        // Ações específicas da Dona Cida.
        if (nomeDoPersonagem == "Dona Cida")
        {
            bool puzzleDoQuartoConcluido =
                PlayerPrefs.GetInt("PuzzleQuartoConcluido", 0) == 1;

            bool dialogoPosPuzzleJaConcluido =
                PlayerPrefs.GetInt("DonaCidaDialogoPosPuzzleConcluido", 0) == 1;

            if (puzzleDoQuartoConcluido && !dialogoPosPuzzleJaConcluido)
            {
                int objetivoAtual = PlayerPrefs.GetInt("ObjetivoAtual", 0);

                if (objetivoAtual == 7)
                {
                    if (objectiveManager != null)
                    {
                        objectiveManager.ConcluirObjetivo();
                        Debug.Log("Objetivo 'Volte à sala e converse com Dona Cida' concluído!");
                    }
                    else
                    {
                        Debug.LogError("Objective Manager não foi conectado à Dona Cida!");
                    }
                }
                else if (objetivoAtual > 7)
                {
                    Debug.Log("Objetivo pós-puzzle da Dona Cida já ultrapassado. Sequência mantida.");
                }
                else
                {
                    Debug.LogWarning(
                        "A conversa pós-puzzle terminou, mas o objetivo atual não é 7. " +
                        "Objetivo atual: " + objetivoAtual
                    );
                }

                PlayerPrefs.SetInt("DonaCidaDialogoPosPuzzleConcluido", 1);
                PlayerPrefs.SetInt("DonaCidaDialogoDoQuarto", 0);
                PlayerPrefs.SetInt("DonaCidaFitaLiberada", 1);
                PlayerPrefs.SetInt("DonaCidaPrimeiraConversaConcluida", 1);
                PlayerPrefs.Save();

                donaCidaDialogoPosPuzzle = false;

                if (itemParaRevelar != null)
                    itemParaRevelar.SetActive(true);

                Debug.Log("Dona Cida liberou a segunda fita VHS!");
            }
            else if (!puzzleDoQuartoConcluido &&
                     PlayerPrefs.GetInt("DonaCidaPrimeiraConversaConcluida", 0) == 0)
            {
                int objetivoAtual = PlayerPrefs.GetInt("ObjetivoAtual", 0);

                if (objetivoAtual == 5)
                {
                    if (objectiveManager == null)
                    {
                        Debug.LogError("Objective Manager não foi conectado à Dona Cida!");
                    }
                    else
                    {
                        objectiveManager.ConcluirObjetivo();

                        PlayerPrefs.SetInt("DonaCidaPrimeiraConversaConcluida", 1);
                        PlayerPrefs.Save();

                        Debug.Log("Objetivo 'Converse com Dona Cida' concluído!");
                    }
                }
                else if (objetivoAtual > 5)
                {
                    PlayerPrefs.SetInt("DonaCidaPrimeiraConversaConcluida", 1);
                    PlayerPrefs.Save();

                    Debug.Log("Objetivo da primeira conversa com Dona Cida já ultrapassado. Registro atualizado.");
                }
                else
                {
                    Debug.LogWarning(
                        "A primeira conversa com Dona Cida terminou antes do objetivo 5. " +
                        "Objetivo atual: " + objetivoAtual + " | Objetivo esperado: 5"
                    );
                }
            }

            return;
        }

        // Conclusão do objetivo configurado para os outros NPCs.
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


using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

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

    [Header("Sprites da Alice - respostas pós-puzzle da Peixaria")]
    public Sprite[] aliceBocaFechadaPorRespostaDepoisPeixaria;
    public Sprite[] aliceBocaAbertaPorRespostaDepoisPeixaria;

    [Header("Índice da resposta da Alice por fala do NPC")]
    public int[] indiceRespostaPorFala;

    [Header("Expressão de fala especial da Alice")]
    public Sprite aliceEspecialBocaFechada;
    public Sprite aliceEspecialBocaAberta;

    [Header("Sprites especiais da Alice - cena da igreja")]
    public Sprite aliceMereceMorrerBocaFechada;
    public Sprite aliceMereceMorrerBocaAberta;
    public Sprite alicePosTransformacaoSpriteFixo;

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

    [Header("Áudio das vozes")]
    public AudioSource audioVoz;
    public AudioClip vozAlice;
    public AudioClip vozNPC;
    [Range(0f, 1f)]
    public float volumeVoz = 1f;

    private Coroutine digitacaoTexto;
    private bool textoDigitando = false;

    [Header("Puzzle dos Peixes")]
    public GameObject painelPuzzlePeixes;

    [Header("Fade do puzzle dos peixes")]
    public CanvasGroup painelFadePeixes;
    public float duracaoFadeOutPeixes = 1f;
    public float duracaoFadeInPeixes = 1f;

    private bool transicaoPuzzleIniciada = false;

    [Header("Indicação de Interação")]
    public GameObject indicadorInteracao;

    [Header("Configuração do NPC")]
    public string nomeDoPersonagem = "Delegado";
    public string nomeProtagonista = "Alice";

    [Header("Configuração do Pastor")]
    public bool pastorOpcional = false;

    [Header("Pensamentos após diálogo com o Pastor")]
    public GerenciadorPensamentos gerenciadorPensamentos;

    [TextArea(2, 4)]
    public string pensamentoPastor1 =
        "Que cara bizarro... não quero falar com ele, ele me dá uma sensação estranha...";

    [TextArea(2, 4)]
    public string pensamentoPastor2 =
        "Bom, vamos voltar para o que eu estava fazendo.";

    [Header("Sprites do primeiro pensamento")]
    public Sprite alicePastorPensamento1Fechada;
    public Sprite alicePastorPensamento1Aberta;

    [Header("Sprites do segundo pensamento")]
    public Sprite alicePastorPensamento2Fechada;
    public Sprite alicePastorPensamento2Aberta;

    [Header("Diálogo automático do pastor")]
    public bool iniciarAutomaticamenteAposFade = false;
    public bool somenteAliceFalando = false;
    public UnityEvent aoFecharDialogo;

    [Header("Diálogo após a transformação do pastor")]
    public UnityEvent aoFecharDialogoPosTransformacao;
    private bool dialogoPosTransformacaoAtivo = false;
    private bool dialogoPastorPosTransformacaoIniciado = false;
    private bool dialogoPastorPosTransformacaoConcluido = false;

    [Header("Falas do NPC")]
    [TextArea(2, 4)]
    public string[] falas;

    [Header("Respostas da Alice")]
    [TextArea(2, 4)]
    public string[] respostasAlice;

    [Header("Falas do Padre Opicional")]
    [TextArea(2, 4)]
    public string[] falasPadreOpicional;

    [Header("Respostas da Alice ao Padre Opicional")]
    [TextArea(2, 4)]
    public string[] respostasAlicePadreOpicional;

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

        PararAudioVoz();

        if (indicadorInteracao != null)
            indicadorInteracao.SetActive(false);

        if (pastorOpcional)
        {
            falas = falasPadreOpicional;
            respostasAlice = respostasAlicePadreOpicional;

            Debug.Log("Diálogo do Pastor opcional configurado!");
        }

        if (nomeDoPersonagem == "Pescador")
        {
            falasOriginais = falas;
            respostasOriginais = respostasAlice;

            pescadorInteracaoBloqueada =
                PlayerPrefs.GetInt("PescadorPrimeiraConversa", 0) == 1 &&
                PlayerPrefs.GetInt("DialogoFinalPescadorConcluido", 0) == 0;

            if (PlayerPrefs.GetInt("PescadorPuzzleConcluido", 0) == 1 &&
                PlayerPrefs.GetInt("DialogoFinalPescadorConcluido", 0) == 0 &&
                PlayerPrefs.GetInt(CHAVE_DIALOGO_PESCADOR_INICIADO, 0) == 1)
            {
                StartCoroutine(RetomarDialogoFinalPescador());
            }
        }

        if (nomeDoPersonagem == "Delegado" && itemParaRevelar != null)
        {
            bool conversou =
                PlayerPrefs.GetInt("DelegadoConversou", 0) == 1;

            bool fitaColetada =
                PlayerPrefs.GetInt("FitaVHS01Coletada", 0) == 1;

            itemParaRevelar.SetActive(conversou && !fitaColetada);
        }

        if (nomeDoPersonagem == "Dona Cida" && itemParaRevelar != null)
        {
            bool fitaLiberada =
                PlayerPrefs.GetInt("DonaCidaFitaLiberada", 0) == 1;

            itemParaRevelar.SetActive(fitaLiberada);
        }

        if (nomeDoPersonagem == "Dona Cida" &&
            PlayerPrefs.GetInt("IniciarDialogoDonaCida", 0) == 1)
        {
            PlayerPrefs.SetInt("IniciarDialogoDonaCida", 0);
            PlayerPrefs.SetInt("DonaCidaDialogoDoQuarto", 1);
            PlayerPrefs.Save();

            donaCidaDialogoPosPuzzle = true;
            StartCoroutine(IniciarDialogoDonaCida());
        }

        if (nomeDoPersonagem == "Pastor" && iniciarAutomaticamenteAposFade)
        {
            StartCoroutine(IniciarDialogoPastorAposFade());
        }
    }

    // =========================================================
    // ÁUDIO DAS VOZES
    // =========================================================

    void IniciarAudioVoz(bool aliceFalando)
    {
        PararAudioVoz();

        if (audioVoz == null)
            return;

        AudioClip vozAtual = aliceFalando ? vozAlice : vozNPC;

        if (vozAtual == null)
            return;

        audioVoz.clip = vozAtual;
        audioVoz.volume = volumeVoz;
        audioVoz.loop = true;
        audioVoz.Play();
    }

    void PararAudioVoz()
    {
        if (audioVoz != null && audioVoz.isPlaying)
            audioVoz.Stop();
    }

    // =========================================================
    // FADE OUT E FADE IN DO PUZZLE DOS PEIXES
    // =========================================================

    private IEnumerator AtivarPuzzleComFade()
    {
        if (transicaoPuzzleIniciada)
            yield break;

        transicaoPuzzleIniciada = true;

        if (painelFadePeixes == null)
        {
            Debug.LogError(
                "Painel Fade Peixes não foi conectado no Inspector!"
            );

            transicaoPuzzleIniciada = false;
            DialogoGlobalAtivo = false;
            yield break;
        }

        DialogoGlobalAtivo = true;

        painelFadePeixes.gameObject.SetActive(true);
        painelFadePeixes.alpha = 0f;
        painelFadePeixes.blocksRaycasts = true;
        painelFadePeixes.interactable = false;

        while (painelFadePeixes.alpha < 1f)
        {
            painelFadePeixes.alpha = Mathf.MoveTowards(
                painelFadePeixes.alpha,
                1f,
                Time.deltaTime / Mathf.Max(0.01f, duracaoFadeOutPeixes)
            );

            yield return null;
        }

        painelFadePeixes.alpha = 1f;

        if (painelPuzzlePeixes != null)
        {
            painelPuzzlePeixes.SetActive(true);
        }

        yield return new WaitForSeconds(0.2f);

        while (painelFadePeixes.alpha > 0f)
        {
            painelFadePeixes.alpha = Mathf.MoveTowards(
                painelFadePeixes.alpha,
                0f,
                Time.deltaTime / Mathf.Max(0.01f, duracaoFadeInPeixes)
            );

            yield return null;
        }

        painelFadePeixes.alpha = 0f;
        painelFadePeixes.blocksRaycasts = false;
        painelFadePeixes.gameObject.SetActive(false);

        DialogoGlobalAtivo = false;
        transicaoPuzzleIniciada = false;

        Debug.Log("Puzzle dos peixes ativado com Fade Out e Fade In!");
    }

    IEnumerator IniciarDialogoPastorAposFade()
    {
        while (FadePreto.TransicaoAtiva)
        {
            yield return null;
        }

        yield return null;

        Debug.Log("Iniciando diálogo automático de Alice com o pastor.");
        IniciarDialogoAutomatico();
    }

    IEnumerator IniciarDialogoDonaCida()
    {
        yield return null;

        Debug.Log("Iniciando diálogo automático com Dona Cida!");

        IniciarDialogoAutomatico();
    }

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
            if (nomeDoPersonagem == "Pastor" &&
                !pastorOpcional &&
                !dialogoAtivo)
            {
                return;
            }

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

    public void IniciarDialogoPosTransformacao()
    {
        if (dialogoAtivo ||
            dialogoPastorPosTransformacaoIniciado ||
            dialogoPastorPosTransformacaoConcluido)
            return;

        dialogoPastorPosTransformacaoIniciado = true;

        somenteAliceFalando = true;
        falas = new string[] { "..." };
        respostasAlice = new string[0];
        dialogoPosTransformacaoAtivo = true;

        Debug.Log("Iniciando diálogo de Alice após a transformação do pastor.");
        IniciarDialogoAutomatico();
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
                "O quarto da Bianca é o primeiro da esquerda para a direita, a porta tá aberta.",
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

        if (!mostrandoResposta && !somenteAliceFalando)
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
        if (mostrandoResposta || somenteAliceFalando)
        {
            nomeNPC.text = nomeProtagonista;

            textoDialogo.text = somenteAliceFalando
                ? falas[falaAtual]
                : respostasAlice[respostaAtual];

            bool falaPosTransformacao =
                dialogoPosTransformacaoAtivo && somenteAliceFalando;

            bool falaMereceMorrer =
                somenteAliceFalando &&
                falas[falaAtual].ToUpperInvariant().Contains("VOCÊ MERECE MORRER");

            if (falaPosTransformacao)
            {
                PararAnimacaoBoca();

                if (imagemPersonagem != null)
                {
                    imagemPersonagem.sprite =
                        alicePosTransformacaoSpriteFixo != null
                        ? alicePosTransformacaoSpriteFixo
                        : imagemDaAlice;
                }
            }
            else
            {
                int indiceSpriteAlice = somenteAliceFalando
                    ? falaAtual
                    : respostaAtual;

                Sprite fechada = imagemDaAlice;
                Sprite aberta = null;

                Sprite[] spritesFechadosAlice;
                Sprite[] spritesAbertosAlice;

                if (nomeDoPersonagem == "Pescador" && dialogoFinalPescador)
                {
                    spritesFechadosAlice =
                        aliceBocaFechadaPorRespostaDepoisPeixaria;

                    spritesAbertosAlice =
                        aliceBocaAbertaPorRespostaDepoisPeixaria;
                }
                else if (nomeDoPersonagem == "Dona Cida" && donaCidaDialogoPosPuzzle)
                {
                    spritesFechadosAlice =
                        aliceBocaFechadaPorRespostaDepoisPuzzle;

                    spritesAbertosAlice =
                        aliceBocaAbertaPorRespostaDepoisPuzzle;
                }
                else
                {
                    spritesFechadosAlice =
                        aliceBocaFechadaPorResposta;

                    spritesAbertosAlice =
                        aliceBocaAbertaPorResposta;
                }

                if (spritesFechadosAlice != null &&
                    indiceSpriteAlice >= 0 &&
                    indiceSpriteAlice < spritesFechadosAlice.Length &&
                    spritesFechadosAlice[indiceSpriteAlice] != null)
                {
                    fechada = spritesFechadosAlice[indiceSpriteAlice];
                }

                if (spritesAbertosAlice != null &&
                    indiceSpriteAlice >= 0 &&
                    indiceSpriteAlice < spritesAbertosAlice.Length &&
                    spritesAbertosAlice[indiceSpriteAlice] != null)
                {
                    aberta = spritesAbertosAlice[indiceSpriteAlice];
                }

                if (falaMereceMorrer)
                {
                    fechada = aliceMereceMorrerBocaFechada != null
                        ? aliceMereceMorrerBocaFechada
                        : fechada;

                    aberta = aliceMereceMorrerBocaAberta != null
                        ? aliceMereceMorrerBocaAberta
                        : aberta;
                }

                ConfigurarAnimacaoAlice(fechada, aberta);
            }
        }
        else
        {
            nomeNPC.text = nomeDoPersonagem;
            textoDialogo.text = falas[falaAtual];

            ConfigurarAnimacaoNPC(falaAtual);
        }

        IniciarDigitacao();

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

        bool aliceFalando = nomeNPC.text == nomeProtagonista;
        IniciarAudioVoz(aliceFalando);

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

        PararAudioVoz();
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

        PararAudioVoz();
    }

    // =========================================================
    // PENSAMENTOS AUTOMÁTICOS APÓS O PASTOR
    // =========================================================

    IEnumerator IniciarPensamentosPastor()
    {
        yield return null;

        if (gerenciadorPensamentos == null)
        {
            Debug.LogError(
                "GerenciadorPensamentos não foi conectado no DialogueManager do Pastor!"
            );

            yield break;
        }

        gerenciadorPensamentos.MostrarPensamento(
            pensamentoPastor1,
            MostrarSegundoPensamentoPastor,
            alicePastorPensamento1Fechada,
            alicePastorPensamento1Aberta
        );

        Debug.Log("Primeiro pensamento após o diálogo com o Pastor iniciado.");
    }

    void MostrarSegundoPensamentoPastor()
    {
        if (gerenciadorPensamentos == null)
            return;

        gerenciadorPensamentos.MostrarPensamento(
            pensamentoPastor2,
            null,
            alicePastorPensamento2Fechada,
            alicePastorPensamento2Aberta
        );

        Debug.Log("Segundo pensamento após o diálogo com o Pastor iniciado.");
    }

    void FinalizarDialogo()
    {
        PararAudioVoz();

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

        if (pastorOpcional)
        {
            jogadorPerto = false;

            if (indicadorInteracao != null)
                indicadorInteracao.SetActive(false);

            StartCoroutine(IniciarPensamentosPastor());

            return;
        }

        if (dialogoPosTransformacaoAtivo)
        {
            dialogoPosTransformacaoAtivo = false;
            dialogoPastorPosTransformacaoConcluido = true;
            aoFecharDialogoPosTransformacao?.Invoke();
            return;
        }

        aoFecharDialogo?.Invoke();

        if (nomeDoPersonagem == "Delegado" &&
            PlayerPrefs.GetInt("DelegadoConversou", 0) == 0)
        {
            PlayerPrefs.SetInt("DelegadoConversou", 1);
            PlayerPrefs.Save();

            if (itemParaRevelar != null)
                itemParaRevelar.SetActive(true);
        }

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

                StartCoroutine(AtivarPuzzleComFade());

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

    // =========================================================
    // LIBERAÇÃO DOS BLOQUEIOS AO DESATIVAR O OBJETO
    // =========================================================

    private void OnDisable()
    {
        PararAudioVoz();
        PararAnimacaoBoca();

        if (digitacaoTexto != null)
        {
            StopCoroutine(digitacaoTexto);
            digitacaoTexto = null;
        }

        textoDigitando = false;
        dialogoAtivo = false;

        // Libera o bloqueio global quando o objeto
        // é desativado ou a cena é descarregada.
        DialogoGlobalAtivo = false;
    }

#if UNITY_EDITOR
    // Reinicia somente o progresso do pescador ao iniciar uma nova sessão
    // de teste no Play Mode. Não afeta versões compiladas do jogo.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ReiniciarPuzzlePeixesAoIniciarPlayMode()
    {
        PlayerPrefs.DeleteKey("PescadorPrimeiraConversa");
        PlayerPrefs.DeleteKey("PescadorPuzzleConcluido");
        PlayerPrefs.DeleteKey("DialogoFinalPescadorConcluido");

        PlayerPrefs.DeleteKey("DialogoFinalPescadorIniciado");
        PlayerPrefs.DeleteKey("DialogoFinalPescadorFala");
        PlayerPrefs.DeleteKey("DialogoFinalPescadorResposta");
        PlayerPrefs.DeleteKey("DialogoFinalPescadorMostrandoResposta");

        PlayerPrefs.Save();
        Debug.Log("Puzzle dos peixes reiniciado para uma nova sessão de teste!");
    }
#endif

}


using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static bool DialogoGlobalAtivo = false;

    public GameObject painelDialogo;
    public TextMeshProUGUI textoDialogo;
    public TextMeshProUGUI nomeNPC;
    public ObjectiveManager objectiveManager;

    [Header("Configuração do NPC")]
    public string nomeDoPersonagem = "Delegado";
    public string nomeProtagonista = "Alice";

    [Header("Falas do NPC")]
    [TextArea(2, 4)]
    public string[] falas;

    [Header("Respostas da Alice")]
    [TextArea(2, 4)]
    public string[] respostasAlice;

    public int objetivoParaConcluir = 1;

    [Header("Item liberado após o diálogo")]
    public GameObject itemParaRevelar;

    private bool jogadorPerto = false;
    private bool dialogoAtivo = false;
    private bool mostrandoResposta = false;
    private bool falaEspecial = false;
    private int falaAtual = 0;

    void Start()
    {
        DialogoGlobalAtivo = false;

        painelDialogo.SetActive(false);

        if (itemParaRevelar != null)
        {
            bool conversou = PlayerPrefs.GetInt("DelegadoConversou", 0) == 1;
            bool fitaColetada = PlayerPrefs.GetInt("FitaVHS01Coletada", 0) == 1;

            itemParaRevelar.SetActive(conversou && !fitaColetada);
        }
    }

    void Update()
    {
        if (jogadorPerto && Input.GetKeyDown(KeyCode.E))
        {
            if (!dialogoAtivo)
                IniciarDialogo();
            else
                AvancarDialogo();
        }
    }

    void IniciarDialogo()
    {
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

        if (falas == null || falas.Length == 0)
            return;

        dialogoAtivo = true;
        DialogoGlobalAtivo = true;

        mostrandoResposta = false;
        falaEspecial = false;
        falaAtual = 0;

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
    }

    void AvancarDialogo()
    {
        if (falaEspecial)
        {
            FinalizarDialogo();
            return;
        }

        if (!mostrandoResposta &&
            respostasAlice != null &&
            falaAtual < respostasAlice.Length &&
            !string.IsNullOrWhiteSpace(respostasAlice[falaAtual]))
        {
            mostrandoResposta = true;
        }
        else
        {
            mostrandoResposta = false;
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
            textoDialogo.text = respostasAlice[falaAtual];
        }
        else
        {
            nomeNPC.text = nomeDoPersonagem;
            textoDialogo.text = falas[falaAtual];
        }
    }

    void FinalizarDialogo()
    {
        dialogoAtivo = false;
        DialogoGlobalAtivo = false;
        falaEspecial = false;

        painelDialogo.SetActive(false);

        // Ações específicas do delegado
        if (nomeDoPersonagem == "Delegado" &&
            PlayerPrefs.GetInt("DelegadoConversou", 0) == 0)
        {
            PlayerPrefs.SetInt("DelegadoConversou", 1);
            PlayerPrefs.Save();

            if (itemParaRevelar != null)
                itemParaRevelar.SetActive(true);
        }

        // Conclusão do objetivo configurado para cada NPC
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

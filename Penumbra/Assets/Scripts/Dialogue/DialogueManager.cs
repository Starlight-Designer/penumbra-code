
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject painelDialogo;
    public TextMeshProUGUI textoDialogo;
    public TextMeshProUGUI nomeNPC;
    public ObjectiveManager objectiveManager;

    private bool jogadorPerto = false;
    private bool dialogoAtivo = false;
    private int falaAtual = 0;

    private string[] falas =
    {
        "Delegado: Você finalmente chegou.",
        "Delegado: Seu tio está esperando por você.",
        "Delegado: Precisamos conversar sobre o desaparecimento das garotas."
    };

    void Start()
    {
        painelDialogo.SetActive(false);
    }

    void Update()
    {
        if (jogadorPerto && Input.GetKeyDown(KeyCode.E))
        {
            if (!dialogoAtivo)
            {
                IniciarDialogo();
            }
            else
            {
                AvancarDialogo();
            }
        }
    }

    void IniciarDialogo()
    {
        dialogoAtivo = true;
        falaAtual = 0;

        painelDialogo.SetActive(true);
        nomeNPC.text = "Delegado";

        MostrarFala();
    }

    void AvancarDialogo()
    {
        falaAtual++;

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
        textoDialogo.text = falas[falaAtual];
    }

    void FinalizarDialogo()
    {
        dialogoAtivo = false;
        painelDialogo.SetActive(false);

        // Só conclui o objetivo de falar com o delegado
        if (objectiveManager != null &&
            PlayerPrefs.GetInt("ObjetivoAtual", 0) == 1)
        {
            objectiveManager.ConcluirObjetivo();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = false;
        }
    }
}

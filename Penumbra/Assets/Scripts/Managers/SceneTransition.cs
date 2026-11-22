using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string nomeCena = "Casa";

    [Header("Onde o Player aparece na cena de destino")]
    public string spawnNoDestino;

    [Header("Pensamentos da Alice")]
    public GerenciadorPensamentos gerenciadorPensamentos;

    [Header("Objetivos")]
    [Tooltip("Opcional. Se conectado, será usado para concluir o objetivo antes da transição.")]
    public ObjectiveManager objectiveManager;

    [Header("Áudio da transição")]
    public bool usarAudio = false;
    public AudioClip audioTransicao;
    public AudioSource fonteAudio;

    [Range(0f, 5f)]
    public float atrasoAudio = 0f;
    public float duracaoFadeAudio = 1.5f;

    [Header("Transição de cena")]
    public FadePreto fade;

    private bool transicaoIniciada = false;
    private const string CHAVE_NOTIFICACAO_PENDENTE = "NotificacaoObjetivoPendente";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || transicaoIniciada)
            return;

        int objetivoAtual = PlayerPrefs.GetInt("ObjetivoAtual", 0);

        // BLOQUEIA A FLORESTA ATÉ O OBJETIVO 9
        if (nomeCena == "Floresta" && objetivoAtual < 9)
        {
            MostrarPensamento("Ainda preciso terminar o que tenho que fazer na casa da Dona Cida antes de explorar a floresta.");
            return;
        }

        if (nomeCena == "Peixaria")
        {
            if (objetivoAtual == 0)
            {
                MostrarPensamento("Eu tenho que ir na delegacia primeiro! Meu tio tá me esperando.");
                return;
            }
            if (objetivoAtual == 1)
            {
                MostrarPensamento("Eu preciso falar com o meu tio!");
                return;
            }
            if (objetivoAtual == 2)
            {
                MostrarPensamento("Preciso pegar a fita VHS com o meu tio antes de ir para a Peixaria.");
                return;
            }
            if (objetivoAtual < 3)
                return;
        }

        if (nomeCena == "Casa_Mae_Bianca")
        {
            if (objetivoAtual < 4)
            {
                MostrarPensamento("Preciso descobrir o que o pescador sabe antes de ir até a casa da Bianca.");
                return;
            }
            if (objetivoAtual == 4)
            {
                PlayerPrefs.SetInt("ObjetivoAtual", 5);
                PlayerPrefs.Save();
            }

            // O padre opcional desaparece definitivamente quando Alice entra na casa.
            PadreOpicional.RegistrarEntradaNaCasaDaBianca();
        }

        if (nomeCena == "Quarto_Bianca")
        {
            if (objetivoAtual < 6)
            {
                MostrarPensamento("Preciso conversar com Dona Cida antes de entrar no quarto da Bianca.");
                return;
            }
            if (objetivoAtual >= 7)
            {
                MostrarPensamento("Já terminei de investigar o quarto da Bianca.");
                return;
            }
        }

        if (nomeCena == "Delegacia" && objetivoAtual == 0)
        {
            PlayerPrefs.SetInt("ObjetivoAtual", 1);
            PlayerPrefs.Save();
        }

        // Conclui o objetivo 9 ao entrar em Caminho_Igreja.
        // A flag pendente será consumida na nova cena quando Alice andar.
        if (nomeCena == "Caminho_Igreja" && objetivoAtual == 9)
        {
            if (objectiveManager != null)
            {
                objectiveManager.ConcluirObjetivo();
            }
            else
            {
                // Fallback se o ObjectiveManager não estiver nesta cena
                // ou não tiver sido conectado no Inspector.
                PlayerPrefs.SetInt("ObjetivoAtual", 10);
                PlayerPrefs.SetInt(CHAVE_NOTIFICACAO_PENDENTE, 1);
                PlayerPrefs.Save();
            }

            Debug.Log("OBJETIVO 9 CONCLUÍDO. Notificação pendente para Caminho_Igreja.");
        }

        SpawnData.proximoSpawn = spawnNoDestino;
        transicaoIniciada = true;

        if (fade != null)
        {
            if (nomeCena == "Igreja_Porta")
            {
                fade.EscurecerECarregarComAudioPadrao(nomeCena);
            }
            else if (usarAudio && audioTransicao != null)
            {
                fade.EscurecerECarregarComAudio(
                    nomeCena, audioTransicao, fonteAudio, atrasoAudio, duracaoFadeAudio
                );
            }
            else
            {
                fade.EscurecerECarregar(nomeCena);
            }
        }
        else
        {
            Debug.LogWarning("FadePreto não foi conectado! Carregando cena sem transição.");
            SceneManager.LoadScene(nomeCena);
        }
    }

    void MostrarPensamento(string mensagem)
    {
        if (gerenciadorPensamentos != null)
            gerenciadorPensamentos.MostrarPensamento(mensagem);
        else
            Debug.LogWarning("Gerenciador de pensamentos não foi conectado ao SceneTransition!");
    }
}

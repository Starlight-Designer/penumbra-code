
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string nomeCena = "Casa";

    [Header("Onde o Player aparece na cena de destino")]
    public string spawnNoDestino;

    [Header("Pensamentos da Alice")]
    public GerenciadorPensamentos gerenciadorPensamentos;

    [Header("Áudio da porta")]
    public AudioSource audioPorta;
    public AudioClip somAbrirPorta;
    [Range(0f, 1f)]
    public float volumeSomPorta = 1f;

    [Header("Transição de cena")]
    public FadePreto fade;

    private bool transicaoIniciada = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (transicaoIniciada)
            return;

        int objetivoAtual = PlayerPrefs.GetInt("ObjetivoAtual", 0);

        // BLOQUEIA A FLORESTA ATÉ O OBJETIVO 9
        if (nomeCena == "Floresta")
        {
            if (objetivoAtual < 9)
            {
                MostrarPensamento("Ainda preciso terminar o que tenho que fazer na casa da Dona Cida antes de explorar a floresta.");
                return;
            }
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

            // O padre opcional desaparece definitivamente
            // quando Alice entra na casa da mãe da Bianca.
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

        if (nomeCena == "Delegacia")
        {
            if (objetivoAtual == 0)
            {
                PlayerPrefs.SetInt("ObjetivoAtual", 1);
                PlayerPrefs.Save();
            }
        }

        // CONCLUI O OBJETIVO DE EXPLORAR A FLORESTA
        // AO ENTRAR NA CENA CAMINHO_IGREJA
        if (nomeCena == "Caminho_Igreja" && objetivoAtual == 9)
        {
            PlayerPrefs.SetInt("ObjetivoAtual", 10);
            PlayerPrefs.Save();
        }

        // Guarda o spawn da próxima cena
        SpawnData.proximoSpawn = spawnNoDestino;

        // Impede que a transição seja iniciada mais de uma vez
        transicaoIniciada = true;

        // Inicia a sequência de áudio e transição
        StartCoroutine(TocarAudioETransicionar());
    }

    private IEnumerator TocarAudioETransicionar()
    {
        // Reproduz o som específico desta porta
        if (somAbrirPorta != null)
        {
            if (audioPorta == null)
                audioPorta = GetComponent<AudioSource>();

            if (audioPorta != null)
            {
                audioPorta.PlayOneShot(somAbrirPorta, volumeSomPorta);
            }
            else
            {
                AudioSource.PlayClipAtPoint(
                    somAbrirPorta,
                    transform.position,
                    volumeSomPorta
                );
            }

            // Aguarda o som terminar
            yield return new WaitForSecondsRealtime(somAbrirPorta.length);
        }

        // Inicia o fade e carrega a próxima cena
        if (fade != null)
        {
            fade.EscurecerECarregar(nomeCena);
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
        {
            gerenciadorPensamentos.MostrarPensamento(mensagem);
        }
        else
        {
            Debug.LogWarning("Gerenciador de pensamentos não foi conectado ao SceneTransition!");
        }
    }
}

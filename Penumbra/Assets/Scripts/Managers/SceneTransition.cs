
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string nomeCena = "Casa";

    [Header("Onde o Player aparece na cena de destino")]
    public string spawnNoDestino;

    [Header("Pensamentos da Alice")]
    public GerenciadorPensamentos gerenciadorPensamentos;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (nomeCena == "Peixaria")
        {
            int objetivoAtual = PlayerPrefs.GetInt("ObjetivoAtual", 0);

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
            int objetivoAtual = PlayerPrefs.GetInt("ObjetivoAtual", 0);

            if (objetivoAtual < 4)
            {
                MostrarPensamento("Preciso descobrir o que o pescador sabe antes de ir até a casa da Bianca.");
                return;
            }
        }

        if (nomeCena == "Delegacia")
        {
            if (PlayerPrefs.GetInt("ObjetivoAtual", 0) == 0)
            {
                PlayerPrefs.SetInt("ObjetivoAtual", 1);
                PlayerPrefs.Save();
            }
        }

        SpawnData.proximoSpawn = spawnNoDestino;
        SceneManager.LoadScene(nomeCena);
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

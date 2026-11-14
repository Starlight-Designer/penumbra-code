using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public string nomeCena = "Casa";

    [Header("Onde o Player aparece na cena de destino")]
    public string spawnNoDestino;

    [Header("Mensagem de bloqueio")]
    public TextMeshProUGUI textoInteracao;
    public float tempoNaTela = 3f;

    private bool mostrandoMensagem = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (nomeCena == "Peixaria")
        {
            int objetivoAtual = PlayerPrefs.GetInt("ObjetivoAtual", 0);

            if (objetivoAtual == 0)
            {
                MostrarMensagem("Eu tenho que ir na delegacia primeiro! Meu tio tá me esperando.");
                return;
            }

            if (objetivoAtual == 1)
            {
                MostrarMensagem("Eu preciso falar com o meu tio!");
                return;
            }

            if (objetivoAtual == 2)
            {
                MostrarMensagem("Preciso pegar a fita VHS com o meu tio antes de ir para a Peixaria.");
                return;
            }

            if (objetivoAtual < 3)
                return;
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

    void MostrarMensagem(string mensagem)
    {
        if (mostrandoMensagem || textoInteracao == null)
            return;

        StartCoroutine(ExibirMensagem(mensagem));
    }

    IEnumerator ExibirMensagem(string mensagem)
    {
        mostrandoMensagem = true;

        textoInteracao.text = mensagem;
        textoInteracao.gameObject.SetActive(true);

        yield return new WaitForSeconds(tempoNaTela);

        textoInteracao.gameObject.SetActive(false);
        mostrandoMensagem = false;
    }
}
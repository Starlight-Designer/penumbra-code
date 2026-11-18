
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FechaduraIgreja : MonoBehaviour
{
    [Header("Painel da fechadura")]
    public GameObject painelFechadura;
    public TextMeshProUGUI areaNumeros;

    [Header("Jogador")]
    public PlayerMovement playerMovement;

    [Header("Configurações")]
    public string senhaCorreta = "62636873";
    public string nomeCenaIgreja = "Igreja";

    private string senhaDigitada = "";
    private bool mostrandoMensagem = false;

    void Start()
    {
        painelFechadura.SetActive(false);
        AtualizarTexto();
    }

    // Abre o painel quando o jogador entra na área
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        painelFechadura.SetActive(true);

        // Esconde e bloqueia o jogador
        if (playerMovement != null)
        {
            playerMovement.BloquearMovimento(true);
        }

        senhaDigitada = "";
        AtualizarTexto();

        Debug.Log("Painel da fechadura aberto!");
    }

    // Chamado pelos botões numéricos
    public void DigitarNumero(string numero)
    {
        Debug.Log("Botão clicado: " + numero);

        if (mostrandoMensagem || senhaDigitada.Length >= 8)
            return;

        senhaDigitada += numero;
        AtualizarTexto();

        Debug.Log("Senha digitada: " + senhaDigitada);

        if (senhaDigitada.Length == 8)
        {
            VerificarSenha();
        }
    }

    // Verifica se os oito números estão corretos
    void VerificarSenha()
    {
        if (senhaDigitada == senhaCorreta)
        {
            Debug.Log("Senha correta! Abrindo a igreja.");

            PlayerPrefs.SetInt("SenhaIgrejaCorreta", 1);

            // Conclui o último objetivo
            int objetivoAtual = PlayerPrefs.GetInt("ObjetivoAtual", 0);

            if (objetivoAtual == 10)
            {
                PlayerPrefs.SetInt("ObjetivoAtual", 11);
                Debug.Log("Último objetivo concluído!");
            }

            PlayerPrefs.Save();

            SceneManager.LoadScene(nomeCenaIgreja);
        }
        else
        {
            Debug.Log("Senha incorreta!");
            StartCoroutine(MostrarSenhaIncorreta());
        }
    }

    // Mostra a mensagem e limpa o campo depois
    IEnumerator MostrarSenhaIncorreta()
    {
        mostrandoMensagem = true;
        areaNumeros.text = "Senha Incorreta";

        yield return new WaitForSeconds(1.5f);

        senhaDigitada = "";
        mostrandoMensagem = false;
        AtualizarTexto();
    }

    // Apaga somente o último número
    public void ApagarUltimoNumero()
    {
        if (mostrandoMensagem || senhaDigitada.Length == 0)
            return;

        senhaDigitada = senhaDigitada.Substring(
            0, senhaDigitada.Length - 1
        );

        AtualizarTexto();
    }

    // Fecha o painel
    public void FecharPainel()
    {
        senhaDigitada = "";
        AtualizarTexto();

        painelFechadura.SetActive(false);

        // Faz o jogador reaparecer e voltar a andar
        if (playerMovement != null)
        {
            playerMovement.BloquearMovimento(false);
        }

        Debug.Log("Painel fechado. Jogador liberado!");
    }

    void AtualizarTexto()
    {
        areaNumeros.text = senhaDigitada;
    }
}

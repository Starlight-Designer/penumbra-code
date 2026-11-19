
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
    private bool jogadorNaFechadura = false;

    void Start()
    {
        painelFechadura.SetActive(false);
        AtualizarTexto();
    }

    // Abre o painel quando o jogador entra na área.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        jogadorNaFechadura = true;

        // Bloqueia o inventário durante a interação.
        InventoryManager.BloquearInventario(true);

        painelFechadura.SetActive(true);

        // Esconde e bloqueia o jogador.
        if (playerMovement != null)
        {
            playerMovement.BloquearMovimento(true);
        }

        senhaDigitada = "";
        AtualizarTexto();

        Debug.Log("Painel da fechadura aberto!");
    }

    // Chamado pelos botões numéricos.
    public void DigitarNumero(string numero)
    {
        Debug.Log("Botão clicado: " + numero);

        if (!jogadorNaFechadura ||
            mostrandoMensagem ||
            senhaDigitada.Length >= 8)
            return;

        senhaDigitada += numero;
        AtualizarTexto();

        Debug.Log("Senha digitada: " + senhaDigitada);

        if (senhaDigitada.Length == 8)
        {
            VerificarSenha();
        }
    }

    // Verifica se os oito números estão corretos.
    void VerificarSenha()
    {
        if (senhaDigitada == senhaCorreta)
        {
            Debug.Log("Senha correta! Abrindo a igreja.");

            PlayerPrefs.SetInt("SenhaIgrejaCorreta", 1);

            int objetivoAtual = PlayerPrefs.GetInt("ObjetivoAtual", 0);

            if (objetivoAtual == 10)
            {
                PlayerPrefs.SetInt("ObjetivoAtual", 11);
                Debug.Log("Último objetivo concluído!");
            }

            PlayerPrefs.Save();

            // Mantém o inventário bloqueado durante a transição.
            SceneManager.LoadScene(nomeCenaIgreja);
        }
        else
        {
            Debug.Log("Senha incorreta!");
            StartCoroutine(MostrarSenhaIncorreta());
        }
    }

    // Mostra a mensagem e limpa o campo depois.
    IEnumerator MostrarSenhaIncorreta()
    {
        mostrandoMensagem = true;
        areaNumeros.text = "Senha Incorreta";

        yield return new WaitForSeconds(1.5f);

        senhaDigitada = "";
        mostrandoMensagem = false;
        AtualizarTexto();
    }

    // Apaga somente o último número.
    public void ApagarUltimoNumero()
    {
        if (!jogadorNaFechadura ||
            mostrandoMensagem ||
            senhaDigitada.Length == 0)
            return;

        senhaDigitada = senhaDigitada.Substring(
            0, senhaDigitada.Length - 1
        );

        AtualizarTexto();
    }

    // Fecha o painel.
    public void FecharPainel()
    {
        senhaDigitada = "";
        AtualizarTexto();

        painelFechadura.SetActive(false);

        jogadorNaFechadura = false;
        mostrandoMensagem = false;

        // Desbloqueia o inventário.
        InventoryManager.BloquearInventario(false);

        // Faz o jogador voltar a andar.
        if (playerMovement != null)
        {
            playerMovement.BloquearMovimento(false);
        }

        Debug.Log("Painel fechado. Jogador e inventário liberados!");
    }

    // Desbloqueia o inventário quando Alice sai da área do cadeado.
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        jogadorNaFechadura = false;

        if (painelFechadura != null)
            painelFechadura.SetActive(false);

        senhaDigitada = "";
        mostrandoMensagem = false;
        AtualizarTexto();

        InventoryManager.BloquearInventario(false);

        if (playerMovement != null)
        {
            playerMovement.BloquearMovimento(false);
        }

        Debug.Log("Alice saiu da área do cadeado. Inventário desbloqueado!");
    }

    void AtualizarTexto()
    {
        if (areaNumeros != null)
            areaNumeros.text = senhaDigitada;
    }
}

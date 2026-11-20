
using System.Collections;
using UnityEngine;
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

    [Header("Transição")]
    public FadePreto fadePreto;

    private string senhaDigitada = "";
    private bool mostrandoMensagem = false;
    private bool jogadorNaFechadura = false;
    private bool transicaoIniciada = false;

    void Start()
    {
        painelFechadura.SetActive(false);
        AtualizarTexto();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        jogadorNaFechadura = true;

        InventoryManager.BloquearInventario(true);

        painelFechadura.SetActive(true);

        if (playerMovement != null)
            playerMovement.BloquearMovimento(true);

        senhaDigitada = "";
        AtualizarTexto();

        Debug.Log("Painel da fechadura aberto!");
    }

    public void DigitarNumero(string numero)
    {
        Debug.Log("Botão clicado: " + numero);

        if (!jogadorNaFechadura ||
            mostrandoMensagem ||
            transicaoIniciada ||
            senhaDigitada.Length >= 8)
            return;

        senhaDigitada += numero;
        AtualizarTexto();

        Debug.Log("Senha digitada: " + senhaDigitada);

        if (senhaDigitada.Length == 8)
            VerificarSenha();
    }

    void VerificarSenha()
    {
        if (senhaDigitada == senhaCorreta)
        {
            Debug.Log("Senha correta! Abrindo a igreja.");

            transicaoIniciada = true;

            PlayerPrefs.SetInt("SenhaIgrejaCorreta", 1);

            int objetivoAtual = PlayerPrefs.GetInt("ObjetivoAtual", 0);

            if (objetivoAtual == 10)
            {
                PlayerPrefs.SetInt("ObjetivoAtual", 11);
                Debug.Log("Último objetivo concluído!");
            }

            PlayerPrefs.Save();

            if (painelFechadura != null)
                painelFechadura.SetActive(false);

            if (fadePreto != null)
            {
                fadePreto.EscurecerECarregar(nomeCenaIgreja);
            }
            else
            {
                Debug.LogWarning("FadePreto não foi conectado! Carregando cena sem transição.");
                UnityEngine.SceneManagement.SceneManager.LoadScene(nomeCenaIgreja);
            }
        }
        else
        {
            Debug.Log("Senha incorreta!");
            StartCoroutine(MostrarSenhaIncorreta());
        }
    }

    IEnumerator MostrarSenhaIncorreta()
    {
        mostrandoMensagem = true;
        areaNumeros.text = "Senha Incorreta";

        yield return new WaitForSeconds(1.5f);

        senhaDigitada = "";
        mostrandoMensagem = false;
        AtualizarTexto();
    }

    public void ApagarUltimoNumero()
    {
        if (!jogadorNaFechadura ||
            mostrandoMensagem ||
            transicaoIniciada ||
            senhaDigitada.Length == 0)
            return;

        senhaDigitada = senhaDigitada.Substring(
            0, senhaDigitada.Length - 1
        );

        AtualizarTexto();
    }

    public void FecharPainel()
    {
        if (transicaoIniciada)
            return;

        senhaDigitada = "";
        AtualizarTexto();

        painelFechadura.SetActive(false);

        jogadorNaFechadura = false;
        mostrandoMensagem = false;

        InventoryManager.BloquearInventario(false);

        if (playerMovement != null)
            playerMovement.BloquearMovimento(false);

        Debug.Log("Painel fechado. Jogador e inventário liberados!");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || transicaoIniciada)
            return;

        jogadorNaFechadura = false;

        if (painelFechadura != null)
            painelFechadura.SetActive(false);

        senhaDigitada = "";
        mostrandoMensagem = false;
        AtualizarTexto();

        InventoryManager.BloquearInventario(false);

        if (playerMovement != null)
            playerMovement.BloquearMovimento(false);

        Debug.Log("Alice saiu da área do cadeado. Inventário desbloqueado!");
    }

    void AtualizarTexto()
    {
        if (areaNumeros != null)
            areaNumeros.text = senhaDigitada;
    }
}

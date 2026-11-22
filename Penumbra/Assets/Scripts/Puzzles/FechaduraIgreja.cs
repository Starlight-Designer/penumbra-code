
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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

    [Header("Áudio da porta abrindo")]
    public AudioClip audioPortaAbrindo;
    public float duracaoFadeAudioPorta = 1.5f;

    private string senhaDigitada = "";
    private bool mostrandoMensagem = false;
    private bool jogadorNaFechadura = false;
    private bool transicaoIniciada = false;

    void Start()
    {
        if (painelFechadura != null)
            painelFechadura.SetActive(false);

        AtualizarTexto();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || transicaoIniciada)
            return;

        if (jogadorNaFechadura)
            return;

        jogadorNaFechadura = true;

        InventoryManager.BloquearInventario(true);

        if (playerMovement != null)
            playerMovement.BloquearMovimento(true);

        senhaDigitada = "";
        AtualizarTexto();

        Debug.Log("Alice entrou na área da fechadura.");

        if (fadePreto != null)
        {
            StartCoroutine(AbrirPainelComFade());
        }
        else
        {
            MostrarPainel();
            Debug.LogWarning("FadePreto não conectado. Painel aberto sem transição.");
        }
    }

    IEnumerator AbrirPainelComFade()
    {
        // A tela fica preta, o cadeado aparece normalmente
        // e somente a tela preta faz o Fade In.
        yield return StartCoroutine(
            fadePreto.TransicaoTelaPreta(() =>
            {
                MostrarPainel();
            })
        );

        Debug.Log("Painel aberto sem transparência ou animação própria.");
    }

    void MostrarPainel()
    {
        if (painelFechadura == null)
            return;

        painelFechadura.SetActive(true);
        AtualizarTexto();
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

            StartCoroutine(AbrirIgreja());
        }
        else
        {
            Debug.Log("Senha incorreta!");
            StartCoroutine(MostrarSenhaIncorreta());
        }
    }

    IEnumerator AbrirIgreja()
    {
        // O painel desaparece instantaneamente.
        // Somente a tela preta faz a transição.
        if (painelFechadura != null)
            painelFechadura.SetActive(false);

        if (fadePreto != null)
        {
            if (audioPortaAbrindo != null)
            {
                fadePreto.EscurecerECarregarComAudio(
                    nomeCenaIgreja,
                    audioPortaAbrindo,
                    fadePreto.audioSource,
                    0f,
                    duracaoFadeAudioPorta
                );
            }
            else
            {
                Debug.LogWarning(
                    "Áudio da porta abrindo não foi atribuído."
                );

                fadePreto.EscurecerECarregar(nomeCenaIgreja);
            }
        }
        else
        {
            SceneManager.LoadScene(nomeCenaIgreja);
        }

        yield break;
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

        FecharPainelImediatamente();
    }

    void FecharPainelImediatamente()
    {
        if (painelFechadura != null)
            painelFechadura.SetActive(false);

        senhaDigitada = "";
        mostrandoMensagem = false;
        jogadorNaFechadura = false;

        AtualizarTexto();

        InventoryManager.BloquearInventario(false);

        if (playerMovement != null)
            playerMovement.BloquearMovimento(false);

        Debug.Log("Painel fechado. Jogador e inventário liberados!");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || transicaoIniciada)
            return;

        StopAllCoroutines();

        FecharPainelImediatamente();

        Debug.Log("Alice saiu da área do cadeado. Inventário desbloqueado!");
    }

    void AtualizarTexto()
    {
        if (areaNumeros != null)
            areaNumeros.text = senhaDigitada;
    }
}


using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortaAlmoxarifado : MonoBehaviour
{
    [Header("Configurações")]
    public string nomeCena = "Almoxarifado";

    [Header("Transição")]
    public FadePreto fadePreto;

    [Header("Bloqueio durante Fade In")]
    public Collider2D colliderPorta;

    private bool transicaoIniciada = false;
    private bool portaLiberada = false;

    private IEnumerator Start()
    {
        if (colliderPorta == null)
            colliderPorta = GetComponent<Collider2D>();

        // Aguarda o Fade In da cena terminar.
        while (FadePreto.TransicaoAtiva)
        {
            if (colliderPorta != null)
                colliderPorta.enabled = false;

            yield return null;
        }

        portaLiberada = true;

        if (colliderPorta != null)
            colliderPorta.enabled = true;

        Debug.Log("Fade In finalizado. Porta do almoxarifado liberada.");
    }

    private void OnMouseDown()
    {
        if (!portaLiberada)
            return;

        if (FadePreto.TransicaoAtiva)
            return;

        if (transicaoIniciada)
            return;

        int objetivoAtual = PlayerPrefs.GetInt("ObjetivoAtual", 0);

        // Só permite entrar quando o objetivo do almoxarifado estiver ativo.
        if (objetivoAtual != 11)
        {
            Debug.Log("Ainda não é hora de investigar o almoxarifado.");
            return;
        }

        Debug.Log("Entrando no almoxarifado!");

        transicaoIniciada = true;

        if (fadePreto != null)
        {
            // Transição com o áudio padrão da porta.
            fadePreto.EscurecerECarregarComAudioPadrao(nomeCena);
        }
        else
        {
            Debug.LogWarning("FadePreto não foi conectado! Carregando cena sem transição.");
            SceneManager.LoadScene(nomeCena);
        }
    }
}

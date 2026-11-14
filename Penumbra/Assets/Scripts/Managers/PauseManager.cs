
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject painelPausa;
    public GameObject painelObjetivos;

    private bool jogoPausado = false;
    private bool fechando = false;

    private AnimacaoQueda animacaoPausa;
    private AnimacaoZoom animacaoObjetivos;

    void Awake()
    {
        if (painelPausa != null)
            animacaoPausa = painelPausa.GetComponent<AnimacaoQueda>();

        if (painelObjetivos != null)
            animacaoObjetivos = painelObjetivos.GetComponent<AnimacaoZoom>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!jogoPausado)
            {
                PausarJogo();
            }
            else
            {
                RetomarJogo();
            }
        }
    }

    void PausarJogo()
    {
        jogoPausado = true;
        fechando = false;

        painelPausa.SetActive(true);
        painelObjetivos.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RetomarJogo()
    {
        if (!jogoPausado || fechando)
            return;

        StartCoroutine(RetomarComAnimacoes());
    }

    private IEnumerator RetomarComAnimacoes()
    {
        fechando = true;

        if (animacaoPausa != null)
            animacaoPausa.Fechar();
        else
            painelPausa.SetActive(false);

        if (animacaoObjetivos != null)
            animacaoObjetivos.Fechar();
        else
            painelObjetivos.SetActive(false);

        yield return new WaitUntil(() =>
            !painelPausa.activeSelf &&
            !painelObjetivos.activeSelf
        );

        FinalizarRetomada();
    }

    private void FinalizarRetomada()
    {
        jogoPausado = false;
        fechando = false;

        painelPausa.SetActive(false);
        painelObjetivos.SetActive(false);

        Time.timeScale = 1f;
    }

    public void VoltarAoMenu()
    {
        Debug.Log("BOTÃO VOLTAR AO MENU FOI CLICADO!");

        GameObject jogador = GameObject.FindGameObjectWithTag("Player");

        if (jogador != null)
        {
            PlayerPrefs.SetString("CenaSalva", SceneManager.GetActiveScene().name);
            PlayerPrefs.SetFloat("PosicaoX", jogador.transform.position.x);
            PlayerPrefs.SetFloat("PosicaoY", jogador.transform.position.y);
            PlayerPrefs.SetInt("TemPartidaSalva", 1);
            PlayerPrefs.SetInt("CarregarPosicaoSalva", 1);
            PlayerPrefs.Save();
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu_Inicial");
    }
}

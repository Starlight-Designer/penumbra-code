
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject painelPausa;

    private bool jogoPausado = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PausarJogo();
        }
        else if (jogoPausado && Input.anyKeyDown && !Input.GetMouseButtonDown(0))
        {
            RetomarJogo();
        }
    }

    void PausarJogo()
    {
        jogoPausado = true;
        painelPausa.SetActive(true);
        Time.timeScale = 0f;
    }

    void RetomarJogo()
    {
        jogoPausado = false;
        painelPausa.SetActive(false);
        Time.timeScale = 1f;
    }

    public void VoltarAoMenu()
    {
        Debug.Log("Botão voltar ao menu clicado!");

        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu_Inicial");
    }
}

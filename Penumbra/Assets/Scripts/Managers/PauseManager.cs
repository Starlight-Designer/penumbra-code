
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
        GameObject jogador = GameObject.FindGameObjectWithTag("Player");

        if (jogador != null)
        {
            PlayerPrefs.SetString("CenaSalva", SceneManager.GetActiveScene().name);
            PlayerPrefs.SetFloat("PosicaoX", jogador.transform.position.x);
            PlayerPrefs.SetFloat("PosicaoY", jogador.transform.position.y);
            PlayerPrefs.SetInt("TemPartidaSalva", 1);
            PlayerPrefs.Save();
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu_Inicial");
    }
}

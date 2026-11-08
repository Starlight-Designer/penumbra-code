using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject painelPausa;

    private bool jogoPausado = false;

    void Start()
    {
        painelPausa.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (jogoPausado)
        {
            if (Input.anyKeyDown)
                RetomarJogo();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            PausarJogo();
        }
    }

    void PausarJogo()
    {
        jogoPausado = true;
        painelPausa.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RetomarJogo()
    {
        jogoPausado = false;
        painelPausa.SetActive(false);
        Time.timeScale = 1f;
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnPosition : MonoBehaviour
{
    public Vector2 posicaoInicial;

    void Start()
    {
        string cenaAtual = SceneManager.GetActiveScene().name;
        string cenaSalva = PlayerPrefs.GetString("CenaSalva", "");

        bool carregarPosicaoSalva =
            PlayerPrefs.GetInt("CarregarPosicaoSalva", 0) == 1;

        if (carregarPosicaoSalva && cenaAtual == cenaSalva)
        {
            transform.position = new Vector3(
                PlayerPrefs.GetFloat("PosicaoX"),
                PlayerPrefs.GetFloat("PosicaoY"),
                transform.position.z
            );
        }
        else
        {
            transform.position = new Vector3(
                posicaoInicial.x,
                posicaoInicial.y,
                transform.position.z
            );
        }

        PlayerPrefs.SetInt("CarregarPosicaoSalva", 0);
        PlayerPrefs.Save();
    }
}

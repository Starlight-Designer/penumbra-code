
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnPosition : MonoBehaviour
{
    public Vector2 posicaoInicial;

    void Start()
    {
        string cenaAtual = SceneManager.GetActiveScene().name;
        string cenaSalva = PlayerPrefs.GetString("CenaSalva", "");

        if (PlayerPrefs.GetInt("TemPartidaSalva", 0) == 1 &&
            cenaAtual == cenaSalva)
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
    }
}


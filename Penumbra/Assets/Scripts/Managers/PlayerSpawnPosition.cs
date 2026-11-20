
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnPosition : MonoBehaviour
{
    public Vector2 posicaoInicial;

    void Start()
    {
        string cenaAtual = SceneManager.GetActiveScene().name;

        // Na floresta, a posição inicial é controlada
        // pelo InicioFugaFloresta.
        if (cenaAtual == "Floresta_Fuga")
        {
            SpawnData.proximoSpawn = null;

            Physics2D.SyncTransforms();

            return;
        }

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
            Vector2 destino = posicaoInicial;

            if (!string.IsNullOrEmpty(SpawnData.proximoSpawn))
            {
                SpawnPoint[] pontos = FindObjectsByType<SpawnPoint>(
                    FindObjectsInactive.Exclude
                );

                foreach (SpawnPoint sp in pontos)
                {
                    if (sp.id == SpawnData.proximoSpawn)
                    {
                        destino = sp.transform.position;
                        break;
                    }
                }
            }

            transform.position = new Vector3(
                destino.x,
                destino.y,
                transform.position.z
            );
        }

        SpawnData.proximoSpawn = null;
        Physics2D.SyncTransforms();

        PlayerPrefs.SetInt("CarregarPosicaoSalva", 0);
        PlayerPrefs.Save();
    }
}

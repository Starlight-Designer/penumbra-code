
using System.Collections;
using UnityEngine;

public class GerenciadorObstaculos : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject obstaculoArbusto;
    public GameObject obstaculoTronco;

    [Header("Posição de aparecimento")]
    public float posicaoXInicial = 12f;

    [Header("Altura de cada obstáculo")]
    public float[] posicoesY = new float[10]
    {
        0.08f,
        -0.03f,
        0.05f,
        0.10f,
        -0.01f,
        0.07f,
        0.02f,
        0.11f,
        -0.04f,
        0.04f
    };

    [Header("Intervalo entre obstáculos")]
    public float intervalo = 3f;

    [Header("Quantidade")]
    public int quantidadeObstaculos = 10;

    private bool obstaculosIniciados = false;

    public void IniciarObstaculos()
    {
        if (obstaculosIniciados)
            return;

        obstaculosIniciados = true;
        StartCoroutine(CriarObstaculos());
    }

    private IEnumerator CriarObstaculos()
    {
        for (int i = 0; i < quantidadeObstaculos; i++)
        {
            GameObject prefabEscolhido;

            if (i % 2 == 0)
                prefabEscolhido = obstaculoArbusto;
            else
                prefabEscolhido = obstaculoTronco;

            Debug.Log(
                "Obstáculo " + (i + 1) +
                " | Arbusto: " + (obstaculoArbusto != null) +
                " | Tronco: " + (obstaculoTronco != null) +
                " | Escolhido: " + (prefabEscolhido != null)
            );

            if (prefabEscolhido != null)
            {
                float altura = posicoesY.Length > i
                    ? posicoesY[i]
                    : -0.04f;

                Vector3 posicao = new Vector3(
                    posicaoXInicial,
                    altura,
                    0f
                );

                Instantiate(
                    prefabEscolhido,
                    posicao,
                    Quaternion.identity
                );

                Debug.Log("Obstáculo " + (i + 1) + " criado!");
            }
            else
            {
                Debug.LogWarning("Prefab de obstáculo não conectado!");
            }

            yield return new WaitForSeconds(intervalo);
        }

        Debug.Log("Os 10 obstáculos foram criados!");
    }
}

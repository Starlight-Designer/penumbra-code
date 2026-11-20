
using System.Collections;
using System.Collections.Generic;
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

    [Header("Final da fuga")]
    public FinalFugaFloresta finalFuga;
    public float esperaAposUltimoObstaculo = 3f;

    private bool obstaculosIniciados = false;
    private List<GameObject> obstaculosCriados = new List<GameObject>();

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

                GameObject novoObstaculo = Instantiate(
                    prefabEscolhido,
                    posicao,
                    Quaternion.identity
                );

                obstaculosCriados.Add(novoObstaculo);

                Debug.Log("Obstáculo " + (i + 1) + " criado!");
            }
            else
            {
                Debug.LogWarning("Prefab de obstáculo não conectado!");
            }

            if (i < quantidadeObstaculos - 1)
                yield return new WaitForSeconds(intervalo);
        }

        Debug.Log("Todos os obstáculos foram criados!");

        // Espera todos os obstáculos desaparecerem.
        while (true)
        {
            obstaculosCriados.RemoveAll(
                obstaculo => obstaculo == null
            );

            if (obstaculosCriados.Count == 0)
                break;

            yield return null;
        }

        Debug.Log("Todos os obstáculos desapareceram!");

        // Espera 3 segundos antes do encerramento.
        yield return new WaitForSeconds(esperaAposUltimoObstaculo);

        if (finalFuga != null)
        {
            finalFuga.IniciarFinal();
        }
        else
        {
            Debug.LogError(
                "FinalFugaFloresta não foi conectado no Inspector!"
            );
        }
    }
}


using System.Collections;
using UnityEngine;

public class MovimentoIgrejaFuga : MonoBehaviour
{
    [Header("Alice")]
    public Transform alice;
    public float alturaFinalAlice = -3f;
    public float velocidadeAlice = 2f;

    [Header("Pastor")]
    public Transform pastor;
    public float alturaFinalPastor = 3f;
    public float velocidadePastor = 2f;

    [Header("Transição para a floresta")]
    public FadePreto fadePreto;
    public float tempoAntesDaTransicao = 3f;
    public string nomeCenaDestino = "Floresta_Fuga";

    private bool movimentoConcluido = false;
    private bool transicaoIniciada = false;

    private void Update()
    {
        if (movimentoConcluido)
            return;

        bool aliceParou = true;
        bool pastorParou = true;

        // Alice desce até a altura escolhida.
        if (alice != null)
        {
            Vector3 destinoAlice = new Vector3(
                alice.position.x,
                alturaFinalAlice,
                alice.position.z
            );

            alice.position = Vector3.MoveTowards(
                alice.position,
                destinoAlice,
                velocidadeAlice * Time.deltaTime
            );

            aliceParou = Mathf.Approximately(
                alice.position.y,
                alturaFinalAlice
            );
        }

        // Pastor sobe até a altura escolhida.
        if (pastor != null)
        {
            Vector3 destinoPastor = new Vector3(
                pastor.position.x,
                alturaFinalPastor,
                pastor.position.z
            );

            pastor.position = Vector3.MoveTowards(
                pastor.position,
                destinoPastor,
                velocidadePastor * Time.deltaTime
            );

            pastorParou = Mathf.Approximately(
                pastor.position.y,
                alturaFinalPastor
            );
        }

        // Quando o pastor terminar de subir, inicia a contagem.
        if (pastorParou && !transicaoIniciada)
        {
            transicaoIniciada = true;
            StartCoroutine(TransicaoParaFloresta());
        }

        if (aliceParou && pastorParou)
        {
            movimentoConcluido = true;
            Debug.Log("Movimento de Alice e do pastor concluído!");
        }
    }

    private IEnumerator TransicaoParaFloresta()
    {
        Debug.Log("Pastor chegou ao destino. Aguardando 3 segundos...");

        yield return new WaitForSeconds(tempoAntesDaTransicao);

        if (fadePreto != null)
        {
            fadePreto.EscurecerECarregar(nomeCenaDestino);
        }
        else
        {
            Debug.LogError(
                "FadePreto não foi conectado no MovimentoIgrejaFuga!"
            );
        }
    }
}

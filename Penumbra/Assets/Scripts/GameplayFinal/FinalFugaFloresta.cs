
using System.Collections;
using UnityEngine;

public class FinalFugaFloresta : MonoBehaviour
{
    [Header("Alice")]
    public Transform alice;
    public Rigidbody2D rbAlice;
    public PlayerMovement movimentoAlice;
    public Animator animatorAlice;
    public float velocidadeAlice = 3f;
    public float posicaoFinalAliceX = 0f;

    [Header("Pastor")]
    public Transform pastor;
    public Rigidbody2D rbPastor;
    public Animator animatorPastor;
    public PastorPerseguicao perseguicaoPastor;
    public float velocidadePastor = 5f;

    [Header("Câmera")]
    public Camera cameraJogo;
    public float margemSaidaPastor = 0.1f;

    [Header("Transição")]
    public FadePreto fadePreto;
    public string nomeCenaCutscene = "NomeDaCutscene";

    private bool finalIniciado = false;

    public void IniciarFinal()
    {
        if (finalIniciado)
            return;

        finalIniciado = true;
        StartCoroutine(SequenciaFinal());
    }

    private IEnumerator SequenciaFinal()
    {
        // Espera qualquer transição anterior terminar.
        while (FadePreto.TransicaoAtiva)
            yield return null;

        // Bloqueia o controle da Alice.
        if (movimentoAlice != null)
            movimentoAlice.enabled = false;

        if (rbAlice != null)
            rbAlice.linearVelocity = Vector2.zero;

        // Impede que o sistema de perseguição interfira.
        if (perseguicaoPastor != null)
            perseguicaoPastor.enabled = false;

        if (rbPastor != null)
            rbPastor.linearVelocity = Vector2.zero;

        // Ativa a animação de corrida dos dois.
        if (animatorAlice != null)
            animatorAlice.SetBool("Andando", true);

        if (animatorPastor != null)
            animatorPastor.SetBool("Andando", true);

        bool aliceChegou = false;
        bool pastorSaiu = false;

        // Alice volta para o centro enquanto o Pastor corre para a esquerda.
        while (!aliceChegou || !pastorSaiu)
        {
            yield return new WaitForFixedUpdate();

            if (!aliceChegou && rbAlice != null)
            {
                Vector2 destinoAlice = new Vector2(
                    posicaoFinalAliceX,
                    rbAlice.position.y
                );

                Vector2 novaPosicaoAlice = Vector2.MoveTowards(
                    rbAlice.position,
                    destinoAlice,
                    velocidadeAlice * Time.fixedDeltaTime
                );

                rbAlice.MovePosition(novaPosicaoAlice);

                if (Mathf.Abs(
                    novaPosicaoAlice.x - posicaoFinalAliceX
                ) <= 0.02f)
                {
                    rbAlice.position = destinoAlice;
                    rbAlice.linearVelocity = Vector2.zero;
                    aliceChegou = true;
                }
            }
            else if (!aliceChegou)
            {
                aliceChegou = true;
            }

            if (!pastorSaiu && rbPastor != null)
            {
                Vector2 novaPosicaoPastor = rbPastor.position +
                    Vector2.left * velocidadePastor * Time.fixedDeltaTime;

                rbPastor.MovePosition(novaPosicaoPastor);

                if (cameraJogo != null)
                {
                    Vector3 posicaoNaTela =
                        cameraJogo.WorldToViewportPoint(
                            new Vector3(
                                novaPosicaoPastor.x,
                                novaPosicaoPastor.y,
                                pastor.position.z
                            )
                        );

                    if (posicaoNaTela.x < -margemSaidaPastor)
                    {
                        pastorSaiu = true;
                        rbPastor.linearVelocity = Vector2.zero;
                    }
                }
                else if (novaPosicaoPastor.x < -15f)
                {
                    pastorSaiu = true;
                    rbPastor.linearVelocity = Vector2.zero;
                }
            }
            else if (!pastorSaiu)
            {
                pastorSaiu = true;
            }
        }

        // Para as animações quando os movimentos terminarem.
        if (animatorAlice != null)
            animatorAlice.SetBool("Andando", false);

        if (animatorPastor != null)
            animatorPastor.SetBool("Andando", false);

        if (rbAlice != null)
            rbAlice.linearVelocity = Vector2.zero;

        if (rbPastor != null)
            rbPastor.linearVelocity = Vector2.zero;

        Debug.Log("Fuga concluída! Iniciando a cutscene.");

        // Escurece a tela e carrega a cutscene.
        if (fadePreto != null)
        {
            fadePreto.EscurecerECarregar(nomeCenaCutscene);
        }
        else
        {
            Debug.LogError(
                "FadePreto não foi conectado no Inspector!"
            );
        }
    }
}

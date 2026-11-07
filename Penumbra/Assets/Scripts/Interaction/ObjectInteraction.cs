using UnityEngine;
using TMPro;
using System.Collections;

public class ObjectInteraction : MonoBehaviour
{
    public TextMeshProUGUI textoInteracao;
    public string mensagem = "Você encontrou uma pista!";
    public float tempoNaTela = 3f;

    void Start()
    {
        textoInteracao.gameObject.SetActive(false);
    }

    void OnMouseDown()
    {
        StopAllCoroutines();
        StartCoroutine(MostrarMensagem());
    }

    IEnumerator MostrarMensagem()
    {
        textoInteracao.text = mensagem;
        textoInteracao.gameObject.SetActive(true);
        yield return new WaitForSeconds(tempoNaTela);
        textoInteracao.gameObject.SetActive(false);
    }
}
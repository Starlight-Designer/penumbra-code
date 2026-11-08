
using UnityEngine;
using TMPro;
using System.Collections;

public class ObjectInteraction : MonoBehaviour
{
    public TextMeshProUGUI textoInteracao;
    public string mensagem = "Você pegou a fita!";
    public float tempoNaTela = 3f;

    public string nomeItem = "Fita VHS";
    public bool coletarItem = true;

    void Start()
    {
        textoInteracao.gameObject.SetActive(false);

        if (coletarItem && InventoryManager.instance != null &&
            InventoryManager.instance.itens.Contains(nomeItem))
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnMouseDown()
    {
        StopAllCoroutines();

        if (coletarItem)
        {
            InventoryManager.instance.AdicionarItem(nomeItem);
        }

        StartCoroutine(MostrarMensagem());
    }

    IEnumerator MostrarMensagem()
    {
        textoInteracao.text = mensagem;
        textoInteracao.gameObject.SetActive(true);

        yield return new WaitForSeconds(tempoNaTela);

        textoInteracao.gameObject.SetActive(false);

        if (coletarItem)
        {
            Destroy(gameObject);
        }
    }
}

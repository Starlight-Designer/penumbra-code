
using System.Collections;
using UnityEngine;

public class TransformacaoPastor : MonoBehaviour
{
    [Header("Sprite do pastor")]
    public SpriteRenderer spriteRenderer;

    [Header("Sprites da transformação")]
    public Sprite sprite1;
    public Sprite sprite2;
    public Sprite sprite3;
    public Sprite sprite4;

    [Header("Configurações do fade")]
    public float duracaoFadeOut = 0.8f;
    public float duracaoFadeIn = 0.8f;
    public float intervaloEntreSprites = 0.3f;

    private bool transformacaoIniciada = false;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && sprite1 != null)
        {
            spriteRenderer.sprite = sprite1;
            AlterarAlpha(1f);
        }
    }

    public void IniciarTransformacao()
    {
        if (transformacaoIniciada)
            return;

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer não encontrado no pastor!");
            return;
        }

        if (sprite1 == null || sprite2 == null ||
            sprite3 == null || sprite4 == null)
        {
            Debug.LogError("Preencha os quatro sprites da transformação!");
            return;
        }

        transformacaoIniciada = true;
        StartCoroutine(RotinaTransformacao());
    }

    private IEnumerator RotinaTransformacao()
    {
        Sprite[] sprites = { sprite1, sprite2, sprite3, sprite4 };

        for (int i = 1; i < sprites.Length; i++)
        {
            // Desaparece o sprite atual.
            yield return StartCoroutine(Fade(1f, 0f, duracaoFadeOut));

            // Troca para o próximo sprite.
            spriteRenderer.sprite = sprites[i];

            // Aguarda um pequeno intervalo.
            yield return new WaitForSeconds(intervaloEntreSprites);

            // Faz o próximo sprite aparecer.
            yield return StartCoroutine(Fade(0f, 1f, duracaoFadeIn));
        }

        // Garante que o monstro permaneça visível.
        AlterarAlpha(1f);

        Debug.Log("Transformação do pastor concluída!");
    }

    private IEnumerator Fade(float inicio, float fim, float duracao)
    {
        float tempo = 0f;

        if (duracao <= 0f)
        {
            AlterarAlpha(fim);
            yield break;
        }

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;

            float progresso = Mathf.Clamp01(tempo / duracao);
            float alpha = Mathf.Lerp(inicio, fim, progresso);

            AlterarAlpha(alpha);

            yield return null;
        }

        AlterarAlpha(fim);
    }

    private void AlterarAlpha(float alpha)
    {
        Color cor = spriteRenderer.color;
        cor.a = alpha;
        spriteRenderer.color = cor;
    }
}

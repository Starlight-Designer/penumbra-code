
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialInicialManager : MonoBehaviour
{
    [System.Serializable]
    public class Instrucao
    {
        [Header("Texto")]
        [TextArea(2, 4)]
        public string mensagem;

        public TextMeshProUGUI textoUI;

        [Header("Posição na tela")]
        public Vector2 posicao;

        [Header("Tamanho da fonte")]
        public float tamanhoFonte = 36f;

        [Header("Duração")]
        public float duracao = 3f;

        [Header("Animação")]
        public float duracaoFadeIn = 0.5f;
        public float duracaoFadeOut = 0.5f;

        [Header("Ativação")]
        public bool ativa = true;
    }

    [Header("Configurações")]
    public ObjectiveManager objectiveManager;

    [Header("Instruções do tutorial")]
    public List<Instrucao> instrucoes = new List<Instrucao>();

    [Header("Intervalo entre instruções")]
    public float intervaloEntreInstrucoes = 0.5f;

    [Header("Painel de objetivos")]
    public bool mostrarPainelObjetivos = true;

    private const string CHAVE_TUTORIAL_CONCLUIDO = "TutorialInicialConcluido";
    private const string CHAVE_TUTORIAL_EM_ANDAMENTO = "TutorialInicialEmAndamento";

    private Coroutine rotinaTutorial;

    private void Start()
    {
        // Verifica se o tutorial já foi concluído.
        if (PlayerPrefs.GetInt(CHAVE_TUTORIAL_CONCLUIDO, 0) == 1)
        {
            EsconderTodasInstrucoes();
            return;
        }

        PlayerPrefs.SetInt(CHAVE_TUTORIAL_EM_ANDAMENTO, 1);
        PlayerPrefs.Save();

        EsconderTodasInstrucoes();

        rotinaTutorial = StartCoroutine(IniciarTutorial());
    }

    private IEnumerator IniciarTutorial()
    {
        // Aguarda um frame para os outros sistemas iniciarem.
        yield return null;

        if (objectiveManager == null)
            objectiveManager = FindObjectOfType<ObjectiveManager>();

        // Mostra o painel de objetivos no início do tutorial.
        if (mostrarPainelObjetivos && objectiveManager != null)
        {
            objectiveManager.MostrarPainelInicial();
        }

        // Percorre todas as instruções cadastradas.
        foreach (Instrucao instrucao in instrucoes)
        {
            if (instrucao == null || !instrucao.ativa)
                continue;

            if (instrucao.textoUI == null)
            {
                Debug.LogWarning("Texto UI não configurado em uma instrução do tutorial.");
                continue;
            }

            TextMeshProUGUI texto = instrucao.textoUI;

            // Configura a mensagem.
            texto.text = instrucao.mensagem;

            // Configura o tamanho da fonte.
            texto.fontSize = instrucao.tamanhoFonte;

            // Configura a posição.
            texto.rectTransform.anchoredPosition = instrucao.posicao;

            // Ativa o objeto de texto.
            texto.gameObject.SetActive(true);

            // Começa invisível.
            Color cor = texto.color;
            cor.a = 0f;
            texto.color = cor;

            // Fade In.
            yield return StartCoroutine(
                FadeTexto(texto, 0f, 1f, instrucao.duracaoFadeIn)
            );

            // Mantém o texto visível pelo tempo configurado.
            yield return new WaitForSecondsRealtime(instrucao.duracao);

            // Fade Out.
            yield return StartCoroutine(
                FadeTexto(texto, 1f, 0f, instrucao.duracaoFadeOut)
            );

            // Desativa o texto.
            texto.gameObject.SetActive(false);

            // Intervalo antes da próxima instrução.
            yield return new WaitForSecondsRealtime(intervaloEntreInstrucoes);
        }

        // Marca o tutorial como concluído.
        PlayerPrefs.SetInt(CHAVE_TUTORIAL_CONCLUIDO, 1);
        PlayerPrefs.SetInt(CHAVE_TUTORIAL_EM_ANDAMENTO, 0);
        PlayerPrefs.Save();

        rotinaTutorial = null;

        Debug.Log("Tutorial inicial concluído!");
    }

    private IEnumerator FadeTexto(
        TextMeshProUGUI texto,
        float alphaInicial,
        float alphaFinal,
        float duracao
    )
    {
        if (texto == null)
            yield break;

        Color cor = texto.color;

        if (duracao <= 0f)
        {
            cor.a = alphaFinal;
            texto.color = cor;
            yield break;
        }

        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.unscaledDeltaTime;

            float progresso = Mathf.Clamp01(tempo / duracao);

            cor.a = Mathf.Lerp(alphaInicial, alphaFinal, progresso);
            texto.color = cor;

            yield return null;
        }

        cor.a = alphaFinal;
        texto.color = cor;
    }

    private void EsconderTodasInstrucoes()
    {
        foreach (Instrucao instrucao in instrucoes)
        {
            if (instrucao == null || instrucao.textoUI == null)
                continue;

            Color cor = instrucao.textoUI.color;
            cor.a = 0f;

            instrucao.textoUI.color = cor;
            instrucao.textoUI.gameObject.SetActive(false);
        }
    }
}

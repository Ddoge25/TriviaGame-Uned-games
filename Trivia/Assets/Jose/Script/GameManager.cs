using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI enunciadoTexto;
    public TextMeshProUGUI categoriaTexto; // 👈 Muestra la dificultad actual
    public Button[] botonesRespuesta;

    [Header("Banco de preguntas (se consumen al usarse)")]
    public List<Pregunta> muyFaciles = new List<Pregunta>();
    public List<Pregunta> faciles = new List<Pregunta>();
    public List<Pregunta> medias = new List<Pregunta>();
    public List<Pregunta> dificiles = new List<Pregunta>();
    public List<Pregunta> muyDificiles = new List<Pregunta>();

    [Header("Eventos")]
    public UnityEvent OnGanar;
    public UnityEvent OnPerder;

    private Pregunta preguntaActual;
    private List<string> respuestasMezcladas = new List<string>();

    private int respuestasCorrectas = 0;
    public int respuestasParaGanar = 5;

    // 👇 Ahora con 5 categorías
    private enum Dificultad { MuyFacil, Facil, Medio, Dificil, MuyDificil, Fin }
    private Dificultad dificultadActual = Dificultad.MuyFacil;

    void Start()
    {
        CargarSiguientePregunta();
    }

    private void CargarSiguientePregunta()
    {
        switch (dificultadActual)
        {
            case Dificultad.MuyFacil:
                if (muyFaciles.Count > 0) CargarPregunta(muyFaciles, "Muy Fácil");
                else { dificultadActual = Dificultad.Facil; CargarSiguientePregunta(); }
                break;

            case Dificultad.Facil:
                if (faciles.Count > 0) CargarPregunta(faciles, "Fácil");
                else { dificultadActual = Dificultad.Medio; CargarSiguientePregunta(); }
                break;

            case Dificultad.Medio:
                if (medias.Count > 0) CargarPregunta(medias, "Normal");
                else { dificultadActual = Dificultad.Dificil; CargarSiguientePregunta(); }
                break;

            case Dificultad.Dificil:
                if (dificiles.Count > 0) CargarPregunta(dificiles, "Difícil");
                else { dificultadActual = Dificultad.MuyDificil; CargarSiguientePregunta(); }
                break;

            case Dificultad.MuyDificil:
                if (muyDificiles.Count > 0) CargarPregunta(muyDificiles, "Muy Difícil");
                else { dificultadActual = Dificultad.Fin; OnGanar.Invoke(); }
                break;

            case Dificultad.Fin:
                Debug.Log("🎉 Juego terminado, no hay más preguntas.");
                break;
        }
    }

    public void CargarPregunta(List<Pregunta> listaPreguntas, string nombreCategoria)
    {
        if (listaPreguntas == null || listaPreguntas.Count == 0)
        {
            Debug.LogWarning("⚠ Lista de preguntas vacía. Pasando a la siguiente dificultad.");
            CargarSiguientePregunta();
            return;
        }

        // 🔄 Resetear botones
        foreach (Button boton in botonesRespuesta)
        {
            boton.onClick.RemoveAllListeners();
            boton.interactable = true;
            boton.gameObject.SetActive(true);
        }

        // 👉 Escoger una pregunta al azar de la lista
        int index = Random.Range(0, listaPreguntas.Count);
        preguntaActual = listaPreguntas[index];
        listaPreguntas.RemoveAt(index); // consumir la pregunta para que no se repita

        // Mostrar enunciado
        enunciadoTexto.text = preguntaActual.enunciado;

        // Mostrar categoría en pantalla
        if (categoriaTexto != null)
            categoriaTexto.text = "Categoría: " + nombreCategoria;

        // Preparar respuestas mezcladas
        respuestasMezcladas.Clear();
        respuestasMezcladas.Add(preguntaActual.respuestaCorrecta);
        respuestasMezcladas.AddRange(preguntaActual.respuestasIncorrectas);

        // Mezclar con Fisher-Yates
        for (int i = 0; i < respuestasMezcladas.Count; i++)
        {
            string temp = respuestasMezcladas[i];
            int randomIndex = Random.Range(i, respuestasMezcladas.Count);
            respuestasMezcladas[i] = respuestasMezcladas[randomIndex];
            respuestasMezcladas[randomIndex] = temp;
        }

        // Asignar respuestas a botones
        for (int i = 0; i < botonesRespuesta.Length; i++)
        {
            if (i < respuestasMezcladas.Count)
            {
                int indice = i;
                botonesRespuesta[i].GetComponentInChildren<TextMeshProUGUI>().text = respuestasMezcladas[i];
                botonesRespuesta[i].onClick.AddListener(() => RevisarRespuesta(indice));
            }
            else
            {
                botonesRespuesta[i].gameObject.SetActive(false);
            }
        }
    }

    public void RevisarRespuesta(int indice)
    {
        string seleccion = respuestasMezcladas[indice];

        // 🔒 Bloquear botones mientras se decide
        foreach (Button boton in botonesRespuesta)
            boton.interactable = false;

        if (seleccion == preguntaActual.respuestaCorrecta)
        {
            Debug.Log("✅ Correcto!");
            respuestasCorrectas++;

            if (respuestasCorrectas >= respuestasParaGanar)
            {
                OnGanar.Invoke();
                respuestasCorrectas = 0;
            }
            else
            {
                Invoke(nameof(CargarSiguientePregunta), 0.3f);
            }
        }
        else
        {
            Debug.Log("❌ Incorrecto!");
            OnPerder.Invoke();
            respuestasCorrectas = 0;

            // 👇 No reinicia desde muy fácil, sigue la secuencia natural
            Invoke(nameof(CargarSiguientePregunta), 0.3f);
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralManager : MonoBehaviour
{
    // 🔹 Activar un objeto
    public void ActivarObjeto(GameObject obj)
    {
        if (obj != null) obj.SetActive(true);
    }

    // 🔹 Desactivar un objeto
    public void DesactivarObjeto(GameObject obj)
    {
        if (obj != null) obj.SetActive(false);
    }

    // 🔹 Cambiar de escena
    public void CambiarEscena(string nombreEscena)
    {
        if (!string.IsNullOrEmpty(nombreEscena))
            SceneManager.LoadScene(nombreEscena);
    }

    // 🔹 Cerrar el juego
    public void SalirJuego()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego..."); // Se ve en el editor
    }
}

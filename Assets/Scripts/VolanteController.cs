using UnityEngine;

public class VolanteController : MonoBehaviour
{
    public GameObject volante;         // Riferimento al GameObject del volante
    public float angoloMassimo = 450f;   // Angolo massimo di rotazione del volante

    public void FixedUpdate()
    {
        // Ottieni l'input di sterzata tramite l'asse "Horizontal"
        float inputSterzata = Input.GetAxis("Horizontal");

        // Calcola l'angolo di rotazione del volante
        float angoloRotazione = inputSterzata * angoloMassimo;

        // Applica la rotazione al volante sull'asse Z
        if (volante != null)
        {
            volante.transform.localRotation = Quaternion.Euler(0, 0, -angoloRotazione);
        }
    }
}

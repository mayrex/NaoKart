using UnityEngine;
using VehiclePhysics;


public class VolanteController : MonoBehaviour
{
    public GameObject volante;         // Riferimento al GameObject del volante
    public float angoloMassimo = 450f;   // Angolo massimo di rotazione del volante

    public VPWheelCollider[] vpWheels;            // Se usi Vehicle Physics Pro
    public Transform[] vpWheelMeshes;        // Mesh 3D delle ruote VPP
    public Transform[] vpSuspMeshes;        // Mesh 3D delle ruote VPP
    private Vector3[] initialMeshRotations;

    void Start()
    {
        // Salva la rotazione iniziale delle mesh
        initialMeshRotations = new Vector3[vpWheelMeshes.Length];
        for (int i = 0; i < vpWheelMeshes.Length; i++)
        {
            initialMeshRotations[i] = vpWheelMeshes[i].localEulerAngles;
        }
    }

    public void FixedUpdate()
    {
        TiresEffects();

        SteeringWheel();
    }

    
    private void TiresEffects()
    {
        for (int i = 0; i < vpWheels.Length; i++)
        {
            // Prendiamo la rotazione corrente della mesh
            Vector3 meshRotation = initialMeshRotations[i];

            // Aggiungiamo l'angolo di sterzo (Y)
            meshRotation.y += vpWheels[i].steerAngle;

            // Facciamo ruotare la ruota mentre gira (X)
            float rotationAmount = vpWheels[i].angularVelocity * Mathf.Rad2Deg * Time.deltaTime;
            meshRotation.x += rotationAmount;

            // Applichiamo la nuova rotazione
            vpWheelMeshes[i].localEulerAngles = meshRotation;
        }
    }
    private void SteeringWheel()
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

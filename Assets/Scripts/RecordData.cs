using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using VehiclePhysics;

public class RecordData : MonoBehaviour
{
    string filename = "";
    public Transform player;
    public Rigidbody rbPlayer;
    public bool InCurva = false;
    public int i = 0;


    

    private void Start()
    {
        player = GetComponent<Transform>();
        // Correggi il percorso se necessario
        filename = Application.dataPath + "/DATASET/test" + i + ".csv";

        // Assicurati che la cartella esista
        Directory.CreateDirectory(Path.GetDirectoryName(filename));

        // Crea il file e scrivi l'intestazione se non esiste già
        if (!File.Exists(filename))
        {
            using (TextWriter tw = new StreamWriter(filename, false))
            {
                tw.WriteLine("X, Y, Z, VX, VY, VZ");
            }
        }

        // Avvia la coroutine per scrivere le coordinate ogni secondo
        StartCoroutine(WriteCSVEverySecond());
    }

    private IEnumerator WriteCSVEverySecond()
    {
        while (true)
        {
            WriteCSV(InCurva);
            yield return new WaitForSeconds(1f);
        }
    }

    public void WriteCSV(bool inCurva)
    {
        if (InCurva)
        {
            using (TextWriter tw = new StreamWriter(filename, true))
            {                
                tw.WriteLine($"{player.position.x}, {player.position.y}, {player.position.z}, {rbPlayer.velocity.x}, {rbPlayer.velocity.y}, {rbPlayer.velocity.z}");
            }
           
        }
        i++;
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using VehiclePhysics;



public class TrafficLightController : MonoBehaviour
{
    public TextMeshProUGUI conferma;
    public Image panelImage;
    public Sprite[] semafori;
    private VPStandardInput standardInput;
    private VehicleBase vehicle;
    private bool semaforo = true;
    public GhostRecorder ghostRecorder;
    public GhostPlayer ghostPlayer;
    public GhostDeltaTimeCalculator delta;
    private void Start()
    {
        vehicle = GetComponent<VehicleBase>();
        standardInput = GetComponent<VPStandardInput>();

        if (standardInput != null)
            standardInput.enabled = false;  
    }
    private void Update()
    {

        if (Input.GetButton("Jump") && semaforo)
        {
            semaforo = false;
            conferma.text = "";
            ChangeImage();
            
        }
    }
    public void ChangeImage()
    {
        panelImage.enabled = true;
        StartCoroutine(ChangeImageCoroutine());
    }

    private IEnumerator ChangeImageCoroutine()
    {
        
        for (int i = 0; i < 7; i++)
        {
            panelImage.sprite = semafori[i];
            if(i == 6)
            {
                standardInput.enabled = true;
                // ghostRecorder.StartRecording();
                ghostPlayer.enabled = true;
                delta.enabled = true;
                yield return new WaitForSeconds(3f);
                panelImage.enabled = false;
                break;
            }
            else
            {
                
                yield return new WaitForSeconds(1f);
            }
            
            
        }
        
    }
}
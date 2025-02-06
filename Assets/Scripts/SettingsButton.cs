using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButton : MonoBehaviour
{

    public GameObject[] panel;
    public bool set = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetSettings(GameObject panel)
    {
        set = !set;
        panel.SetActive(set);
    }
    
}

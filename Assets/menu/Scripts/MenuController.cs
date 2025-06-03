
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject[] panels;
    private int currentIndex = 0;

    public void ShowPanel(int index)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            bool isActive = (i == index);
            panels[i].SetActive(isActive);
        }
        currentIndex = index;
    }
}

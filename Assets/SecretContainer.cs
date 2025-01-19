using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SecretContainer : MonoBehaviour
{
    public string SecretWord;
    public GameObject WinPanel;
    public GridGenerator _GridGenerator;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SectretWordPoisk()
    {
        SecretWord = _GridGenerator.secretWord;
        string q = "";
        for (int i = 0; i < transform.childCount; i++)
        {
            q += transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text;
        }
        if(q == SecretWord)
        {
            WinPanel.SetActive(true);
        }
    }

    public void ClearSecretBukvi()
    {
        StartCoroutine(cor());
    }

    IEnumerator cor()
    {
        while (transform.childCount > 0)
        {
            yield return new WaitForEndOfFrame();
            transform.GetChild(0).GetComponent<CellController>().cellParent.ResetCell();
            Destroy(transform.GetChild(0).gameObject);
        }
        yield return null;
    }
}

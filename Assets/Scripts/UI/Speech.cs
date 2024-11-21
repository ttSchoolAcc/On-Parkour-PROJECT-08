using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Speech : MonoBehaviour
{

    public TextMeshProUGUI text;
    public GameObject textBoxComplex;
    int currVisibleTextAmount;
    public string[] texts; 
    string currText;

    public void StartChat(int selectedTextIndex, int conversationRange)
    {
        textBoxComplex.SetActive(true);
        currText = texts[selectedTextIndex];
        text.text = currText;

        currVisibleTextAmount = 0;
        text.maxVisibleCharacters = 0;
        StartCoroutine(TypeOut(selectedTextIndex, conversationRange));
    }

    IEnumerator TypeOut(int selectedTextIndex, int conversationRange)
    {
        while(currVisibleTextAmount < currText.Length)
        {
            currVisibleTextAmount++;
            text.maxVisibleCharacters = currVisibleTextAmount;
            //text.ForceMeshUpdate();
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(4f);

        textBoxComplex.SetActive(false);

        if(conversationRange > 0)
        {
            conversationRange--;
            selectedTextIndex++;
            if(selectedTextIndex < texts.Length)
            {
                StartChat(selectedTextIndex, conversationRange);
            }
        }
    }
}

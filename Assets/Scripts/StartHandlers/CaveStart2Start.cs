using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveStart2Start : MonoBehaviour
{
    [SerializeField]
    Speech orbSpeech;
    [SerializeField]
    int conversationRange = 0;

    // Start is called before the first frame update
    void Start()
    {
        orbSpeech.StartChat(0, conversationRange);
    }
}

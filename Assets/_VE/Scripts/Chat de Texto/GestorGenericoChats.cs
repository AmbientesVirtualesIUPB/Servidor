using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestorGenericoChats : MonoBehaviour
{
    public GameObject prChats;
    public Transform padre;

    public static GestorGenericoChats singleton;

    private void Awake()
    {
        singleton = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

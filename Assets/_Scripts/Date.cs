using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Date : MonoBehaviour
{
    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        string date = DateTime.Now.ToString("MMMM dd");
        text.text = date;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combo : MonoBehaviour
{
    Text combo;

    // Start is called before the first frame update
    void Start()
    {
        combo = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        combo.text = "Highest Combo: " + ScoreManager.MaxCombo;
    }
}

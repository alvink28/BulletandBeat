using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    //public AudioSource hitSFX;
    //public AudioSource missSFX;
    public TMPro.TextMeshPro scoreText;
    static int comboScore;
    public static int MaxCombo;

    void Start()
    {
        Instance = this;
        comboScore = 0;
    }

    public static void Hit()
    {
        comboScore += 1;
        //Instance.hitSFX.Play();
    }

    public static void Miss()
    {
        comboScore = 0;
        //Instance.missSFX.Play();
    }

    public static void Maxcombo()
    {
        MaxCombo = 0;
    }

    // Update is called once per frame
    public void Update()
    {
        scoreText.text = "Combo: " + comboScore.ToString();
        if (comboScore > MaxCombo)
        {
            MaxCombo = comboScore;
        }
    }
}

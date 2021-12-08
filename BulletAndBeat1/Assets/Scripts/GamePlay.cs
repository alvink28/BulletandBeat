using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.Net.Mime.MediaTypeNames;

public class GamePlay : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject bulletPrefab;
    public AudioSource audioSource;
    public CameraControl cameraShake;

    public float timeLeft = 102f; //102

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("Fire!");
            audioSource.Play();
            GameObject bulletObject = Instantiate(bulletPrefab);
            bulletObject.transform.position = playerCamera.transform.position + playerCamera.transform.forward;
            bulletObject.transform.forward = playerCamera.transform.forward;

            StartCoroutine(cameraShake.Shake(.15f, .4f));
        }

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        UnityEngine.Debug.Log("timesup!!");
        SceneManager.LoadScene("GameOver");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public Transform boss;
    private Canvas canvas;
    private bool gameStarted = false;
    private float cutscene = 2f;
    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) gameStarted = true;
        if (gameStarted)
        {
            cutscene -= Time.deltaTime;
            canvas.enabled = false;
            boss.position = AnimMath.Slide(boss.position, new Vector3(-150, -15, 0), 0.25f);
            if (cutscene <= 0) SceneManager.LoadScene("AnimatedBoss");
        }
    }
}

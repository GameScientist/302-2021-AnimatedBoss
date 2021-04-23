using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Plays a cutscene before transitioning to the next scene when commanded to.
/// </summary>
public class TitleScreen : MonoBehaviour
{
    /// <summary>
    /// Whether the player has ordered to be taken to the next scene.
    /// </summary>
    private bool gameStarted = false;
    /// <summary>
    /// The length of the cutscene before getting into the game.
    /// </summary>
    private float cutscene = 2f;
    /// <summary>
    /// A sound effect that plays when the boss is rising up from the water.
    /// </summary>
    public AudioSource bubbles;
    /// <summary>
    /// The UI that is disabled when the game is started.
    /// </summary>
    private Canvas canvas;
    /// <summary>
    /// The model of the boss seen rising from the water.
    /// </summary>
    public Transform boss;
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
            bubbles.Play();
        }
    }
}

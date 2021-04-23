using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Sinks a raft downward when a signal is recieved.
/// </summary>
public class Area : MonoBehaviour
{
    /// <summary>
    /// If the raft is currently sinking.
    /// </summary>
    public bool sinking;

    // Update is called once per frame
    void Update()
    {
        if (sinking) transform.position -= transform.up * 7.5f * Time.deltaTime;
        if (transform.position.y <= -2500) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HeroController hero = other.GetComponent<HeroController>();
            hero.currentArea = this;
        }
    }
}

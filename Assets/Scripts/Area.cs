using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    public bool sinking;
    // Start is called before the first frame update
    void Start()
    {
        
    }

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

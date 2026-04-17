using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionWreck : MonoBehaviour
{
    [SerializeField] GameObject _wreck;
    [SerializeField] GameObject _explosion;
    [SerializeField] GameObject _particle;

    Coroutine _coroutine;

    void Start()
    {
        _wreck.SetActive(true);
        _explosion.SetActive(false);
        _particle.SetActive(false);
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Test Player")
        {
            if (_coroutine != null)
                return;

            _coroutine = StartCoroutine(StartExplosion());
        }
    }

    IEnumerator StartExplosion()
    {
        // Æø¹ß Áö¿¬
        //yield return new WaitForSeconds(1f);

        _wreck.SetActive(false);
        _explosion.SetActive(true);
        _particle.SetActive(true);

        yield return new WaitForSeconds(3f);
    }
}

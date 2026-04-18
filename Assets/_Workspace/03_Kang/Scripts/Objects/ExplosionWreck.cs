using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionWreck : MonoBehaviour
{
    [SerializeField] GameObject _wreck;
    [SerializeField] GameObject _explosion;
    [SerializeField] GameObject _particle;
    [SerializeField] GameObject _dangerArea;
    [SerializeField] GameObject _TwinklingEffect;

    void Start()
    {
        _wreck.SetActive(true);
        _explosion.SetActive(false);
        _particle.SetActive(false);
        _dangerArea.GetComponent<SpriteRenderer>().enabled = false;

        _explosion.transform.localPosition = Vector3.zero;
        _dangerArea.transform.localPosition = Vector3.zero;
    }

    public void ActiveFuse()
    {
        StartCoroutine(BlinkEffect());
        StartCoroutine(StartExplosion());
    }

    [SerializeField] private float _delay = 1f;
    IEnumerator StartExplosion()
    {
        // Æø¹ß Áö¿¬
        yield return new WaitForSeconds(_delay);

        _wreck.SetActive(false);
        _dangerArea.SetActive(false);
        _TwinklingEffect.SetActive(false);

        _explosion.SetActive(true);
        _particle.SetActive(true);

        yield return new WaitForSeconds(3f);
    }

    IEnumerator BlinkEffect()
    {
        _dangerArea.GetComponent<CircleCollider2D>().enabled = false;

        float repeat = (_delay / 0.2f);

        for (int i = 0; i < repeat; i++)
        {
            Debug.Log($"repeat : {repeat}");
            _dangerArea.GetComponent<SpriteRenderer>().enabled = !_dangerArea.GetComponent<SpriteRenderer>().enabled;
            yield return new WaitForSeconds(0.2f);
        }
    }
}

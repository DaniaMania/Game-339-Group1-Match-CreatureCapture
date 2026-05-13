using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Attach to a TMP UGUI text. Call Play() to set the text/color and start the float-up-and-fade animation.
/// The GameObject destroys itself when the animation finishes.
/// Use as a prefab — CharacterView instantiates it on demand.
/// </summary>
public class FloatingNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    [Header("Animation")]
    [SerializeField] private float _duration = 1f;
    [SerializeField] private float _floatDistance = 80f;
    [SerializeField] private float _horizontalJitter = 20f;

    public void Play(string content, Color color)
    {
        if (_text == null) _text = GetComponent<TextMeshProUGUI>();
        _text.text = content;
        _text.color = color;

        // Small random horizontal offset so multiple numbers spawned together don't perfectly overlap.
        float jitter = Random.Range(-_horizontalJitter, _horizontalJitter);
        transform.localPosition += new Vector3(jitter, 0f, 0f);

        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        Vector3 start = transform.localPosition;
        Color startColor = _text.color;
        float elapsed = 0f;
        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _duration);
            transform.localPosition = start + Vector3.up * (_floatDistance * t);
            Color c = startColor;
            c.a = 1f - t;
            _text.color = c;
            yield return null;
        }
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public Rigidbody rb;
    public float forceMin;
    public float forceMax;
    private float lifetime = 4f;
    private float fadetime = 2f;
    void Start()
    {
        rb.AddForce(rb.transform.right * Random.Range(forceMin, forceMax));
        rb.AddTorque(Random.insideUnitSphere * Random.Range(forceMin, forceMax));
        StartCoroutine(Fade());
    }

    private IEnumerator  Fade()
    {
        yield return new WaitForSeconds(lifetime);
        float percent = 0;
        float speed = 1 / fadetime;
        Material mat = GetComponent<Renderer>().material;
        Color initialColor = mat.color;
        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            mat.color = Color.Lerp(initialColor, Color.clear, percent);
            yield return null;
        }
        Destroy(gameObject);
    }


    void Update()
    {
        
    }
}

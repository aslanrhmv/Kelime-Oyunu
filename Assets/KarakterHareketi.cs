using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KarakterHareketi : MonoBehaviour
{
    public float hareketHizi = 10f;
    public float ziplamaGucu = 7f; 

    private Rigidbody rb;
    private bool yerdeMi = true; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float yatay = Input.GetAxis("Horizontal");
        float dikey = Input.GetAxis("Vertical");

        Vector3 hareket = new Vector3(yatay, 0, dikey) * hareketHizi * Time.deltaTime;
        transform.Translate(hareket);

        if (Input.GetKeyDown(KeyCode.Space) && yerdeMi)
        {
            rb.AddForce(Vector3.up * ziplamaGucu, ForceMode.Impulse);
            yerdeMi = false;
        }
    }
    void OnCollisionEnter(Collision temas)
    {
        if (temas.gameObject.CompareTag("Zemin"))
        {
            yerdeMi = true; 
        }
    }
}
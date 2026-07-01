using UnityEngine;

public class Carpisma : MonoBehaviour
{
    OyunYoneticisi yonetici;

    void Start()
    {
        yonetici = FindFirstObjectByType<OyunYoneticisi>();
    }

    void OnCollisionEnter(Collision temas)
    {
        KontrolEt(temas.gameObject);
    }

    void OnTriggerEnter(Collider temas)
    {
        KontrolEt(temas.gameObject);
    }

    void KontrolEt(GameObject carpanObje)
    {

        if (carpanObje.CompareTag("Dogru"))
        {
            Debug.Log("Doğru topa çarptım");
            yonetici.CevapVerildi(true);
        }
        else if (carpanObje.CompareTag("Yanlis"))
        {
            Debug.Log("Yanlış topa çarptım");
            yonetici.CevapVerildi(false);
        }
    }
}
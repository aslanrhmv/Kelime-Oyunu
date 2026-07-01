using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OyunYoneticisi : MonoBehaviour
{
    [Header("--- SAHNE NESNELERİ ---")]
    public TextMesh soruMetni;     
    public TextMesh oyuncuIsmi3D;    
    public Transform oyuncu;
    public Transform baslangicNoktasi;

    [Header("--- UI KUTULARI ---")]
    public GameObject girisPaneli;
    public InputField isimKutusu;
    public Text isimText;
    public Text soruSayacText;
    public Text skorText;
    public Text feedbackText;
    public GameObject yardimPaneli;
    public Button sesButonu;

    [Header("--- SESLER ---")]
    public AudioClip dogruSesi;
    public AudioClip yanlisSesi;
    private AudioSource sesKaynagi;
    private bool sesAcik = true;

    int toplamSoru = 20;
    int kacinciSoru = 0;
    int dogru = 0;
    int yanlis = 0;

    bool cevaplandi = false;

    public class Soru { public string Kelime; public bool DogruMu; }
    List<Soru> soruListesi;
    Soru aktifSoru;

    void Start()
    {
        Time.timeScale = 0;
        sesKaynagi = gameObject.AddComponent<AudioSource>();
        if (dogruSesi == null) dogruSesi = Resources.Load<AudioClip>("dogru_ses");
        if (yanlisSesi == null) yanlisSesi = Resources.Load<AudioClip>("yanlis_ses");

        if (soruMetni == null) soruMetni = FindFirstObjectByType<TextMesh>();
        if (oyuncu == null && GameObject.Find("KARAKTER") != null)
            oyuncu = GameObject.Find("KARAKTER").transform;

        if (girisPaneli != null) girisPaneli.SetActive(true);
        if (yardimPaneli != null) yardimPaneli.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";

        SorulariYukle();
        UIGuncelle();
    }

    public void OyunaBasla()
    {
        if (girisPaneli != null)
        {
            string isim = "Adsız Kahraman";
            if (isimKutusu != null && isimKutusu.text.Length > 0) isim = isimKutusu.text;

            if (isimText != null) isimText.text = "Oyuncu: " + isim;
            if (oyuncuIsmi3D != null) oyuncuIsmi3D.text = isim;

            girisPaneli.SetActive(false);
            Time.timeScale = 1;
            YeniSoruSor();
        }
    }

    public void SesAcKapat()
    {
        sesAcik = !sesAcik;
        if (sesButonu != null)
            sesButonu.GetComponentInChildren<Text>().text = sesAcik ? "SES: AÇIK" : "SES: KAPALI";
    }

    public void YardimAc()
    {
        if (yardimPaneli != null) yardimPaneli.SetActive(true);
        Time.timeScale = 0;
    }

    public void YardimKapat()
    {
        if (yardimPaneli != null) yardimPaneli.SetActive(false);
        if (kacinciSoru > 0) Time.timeScale = 1;
    }

    void YeniSoruSor()
    {
        if (kacinciSoru >= toplamSoru) { OyunBitti(); return; }

        cevaplandi = false;

        if (soruListesi != null && soruListesi.Count > 0)
        {
            aktifSoru = soruListesi[Random.Range(0, soruListesi.Count)];
            kacinciSoru++;
            UIGuncelle(); 

            if (soruMetni != null)
            {
                soruMetni.text = aktifSoru.Kelime;
                soruMetni.color = Color.white;
            }
        }
    }

    public void CevapVerildi(bool oyuncuDogruDedi)
    {
        if (cevaplandi == true) return; 
        cevaplandi = true;

        if (aktifSoru == null) return;
        bool dogruMu = (oyuncuDogruDedi == aktifSoru.DogruMu);

        if (dogruMu) { dogru++; GeriBildirim(true); }
        else { yanlis++; GeriBildirim(false); }

        UIGuncelle(); 

        if (oyuncu != null)
        {
            Vector3 hedefYer = Vector3.zero;
            if (baslangicNoktasi != null) hedefYer = baslangicNoktasi.position;
            oyuncu.position = hedefYer + Vector3.up;
            if (oyuncu.GetComponent<Rigidbody>() != null) oyuncu.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }
        Invoke("YeniSoruSor", 1.5f);
    }

    void GeriBildirim(bool basarili)
    {
        if (sesAcik && sesKaynagi != null) sesKaynagi.PlayOneShot(basarili ? dogruSesi : yanlisSesi);

        if (feedbackText != null)
        {
            feedbackText.text = basarili ? "✅ DOĞRU!" : "❌ YANLIŞ!";
            feedbackText.color = basarili ? Color.green : Color.red;
            StartCoroutine(Animasyon());
            Invoke("Temizle", 1.2f);
        }
    }

    IEnumerator Animasyon()
    {
        if (feedbackText != null)
        {
            feedbackText.transform.localScale = Vector3.one * 1.5f;
            yield return new WaitForSeconds(0.2f);
            feedbackText.transform.localScale = Vector3.one;
        }
    }

    void Temizle() { if (feedbackText != null) feedbackText.text = ""; }

    void UIGuncelle()
    {
        if (soruSayacText != null)
        {
            int kalan = toplamSoru - kacinciSoru;
            if (kalan < 0) kalan = 0;

            soruSayacText.text = "Toplam soru : " + toplamSoru + "\nKalan soru : " + kalan;
        }

        if (skorText != null) skorText.text = "✅: " + dogru + "   ❌: " + yanlis;
    }

    void OyunBitti()
    {
        Time.timeScale = 0;

        if (soruMetni != null)
        {
            soruMetni.color = Color.yellow;
            soruMetni.text = "OYUN BİTTİ!\nDoğru: " + dogru + "\nYanlış: " + yanlis;
        }

        if (feedbackText != null) feedbackText.text = "";
    }

    void SorulariYukle()
    {
        TextAsset json = Resources.Load<TextAsset>("kelimeler");
        if (json != null) soruListesi = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Soru>>(json.text);
        else
        {
            soruListesi = new List<Soru>();
            soruListesi.Add(new Soru { Kelime = "ELMA (D)", DogruMu = true });
            soruListesi.Add(new Soru { Kelime = "ARMUT (Y)", DogruMu = false });
        }
    }
}
using Assets._Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExplodirNaveMae : MonoBehaviour
{
    public Transform explosao;
    public int acertos = 5;
    public Text placar;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);

        if (Global.navesDestruidas == 2)
        {
            Global.nivelDanos++;

            if (explosao)
            {
                if (Global.nivelDanos > acertos)
                {
                    GameObject explodir = ((Transform)Instantiate(explosao, this.transform.position, this.transform.rotation)).gameObject;
                    Destroy(explodir, 2.0f);
                }
            }

            if (Global.nivelDanos > acertos)
            {
                Destroy(this.gameObject);
                AtualizaPlacar();
            }
        }
    }

    public void AtualizaPlacar()
    {
        Global.placarJogo += 10;
        placar.text = "Placar: " + Global.placarJogo.ToString();

        PlayerPrefs.SetInt("PlacarJogo", Global.placarJogo);
        PlayerPrefs.Save();

        Global.placarJogo = PlayerPrefs.GetInt("PlacarJogo");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Global.nivelDanos = 0;
    }
}

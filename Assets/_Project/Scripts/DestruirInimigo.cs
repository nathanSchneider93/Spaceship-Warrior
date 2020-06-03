using Assets._Scripts;
using UnityEngine;
using UnityEngine.UI;

public class DestruirInimigo : MonoBehaviour {
    public Transform explosao;
    public Text placar;

    // Use this for initialization
    void Start () {
        if (PlayerPrefs.HasKey("PlacarJogo"))
        {
            Global.placarJogo = PlayerPrefs.GetInt("PlacarJogo");
        }
        else
        {
            PlayerPrefs.SetInt("PlacarJogo", Global.placarJogo);
            PlayerPrefs.Save();
        }

        placar.text = "Placar: " + Global.placarJogo.ToString();
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);

        if (explosao)
        {
            GameObject explodir = ((Transform)Instantiate(explosao, this.transform.position, this.transform.rotation)).gameObject;
            Destroy(explodir, 2.0f);
        }

        Destroy(gameObject);
        AtualizaPlacar();
    }

    public void AtualizaPlacar()
    {
        Global.placarJogo++;
        Global.navesDestruidas++;
        placar.text = "Placar: " + Global.placarJogo.ToString();

        PlayerPrefs.SetInt("PlacarJogo", Global.placarJogo);
        PlayerPrefs.Save();
    }
}

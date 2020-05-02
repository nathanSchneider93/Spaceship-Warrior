using UnityEngine;

public class TiroJogador : MonoBehaviour {
    public float tempoVida = 2.0f;
    public float velocidade = 10.0f;

    // Use this for initialization
    void Start () {
        Destroy(gameObject, tempoVida);
    }

    // Update is called once per frame
    void Update () {
        transform.Translate(Vector3.forward * Time.deltaTime * velocidade);
    }
}

using UnityEngine;

public class MoveNaveInimiga : MonoBehaviour {
    public float posicaoX = 0;
    public float posicaoY = 0;
    public float posicaoZ = 3.5f;
    public float velocidade = 1;

    private float incremento = 0.2f;

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody>().position = new Vector3(posicaoX, posicaoY, posicaoZ);
    }
	
	// Update is called once per frame
	void Update ()
    {
        posicaoX *= velocidade;

        if (posicaoX < -3.0f)
        {
            incremento = 0.2f;
        }
        else if (posicaoX > 3.0f)
        {
            incremento = -0.2f;
        }

        GetComponent<Rigidbody>().position = new Vector3(posicaoX,posicaoY,posicaoZ);
        posicaoX += incremento;
    }
}

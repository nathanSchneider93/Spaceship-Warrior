using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class ControladorJogador : MonoBehaviour {
    public float velocidadeJogador = 1;
    public float inclinacao = 1;
    public float posicaoZ = -6.0f;
    public float valorMinX = 0;
    public float valorMaxX = 0;

    public Transform tiro;
    public float distancia = 0.2f;
    public float intervalo = 0.5f;

    private float proximoTiro = 0.0f;

    public float speed;
    public float amountToMove;
    SerialPort sp = new SerialPort("COM3", 9600);

    // Use this for initialization
    void Start ()
    {
        sp.Open();
        sp.ReadTimeout = 1;
    }
	
	// Update is called once per frame
	void Update ()
    {
        
    }
    void MoveObject(int direction)
    {
        if (direction == 1)
        {
            transform.Translate(Vector3.left * amountToMove, Space.World);
        }
        if (direction == 2)
        {
            transform.Translate(Vector3.right * amountToMove, Space.World);
        }

    }

    void FixedUpdate()
    {

        float moverNave = Input.GetAxis("Horizontal");
        Vector3 movimento = new Vector3(moverNave, 0.0f, posicaoZ);
        //GetComponent<Rigidbody>().velocity = movimento * velocidadeJogador;

        //GetComponent<Rigidbody>().position = new Vector3
        //(
        //    Mathf.Clamp(GetComponent<Rigidbody>().position.x, valorMinX, valorMaxX),
        //    0.0f,
        //    Mathf.Clamp(GetComponent<Rigidbody>().position.z, posicaoZ, posicaoZ)
        //);

        GetComponent<Rigidbody>().rotation = Quaternion.Euler(0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * -inclinacao);

        //Atira();
    }

    void Atira()
    {
        if (Input.GetKey(KeyCode.Space) && (proximoTiro < 0))
        {
            proximoTiro = intervalo;

            float posicaoX = this.transform.position.x + (Mathf.Sin((transform.localEulerAngles.z - 90) * Mathf.Deg2Rad) * -distancia);
            Instantiate(tiro, new Vector3(posicaoX, this.transform.position.y, -5.5f), this.transform.rotation);
        }

        proximoTiro -= Time.deltaTime;
    }
}




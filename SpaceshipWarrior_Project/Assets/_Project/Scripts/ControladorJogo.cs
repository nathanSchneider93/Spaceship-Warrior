using UnityEngine;

public class ControladorJogo : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            PlayerPrefs.DeleteAll();
        }
    }
}

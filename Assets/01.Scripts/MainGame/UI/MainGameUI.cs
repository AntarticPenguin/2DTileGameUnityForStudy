using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameUI : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
		
	}

    // Update is called once per frame
    void Update()
    {

    }


    //Play UI

    public GameObject HPGuagePrefabs;
    public GameObject CooltimeGuagePrefabs;

    public Slider CreateHPSlider()
    {
        GameObject hpObject = GameObject.Instantiate(HPGuagePrefabs);
        Slider slider = hpObject.GetComponent<Slider>();
        return slider;
    }

    public Slider CrateCooltimeSlider()
    {
        GameObject cooltimeObject = GameObject.Instantiate(CooltimeGuagePrefabs);
        Slider slider = cooltimeObject.GetComponent<Slider>();
        return slider;
    }
}

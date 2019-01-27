using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDeathScene : MonoBehaviour {

    [SerializeField] Image bg;
    [SerializeField] Text youDied;
    [SerializeField] float bgEndAlpha = 0.9f;
    [SerializeField] float youDiedEndAlpha = 1f;

    [SerializeField] AudioClip hubClip;


    public float m_Time = 0f;
    public float m_TimeLimit = 1f;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        m_Time += Time.deltaTime;
        if(m_Time < m_TimeLimit)
        {
            var tempColor = bg.color;
            tempColor.a = Mathf.Lerp(0f, bgEndAlpha, m_Time / m_TimeLimit);
            bg.color = tempColor;

            tempColor = youDied.color;
            tempColor.a = Mathf.Lerp(0f, youDiedEndAlpha, m_Time / m_TimeLimit);
            youDied.color = tempColor;
        }
    }

    public void GoToHub()
    {
        StartOptions start = FindObjectOfType<StartOptions>();
        if (start)
        {
            start.sceneToStart = 1;
            start.menuSettingsData.musicLoopToChangeTo = hubClip;
            start.StartButtonClicked();
        }
    }

   
}

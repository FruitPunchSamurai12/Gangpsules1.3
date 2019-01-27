using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    public Image m_Img;
    public Text m_Text;
    public int min;
    public int max;
    float m_Value;
    float m_Percentage;

    public void SetHealth(int health)
    {
        if (health != m_Value)
        {
            if (max - min == 0)
            {
                m_Value = 0;
                m_Percentage = 0;
            }
            else
            {
                m_Value = health;
                m_Percentage = (float)m_Value / (float)(max - min);
            }
            m_Text.text = string.Format("{0}%", Mathf.RoundToInt(m_Percentage * 100));
            m_Img.fillAmount = m_Percentage;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

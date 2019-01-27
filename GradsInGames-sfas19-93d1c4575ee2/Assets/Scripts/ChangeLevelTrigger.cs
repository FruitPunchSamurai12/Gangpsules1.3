using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLevelTrigger : MonoBehaviour {

    [SerializeField] int m_SceneIndex;
    public string m_Message;
    [SerializeField] AudioClip m_ClipToChangeInto;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeLevel()
    {
        StartOptions start = FindObjectOfType<StartOptions>();
        start.sceneToStart = m_SceneIndex;
        start.menuSettingsData.musicLoopToChangeTo = m_ClipToChangeInto;
        start.StartButtonClicked();
    }

}

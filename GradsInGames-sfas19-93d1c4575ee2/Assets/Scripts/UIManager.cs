using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    // --------------------------------------------------------------
    int m_SceneIndex;
    AudioClip m_SceneClip;
    [SerializeField] AudioClip m_DeathClip;


    [SerializeField]
    Text m_BulletText;

    [SerializeField]
    Text m_DialogueText;

    [SerializeField]
    Text m_ArcadeEnemiesRemainingText;

    [SerializeField]
    GameObject m_DialogueBox;

    [SerializeField]
    Text m_ObjectInFrontText;

    [SerializeField]
    GameObject m_ObjectInFrontBox;

    [SerializeField]
    Text m_ArcadeLevelText;

    [SerializeField]
    GameObject m_DeathScene;

    NPCDialogue m_Dialogue;

    HealthBar healthBar;

    public bool talking = false;

    // --------------------------------------------------------------

    private void Start()
    {
        healthBar = GetComponentInChildren<HealthBar>();
        m_DialogueBox.SetActive(false);
        m_ObjectInFrontBox.SetActive(false);
        m_DeathScene.SetActive(false);
        var start = FindObjectOfType<StartOptions>();
        if (start)
        {
            m_SceneIndex = start.sceneToStart;
            m_SceneClip = start.menuSettingsData.musicLoopToChangeTo;
        }
    }

    public void TalkToNPC(NPCDialogue dialogue)
    {
        m_Dialogue = dialogue;
        m_DialogueText.text = m_Dialogue.mText;
        talking = true;
        m_DialogueBox.SetActive(true);
    }

    public void SetAmmoText(int bulletCount)
    {
        if(m_BulletText)
        {
            m_BulletText.text = "Bullets: " + bulletCount;
        }        
       
    }

    public void SetAmmoText(bool dontDisplay = true)
    {
        if (dontDisplay)
        {
            m_BulletText.text = "";
        }

    }

    public void SetHealth(int health)
    {
        healthBar.SetHealth(health);
    }

    public void ActivateDeathScene()
    {
        m_DeathScene.SetActive(true);
        var start = FindObjectOfType<StartOptions>();
        if (start)
        {
            start.menuSettingsData.musicLoopToChangeTo = m_DeathClip;
            start.PlayNewMusic();
        }
    }

    public void UpdateArcadeUI(int enemyNumber,int currentLevel)
    {
        if(m_ArcadeEnemiesRemainingText)
        {
            m_ArcadeEnemiesRemainingText.text = "Enemies: " + enemyNumber;
        }
        if(m_ArcadeLevelText)
        {
            m_ArcadeLevelText.text = "Level: " + currentLevel;
        }
    }

    public void UpdateArcadeUI(bool dontDisplay = true)
    {
        if (m_ArcadeEnemiesRemainingText)
        {
            m_ArcadeEnemiesRemainingText.text = "";
        }
        if (m_ArcadeLevelText)
        {
            m_ArcadeLevelText.text = "";
        }
    }

    public void ManageDialogues()
    {
        m_Dialogue = m_Dialogue.nextDialogue;
        if(m_Dialogue)
        {
            m_DialogueText.text = m_Dialogue.mText;
        }
        else
        {
            talking = false;
            m_DialogueBox.SetActive(false);
        }
    }

    public void UpdateObjectInFront(string message,bool anythingToDisplay)
    {
        if(talking)
        {
            anythingToDisplay = false;
        }
        if (anythingToDisplay)
        {
            m_ObjectInFrontBox.SetActive(true);
            m_ObjectInFrontText.text = message;
        }
        else
        {
            m_ObjectInFrontBox.SetActive(false);
            m_ObjectInFrontText.text = "";
        }
    }

    public void Retry()
    {
        StartOptions start = FindObjectOfType<StartOptions>();
        if (start)
        {
            start.sceneToStart = m_SceneIndex;
            start.menuSettingsData.musicLoopToChangeTo = m_SceneClip;
            start.StartButtonClicked();
        }
    }

}

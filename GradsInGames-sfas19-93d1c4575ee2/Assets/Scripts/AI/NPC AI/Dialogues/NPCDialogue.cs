using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "NPC Dialogue")]
public class NPCDialogue : ScriptableObject {

    [TextArea(14,10)] public string mText;

    public NPCDialogue nextDialogue;

}

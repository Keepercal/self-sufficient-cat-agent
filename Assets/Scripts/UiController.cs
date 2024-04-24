using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiController : MonoBehaviour
{

    public CatAgent agentScript;
    
    public TMP_Text healthText;
    public TMP_Text funText;
    public TMP_Text thirstText;
    public TMP_Text hungerText;
    public TMP_Text deathText;

    private float healthValue;
    private float funValue;
    private float thirstValue;
    private float hungerValue;
    private float deathValue;

    // Update is called once per frame
    void Update()
    {
        healthValue = agentScript.agentHealth;
        healthText.text = $"Health: {healthValue.ToString("F2")}";

        funValue = agentScript.agentFun;
        funText.text = $"Fun: {funValue.ToString("F2")}";

        thirstValue = agentScript.agentThirst;
        thirstText.text = $"Thirst: {thirstValue.ToString("F2")}";

        hungerValue = agentScript.agentHunger;
        hungerText.text = $"Hunger: {hungerValue.ToString("F2")}";

        deathValue = agentScript.numDeaths;
        deathText.text = $"Agent Deaths: {deathValue.ToString("F2")}";
    }
}

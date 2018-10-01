using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using cruelMemory;

public class cruelMemoryScript : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;

    public TextMesh screenDisplay;
    private int screenIndex = 0;
    public Material[] buttonMaterialOptions;
    public Button[] buttons;
    public Material strikingColour;

    private List<int> selectedMatIndex = new List<int>();
    private List<int> selectedLabelIndex = new List<int>();

    public Material[] stageLightOptions;
    public Renderer[] stageLights;

    public String[] materialNames;

    public List<int> allLabels = new List<int>();
    public List<String> allColours = new List<String>();

    public String correctColour = "";
    public int correctPosition = 0;
    public int correctLabel = 0;
    public List<int> correctStagePositions = new List<int>();
    public List<int> correctStageLabels = new List<int>();
    public List<String> correctStageColours = new List<String>();

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    public int stage = 1;
    private bool striking;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        foreach (Button button in buttons)
        {
            Button pressedButton = button;
            pressedButton.selectable.OnInteract += delegate () { OnButtonPress(pressedButton); return false; };
        }
    }

    void Start()
    {
        striking = true;
        foreach(Renderer stageLight in stageLights)
        {
            stageLight.material = stageLightOptions[0];
        }
        stageLights[0].material = stageLightOptions[1];
        SetUpButtons();
    }

    void SetUpButtons()
    {
        StartCoroutine(screenSelection());
        for(int i = 0; i <= 5; i++)
        {
            int matIndex = UnityEngine.Random.Range(0,6);
            while(selectedMatIndex.Contains(matIndex))
            {
              matIndex = UnityEngine.Random.Range(0,6);
            }
            selectedMatIndex.Add(matIndex);
            allColours.Add(materialNames[matIndex]);

            int labelIndex = UnityEngine.Random.Range(1,7);
            while(selectedLabelIndex.Contains(labelIndex))
            {
              labelIndex = UnityEngine.Random.Range(1,7);
            }
            selectedLabelIndex.Add(labelIndex);
            allLabels.Add(labelIndex);

            buttons[i].rend.material = buttonMaterialOptions[matIndex];
            buttons[i].label.text = labelIndex.ToString();
            buttons[i].labelName = labelIndex;
            buttons[i].colourName = materialNames[matIndex];
        }
        selectedLabelIndex.Clear();
        selectedMatIndex.Clear();
        Debug.LogFormat("[Horrible Memory #{0}] Stage {1} buttons.", moduleId, stage);
        foreach(Button button in buttons)
        {
            Debug.LogFormat("[Horrible Memory #{0}] Position {1}; Label: {2}; Colour: {3}.", moduleId, button.position, button.labelName, button.colourName);
        }
        CalculateAnswer();
    }

    IEnumerator screenSelection()
    {
        screenDisplay.text = "";
        screenIndex = UnityEngine.Random.Range(1,7);
        Debug.LogFormat("[Horrible Memory #{0}] The displayed number is {1}.", moduleId, screenIndex);
        yield return new WaitForSeconds(2f);
        screenDisplay.text = screenIndex.ToString();
        striking = false;
    }

    void CalculateAnswer()
    {
        if(stage == 1)
        {
            if(screenIndex == 1)
            {
                correctLabel = 6;
                correctPosition = 0;
                correctColour = "";
                Debug.LogFormat("[Horrible Memory #{0}] Press label 6.", moduleId);
            }
            else if(screenIndex == 2)
            {
                correctLabel = 0;
                correctPosition = 1;
                correctColour = "";
                Debug.LogFormat("[Horrible Memory #{0}] Press position 1.", moduleId);
            }
            else if(screenIndex == 3)
            {
                correctLabel = 0;
                correctPosition = 0;
                correctColour = "green";
                Debug.LogFormat("[Horrible Memory #{0}] Press green.", moduleId);
            }
            else if(screenIndex == 4)
            {
                correctLabel = 0;
                correctPosition = 3;
                correctColour = "";
                Debug.LogFormat("[Horrible Memory #{0}] Press position 3.", moduleId);
            }
            else if(screenIndex == 5)
            {
                correctLabel = 2;
                correctPosition = 0;
                correctColour = "";
                Debug.LogFormat("[Horrible Memory #{0}] Press label 2.", moduleId);
            }
            else if(screenIndex == 6)
            {
                correctLabel = 0;
                correctPosition = 0;
                correctColour = "orange";
                Debug.LogFormat("[Horrible Memory #{0}] Press orange.", moduleId);
            }
        }
        else if (stage == 2)
        {
            if(screenIndex == 1)
            {
                correctLabel = 0;
                correctPosition = correctStagePositions[0];
                correctColour = "";
                Debug.LogFormat("[Horrible Memory #{0}] Press position {1}.", moduleId, correctPosition);
            }
            else if(screenIndex == 2)
            {
                correctLabel = 0;
                correctPosition = 0;
                correctColour = "purple";
                Debug.LogFormat("[Horrible Memory #{0}] Press purple.", moduleId);
            }
            else if(screenIndex == 3)
            {
                correctLabel = 1;
                correctPosition = 0;
                correctColour = "";
                Debug.LogFormat("[Horrible Memory #{0}] Press label 1.", moduleId);
            }
            else if(screenIndex == 4)
            {
                correctLabel = correctStageLabels[0];
                correctPosition = 0;
                correctColour = "";
                Debug.LogFormat("[Horrible Memory #{0}] Press label {1}.", moduleId, correctLabel);
            }
            else if(screenIndex == 5)
            {
                correctLabel = 0;
                correctPosition = 6;
                correctColour = "";
                Debug.LogFormat("[Horrible Memory #{0}] Press position 6.", moduleId);
            }
            else if(screenIndex == 6)
            {
                correctLabel = 0;
                correctPosition = 0;
                correctColour = correctStageColours[0];
                Debug.LogFormat("[Horrible Memory #{0}] Press {1}.", moduleId, correctColour);
            }
        }
        else if (stage == 3)
        {
            if(screenIndex == 1)
            {
                correctLabel = allLabels[3];
                correctPosition = 0;
                correctColour = "";
                Debug.LogFormat("[Horrible Memory #{0}] Press label {1}.", moduleId, correctLabel);
            }
            else if(screenIndex == 2)
            {
                for(int i = 6; i <= 11; i++)
                {
                    if(allColours[i] == "green")
                    {
                        correctPosition = (i%6)+1;
                    }
                }
                correctLabel = 0;
                correctColour = "";
                Debug.LogFormat("[Horrible Memory #{0}] Press position {1}.", moduleId, correctPosition);
            }
            else if(screenIndex == 3)
            {
                for(int i = 6; i <= 11; i++)
                {
                    if(allLabels[i] == 5)
                    {
                        correctColour = allColours[i];
                    }
                }
                correctLabel = 0;
                correctPosition = 0;
                Debug.LogFormat("[Horrible Memory #{0}] Press {1}.", moduleId, correctColour);
            }
            else if(screenIndex == 4)
            {
                correctLabel = allLabels[0];
                correctPosition = 0;
                correctColour = "";
                Debug.LogFormat("[Horrible Memory #{0}] Press label {1}.", moduleId, correctLabel);
            }
            else if(screenIndex == 5)
            {
                correctLabel = 0;
                correctPosition = correctStagePositions[1];
                correctColour = "";
                Debug.LogFormat("[Horrible Memory #{0}] Press position {1}.", moduleId, correctPosition);
            }
            else if(screenIndex == 6)
            {
                correctLabel = 0;
                correctPosition = 0;
                correctColour = allColours[2];
                Debug.LogFormat("[Horrible Memory #{0}] Press {1}.", moduleId, correctColour);
            }
        }
        else if(stage == 4)
        {
            if(screenIndex == 1)
            {
                for(int i = 0; i <= 5; i++)
                {
                    if(allLabels[i] == 2)
                    {
                        correctPosition = i+1;
                    }
                }
                correctLabel = 0;
                correctColour = "";
                Debug.LogFormat("[Horrible Memory #{0}] Press position {1}.", moduleId, correctPosition);
            }
            else if(screenIndex == 2)
            {
                correctPosition = 0;
                correctLabel = allLabels[13];
                correctColour = "";
                Debug.LogFormat("[Horrible Memory #{0}] Press label {1}.", moduleId, correctLabel);
            }
            else if(screenIndex == 3)
            {
                correctColour = correctStageColours[1];
                correctLabel = 0;
                correctPosition = 0;
                Debug.LogFormat("[Horrible Memory #{0}] Press {1}.", moduleId, correctColour);
            }
            else if(screenIndex == 4)
            {
                correctLabel = 0;
                correctPosition = correctStagePositions[2];
                correctColour = "";
                Debug.LogFormat("[Horrible Memory #{0}] Press position {1}.", moduleId, correctPosition);
            }
            else if(screenIndex == 5)
            {
                for(int i = 0; i <= 5; i++)
                {
                    if(allLabels[i] == 4)
                    {
                        correctColour = allColours[i];
                    }
                }
                correctLabel = 0;
                correctPosition = 0;
                Debug.LogFormat("[Horrible Memory #{0}] Press {1}.", moduleId, correctColour);
            }
            else if(screenIndex == 6)
            {
                correctLabel = allLabels[17];
                correctPosition = 0;
                correctColour = "";
                Debug.LogFormat("[Horrible Memory #{0}] Press label {1}.", moduleId, correctLabel);
            }
        }
        else if(stage == 5)
        {
            if(screenIndex == 1)
            {
                correctPosition = 0;
                correctLabel = 0;
                correctColour = allColours[20];
                Debug.LogFormat("[Horrible Memory #{0}] Press {1}.", moduleId, correctColour);
            }
            else if(screenIndex == 2)
            {
                for(int i = 12; i <= 17; i++)
                {
                    if(allLabels[i] == 6)
                    {
                        correctPosition = (i%6)+1;
                    }
                }
                correctLabel = 0;
                correctColour = "";
                Debug.LogFormat("[Horrible Memory #{0}] Press position {1}.", moduleId, correctPosition);
            }
            else if(screenIndex == 3)
            {
                correctPosition = 0;
                correctLabel = correctStageLabels[3];
                correctColour = "";
                Debug.LogFormat("[Horrible Memory #{0}] Press label {1}.", moduleId, correctLabel);
            }
            else if(screenIndex == 4)
            {
                for(int i = 0; i <= 5; i++)
                {
                    if(allColours[i] == "red")
                    {
                        correctLabel = allLabels[i];
                    }
                }
                correctPosition = 0;
                correctColour = "";
                Debug.LogFormat("[Horrible Memory #{0}] Press label {1}.", moduleId, correctLabel);
            }
            else if(screenIndex == 5)
            {
                correctPosition = 0;
                correctLabel = 0;
                correctColour = correctStageColours[2];
                Debug.LogFormat("[Horrible Memory #{0}] Press {1}.", moduleId, correctColour);
            }
            else if(screenIndex == 6)
            {
                for(int i = 6; i <= 11; i++)
                {
                    if(allColours[i] == "blue")
                    {
                        correctPosition = (i%6)+1;
                    }
                }
                correctLabel = 0;
                correctColour = "";
                Debug.LogFormat("[Horrible Memory #{0}] Press position {1}.", moduleId, correctPosition);
            }
        }
    }

    public void OnButtonPress(Button pressedButton)
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        pressedButton.selectable.AddInteractionPunch();
        if(striking || moduleSolved)
        {
            return;
        }
        if((pressedButton.colourName == correctColour || pressedButton.position == correctPosition || pressedButton.labelName == correctLabel) && stage != 5)
        {
            striking = true;
            correctStagePositions.Add(pressedButton.position);
            correctStageLabels.Add(pressedButton.labelName);
            correctStageColours.Add(pressedButton.colourName);
            stageLights[stage-1].material = stageLightOptions[2];
            stageLights[stage].material = stageLightOptions[1];
            stage++;
            if(correctColour != "")
            {
                Debug.LogFormat("[Horrible Memory #{0}] You pressed {1}. That is correct.", moduleId, correctColour);
            }
            else if(correctLabel != 0)
            {
                Debug.LogFormat("[Horrible Memory #{0}] You pressed label {1}. That is correct.", moduleId, correctLabel);
            }
            else if (correctPosition != 0)
            {
                Debug.LogFormat("[Horrible Memory #{0}] You pressed position {1}. That is correct.", moduleId, correctPosition);
            }
            SetUpButtons();
        }
        else if(pressedButton.colourName == correctColour || pressedButton.position == correctPosition || pressedButton.labelName == correctLabel)
        {
            moduleSolved = true;
            stageLights[stage-1].material = stageLightOptions[2];
            correctStagePositions.Add(pressedButton.position);
            correctStageLabels.Add(pressedButton.labelName);
            correctStageColours.Add(pressedButton.colourName);
            stage = 0;
            if(correctColour != "")
            {
                Debug.LogFormat("[Horrible Memory #{0}] You pressed {1}. That is correct. Module disarmed.", moduleId, correctColour);
            }
            else if(correctLabel != 0)
            {
                Debug.LogFormat("[Horrible Memory #{0}] You pressed label {1}. That is correct. Module disarmed.", moduleId, correctLabel);
            }
            else if (correctPosition != 0)
            {
                Debug.LogFormat("[Horrible Memory #{0}] You pressed position {1}. That is correct. Module disarmed.", moduleId, correctPosition);
            }
            GetComponent<KMBombModule>().HandlePass();
        }
        else
        {
            GetComponent<KMBombModule>().HandleStrike();
            if(correctColour != "")
            {
                Debug.LogFormat("[Horrible Memory #{0}] Strike! You pressed {1}. That is incorrect.", moduleId, pressedButton.colourName);
            }
            else if(correctLabel != 0)
            {
                Debug.LogFormat("[Horrible Memory #{0}] Strike! You pressed label {1}. That is incorrect.", moduleId, pressedButton.labelName);
            }
            else if (correctPosition != 0)
            {
                Debug.LogFormat("[Cruel Memory #{0}] Strike! You pressed position {1}. That is incorrect.", moduleId, pressedButton.position);
            }
            striking = true;
            StartCoroutine(strikeLights());
        }
    }

    IEnumerator strikeLights()
    {
        foreach(Renderer stageLight in stageLights)
        {
            stageLight.material = stageLightOptions[3];
        }
        screenDisplay.text = "";
        foreach(Button button in buttons)
        {
            button.label.text = "";
            button.rend.material = strikingColour;
        }
        yield return new WaitForSeconds(2f);
        foreach(Renderer stageLight in stageLights)
        {
            stageLight.material = stageLightOptions[0];
        }
        stageLights[0].material = stageLightOptions[1];
        correctStageLabels.Clear();
        correctStageColours.Clear();
        correctStagePositions.Clear();
        allLabels.Clear();
        allColours.Clear();
        stage = 1;
        SetUpButtons();
    }

}

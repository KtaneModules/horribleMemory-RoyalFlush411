using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class cruelMemoryScript : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMRuleSeedable RuleSeedable;
    public KMColorblindMode ColorblindMode;

    public TextMesh screenDisplay;
    private int screenIndex = 0;
    public Material[] buttonMaterialOptions;
    public Button[] buttons;
    public Material strikingColour;

    private List<int> selectedMatIndex = new List<int>();
    private List<int> selectedLabelIndex = new List<int>();

    public Material[] stageLightOptions;
    public Renderer[] stageLights;

    public string[] materialNames;

    public List<int> allLabels = new List<int>();
    public List<string> allColours = new List<string>();

    public string correctColour = "";
    public int correctPosition = 0;
    public int correctLabel = 0;
    public List<int> correctStagePositions = new List<int>();
    public List<int> correctStageLabels = new List<int>();
    public List<string> correctStageColours = new List<string>();

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    public int stage = 1;
    private bool striking;
    private bool moduleSolved;
    private Rule[][] rules;
    private bool colorblindModeEnabled;

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
        foreach (Renderer stageLight in stageLights)
        {
            stageLight.material = stageLightOptions[0];
        }
        stageLights[0].material = stageLightOptions[1];

        var rnd = RuleSeedable.GetRNG();
        Debug.LogFormat("[Horrible Memory #{0}] Using rule seed: {1}", moduleId, rnd.Seed);
        if (rnd.Seed == 1)
            rules = null;
        else
        {
            rules = new Rule[5][];
            for (int stage = 0; stage < 5; stage++)
            {
                rules[stage] = new Rule[6];
                for (int displayNumber = 0; displayNumber < 6; displayNumber++)
                {
                    rules[stage][displayNumber] = stage == 0
                        ? new Rule { Type = (RuleType) rnd.Next(3), Parameter = rnd.Next(6) }
                        : new Rule { Type = (RuleType) rnd.Next(12), Parameter = rnd.Next(6), EarlierStage = rnd.Next(stage) };
                }
            }
        }

        colorblindModeEnabled = ColorblindMode.ColorblindModeActive;
        SetUpButtons();
    }

    void SetUpButtons()
    {
        StartCoroutine(screenSelection());
        for (int i = 0; i <= 5; i++)
        {
            int matIndex = UnityEngine.Random.Range(0, 6);
            while (selectedMatIndex.Contains(matIndex))
            {
                matIndex = UnityEngine.Random.Range(0, 6);
            }
            selectedMatIndex.Add(matIndex);
            allColours.Add(materialNames[matIndex]);

            int labelIndex = UnityEngine.Random.Range(1, 7);
            while (selectedLabelIndex.Contains(labelIndex))
            {
                labelIndex = UnityEngine.Random.Range(1, 7);
            }
            selectedLabelIndex.Add(labelIndex);
            allLabels.Add(labelIndex);

            buttons[i].rend.material = buttonMaterialOptions[matIndex];
            buttons[i].labelName = labelIndex;
            buttons[i].colourName = materialNames[matIndex];
        }
        setButtonLabels();
        selectedLabelIndex.Clear();
        selectedMatIndex.Clear();
        Debug.LogFormat("[Horrible Memory #{0}] Stage {1} buttons.", moduleId, stage);
        foreach (Button button in buttons)
        {
            Debug.LogFormat("[Horrible Memory #{0}] Position {1}; Label: {2}; Colour: {3}.", moduleId, button.position, button.labelName, button.colourName);
        }
        CalculateAnswer();
    }

    private void setButtonLabels()
    {
        for (int i = 0; i <= 5; i++)
            buttons[i].label.text = buttons[i].labelName.ToString() + (colorblindModeEnabled ? "\n" + (buttons[i].colourName == "pink" ? 'I' : char.ToUpperInvariant(buttons[i].colourName[0])) : null);
    }

    IEnumerator screenSelection()
    {
        screenDisplay.text = "";
        screenIndex = UnityEngine.Random.Range(1, 7);
        Debug.LogFormat("[Horrible Memory #{0}] The displayed number is {1}.", moduleId, screenIndex);
        yield return new WaitForSeconds(2f);
        screenDisplay.text = screenIndex.ToString();
        striking = false;
    }

    void CalculateAnswer()
    {
        if (rules == null)
        {
            // Follow the default rules (seed 1)
            if (stage == 1)
            {
                if (screenIndex == 1)
                {
                    correctLabel = 6;
                    correctPosition = 0;
                    correctColour = "";
                    Debug.LogFormat("[Horrible Memory #{0}] Press label 6.", moduleId);
                }
                else if (screenIndex == 2)
                {
                    correctLabel = 0;
                    correctPosition = 1;
                    correctColour = "";
                    Debug.LogFormat("[Horrible Memory #{0}] Press position 1.", moduleId);
                }
                else if (screenIndex == 3)
                {
                    correctLabel = 0;
                    correctPosition = 0;
                    correctColour = "green";
                    Debug.LogFormat("[Horrible Memory #{0}] Press green.", moduleId);
                }
                else if (screenIndex == 4)
                {
                    correctLabel = 0;
                    correctPosition = 3;
                    correctColour = "";
                    Debug.LogFormat("[Horrible Memory #{0}] Press position 3.", moduleId);
                }
                else if (screenIndex == 5)
                {
                    correctLabel = 2;
                    correctPosition = 0;
                    correctColour = "";
                    Debug.LogFormat("[Horrible Memory #{0}] Press label 2.", moduleId);
                }
                else if (screenIndex == 6)
                {
                    correctLabel = 0;
                    correctPosition = 0;
                    correctColour = "orange";
                    Debug.LogFormat("[Horrible Memory #{0}] Press orange.", moduleId);
                }
            }
            else if (stage == 2)
            {
                if (screenIndex == 1)
                {
                    correctLabel = 0;
                    correctPosition = correctStagePositions[0];
                    correctColour = "";
                    Debug.LogFormat("[Horrible Memory #{0}] Press position {1}.", moduleId, correctPosition);
                }
                else if (screenIndex == 2)
                {
                    correctLabel = 0;
                    correctPosition = 0;
                    correctColour = "purple";
                    Debug.LogFormat("[Horrible Memory #{0}] Press purple.", moduleId);
                }
                else if (screenIndex == 3)
                {
                    correctLabel = 1;
                    correctPosition = 0;
                    correctColour = "";
                    Debug.LogFormat("[Horrible Memory #{0}] Press label 1.", moduleId);
                }
                else if (screenIndex == 4)
                {
                    correctLabel = correctStageLabels[0];
                    correctPosition = 0;
                    correctColour = "";
                    Debug.LogFormat("[Horrible Memory #{0}] Press label {1}.", moduleId, correctLabel);
                }
                else if (screenIndex == 5)
                {
                    correctLabel = 0;
                    correctPosition = 6;
                    correctColour = "";
                    Debug.LogFormat("[Horrible Memory #{0}] Press position 6.", moduleId);
                }
                else if (screenIndex == 6)
                {
                    correctLabel = 0;
                    correctPosition = 0;
                    correctColour = correctStageColours[0];
                    Debug.LogFormat("[Horrible Memory #{0}] Press {1}.", moduleId, correctColour);
                }
            }
            else if (stage == 3)
            {
                if (screenIndex == 1)
                {
                    correctLabel = allLabels[3];
                    correctPosition = 0;
                    correctColour = "";
                    Debug.LogFormat("[Horrible Memory #{0}] Press label {1}.", moduleId, correctLabel);
                }
                else if (screenIndex == 2)
                {
                    for (int i = 6; i <= 11; i++)
                    {
                        if (allColours[i] == "green")
                        {
                            correctPosition = (i % 6) + 1;
                        }
                    }
                    correctLabel = 0;
                    correctColour = "";
                    Debug.LogFormat("[Horrible Memory #{0}] Press position {1}.", moduleId, correctPosition);
                }
                else if (screenIndex == 3)
                {
                    for (int i = 6; i <= 11; i++)
                    {
                        if (allLabels[i] == 5)
                        {
                            correctColour = allColours[i];
                        }
                    }
                    correctLabel = 0;
                    correctPosition = 0;
                    Debug.LogFormat("[Horrible Memory #{0}] Press {1}.", moduleId, correctColour);
                }
                else if (screenIndex == 4)
                {
                    correctLabel = allLabels[0];
                    correctPosition = 0;
                    correctColour = "";
                    Debug.LogFormat("[Horrible Memory #{0}] Press label {1}.", moduleId, correctLabel);
                }
                else if (screenIndex == 5)
                {
                    correctLabel = 0;
                    correctPosition = correctStagePositions[1];
                    correctColour = "";
                    Debug.LogFormat("[Horrible Memory #{0}] Press position {1}.", moduleId, correctPosition);
                }
                else if (screenIndex == 6)
                {
                    correctLabel = 0;
                    correctPosition = 0;
                    correctColour = allColours[2];
                    Debug.LogFormat("[Horrible Memory #{0}] Press {1}.", moduleId, correctColour);
                }
            }
            else if (stage == 4)
            {
                if (screenIndex == 1)
                {
                    for (int i = 0; i <= 5; i++)
                    {
                        if (allLabels[i] == 2)
                        {
                            correctPosition = i + 1;
                        }
                    }
                    correctLabel = 0;
                    correctColour = "";
                    Debug.LogFormat("[Horrible Memory #{0}] Press position {1}.", moduleId, correctPosition);
                }
                else if (screenIndex == 2)
                {
                    correctPosition = 0;
                    correctLabel = allLabels[13];
                    correctColour = "";
                    Debug.LogFormat("[Horrible Memory #{0}] Press label {1}.", moduleId, correctLabel);
                }
                else if (screenIndex == 3)
                {
                    correctColour = correctStageColours[1];
                    correctLabel = 0;
                    correctPosition = 0;
                    Debug.LogFormat("[Horrible Memory #{0}] Press {1}.", moduleId, correctColour);
                }
                else if (screenIndex == 4)
                {
                    correctLabel = 0;
                    correctPosition = correctStagePositions[2];
                    correctColour = "";
                    Debug.LogFormat("[Horrible Memory #{0}] Press position {1}.", moduleId, correctPosition);
                }
                else if (screenIndex == 5)
                {
                    for (int i = 0; i <= 5; i++)
                    {
                        if (allLabels[i] == 4)
                        {
                            correctColour = allColours[i];
                        }
                    }
                    correctLabel = 0;
                    correctPosition = 0;
                    Debug.LogFormat("[Horrible Memory #{0}] Press {1}.", moduleId, correctColour);
                }
                else if (screenIndex == 6)
                {
                    correctLabel = allLabels[17];
                    correctPosition = 0;
                    correctColour = "";
                    Debug.LogFormat("[Horrible Memory #{0}] Press label {1}.", moduleId, correctLabel);
                }
            }
            else if (stage == 5)
            {
                if (screenIndex == 1)
                {
                    correctPosition = 0;
                    correctLabel = 0;
                    correctColour = allColours[20];
                    Debug.LogFormat("[Horrible Memory #{0}] Press {1}.", moduleId, correctColour);
                }
                else if (screenIndex == 2)
                {
                    for (int i = 12; i <= 17; i++)
                    {
                        if (allLabels[i] == 6)
                        {
                            correctPosition = (i % 6) + 1;
                        }
                    }
                    correctLabel = 0;
                    correctColour = "";
                    Debug.LogFormat("[Horrible Memory #{0}] Press position {1}.", moduleId, correctPosition);
                }
                else if (screenIndex == 3)
                {
                    correctPosition = 0;
                    correctLabel = correctStageLabels[3];
                    correctColour = "";
                    Debug.LogFormat("[Horrible Memory #{0}] Press label {1}.", moduleId, correctLabel);
                }
                else if (screenIndex == 4)
                {
                    for (int i = 0; i <= 5; i++)
                    {
                        if (allColours[i] == "red")
                        {
                            correctLabel = allLabels[i];
                        }
                    }
                    correctPosition = 0;
                    correctColour = "";
                    Debug.LogFormat("[Horrible Memory #{0}] Press label {1}.", moduleId, correctLabel);
                }
                else if (screenIndex == 5)
                {
                    correctPosition = 0;
                    correctLabel = 0;
                    correctColour = correctStageColours[2];
                    Debug.LogFormat("[Horrible Memory #{0}] Press {1}.", moduleId, correctColour);
                }
                else if (screenIndex == 6)
                {
                    for (int i = 6; i <= 11; i++)
                    {
                        if (allColours[i] == "blue")
                        {
                            correctPosition = (i % 6) + 1;
                        }
                    }
                    correctLabel = 0;
                    correctColour = "";
                    Debug.LogFormat("[Horrible Memory #{0}] Press position {1}.", moduleId, correctPosition);
                }
            }
        }
        else
        {
            var ruleSet = rules[stage - 1];
            var rule = ruleSet[screenIndex - 1];
            correctLabel = 0;
            correctPosition = 0;
            correctColour = "";
            var colourNames = "blue,green,orange,pink,purple,red".Split(',');
            switch (rule.Type)
            {
                case RuleType.SimpleLabel: correctLabel = rule.Parameter + 1; break;
                case RuleType.SimpleColour: correctColour = colourNames[rule.Parameter]; break;
                case RuleType.SimplePosition: correctPosition = rule.Parameter + 1; break;

                case RuleType.EarlierPressedLabel: correctLabel = correctStageLabels[rule.EarlierStage]; break;
                case RuleType.EarlierPressedColour: correctColour = correctStageColours[rule.EarlierStage]; break;
                case RuleType.EarlierPressedPosition: correctPosition = correctStagePositions[rule.EarlierStage]; break;

                case RuleType.EarlierLabelByColour:
                    for (int i = 6 * rule.EarlierStage; i < 6 * (rule.EarlierStage + 1); i++)
                        if (allColours[i] == colourNames[rule.Parameter])
                            correctLabel = allLabels[i];
                    break;

                case RuleType.EarlierLabelByPosition:
                    correctLabel = allLabels[6 * rule.EarlierStage + rule.Parameter];
                    break;

                case RuleType.EarlierColourByLabel:
                    for (int i = 6 * rule.EarlierStage; i < 6 * (rule.EarlierStage + 1); i++)
                        if (allLabels[i] == rule.Parameter + 1)
                            correctColour = allColours[i];
                    break;

                case RuleType.EarlierColourByPosition:
                    correctColour = allColours[6 * rule.EarlierStage + rule.Parameter];
                    break;

                case RuleType.EarlierPositionByColour:
                    for (int i = 6 * rule.EarlierStage; i < 6 * (rule.EarlierStage + 1); i++)
                        if (allColours[i] == colourNames[rule.Parameter])
                            correctPosition = (i % 6) + 1;
                    break;

                case RuleType.EarlierPositionByLabel:
                    for (int i = 6 * rule.EarlierStage; i < 6 * (rule.EarlierStage + 1); i++)
                        if (allLabels[i] == rule.Parameter + 1)
                            correctPosition = (i % 6) + 1;
                    break;
            }

            if (correctPosition != 0)
                Debug.LogFormat("[Horrible Memory #{0}] Press position {1}.", moduleId, correctPosition);
            else if (correctLabel != 0)
                Debug.LogFormat("[Horrible Memory #{0}] Press label {1}.", moduleId, correctLabel);
            else if (correctColour != "")
                Debug.LogFormat("[Horrible Memory #{0}] Press colour {1}.", moduleId, correctColour);
            else
                Debug.LogFormat("[Horrible Memory #{0}] Error in rule-seed! Please contact Timwi about this bug.", moduleId);
        }
    }

    public void OnButtonPress(Button pressedButton)
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        pressedButton.selectable.AddInteractionPunch();
        if (striking || moduleSolved)
        {
            return;
        }
        if ((pressedButton.colourName == correctColour || pressedButton.position == correctPosition || pressedButton.labelName == correctLabel) && stage != 5)
        {
            striking = true;
            correctStagePositions.Add(pressedButton.position);
            correctStageLabels.Add(pressedButton.labelName);
            correctStageColours.Add(pressedButton.colourName);
            stageLights[stage - 1].material = stageLightOptions[2];
            stageLights[stage].material = stageLightOptions[1];
            stage++;
            if (correctColour != "")
            {
                Debug.LogFormat("[Horrible Memory #{0}] You pressed {1}. That is correct.", moduleId, correctColour);
            }
            else if (correctLabel != 0)
            {
                Debug.LogFormat("[Horrible Memory #{0}] You pressed label {1}. That is correct.", moduleId, correctLabel);
            }
            else if (correctPosition != 0)
            {
                Debug.LogFormat("[Horrible Memory #{0}] You pressed position {1}. That is correct.", moduleId, correctPosition);
            }
            SetUpButtons();
        }
        else if (pressedButton.colourName == correctColour || pressedButton.position == correctPosition || pressedButton.labelName == correctLabel)
        {
            moduleSolved = true;
            stageLights[stage - 1].material = stageLightOptions[2];
            correctStagePositions.Add(pressedButton.position);
            correctStageLabels.Add(pressedButton.labelName);
            correctStageColours.Add(pressedButton.colourName);
            stage = 0;
            if (correctColour != "")
            {
                Debug.LogFormat("[Horrible Memory #{0}] You pressed {1}. That is correct. Module disarmed.", moduleId, correctColour);
            }
            else if (correctLabel != 0)
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
            if (correctColour != "")
            {
                Debug.LogFormat("[Horrible Memory #{0}] Strike! You pressed {1}. That is incorrect.", moduleId, pressedButton.colourName);
            }
            else if (correctLabel != 0)
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
        foreach (Renderer stageLight in stageLights)
        {
            stageLight.material = stageLightOptions[3];
        }
        screenDisplay.text = "";
        foreach (Button button in buttons)
        {
            button.label.text = "";
            button.rend.material = strikingColour;
        }
        yield return new WaitForSeconds(2f);
        foreach (Renderer stageLight in stageLights)
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

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} position 2, !{0} pos 2, !{0} p 2 [2nd position] | !{0} label 3, !{0} lab 3, !{0} l 3 [label 3] | !{0} colour blue, !{0} colour b, !{0} col b, !{0} c b [colour blue] [b=blue, g=green, r=red, o=orange, p=purple, i=pink] | !{0} colorblind [enable colorblind mode]";
#pragma warning restore 414

    KMSelectable[] ProcessTwitchCommand(string command)
    {
        if (Regex.IsMatch(command, @"^\s*colorblind\s*$", RegexOptions.IgnoreCase))
        {
            colorblindModeEnabled = true;
            setButtonLabels();
            return new KMSelectable[0];
        }

        var match = Regex.Match(command, @"^\s*(?:p|pos|position)\s+([1-6])\s*$", RegexOptions.IgnoreCase);
        if (match.Success)
            return new[] { buttons[int.Parse(match.Groups[1].Value) - 1].selectable };

        match = Regex.Match(command, @"^\s*(?:c|col|color|colour)\s+(b|blue|g|green|r|red|o|orange|p|purple|i|pink)\s*$", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            var color = match.Groups[1].Value;
            switch (color)
            {
                case "b": color = "blue"; break;
                case "g": color = "green"; break;
                case "r": color = "red"; break;
                case "o": color = "orange"; break;
                case "p": color = "purple"; break;
                case "i": color = "pink"; break;
            }
            return new[] { buttons.First(b => b.colourName == color).selectable };
        }

        match = Regex.Match(command, @"^\s*(?:l|lab|label)\s+([1-6])\s*$", RegexOptions.IgnoreCase);
        if (match.Success)
            return new[] { buttons.First(b => b.labelName == int.Parse(match.Groups[1].Value)).selectable };

        return null;
    }
}

enum RuleType
{
    SimpleLabel,     // “press label 6”
    SimpleColour,     // “press the blue button”
    SimplePosition, // “press the 4th button”
    EarlierPressedLabel,        // “press the button with the same label as the one you pressed in Stage 1”
    EarlierPressedColour,        // “press the button with the same colour as the one you pressed in Stage 1”
    EarlierPressedPosition,    // “press the button in the same position as the one you pressed in Stage 1”
    EarlierLabelByColour,        // “press the button with the same label as the one that was blue in Stage 1”
    EarlierLabelByPosition,    // “press the button with the same label as the one that was in position 2 in Stage 1”
    EarlierColourByLabel,        // “press the button with the same colour as the one that had the label 3 in Stage 1”
    EarlierColourByPosition,    // “press the button with the same colour as the one that was in position 2 in Stage 1”
    EarlierPositionByColour,    // “press the button in the same position as the one that was blue in Stage 1”
    EarlierPositionByLabel,    // “press the button in the same position as the one that had the label 3 in Stage 1”
}

sealed class Rule
{
    public RuleType Type;
    public int EarlierStage;
    public int Parameter;   // the label/colour/position the button had in the earlier stage
}

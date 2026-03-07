using System;
using UnityEngine;

[Serializable]
public class ChoiceData
{
    [TextArea] public string choiceText;

    public int goldEffect;
    public int intelligenceEffect;
    public int respectEffect;
}
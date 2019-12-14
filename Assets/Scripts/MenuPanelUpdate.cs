using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanelUpdate : MonoBehaviour
{
    // Symbol to turn textbox when Instruciton is complete
    public Sprite DoneSymbol;
    // Symbol to show when instruction not complete
    private Sprite InitSymbol;
    private Vector3 InitScale;
    //public Sprite InstructionSymbol;
    // Starting instruction symbol
    private SpriteRenderer Instruction;

    private float targetHeight;

    void Start()
    {
        Instruction = GetComponentInChildren<SpriteRenderer>();
        InitSymbol = Instruction.sprite;
        targetHeight = Instruction.sprite.rect.height;
        InitScale = Instruction.transform.localScale;

    }
    public void InstructionComplete()
    {
        Instruction.sprite = DoneSymbol;
        float factor = Instruction.transform.localScale.y  * (targetHeight / DoneSymbol.rect.height);
        Instruction.transform.localScale = new Vector3(factor,factor,factor);
    }

    public void InstructionReset()
    {
        Instruction.sprite = InitSymbol;
        Instruction.transform.localScale = InitScale;
    }
}

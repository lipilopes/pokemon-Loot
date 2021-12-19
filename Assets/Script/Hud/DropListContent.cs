using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DropListContent : MonoBehaviour
{
    [SerializeField] protected Image              dropListExampleBoarder;
    [SerializeField] protected Image              dropListExampleBackground;
    [SerializeField] protected Image              dropListExampleRegister;    
    [SerializeField] protected Image              dropListExampleGender;
    [SerializeField] protected GameObject         dropListExampleShiny;
    [SerializeField] protected GameObject         dropListExampleShinyFront;
    [SerializeField] protected TextMeshProUGUI    dropListExampleName;
    [SerializeField] protected Image              dropListExamplePokemon;

    public void AddPokemon(Sprite bg,Sprite gender,Sprite pokemon,string IdName,bool registered,Color borderColor,bool shiny)
    {
        dropListExampleBoarder.color        = borderColor;

        dropListExampleRegister.enabled     = !registered;
        dropListExampleShiny.SetActive(shiny);
        dropListExampleShinyFront.SetActive(shiny);

        

        dropListExampleBackground.sprite    = bg;
        dropListExampleGender.sprite        = gender;
        dropListExamplePokemon.sprite       = pokemon;
        dropListExampleName.color           = borderColor;
        dropListExampleName.text            = (shiny ? "<color=yellow>"+IdName.ToUpper()+"</color>" : IdName.ToUpper());

        this.gameObject.SetActive(true);
    }
}

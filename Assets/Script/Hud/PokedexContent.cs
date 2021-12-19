using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PokedexContent : DropListContent
{
    [SerializeField] protected TextMeshProUGUI    dropListExampleAmount;
    [SerializeField] protected Button             dropListButton;

    public Button Button{ get {return dropListButton;}}

    bool register = false;

    public void AddPokemonPokedex(Sprite bg,Sprite gender,Sprite pokemon,string IdName,bool registered,Color borderColor,bool shiny)
    {      
        if(registered)
            register = true;
        else
        if(register)
            return;


        dropListExampleBoarder.color        = registered ? borderColor: Color.black;

        dropListExamplePokemon.color        = registered ? Color.white : Color.black;
        dropListExampleShiny.SetActive(registered ? shiny : false);
        dropListExampleShinyFront.SetActive(registered ?shiny: false);

        dropListExampleGender.enabled = registered;

        dropListExampleBackground.sprite    = bg;
        dropListExampleGender.sprite        = gender;
        dropListExamplePokemon.sprite       = pokemon;
        dropListExampleName.color           = registered ? borderColor : Color.black;
        dropListExampleName.text            = registered ? (shiny ? "<color=yellow>"+IdName.ToUpper()+"</color>" : IdName.ToUpper()) : "???";

        this.gameObject.SetActive(true);
    }

    public void UpdateAmount(int amount)
    {
        dropListExampleAmount.gameObject.SetActive(amount > 1);
        dropListExampleAmount.text = "x"+amount;
    }
}

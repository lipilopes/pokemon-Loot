using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;

public class HudPokedex : MonoBehaviour
{
    public static HudPokedex Instance;
    [SerializeField] GameObject         pokedexGo;
    [SerializeField] GameObject         pokedexContent;
    [SerializeField] GameObject         pokedexExample;
    [SerializeField] TextMeshProUGUI    pokedexCompleteText;
    [SerializeField] TextMeshProUGUI    pokedexShinyText;
    public List<GameObject>                    pokedexExamplePool      =   new List<GameObject>();
    bool                                firstLoadPokedex        =   false;
    [Header("Pokedex Scene - Detail")]
    public GameObject         pokedexDetailGo;
    [SerializeField] TextMeshProUGUI    pokedexDetailNameText;
    [SerializeField] TextMeshProUGUI    pokedexDetailDescriptionText;
    public TextMeshProUGUI    pokedexDetailAmountText;
    [SerializeField] Image              pokedexDetailBgImage;
    [SerializeField] Image              pokedexDetailPokemonImage;
    [SerializeField] Image              pokedexDetailGenderImage;
    [SerializeField] Image              pokedexDetailEvolveImage;
    [SerializeField] Button             pokedexDetailAlolanFormButton;
    [SerializeField] Button             pokedexDetailMaleButton;
    [SerializeField] Button             pokedexDetailFemaleButton;
    [SerializeField] Button             pokedexDetailShinyButton;
    [SerializeField] Button             pokedexDetailEvolveButton;
    public LootScriptable     pokedexDetailSelect;
    public bool               pokedexDetailShinyTrigger  = false;
    public bool               pokedexDetailGenderTrigger = false;
    public bool               pokedexDetailAlolanFormTrigger = false;

    Pokedex pdx;

    private void Start() 
    {
        pdx  = Pokedex.Instance;
    }

     private void Awake() 
    {
         if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);       
    }

    public void OpenPokedex(bool open=true)
    {
        HudManager.Instance.tooltipPokedexCanActive = !open;

            if(open)
            {
                HudManager.Instance.UpdateCurrentScene = CurrentScene.PokedexScene;

                pokedexContent.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                    
                if(!firstLoadPokedex)
                    FirstLoadPokedex();
                else
                    UpdateAmountPokedex();
            }
            else
                HudManager.Instance.UpdateCurrentScene = CurrentScene.MainScene;

        HudManager.Instance.DisablePokeballAndPokemonMainScreen(open);
        pokedexGo.SetActive(open);
    }

    public void UpdatePokedex(LootScriptable  pk)
    {
        if(!firstLoadPokedex)
            FirstLoadPokedex();

        int             i   =   pk.id; 

        PokedexContent pkC  =   pokedexExamplePool[i-1].GetComponent<PokedexContent>(); 

         int male   = pdx.GetTotalCatchesGender(pk, Gender.Male,   true, true, true);  
         int female = pdx.GetTotalCatchesGender(pk, Gender.Female, true, true, true);

         pk.gender = female > male ? Gender.Female : Gender.Male;

        bool shiny  = pdx.GetTotalCatches(pk,onlyShiny: true) > 0;

        bool register = pdx.GetPokemonRegistered(pk);

        string id = "#"+(pk.id < 10 ? "0" : "")+(pk.id < 100 ? "0" : "")+pk.id;

        string name = id+" "+pk.Name;

        Color color = HudManager.Instance.LootTypeColors[(int)pdx.GetLootDrop(pk).rarity];

         pkC.AddPokemonPokedex
         (
             HudManager.Instance.backgroundTypes[(int)pk.types[0]],
             HudManager.Instance.genders[(int)pk.gender],
             pk.GetSprite(pk.gender,shiny),
             name,
             register,
             color,
             shiny       
         );

        pkC.UpdateAmount(pdx.GetTotalCatches(pk,true,form: true));

        
        #if UNITY_EDITOR
            pkC.gameObject.name = (shiny? "*" :"")+pk;
        #endif
    }

    public void UpdateAmountPokedex()
    {
        if(!firstLoadPokedex)
            FirstLoadPokedex();

        pokedexCompleteText.text    = "Complete: <b>"+pdx.GetNumberCompleteDex()+"</b> / <b>"+pdx.GetMaxIndex+"</b>";
        pokedexShinyText.text       = "Shiny: <b>"+pdx.GetNumberCompleteShinyDex()+"</b> / <b>"+pdx.GetMaxIndex+"</b>";

        int count = pdx.GetAllPokemons.Count;
        for (int p = 0; p < count; p++)
        {   
            LootDrop        ld  =   pdx.GetAllPokemons[p];
            LootScriptable  pk  =   ld.loot;
                    
            PokedexContent pkC  =   pokedexExamplePool[pk.id-1].GetComponent<PokedexContent>();
            pkC.UpdateAmount(pdx.GetTotalCatches(pk,true,form: true));   
        }
    }

    public void FirstLoadPokedex()
    {
        firstLoadPokedex = true;

        int pCount = pdx.GetMaxIndex;
        
        HudManager.Instance.GetDropListPool(pokedexExample,false,pCount,pokedexContent.transform); 
                
        float size = (float)pCount/3;           
        pokedexContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 521 * (size));
                                           
        int count = pdx.GetAllPokemons.Count;
        for (int p = 0; p < count; p++)
        {
            LootDrop        ld  =   pdx.GetAllPokemons[p];
            LootScriptable  pk  =   ld.loot;
            UpdatePokedex(pk);

            PokedexContent pkC  =   pokedexExamplePool[pk.id-1].GetComponent<PokedexContent>();
            pkC.Button.onClick.AddListener(() => LoadPokemonDetail(pk));       
        }
               
        pokedexDetailMaleButton.onClick.AddListener(() => UpdatePokemonImagePokedexDetail(true));
        pokedexDetailFemaleButton.onClick.AddListener(() => UpdatePokemonImagePokedexDetail(false));
        pokedexDetailShinyButton.onClick.AddListener(() => ShinyTriggerPokedexDetail());

        pokedexCompleteText.text    = "Complete: <b>"+pdx.GetNumberCompleteDex()+"</b> / <b>"+pdx.GetMaxIndex+"</b>";
        pokedexShinyText.text       = "Shiny: <b>"+pdx.GetNumberCompleteShinyDex()+"</b> / <b>"+pdx.GetMaxIndex+"</b>";       
    }


     #region Pokedex Detail
     //------------ Pokedex Detail-------------
    public Gender PokemonDetailGender()
    {
        if(pokedexDetailSelect == null)
            return Gender.Male;

        Gender gender = Gender.Unknown;

        if(pokedexDetailSelect.genderRatio != GenderRatio.Genderless)
            gender = pokedexDetailGenderTrigger ? Gender.Male : Gender.Female;

        pokedexDetailSelect.gender = gender;

        return gender;       
    }

    public void OpenPokedexDetail(bool open=true)
    {
        pokedexDetailGo.SetActive(open);

        HudManager.Instance.UpdateCurrentScene =  open ? CurrentScene.DetailPokedexScene : CurrentScene.PokedexScene;
    }

    public void UpdatePokemonImagePokedexDetail(bool male)
    {
        if(pokedexDetailGenderTrigger != male)
            pokedexDetailGenderTrigger = male;

        Gender gender = PokemonDetailGender();

        bool shiny = pokedexDetailShinyTrigger;

        if(pokedexDetailSelect.genderRatio != GenderRatio.Genderless)
            gender = male ? Gender.Male : Gender.Female;

        int amount = pdx.GetIntNamePlayerPrefs(pokedexDetailSelect,gender,shiny);

        if(shiny)
        {
            shiny = amount > 0;
            pokedexDetailShinyButton.gameObject.SetActive(shiny); 
        }

        pokedexDetailPokemonImage.sprite    =    pokedexDetailSelect.GetSprite(gender, shiny);
        pokedexDetailGenderImage.sprite     =    HudManager.Instance.genders[(int)gender];

        
        pokedexDetailAmountText.gameObject.SetActive(amount > 1);
        pokedexDetailAmountText.text = "x"+amount;

        UpdatePokemonDetailCanEvolved();

        UpdatePokemonDetailButtons();   
    }

    public void ShinyTriggerPokedexDetail()
    {
        pokedexDetailShinyTrigger = !pokedexDetailShinyTrigger;  
        pokedexDetailSelect.shiny = pokedexDetailShinyTrigger;      

        UpdatePokemonImagePokedexDetail(pokedexDetailGenderTrigger);
    }

    public void UpdateFormPokemonImagePokedexDetail()
    {
        pokedexDetailAlolanFormTrigger = !pokedexDetailAlolanFormTrigger;

        LootScriptable pkSwitch = pdx.GetPokemon(pokedexDetailSelect.id,pokedexDetailAlolanFormTrigger ? Form.Normal : Form.Alolan);
        LootScriptable pk       = pdx.GetPokemon(pokedexDetailSelect.id,pokedexDetailAlolanFormTrigger ? Form.Alolan : Form.Normal);
        
        pokedexDetailAlolanFormButton.GetComponent<Image>().sprite = pkSwitch.GetSprite(Gender.Male,pokedexDetailShinyTrigger);

        pokedexDetailSelect = pk;

        pokedexDetailDescriptionText.text   = pk.description;

        UpdatePokemonImagePokedexDetail(pokedexDetailGenderTrigger);
    }

    public void LoadPokemonDetail(LootScriptable pk)
    {
        bool registered = pdx.GetPokemonRegistered(pk);

        if(!registered)
            return;      

        pokedexDetailSelect = pk;

        OpenPokedexDetail(true);

        bool male   = pdx.GetIntNamePlayerPrefs(pk, Gender.Male,   pokedexDetailShinyTrigger)   > 0;  
        bool female = pdx.GetIntNamePlayerPrefs(pk, Gender.Female, pokedexDetailShinyTrigger) > 0;

        if(pk.genderRatio == GenderRatio.Genderless)
            pokedexDetailGenderTrigger = true;
        else
        if(pk.gender == Gender.Male)
            pokedexDetailGenderTrigger = true;
        else
            pokedexDetailGenderTrigger = false;

        Gender gender = PokemonDetailGender();

        bool shiny  = pdx.GetIntNamePlayerPrefs(pk, gender,        true) > 0;
        bool alolan = pdx.GetIntNamePlayerPrefs(pk, gender,pokedexDetailShinyTrigger, Form.Alolan) > 0;
        int amount  = pdx.GetIntNamePlayerPrefs(pk); 

        if(!shiny)
            pokedexDetailShinyTrigger = false;

        string name = "#"+(pk.id < 10 ? "0" : "")+(pk.id < 100 ? "0" : "")+pk.id+" "+pk.Name;

        pokedexDetailBgImage.sprite         = HudManager.Instance.backgroundTypes[(int)pk.types[0]];

        pokedexDetailPokemonImage.color     = Color.white;
        pokedexDetailPokemonImage.sprite    = pk.GetSprite(pk.gender, pokedexDetailShinyTrigger);
        
        pokedexDetailNameText.color         = HudManager.Instance.LootTypeColors[(int)pdx.GetLootDrop(pk).rarity];
        pokedexDetailNameText.text          = name.ToUpper();

        pokedexDetailDescriptionText.text   = pk.description;

        pokedexDetailGenderImage.enabled    = registered;
        pokedexDetailGenderImage.sprite     = HudManager.Instance.genders[(int)pk.gender];   

        UpdatePokemonDetailButtons();

        pokedexDetailAmountText.gameObject.SetActive(amount > 1);
        pokedexDetailAmountText.text = "x"+amount;
        
        if(alolan)
        {
            pokedexDetailAlolanFormButton.GetComponent<Image>().sprite = pdx.GetPokemon(pk.Name,pk.form == Form.Normal ? Form.Alolan : Form.Normal).GetSprite(pk.gender,pokedexDetailShinyTrigger);
            pokedexDetailAlolanFormButton.gameObject.SetActive(alolan && pdx.GetIntNamePlayerPrefs(pk,Gender.Female,form: pk.form == Form.Normal ? Form.Alolan : Form.Normal) > 0);
        }
        else
            pokedexDetailAlolanFormButton.gameObject.SetActive(false);

        UpdatePokemonDetailCanEvolved();
    }

    private void UpdatePokemonDetailButtons()
    {
        Gender gender = PokemonDetailGender();

        bool m = false;
        bool f = false;
        bool s = false;

        if(pokedexDetailSelect.genderRatio == GenderRatio.OnlyFemale)
        {
            f = true;
        }
        else
        if(pokedexDetailSelect.genderRatio == GenderRatio.OnlyMale || pokedexDetailSelect.genderRatio == GenderRatio.Genderless)
        {
            m = true;
        }
        else
        {
            m   = pokedexDetailGenderTrigger ? false :     pdx.GetIntNamePlayerPrefs(pokedexDetailSelect, Gender.Male,   pokedexDetailShinyTrigger)    > 0;  
            f   = pokedexDetailGenderTrigger ?             pdx.GetIntNamePlayerPrefs(pokedexDetailSelect, Gender.Female, pokedexDetailShinyTrigger)    > 0 : false;     
        }

        s   =   pdx.GetIntNamePlayerPrefs(pokedexDetailSelect, gender,        true)     > 0;

        pokedexDetailMaleButton.gameObject.SetActive(m);        
        pokedexDetailFemaleButton.gameObject.SetActive(f);
        pokedexDetailShinyButton.gameObject.SetActive(s);
    }

    private void UpdatePokemonDetailCanEvolved()
    {
        if(pdx.CanEvolved(pokedexDetailSelect))
        {
            pokedexDetailEvolveButton.gameObject.SetActive(true);
            pokedexDetailEvolveButton.onClick.AddListener(() => HudManager.Instance.OpenEvolutionScene(pokedexDetailSelect));
            pokedexDetailEvolveImage.sprite = pokedexDetailSelect.GetSpriteNextEvolution(pokedexDetailSelect.gender, pokedexDetailShinyTrigger);
            pokedexDetailEvolveImage.color  = pdx.GetPokemonRegistered(pokedexDetailSelect.nextEvolution) ? Color.white : Color.black;
        }
        else
            pokedexDetailEvolveButton.gameObject.SetActive(false);
    }
    #endregion
}

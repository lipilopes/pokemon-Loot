using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;

#if UNITY_EDITOR
    using UnityEditor;
#endif


[System.Serializable] 
public class TooltipPokedexLine 
{ 
    public Sprite img;
    public string name;
    public Gender gender;
    public bool shiny;
}

[System.Serializable] 
public enum CurrentScene
{
    MainScene,
    LootPokemonScene,
    DropListScene,
    PokedexScene,
    DetailPokedexScene,
    EvolveScene 
}



public class HudManager : MonoBehaviour
{
    public static HudManager Instance;
    [Header("Main Screen")]
    [SerializeField] CanvasGroup        mainScreenCanvasGroup;
    [SerializeField] SpriteRenderer     mainPokemon;
    /// <summary>Use disable</summary>
    [SerializeField] Image              registerIconMainPokemon;
    /// <summary>Use genders[Male,Female,Unknown]</summary>
    [SerializeField] Image              genderIconMainPokemon;
    [SerializeField] TextMeshProUGUI    nameMainPokemon;
    [SerializeField] Toggle             skipAnimToggle;
    [SerializeField] TextMeshProUGUI    LootCountText;
    [SerializeField] Button             caughtButton;
    [SerializeField] Image              mainBackgroundImage;
    [Header("Tooltip Pokedex")]
    [SerializeField] Animator           tooltipPokedexAnim;
    [SerializeField] Image              tooltipPokedexPokemonImage;
    [SerializeField] TextMeshProUGUI    tooltipPokedexText;
    /// <summary>Use genders[Male,Female,Unknown]</summary>
    [SerializeField] Image              tooltipPokedexGenderImage;
                    bool                tooltipPokedexActive        =   false;
    public          bool                tooltipPokedexCanActive     =   true;
    List<TooltipPokedexLine>            tooltipPokedexLine          =   new List<TooltipPokedexLine>();
    [Header("Drop List")]
    [SerializeField] GameObject         dropListGo;
    [SerializeField] GameObject         dropListContent;
    [SerializeField] GameObject         dropListExample;
    [SerializeField] TextMeshProUGUI    dropListDetailCommon;
    [SerializeField] TextMeshProUGUI    dropListDetailRare;
    [SerializeField] TextMeshProUGUI    dropListDetailEpic;
    [SerializeField] TextMeshProUGUI    dropListDetailLegendary;
    [SerializeField] TextMeshProUGUI    dropListDetailShiny;
    List<LootScriptable>                dropListPokemons        =   new List<LootScriptable>();
    List<GameObject>                    dropListExamplePool     =   new List<GameObject>();
    [Header("Pokedex Scene")]
    [SerializeField] GameObject         pokedexGo;
    [SerializeField] GameObject         pokedexContent;
    [SerializeField] GameObject         pokedexExample;
    [SerializeField] TextMeshProUGUI    pokedexCompleteText;
    [SerializeField] TextMeshProUGUI    pokedexShinyText;
    List<GameObject>                    pokedexExamplePool      =   new List<GameObject>();
    bool                                firstLoadPokedex        =   false;
    [Header("Pokedex Scene - Detail")]
    [SerializeField] GameObject         pokedexDetailGo;
    [SerializeField] TextMeshProUGUI    pokedexDetailNameText;
    [SerializeField] TextMeshProUGUI    pokedexDetailDescriptionText;
    [SerializeField] TextMeshProUGUI    pokedexDetailAmountText;
    [SerializeField] Image              pokedexDetailBgImage;
    [SerializeField] Image              pokedexDetailPokemonImage;
    [SerializeField] Image              pokedexDetailGenderImage;
    [SerializeField] Image              pokedexDetailEvolveImage;
    [SerializeField] Button             pokedexDetailAlolanFormButton;
    [SerializeField] Button             pokedexDetailMaleButton;
    [SerializeField] Button             pokedexDetailFemaleButton;
    [SerializeField] Button             pokedexDetailShinyButton;
    [SerializeField] Button             pokedexDetailEvolveButton;
    LootScriptable     pokedexDetailSelect;
    bool               pokedexDetailShinyTrigger  = false;
    bool               pokedexDetailGenderTrigger = false;
    bool               pokedexDetailAlolanFormTrigger = false;
    [Header("Evolution")]
    [SerializeField] GameObject         evolutionSceneGo;
    [SerializeField] Image              evolutionScenePokemonImage;
    [SerializeField] TextMeshProUGUI    evolutionScenePanelText;
    [SerializeField] Button             evolutionSceneCancelButton;
    [SerializeField] Button             evolutionSceneEvolutionButton;
    [SerializeField] Image              evolutionSceneEvolutionButtonCurrentPokemonImage;
    [SerializeField] Image              evolutionSceneEvolutionButtonNextPokemonImage;
    [SerializeField] TextMeshProUGUI    evolutionSceneAmountEvolutionButtonText;
    List<LootScriptable>                evolutionListPokemons        =   new List<LootScriptable>();
    bool                                pokemonEvolving = false;
    WaitForSeconds                      timeToEvolve                = new WaitForSeconds(5.5f);
    WaitForSeconds                      timeAutoOpenScene           = new WaitForSeconds(2.5f);
    WaitForSeconds                      timeNextListEvolutionScene  = new WaitForSeconds(5f);
    [Header("Exp/Level")]
    [SerializeField] GameObject         tooltipExpLevelGo;
    [SerializeField] Animator           tooltipExpLevelAnim;
    [SerializeField] Image              tooltipExpLevelNewImage;//white
    [SerializeField] Image              tooltipExpLevelTotalImage;//red
    [SerializeField] TextMeshProUGUI    tooltipExpLevelText;
    [Header("Config")]
    [SerializeField] AudioMixer         audioMixer;
    [SerializeField] GameObject         configGo;
    [SerializeField] TextMeshProUGUI    configResetSaveText;
    [SerializeField] Button             configButton;
    [SerializeField] Slider             configFxSlider;
    [SerializeField] Slider             configMusicSlider;
    int                                 countToReset = 0;
    WaitForSeconds                      waitResetToReset = new WaitForSeconds(3);
    [Header("Bonus")]
    [Tooltip("When SkipAnim is deactivated it gains a glow bonus"),Range(0,1)]
    [SerializeField] float              skipAnimDisableBonusShiny   = 0.04f;
    [SerializeField] GameObject         LootAmountGo;
    [SerializeField] TextMeshProUGUI    LootAmountText;
    [SerializeField] GameObject         LootTypeGo;
    [SerializeField] TMP_Dropdown       LootTypeDropdown;
    [SerializeField] GameObject         BonusTooltipGo;
    [SerializeField] TextMeshProUGUI    BonusTooltipText;
    [Header("Sprites")]
    [SerializeField] Sprite[]           backgroundTypes;
    [SerializeField] Sprite[]           genders;
    [SerializeField] Color[]            lootTypeColors;

    public bool SkipAnim{ get; set;}

    public Color[] LootTypeColors{ get { return lootTypeColors; } }

    [SerializeField]
    CurrentScene currentScene = CurrentScene.MainScene;

    public CurrentScene UpdateCurrentScene{get{return currentScene;} set{ currentScene = value;}}

    Pokedex pdx;
    ExpLevelManager expM;

    public bool LootPokemon;

    private void Awake() 
    {
         if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);       
    }

    void Start() 
    {              
        pdx  = Pokedex.Instance;
        expM = ExpLevelManager.Instance;

        LoadingSave();

        dropListExamplePool.Add(dropListExample);   
    }

    void LoadingSave()
    {
        //Toggle
        SkipAnim = PlayerPrefs.GetInt("SkipAnim", 0) != 0;
        skipAnimToggle.isOn = SkipAnim;

        //Scene
        string pkId = PlayerPrefs.GetString("LastPokemonLoot","");
        if(pkId != "")
        {
            Debug.LogWarning("LastPk "+pkId);
            DisableMainScreen();
            UpdateScene(pdx.GetPokemonLoot(pkId),saveInPokedex: false);
        }

        //Slider
        configFxSlider.value    = PlayerPrefs.GetFloat("fxVol",3);
        configMusicSlider.value = PlayerPrefs.GetFloat("musicVol",3);     
    }

    public void ResetMainPokemon()
    {
        mainPokemon.sprite = null;

        nameMainPokemon.text = "???";
        registerIconMainPokemon.enabled = false;
        genderIconMainPokemon.sprite = genders[2];
    }

    public void DisableMainScreen(bool active=true,float alpha=1,bool pokeball=false)
    {
        mainScreenCanvasGroup.interactable = !active;
        mainScreenCanvasGroup.alpha = alpha;

        //mainPokemon.gameObject.SetActive(!active);
        if(pokeball)
        {
            DropPokeball.Instance.gameObject.SetActive(!active);
        }
    }

    public void DisablePokeballAndPokemonMainScreen(bool disable = true)
    {
        DropPokeball.Instance.gameObject.SetActive(!disable);
        mainPokemon.gameObject.SetActive(!disable);
    }

    
    public void CaughtButton(bool active)
    {
       caughtButton.gameObject.SetActive(active);
    }

    #region Main Screen
    // ------------ Main Screen-------------
    public void UpdateScene(LootScriptable pk,bool activeCaughtButton = true,bool saveInPokedex = true,bool playAudio=true)
    {
        PlayAudio   audio = mainPokemon.GetComponent<PlayAudio>();
        Animator    anim  = mainPokemon.GetComponent<Animator>();

        string name     = /*"#"+pk.id+*/" "+pk.Name;
        bool register   = !pdx.GetPokemonRegistered(pk);//PlayerPrefs.GetInt(pdx.GetKeyNamePlayerPrefs(pk)) <= 1;
        
        mainPokemon.sprite              = pk.pokemon == null ? pk.GetSprite(pk.gender , pk.shiny) : pk.pokemon;
        registerIconMainPokemon.enabled = register;
        genderIconMainPokemon.sprite    = genders[(int)pk.gender];
        nameMainPokemon.color           = LootTypeColors[(int)pdx.GetLootDrop(pk).rarity];
        nameMainPokemon.text            = (pk.shiny ? "<color=yellow>"+name+"</color>" : name);
  
        if(playAudio)
            if(pk.sound != null)
            {
                audio.AddAudioClip(pk.sound,true);
            }
            else
                Debug.LogError(pk.name+",Error Audio Not Found");

        anim.SetTrigger("Start");
        anim.SetBool("Shiny",pk.shiny); 

        UpdateBackground(pk.types[0]);
       
        if(saveInPokedex){
            pdx.SaveInPokedex(pk);

            if(register)
                UpdatePokedex(pk);
        }

        if(activeCaughtButton)
        {
            CaughtButton(true);
            DisableMainScreen(false);

            if(saveInPokedex)
                NextListEvolutionScene();
        }

        LootCountText.text = "Loots: x"+ PlayerPrefs.GetInt("TotalLoot",0);
    }

    public void UpdateBackground(PokemonType type)
    {
        int i = (int)type;

        if(i > backgroundTypes.Length)
            return;
        
        mainBackgroundImage.sprite = backgroundTypes[(int)type];
    }

    public void UpdateSkipAnimToggle()
    {
        SkipAnim = skipAnimToggle.isOn;

        PlayerPrefs.SetInt("SkipAnim",(SkipAnim ? 1 : 0));

        DropPokeball.Instance.ShinyBonus = (SkipAnim ? -skipAnimDisableBonusShiny : skipAnimDisableBonusShiny);// 5% or 1%

        skipAnimToggle.GetComponent<PlayAudio>().PlayAudioClip(SkipAnim ? 1 : 0);
    }
    #endregion

    #region Tooltip Pokedex
    // ------------ Tooltip Pokedex-------------
    public void ToolTipPokedex(Sprite img,string name,Gender gender,bool shiny)
    {       
        if(img != null)
        {
            TooltipPokedexLine tt = new TooltipPokedexLine();
            tt.img    = img;
            tt.name   = name;
            tt.gender = gender;
            tt.shiny  = shiny;
            tooltipPokedexLine.Add(tt);
        }
            

        if(tooltipPokedexActive || tooltipPokedexLine.Count <= 0 || !tooltipPokedexCanActive)
            return; 

        Invoke("ShowToolTipPokedex", 1.0f);
    }

    void ShowToolTipPokedex()
    {   
        if(tooltipPokedexLine.Count < 1 || !tooltipPokedexCanActive)
            return;

        TooltipPokedexLine tt = tooltipPokedexLine[0];

        tooltipPokedexLine.RemoveAt(0);
        tooltipPokedexActive = true;
        tooltipPokedexPokemonImage.sprite = tt.img;
        tooltipPokedexText.text    = (tt.shiny ? "<color=yellow>"+tt.name.ToUpper()+"</color>" : tt.name.ToUpper())+"'s data was\nadded to the POKEDEX.";
        tooltipPokedexGenderImage.sprite = genders[(int)tt.gender];

        tooltipPokedexAnim.SetTrigger("Show");

        Invoke("CloseToolTipPokedex", 7.0f);
    }

    void CloseToolTipPokedex()
    {
        tooltipPokedexAnim.SetTrigger("Close");

        if(tooltipPokedexLine.Count >= 1)
        {
            Invoke("ShowToolTipPokedex", 2.0f);
            return;
        }

        tooltipPokedexActive = false;   
    }
    #endregion

    #region DropList
    // ------------ DropList-------------
    public void LootDropList(List<LootScriptable> loots,float common=-1,float rare =-1,float epic =-1,float legendary =-1,float shiny =-1)
    {
        DisableMainScreen(pokeball: true);

        dropListPokemons.Clear();

        dropListPokemons = (loots);

        ActiveDropList();

        //dropListDetailCommon.gameObject.SetActive(common>0);
        //if(dropListDetailCommon.gameObject.activeSelf)
            dropListDetailCommon.text = common>0?"Common: <b>"+common+"</b>" : "-";

        //dropListDetailRare.gameObject.SetActive(rare>0);
        //if(dropListDetailRare.gameObject.activeSelf)
            dropListDetailRare.text = rare>0?"Rare: <b>"+rare+"</b>" : "-";

        //dropListDetailEpic.gameObject.SetActive(epic>0);
        //if(dropListDetailEpic.gameObject.activeSelf)
            dropListDetailEpic.text = epic>0?"Epic: <b>"+epic+"</b>" : "-";

        //dropListDetailLegendary.gameObject.SetActive(legendary>0);        
        //if(dropListDetailLegendary.gameObject.activeSelf)
            dropListDetailLegendary.text = legendary>0 ? "Legendary: <b>"+legendary+"</b>" : "-";

        dropListDetailShiny.text = shiny>0 ? "Shiny: <b>"+shiny+"</b>" : "-";
    }

    PokedexContent GetDropListPool(GameObject pool,bool dropList,int comparePoolCount,Transform transform)
    {
        int poolCount = dropList ? dropListExamplePool.Count : pokedexExamplePool.Count;

        GameObject obj;

        if(poolCount < comparePoolCount)
        {
            int count = comparePoolCount - poolCount;
            
            for (int i = 0; i < count; i++)
            {
                obj = Instantiate(pool, transform) as GameObject;

                obj.name += " "+i;

                obj.SetActive(false);

                if(dropList)
                    dropListExamplePool.Add(obj);
                else  
                    pokedexExamplePool.Add(obj);
            }
        }

        for (int i = 0; i < poolCount; i++)
        {           
            if(dropList && !dropListExamplePool[i].activeSelf || !dropList && !pokedexExamplePool[i].activeSelf)
            {
                obj = dropList ? dropListExamplePool[i] : pokedexExamplePool[i];
                return obj.GetComponent<PokedexContent>();
            }           
        }

        obj = Instantiate(pool, transform) as GameObject;

        obj.name += " "+poolCount+1;

        if(dropList)
            dropListExamplePool.Add(obj);
        else  
            pokedexExamplePool.Add(obj);

        return obj.GetComponent<PokedexContent>();  
    }

    public void ActiveDropList(bool active=true)
    {     
        if(active &&  dropListPokemons.Count < 1)
            return;        

        tooltipPokedexCanActive = !active;

        int poolCount = dropListExamplePool.Count;

        mainPokemon.enabled = (!active);
        DropPokeball.Instance.gameObject.SetActive(!active);

        if(!active && poolCount > 0)
        {
            UpdateCurrentScene = CurrentScene.MainScene;

            for (int i = 0; i < poolCount; i++)
                dropListExamplePool[i].gameObject.SetActive(false); 

            if(!tooltipPokedexActive || tooltipPokedexLine.Count > 0)
                Invoke("ShowToolTipPokedex", 1.0f);

            NextListEvolutionScene();
        }

        if(active)
        {
            UpdateCurrentScene = CurrentScene.DropListScene;

            int pCount = dropListPokemons.Count;
            Transform transform = dropListContent.transform;
            for (int p = 0; p < pCount; p++)
            {
                LootScriptable pk = dropListPokemons[p];

                string id = "#"+(pk.id < 10 ? "0" : "")+(pk.id < 100 ? "0" : "")+pk.id;

                string name = id+" "+pk.Name;

                Color color = LootTypeColors[(int)pdx.GetLootDrop(pk).rarity];

                bool register = Pokedex.Instance.GetIntNamePlayerPrefs(pk) > 1;

                PokedexContent pkC = GetDropListPool(dropListExample,true,pCount,transform);

                #if UNITY_EDITOR
                    pkC.gameObject.name = (pk.shiny? "*" :"")+name;
                #endif

                pkC.AddPokemon
                (
                    backgroundTypes[(int)pk.types[0]],
                    genders[(int)pk.gender],
                    pk.pokemon,
                    name,
                    register,
                    color,
                    pk.shiny
                );

                pkC.gameObject.SetActive(true);
            }

            RectTransform rt = dropListContent.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(0, 520);
            float size = (float)pCount/3;
            if (size > 1)              
                rt.sizeDelta = new Vector2(0, 520 * (size+0.5f));

            rt.localPosition = new Vector3(0, 0, 0);           
        }
        
        dropListGo.SetActive(active);
    }
    #endregion

    #region Pokedex
    // ------------ Pokedex-------------
    public void OpenPokedex(bool open=true)
    {
        tooltipPokedexCanActive = !open;

            if(open)
            {
                UpdateCurrentScene = CurrentScene.PokedexScene;

                pokedexContent.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                    
                if(!firstLoadPokedex)
                    FirstLoadPokedex();
                else
                    UpdateAmountPokedex();
            }
            else
                UpdateCurrentScene = CurrentScene.MainScene;

        DisablePokeballAndPokemonMainScreen(open);
        pokedexGo.SetActive(open);
    }

    void UpdatePokedex(LootScriptable  pk)
    {
        if(!firstLoadPokedex)
            FirstLoadPokedex();

        int             i   =   pk.id; 

        PokedexContent pkC  =   pokedexExamplePool[i-1].GetComponent<PokedexContent>(); 

        // int male   = pdx.GetTotalCatchesGender(pk, Gender.Male,   true, true, true);  
        // int female = pdx.GetTotalCatchesGender(pk, Gender.Female, true, true, true);

        // pk.gender = female > male ? Gender.Female : Gender.Male;

        bool shiny  = pdx.GetTotalCatches(pk,onlyShiny: true) > 0;

        bool register = pdx.GetPokemonRegistered(pk);

        string id = "#"+(pk.id < 10 ? "0" : "")+(pk.id < 100 ? "0" : "")+pk.id;

        string name = id+" "+pk.Name;

        Color color = LootTypeColors[(int)pdx.GetLootDrop(pk).rarity];

        pkC.AddPokemonPokedex
        (
            backgroundTypes[(int)pk.types[0]],
            genders[(int)pk.gender],
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

    void UpdateAmountPokedex()
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

    void FirstLoadPokedex()
    {
        firstLoadPokedex = true;

        int pCount = pdx.GetMaxIndex;
        
        GetDropListPool(pokedexExample,false,pCount,pokedexContent.transform); 
                
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
    // ------------ Pokedex Detail-------------
    Gender PokemonDetailGender()
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

        UpdateCurrentScene =  open ? CurrentScene.DetailPokedexScene : CurrentScene.PokedexScene;
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
        pokedexDetailGenderImage.sprite     =    genders[(int)gender];

        
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

        pokedexDetailBgImage.sprite         = backgroundTypes[(int)pk.types[0]];

        pokedexDetailPokemonImage.color     = Color.white;
        pokedexDetailPokemonImage.sprite    = pk.GetSprite(pk.gender, pokedexDetailShinyTrigger);
        
        pokedexDetailNameText.color         = LootTypeColors[(int)pdx.GetLootDrop(pk).rarity];
        pokedexDetailNameText.text          = name.ToUpper();

        pokedexDetailDescriptionText.text   = pk.description;

        pokedexDetailGenderImage.enabled    = registered;
        pokedexDetailGenderImage.sprite     = genders[(int)pk.gender];   

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
            pokedexDetailEvolveButton.onClick.AddListener(() => OpenEvolutionScene(pokedexDetailSelect));
            pokedexDetailEvolveImage.sprite = pokedexDetailSelect.GetSpriteNextEvolution(pokedexDetailSelect.gender, pokedexDetailShinyTrigger);
            pokedexDetailEvolveImage.color  = pdx.GetPokemonRegistered(pokedexDetailSelect.nextEvolution) ? Color.white : Color.black;
        }
        else
            pokedexDetailEvolveButton.gameObject.SetActive(false);
    }
    #endregion
    #endregion

    #region Evolution
    // ------------ Evolution -------------
    public bool OpenEvolutionScene(LootScriptable pk)
    {
        if(!pdx.CanEvolved(pk))
            return false;

        if(pokemonEvolving || (UpdateCurrentScene != CurrentScene.DetailPokedexScene && UpdateCurrentScene != CurrentScene.MainScene && UpdateCurrentScene != CurrentScene.EvolveScene))
        {
            if(!evolutionListPokemons.Contains(pk))
                evolutionListPokemons.Add(pk);

            return false;
        }

        tooltipPokedexCanActive = false;
        
        UpdateCurrentScene = CurrentScene.EvolveScene;

        DisablePokeballAndPokemonMainScreen(true);

        pokedexDetailSelect = pk;
        
        LootScriptable pkE   = pk.nextEvolution;
        bool shiny = pokedexDetailShinyTrigger;

        Sprite p    = pk.GetSprite(pk.gender,shiny);
        Sprite pE   = pk.GetSpriteNextEvolution(pk.gender,shiny);

        int needEvolved = pk.needAmountToEvolution;
        int amount      = pdx.GetIntNamePlayerPrefs(pokedexDetailSelect);

        evolutionSceneGo.SetActive(true);
        evolutionSceneEvolutionButton.gameObject.SetActive(true);

        evolutionSceneEvolutionButtonCurrentPokemonImage.sprite = p;
        evolutionSceneEvolutionButtonNextPokemonImage.sprite    = pE;
        evolutionSceneEvolutionButtonNextPokemonImage.color     = pdx.GetPokemonRegistered(pkE) ? Color.white : Color.black;
        
        evolutionSceneAmountEvolutionButtonText.text = "x"+needEvolved;

        string name = shiny ? "<color=yellow>"+pokedexDetailSelect.Name.ToUpper()+"</color>" : pokedexDetailSelect.Name.ToUpper();
        
        evolutionScenePokemonImage.sprite   = p;
        evolutionScenePanelText.text        = "You'll change x<b>"+needEvolved+"<b>/"+amount+" of <b>"+name+"s</b> to evolve";

        return true;
    }

    public void EvolveButton()
    {
        pokemonEvolving = true;

       // evolutionScenePokemonImage.sprite   = pokedexDetailSelect.pokemon;

        string name = pokedexDetailShinyTrigger ? "<color=yellow>"+pokedexDetailSelect.Name.ToUpper()+"</color>" : pokedexDetailSelect.Name.ToUpper();

        evolutionScenePanelText.text        = "What?\n"+name+" is evolving!";

        evolutionSceneCancelButton.gameObject.SetActive(false);
        evolutionSceneEvolutionButton.gameObject.SetActive(false);

        evolutionSceneGo.GetComponent<PlayAudio>().PlayAudioClip(0);

        AnimEvolutionScene(true);

        //if !skip
        StartCoroutine(IAnimEvolutionScene(false));
    }

     IEnumerator IAnimEvolutionScene(bool start)
    {
        yield return timeToEvolve;

        if(!start)
            EndEvolved();

        AnimEvolutionScene(start);
    }

    void EndEvolved()
    {      
        pokemonEvolving = false;    

        bool shiny = pokedexDetailShinyTrigger;

        LootScriptable pkE   = pokedexDetailSelect.nextEvolution;

        Sprite pE   = pkE.GetSprite(pokedexDetailSelect.gender,shiny);

        Gender gender = Gender.Unknown;
        if(pkE.genderRatio != GenderRatio.Genderless)
            gender = pokedexDetailGenderTrigger ? Gender.Male : Gender.Female;

        UpdateScene(pdx.Evolved(pokedexDetailSelect,gender,shiny),false,false,false);

        string Oldname  = shiny ? "<color=yellow>"+pokedexDetailSelect.Name.ToUpper()+"</color>"    : pokedexDetailSelect.Name.ToUpper();
        string Evname   = shiny ? "<color=yellow>"+pkE.Name.ToUpper()+"</color>"                    : pkE.Name.ToUpper();
        
        evolutionScenePokemonImage.sprite   = pE;
        evolutionScenePanelText.text        = "Congratulations! Your "+Oldname+" evolved into "+Evname;

        evolutionSceneCancelButton.gameObject.SetActive(true);
         
        evolutionSceneEvolutionButton.gameObject.SetActive(pdx.CanEvolved(pokedexDetailSelect));
        evolutionSceneEvolutionButtonNextPokemonImage.color     = pdx.GetPokemonRegistered(pkE) ? Color.white : Color.black;

        UpdateAmountPokedex();
        UpdatePokedex(pkE);
    }

    void AnimEvolutionScene(bool start)
    {
        Animator anim = evolutionSceneGo.GetComponent<Animator>();

        if(start)
            anim.SetTrigger("Start");
        else
            anim.SetTrigger("Stop");
    }

    public void CancelEvolution()
    {
        evolutionSceneGo.SetActive(false);

        if(pokedexDetailGo.activeSelf == false)
        {
            DisablePokeballAndPokemonMainScreen(false);
            UpdateCurrentScene = CurrentScene.MainScene;
        }
        else
            {
                int amount  = pdx.GetIntNamePlayerPrefs(pokedexDetailSelect);
                pokedexDetailAmountText.gameObject.SetActive(amount > 1);
                pokedexDetailAmountText.text = "x"+amount;
                UpdateCurrentScene = CurrentScene.DetailPokedexScene;

                LoadPokemonDetail(pokedexDetailSelect);
            }

        if(evolutionListPokemons.Count >= 1)
            NextListEvolutionScene();
        else
        {
            if(!tooltipPokedexActive || tooltipPokedexLine.Count > 0)
                Invoke("ShowToolTipPokedex", 1.0f);

            tooltipPokedexCanActive = true;
        }
    }

    void NextListEvolutionScene()
    {
        if(evolutionListPokemons.Count <= 0)
        {
            if(!tooltipPokedexActive || tooltipPokedexLine.Count > 0)
                Invoke("ShowToolTipPokedex", 1.0f);
                
            tooltipPokedexCanActive = true;

            return;
        }

        if(!evolutionSceneGo.activeSelf)
        {
            pokemonEvolving = false;

            LootScriptable pk = evolutionListPokemons[0];

            if(OpenEvolutionScene(pk))
                evolutionListPokemons.RemoveAt(0); 
        }           
    }
    #endregion

    #region Bonus
    public void ToolTipNewBonus(string text)
    {
        if(currentScene != CurrentScene.LootPokemonScene)
            DisablePokeballAndPokemonMainScreen(true);
    
        if(BonusTooltipGo.activeSelf)
            BonusTooltipText.text +="\n"+text;
        else
            BonusTooltipText.text = text;

        BonusTooltipGo.SetActive(true);
    }

    public void CloseToolTipNewBonus()
    {
        if(currentScene == CurrentScene.MainScene)
            DisablePokeballAndPokemonMainScreen(false);

        BonusTooltipGo.SetActive(false);
    }

    public void ActiveLootAmount()
    {
        LootAmountGo.SetActive(true);
        LootAmountText.text = "x"+PlayerPrefs.GetInt("LootNumber", 1);
    }

    public void LootAmountSumButton(bool sum=true)
    {
        int v = PlayerPrefs.GetInt("LootNumber", 1);

        if(sum)
            v++;
        else
            v--;

        if(v > expM.MaxLootNumber)
            v = 1;
        if(v < 1)
            v =  expM.MaxLootNumber;

        PlayerPrefs.SetInt("LootNumber", v);

        DropPokeball.Instance.LootNumber = v;

        LootAmountText.text = "x"+v;
    }

    public void ActiveLootType()
    {
        LootTypeGo.SetActive(true);
        LootTypeDropdown.value = PlayerPrefs.GetInt("LootType", 0); //0 = Normal
    }

    public void SaveDropdownLootType(int v)
    {
        if(v > 0)
            v += 8;

        PlayerPrefs.SetInt("LootType", v);

        //Ban -> dragon, Ghost ,Dark,Steel fairy

        DropPokeball.Instance.LootType = (LootType)v;

        Debug.Log("Dropdown["+v+"] -> "+(LootType)v);
    }

    #endregion

    #region Exp/Level
    // ------------ Exp/Level ------------- 
    public void UpdateExpLevel(bool anim=true,bool playAudio=true)
    {
        ExpLevelManager expM = ExpLevelManager.Instance;

        if(anim)
        {
            tooltipExpLevelAnim.SetTrigger("Show");
            
            if(tooltipExpLevelText.text != ExpLevelManager.Instance.Level.ToString())
            {
                UpdateExpLevelUP();
                tooltipExpLevelTotalImage.fillAmount = 0;
            }             
        }

        tooltipExpLevelNewImage.fillAmount = expM.Exp / expM.MaxExp;

        StartCoroutine(UpdateExpLevelTotalBar(anim,playAudio));
    }

    WaitForSeconds waitExpLevel   = new WaitForSeconds(0.45f+0.05f);//Anim Time + delay
    WaitForSeconds waitExpLevelToClose   = new WaitForSeconds(2f);
    IEnumerator UpdateExpLevelTotalBar(bool anim=true,bool playAudio=true)
    {
        yield return waitExpLevel;

        if(playAudio)
            tooltipExpLevelGo.GetComponent<PlayAudio>().PlayAudioClip(2);

        while (tooltipExpLevelNewImage.fillAmount != tooltipExpLevelTotalImage.fillAmount)
        {
            if (tooltipExpLevelTotalImage.fillAmount > tooltipExpLevelNewImage.fillAmount)
            {
                tooltipExpLevelTotalImage.fillAmount = tooltipExpLevelNewImage.fillAmount;
                break;
            }
            else
                if (tooltipExpLevelTotalImage.fillAmount < tooltipExpLevelNewImage.fillAmount)
            {
                if ((tooltipExpLevelNewImage.fillAmount - tooltipExpLevelTotalImage.fillAmount) <= 0.19f)//0.77f
                    tooltipExpLevelTotalImage.fillAmount += 0.004f;
                else
                    tooltipExpLevelTotalImage.fillAmount += 0.01f;//0.009f;

                yield return null;
            }
        }

        if(tooltipExpLevelTotalImage.fillAmount == 1)
        {   
            UpdateExpLevelUP();  

            yield return waitExpLevel;  

            tooltipExpLevelTotalImage.fillAmount = 0;
            tooltipExpLevelNewImage.fillAmount   = 0;
        }

        yield return waitExpLevelToClose;

        if(anim && tooltipExpLevelNewImage.fillAmount == tooltipExpLevelTotalImage.fillAmount)
            tooltipExpLevelAnim.SetTrigger("Close");
    }

    public void UpdateExpLevelUP(bool playAudio=true)
    {
        tooltipExpLevelText.text = ""+ExpLevelManager.Instance.Level;

        if(playAudio)
            tooltipExpLevelGo.GetComponent<PlayAudio>().PlayAudioClip(3,false);
    }
    #endregion

    #region Config 
    public void ConfigButton(bool open)
    {
        DisablePokeballAndPokemonMainScreen(open);
        configGo.SetActive(open);

        if(open)
        {
            countToReset = 0;
            ResetSave(false);
        }    
        else
        {
            StopCoroutine(IResetCountToReset());
        }   
    }

    public void ResetSave(bool sum =true)
    {
        if(sum)
            countToReset++;

        switch (countToReset)
        {
            default:
                configResetSaveText.text = "RESET SAVE";
            break;

            case 1:
                configResetSaveText.text = "<b>Click Again\nTo RESET</b>";
                StartCoroutine(IResetCountToReset());
            break;

            case 2:
                PlayerPrefs.DeleteAll();
                #if UNITY_EDITOR
                    EditorApplication.isPlaying = false;
                #else
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                #endif       
            break;
        }      
    }
    
    IEnumerator IResetCountToReset()
    {
        yield return waitResetToReset;

        countToReset = 0;
        ResetSave(false);
    }

    public void SetFxVolume(float value)
    {
        
        if(value == -25)
            value = -80;
        
        audioMixer.SetFloat("fxVol",value);

        PlayerPrefs.SetFloat("fxVol",value);
    }

    public void SetMusicVolume(float value)
    {
        
        if(value == -25)
            value = -80; 

        audioMixer.SetFloat("musicVol",value);

        PlayerPrefs.SetFloat("musicVol",value);
    }
    #endregion

}

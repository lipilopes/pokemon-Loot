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
    public Sprite[]           backgroundTypes;
    public Sprite[]           genders;
    public Color[]            lootTypeColors;

    public bool SkipAnim{ get; set;}

    public Color[] LootTypeColors{ get { return lootTypeColors; } }

    [SerializeField]
    CurrentScene currentScene = CurrentScene.MainScene;

    public CurrentScene UpdateCurrentScene{get{return currentScene;} set{ currentScene = value;}}

    Pokedex pdx;
    ExpLevelManager expM;
    HudToolTipPokedex hTTP;

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
        hTTP = HudToolTipPokedex.Instance;

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
                HudPokedex.Instance.UpdatePokedex(pk);
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

    public PokedexContent GetDropListPool(GameObject pool,bool dropList,int comparePoolCount,Transform transform)
    {
        int poolCount = dropList ? dropListExamplePool.Count : HudPokedex.Instance.pokedexExamplePool.Count;

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
                    HudPokedex.Instance.pokedexExamplePool.Add(obj);
            }
        }

        for (int i = 0; i < poolCount; i++)
        {           
            if(dropList && !dropListExamplePool[i].activeSelf || !dropList && !HudPokedex.Instance.pokedexExamplePool[i].activeSelf)
            {
                obj = dropList ? dropListExamplePool[i] : HudPokedex.Instance.pokedexExamplePool[i];
                return obj.GetComponent<PokedexContent>();
            }           
        }

        obj = Instantiate(pool, transform) as GameObject;

        obj.name += " "+poolCount+1;

        if(dropList)
            dropListExamplePool.Add(obj);
        else  
            HudPokedex.Instance.pokedexExamplePool.Add(obj);

        return obj.GetComponent<PokedexContent>();  
    }

    public void ActiveDropList(bool active=true)
    {     
        if(active &&  dropListPokemons.Count < 1)
            return;        

        hTTP.tooltipPokedexCanActive = !active;

        int poolCount = dropListExamplePool.Count;

        mainPokemon.enabled = (!active);
        DropPokeball.Instance.gameObject.SetActive(!active);

        if(!active && poolCount > 0)
        {
            UpdateCurrentScene = CurrentScene.MainScene;

            for (int i = 0; i < poolCount; i++)
                dropListExamplePool[i].gameObject.SetActive(false); 

            if(!hTTP.tooltipPokedexActive || hTTP.tooltipPokedexLine.Count > 0)
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

        hTTP.tooltipPokedexCanActive = false;
        
        UpdateCurrentScene = CurrentScene.EvolveScene;

        DisablePokeballAndPokemonMainScreen(true);

        HudPokedex.Instance.pokedexDetailSelect = pk;
        
        LootScriptable pkE   = pk.nextEvolution;
        bool shiny = HudPokedex.Instance.pokedexDetailShinyTrigger;

        Sprite p    = pk.GetSprite(pk.gender,shiny);
        Sprite pE   = pk.GetSpriteNextEvolution(pk.gender,shiny);

        int needEvolved = pk.needAmountToEvolution;
        int amount      = pdx.GetIntNamePlayerPrefs(HudPokedex.Instance.pokedexDetailSelect);

        evolutionSceneGo.SetActive(true);
        evolutionSceneEvolutionButton.gameObject.SetActive(true);

        evolutionSceneEvolutionButtonCurrentPokemonImage.sprite = p;
        evolutionSceneEvolutionButtonNextPokemonImage.sprite    = pE;
        evolutionSceneEvolutionButtonNextPokemonImage.color     = pdx.GetPokemonRegistered(pkE) ? Color.white : Color.black;
        
        evolutionSceneAmountEvolutionButtonText.text = "x"+needEvolved;

        string name = shiny ? "<color=yellow>"+HudPokedex.Instance.pokedexDetailSelect.Name.ToUpper()+"</color>" : HudPokedex.Instance.pokedexDetailSelect.Name.ToUpper();
        
        evolutionScenePokemonImage.sprite   = p;
        evolutionScenePanelText.text        = "You'll change x<b>"+needEvolved+"<b>/"+amount+" of <b>"+name+"s</b> to evolve";

        return true;
    }

    public void EvolveButton()
    {
        pokemonEvolving = true;

       // evolutionScenePokemonImage.sprite   = pokedexDetailSelect.pokemon;

        string name = HudPokedex.Instance.pokedexDetailShinyTrigger ? "<color=yellow>"+HudPokedex.Instance.pokedexDetailSelect.Name.ToUpper()+"</color>" : HudPokedex.Instance.pokedexDetailSelect.Name.ToUpper();

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

        bool shiny = HudPokedex.Instance.pokedexDetailShinyTrigger;

        LootScriptable pkE   = HudPokedex.Instance.pokedexDetailSelect.nextEvolution;

        Sprite pE   = pkE.GetSprite(HudPokedex.Instance.pokedexDetailSelect.gender,shiny);

        Gender gender = Gender.Unknown;
        if(pkE.genderRatio != GenderRatio.Genderless)
            gender = HudPokedex.Instance.pokedexDetailGenderTrigger ? Gender.Male : Gender.Female;

        UpdateScene(pdx.Evolved(HudPokedex.Instance.pokedexDetailSelect,gender,shiny),false,false,false);

        string Oldname  = shiny ? "<color=yellow>"+HudPokedex.Instance.pokedexDetailSelect.Name.ToUpper()+"</color>"    : HudPokedex.Instance.pokedexDetailSelect.Name.ToUpper();
        string Evname   = shiny ? "<color=yellow>"+pkE.Name.ToUpper()+"</color>"                    : pkE.Name.ToUpper();
        
        evolutionScenePokemonImage.sprite   = pE;
        evolutionScenePanelText.text        = "Congratulations! Your "+Oldname+" evolved into "+Evname;

        evolutionSceneCancelButton.gameObject.SetActive(true);
         
        evolutionSceneEvolutionButton.gameObject.SetActive(pdx.CanEvolved(HudPokedex.Instance.pokedexDetailSelect));
        evolutionSceneEvolutionButtonNextPokemonImage.color     = pdx.GetPokemonRegistered(pkE) ? Color.white : Color.black;

        HudPokedex.Instance.UpdateAmountPokedex();
        HudPokedex.Instance.UpdatePokedex(pkE);
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

        if(HudPokedex.Instance.pokedexDetailGo.activeSelf == false)
        {
            DisablePokeballAndPokemonMainScreen(false);
            UpdateCurrentScene = CurrentScene.MainScene;
        }
        else
            {
                int amount  = pdx.GetIntNamePlayerPrefs(HudPokedex.Instance.pokedexDetailSelect);
                HudPokedex.Instance.pokedexDetailAmountText.gameObject.SetActive(amount > 1);
                HudPokedex.Instance.pokedexDetailAmountText.text = "x"+amount;
                UpdateCurrentScene = CurrentScene.DetailPokedexScene;

                HudPokedex.Instance.LoadPokemonDetail(HudPokedex.Instance.pokedexDetailSelect);
            }

        if(evolutionListPokemons.Count >= 1)
            NextListEvolutionScene();
        else
        {
            if(!hTTP.tooltipPokedexActive || hTTP.tooltipPokedexLine.Count > 0)
                Invoke("ShowToolTipPokedex", 1.0f);

            hTTP.tooltipPokedexCanActive = true;
        }
    }

    void NextListEvolutionScene()
    {
        if(evolutionListPokemons.Count <= 0)
        {
            if(!hTTP.tooltipPokedexActive || hTTP.tooltipPokedexLine.Count > 0)
                Invoke("ShowToolTipPokedex", 1.0f);
                
            hTTP.tooltipPokedexCanActive = true;

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
}

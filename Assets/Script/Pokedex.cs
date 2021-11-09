using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class LootDrop 
{ 
    public LootScriptable loot;
    public DropRarity     rarity;
    public bool           canDrop=true;
}

public class Pokedex : MonoBehaviour
{
    public static Pokedex Instance;

    //#if UNITY_EDITOR
    [ContextMenuItem("Check % Dex", "GetPokedexStatus")]
    [ContextMenuItem("Reset Save", "ResetSave")]
    public bool DeleteThisVarAfter;
    //#endif
    [SerializeField]
    List<LootDrop> pokemons = new List<LootDrop>();

    public List<LootDrop> GetAllPokemons { get{ return pokemons;}}

    int maxID=0;
    public int GetMaxIndex{ get{ return maxID; }}

    ExpLevelManager expM;

    private void Awake() 
    {
         if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this); 
    }

    private void Start() 
    {
        CountMaxIndex();
        expM = ExpLevelManager.Instance; 
    }

    void CountMaxIndex()
    {
        int count       =   pokemons.Count,
            currentId   =   0,
            BackId      =   -1;

        for (int i = 0; i < count; i++)
        {
            int id = pokemons[i].loot.id;

            currentId = id;

            if(currentId != BackId)
            {
                maxID++;
                BackId =  currentId;
            }
        }
    }

    public LootDrop GetLootDrop(string name,Form? form = null)
    {
        if(form ==null)
            return pokemons.Find(x => x.loot.name == name);
        else
            return pokemons.Find(x => x.loot.name == name && x.loot.form == form);
    }

    public List<LootDrop> GetLoot(LootType lootType)
    {
        List<LootDrop> r = new List<LootDrop>();

            switch (lootType)
            {
                default://DropPokeball  -> OnlyShiny,Commom,Rare,Epic,Legendary 
                    r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop;}
                    );
                break;

                case LootType.OnlyAlolan:
                     r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.form == Form.Alolan;}
                    );
                break;

                case LootType.OnlyMale:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.genderRatio == GenderRatio.OnlyMale;}
                    );
                break;

                case LootType.OnlyFemale:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.genderRatio == GenderRatio.OnlyFemale;}
                    );
                break;

                case LootType.OnlyBasicEvolution:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.evolution == Evolution.BasicEvolution;}
                    );
                break;

                case LootType.OnlyFirstEvolution:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.evolution == Evolution.FirstEvolution;}
                    );
                break;

                case LootType.OnlySecondEvolution:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.evolution == Evolution.SecondEvolution;}
                    );
                break;

                case LootType.OnlyNormal:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Normal);}
                    );
                break;

                case LootType.OnlyFire:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Fire);}
                    );
                break;

                case LootType.OnlyFighting:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Fighting);}
                    );
                break;

                case LootType.OnlyWater:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Water);}
                    );
                break;

                case LootType.OnlyFlying:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Flying);}
                    );
                break;

                case LootType.OnlyGrass:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Grass);}
                    );
                break;

                case LootType.OnlyPoison:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Poison);}
                    );
                break;

                case LootType.OnlyElectric:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Electric);}
                    );
                break;

                case LootType.OnlyGround:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Ground);}
                    );
                break;

                case LootType.OnlyPsychic:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Psychic);}
                    );
                break;

                case LootType.OnlyRock:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Rock);}
                    );
                break;

                case LootType.OnlyIce:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Ice);}
                    );
                break;

                case LootType.OnlyBug:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Bug);}
                    );
                break;

                case LootType.OnlyDragon:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Dragon);}
                    );
                break;

                case LootType.OnlyGhost:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Ghost);}
                    );
                break;

                case LootType.OnlyDark:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Dark);}
                    );
                break;

                case LootType.OnlySteel:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Steel);}
                    );
                break;

                case LootType.OnlyFairy:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Fairy);}
                    );
                break;
            }

        return r;
    }

    public LootDrop GetLootDrop(int Id,Form? form = null)
    {
         if(form ==null)
            return pokemons.Find(x => x.loot.id == Id);
        else
            return pokemons.Find(x => x.loot.id == Id && x.loot.form == form);
    }

    public LootDrop GetLootDrop(LootScriptable loot,Form? form = null)
    {
        if(form ==null)
            return pokemons.Find(x => x.loot == loot);
        else
            return pokemons.Find(x => x.loot == loot && x.loot.form == form);
    }

    public LootScriptable GetPokemon(string name,Form? form = null)
    {
        if(form ==null)
            return pokemons.Find(x => x.loot.name == name).loot;
        else
            return pokemons.Find(x => x.loot.name == name && x.loot.form == form).loot;
    }

    public LootScriptable GetPokemon(int Id,Form? form = null)
    {
        if(form ==null)
            return pokemons.Find(x => x.loot.id == Id).loot;
        else
            return pokemons.Find(x => x.loot.id == Id && x.loot.form == form).loot;
    }

    public LootScriptable GetPokemonLoot(string key)
    {
        return pokemons.Find(x => x.loot.ToString() == key).loot;
    }

    public List<LootScriptable> GetPokemons(int Id)
    {
        List<LootScriptable> r = new List<LootScriptable>();
        r.Add(pokemons.Find(x => x.loot.id == Id  /*&& x.loot.form == Form.Normal*/).loot);
        return r;
    }

    public List<LootScriptable> GetPokemons(LootScriptable loot)
    {
        List<LootScriptable> r = new List<LootScriptable>();
        r.Add(pokemons.Find(x => x.loot == loot).loot);
        return r;
    }

    public void SaveInPokedex(LootScriptable pk)
    {
        PlayerPrefs.SetString("LastPokemonLoot",""+pk);
        PlayerPrefs.SetInt("TotalLoot", PlayerPrefs.GetInt("TotalLoot",0)+1);

        string key = GetKeyNamePlayerPrefs(pk);

        if(GetPokemonRegistered(pk))
        {           
            int number = PlayerPrefs.GetInt(key) + 1;
            PlayerPrefs.SetInt(key, number);
        }
        else
        {
            PlayerPrefs.SetInt(key, 1);

            HudManager.Instance.ToolTipPokedex(pk.pokemon,pk.name,pk.gender,pk.shiny);
           
            DropRarity r = GetLootDrop(pk).rarity;
            expM.SetExp(expM.GetRarityExp(r),pk.shiny);
        }

        if(CanEvolved(pk) && GetAmount(pk.nextEvolution) < 1)
                HudManager.Instance.OpenEvolutionScene(pk);

        CompleteDex();

        #if UNITY_EDITOR
            DropRarity rarity = GetLootDrop(pk).rarity;
            string[] colors = {"white","green","blue","red"};
            Debug.Log("<color="+colors[(int)rarity]+">"+(pk.shiny ? "<color=yellow><b>*</b></color> " : "" )+pk.name+" ["+rarity+"] -> "+key+" = x"+PlayerPrefs.GetInt(key)+" / "+PlayerPrefs.GetInt("TotalLoot")+"</color>");
        #endif
    }

    public bool CanEvolved(LootScriptable pk)
    {
        if(pk.nextEvolution != null)
        {
            int amount = GetAmount(pk);

            if(amount >= pk.needAmountToEvolution)
                return true;
        }

        return false;
    }

    public void Evolved(LootScriptable pk,Gender gender,bool shiny)
    {
        int amount = pk.needAmountToEvolution;

        string key = GetKeyNamePlayerPrefs(pk);

        int number = PlayerPrefs.GetInt(key) - amount;
        PlayerPrefs.SetInt(key, number);

        LootScriptable pkE = pk.nextEvolution;

        pkE.pokemon = pkE.GetSprite(gender,shiny);

        pkE.gender = gender;
        pkE.shiny  = shiny;

        SaveInPokedex(pkE);
    }

    public int GetNumberCompleteDex()
    {
        int count   = pokemons.Count;

        if(PlayerPrefs.GetInt("CompleteDex") == 1)
            return GetMaxIndex;

        int r = 0;
        int loot = GetTotalCatches(pokemons[0].loot,true);
        string name = "";

        for (int i = 0; i < count; i++)
        {                   
            LootScriptable pk = pokemons[i].loot;

            if(pk.name != name)
            {
                if(loot > 0)
                    r++; 

                loot = GetTotalCatches(pk,true);
                name = pk.name;               
            }
            else
            {
                loot += GetTotalCatches(pk,true);  
            }                     
        }  

        if(r == GetMaxIndex)
        {
            PlayerPrefs.SetInt("CompleteDex", 1); 
            Debug.LogError("DEX COMPLETs"); 
        }

        return r;
    }

    public int GetNumberCompleteShinyDex()
    {
        int count   = pokemons.Count;

        int r = 0;
        int loot = GetTotalCatches(pokemons[0].loot,onlyShiny: true);
        string name = "";

        for (int i = 0; i < count; i++)
        {                   
            LootScriptable pk = pokemons[i].loot;

            if(pk.name != name)
            {
                if(loot > 0)
                    r++; 

                loot = GetTotalCatches(pk,onlyShiny: true);
                name = pk.name;               
            }
            else
            {
                loot += GetTotalCatches(pk,onlyShiny: true);  
            }                     
        }   

        return r;
    }

    public bool CompleteDex()
    {
        int count   = pokemons.Count;

        if(PlayerPrefs.GetInt("TotalLoot") < count)
            return false;

         GetNumberCompleteDex();//Set PlayerPrefs -> CompleteDex = 1

        if(PlayerPrefs.GetInt("CompleteDex") == 1)
            return true;

        return false;
    }

    public bool GetPokemonRegistered(LootScriptable pk)
    {
        bool shiny = pk.shiny;
        return GetTotalCatches(pk,onlyShiny: shiny) > 0;
    }

    public int GetTotalCatches(LootScriptable pk,bool shiny = false,bool onlyShiny=false,bool form =false)
    {
        int r = 0;
        
        if(!onlyShiny)
            r += PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk,Gender.Male,false))+PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk,Gender.Female,false))+PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk,Gender.Unknown,false));

        if(shiny || onlyShiny)
            r += PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk,Gender.Male,true))+PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk,Gender.Female,true))+PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk,Gender.Unknown,true));

        if(form)
        {
            if(!onlyShiny)
                r += PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk,Gender.Male,Form.Alolan,false))+PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk,Gender.Female,Form.Alolan,false))+PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk,Gender.Unknown,Form.Alolan,false));

            if(shiny || onlyShiny)
                r+= PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk,Gender.Male,Form.Alolan,true))+PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk,Gender.Female,Form.Alolan,true))+PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk,Gender.Unknown,Form.Alolan,true));
        }

        return r;
    }

    public int GetTotalCatchesAlolanRegistered(LootScriptable pk,bool shiny = false,bool onlyShiny=false)
    {
        int r = 0;
        
        if(!onlyShiny)
            r += PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk,Gender.Male,Form.Alolan,false))+PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk,Gender.Female,Form.Alolan,false))+PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk,Gender.Unknown,Form.Alolan,false));

        if(shiny || onlyShiny)
            r += PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk,Gender.Male,Form.Alolan,true))+PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk,Gender.Female,Form.Alolan,true))+PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk,Gender.Unknown,Form.Alolan,true));

        return r;
    }

    public int GetAmount(LootScriptable pk)
    {
        return PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk));
    }

    public string GetKeyNamePlayerPrefs(LootScriptable pk)
    {
        string form   = (pk.form   != Form.Normal ? pk.form+"_" : "");

        return "#"+pk.id+"_"+form+pk.gender+"_"+pk.shiny;
    }

    public string GetKeyNamePlayerPrefs(LootScriptable pk,Gender gender)
    {
        string form   = (pk.form   != Form.Normal ? pk.form+"_" : "");

        return "#"+pk.id+"_"+form+gender+"_"+pk.shiny;
    }

    public string GetKeyNamePlayerPrefs(LootScriptable pk,Gender gender,Form form)
    {
        string f   = (form   != Form.Normal ? form+"_" : "");

        return "#"+pk.id+"_"+f+gender+"_"+pk.shiny;
    }

    public string GetKeyNamePlayerPrefs(LootScriptable pk,bool shiny)
    {
        string form   = (pk.form   != Form.Normal ? pk.form+"_" : "");

        return "#"+pk.id+"_"+form+pk.gender+"_"+shiny;
    }

    public string GetKeyNamePlayerPrefs(LootScriptable pk,Gender gender,bool shiny)
    {
        string form   = (pk.form   != Form.Normal ? pk.form+"_" : "");

        return "#"+pk.id+"_"+form+gender+"_"+shiny;
    }

    public string GetKeyNamePlayerPrefs(LootScriptable pk,Gender gender,Form form,bool shiny)
    {
        string f   = (form   != Form.Normal ? form+"_" : "");

        return "#"+pk.id+"_"+f+gender+"_"+shiny;
    }

    public void ResetSave()
    {
        PlayerPrefs.DeleteAll();

        Debug.LogWarning("Your save has been Reset");
    }

    void GetPokedexStatus()
    {
        int count               = pokemons.Count;
        float registered        = 0;
        float shinyRegistered   = 0;

        float C=0,R=0,E=0,L=0,S=0;

        if(PlayerPrefs.GetInt("TotalLoot") > 0)
            for (int i = 0; i < count; i++)
            {
                LootScriptable pk   = pokemons[i].loot;
                int maleLoot        = PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk),0);
                int shinyMaleLoot   = PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk,true),0);
                int femaleLoot      = 0;
                int shinyFemaleLoot = 0;
                int loots           = maleLoot + shinyMaleLoot;          

                if(pk.genderRatio != GenderRatio.OnlyMale && pk.genderRatio != GenderRatio.Genderless)
                {
                    femaleLoot  = PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk,Gender.Female));
                    loots       += femaleLoot;

                    shinyFemaleLoot = PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk,Gender.Female,true));
                    loots           += shinyFemaleLoot;
                }

                if(pk.shiny) S+= shinyFemaleLoot+shinyMaleLoot;

                if(pokemons[i].rarity == DropRarity.Common) C+=loots;
                if(pokemons[i].rarity == DropRarity.Rare) R+=loots;
                if(pokemons[i].rarity == DropRarity.Epic) E+=loots;
                if(pokemons[i].rarity == DropRarity.Legendary) L+=loots; 

                if(loots > 0)//GetTotalCatches
                {
                    registered++;
                    Debug.Log(pk.name+"["+pokemons[i].rarity+"] <color=blue>♂["+maleLoot+" / <color=yellow><b>"+shinyMaleLoot+"</b></color>]</color> <color=magenta>♀["+femaleLoot+" / <color=yellow><b>"+shinyFemaleLoot+"</b></color>]</color> - "+loots);
                
                    shinyRegistered += shinyFemaleLoot + shinyMaleLoot;
                }
                else
                    Debug.LogError(pk.name+" ["+pokemons[i].rarity+"] Not Found");
            }

        int total = PlayerPrefs.GetInt("TotalLoot",0);
        Debug.LogWarning("Common ["+((C/total)*100)+"%] "+C+"/"+total);
        Debug.LogWarning("Rare ["+((R/total)*100)+"%] "+R+"/"+total);
        Debug.LogWarning("Epic ["+((E/total)*100)+"%] "+E+"/"+total);
        Debug.LogWarning("Legendary ["+((L/total)*100)+"%] "+L+"/"+total);
        Debug.LogWarning("Shiny ["+((S/total)*100)+"%] "+S+"/"+total);
        Debug.LogWarning("Pokedex["+((registered/count)*100).ToString("F2")+"%] "+registered+" of "+count+" - <color=yellow>"+shinyRegistered+"</color>/"+total);
    }
}

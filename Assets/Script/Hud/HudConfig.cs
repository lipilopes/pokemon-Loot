using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class HudConfig : MonoBehaviour
{
    [SerializeField] AudioMixer         audioMixer;
    [SerializeField] GameObject         configGo;
    [SerializeField] TextMeshProUGUI    configResetSaveText;
    [SerializeField] Button             configButton;
    [SerializeField] Slider             configFxSlider;
    [SerializeField] Slider             configMusicSlider;
    int                                 countToReset = 0;
    WaitForSeconds                      waitResetToReset = new WaitForSeconds(3);

    void Start() 
    {   
        configFxSlider.value    = PlayerPrefs.GetFloat("fxVol",3);
        configMusicSlider.value = PlayerPrefs.GetFloat("musicVol",3);          
    }

    public void ConfigButton(bool open)
    {
        HudManager.Instance.DisablePokeballAndPokemonMainScreen(open);
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
}

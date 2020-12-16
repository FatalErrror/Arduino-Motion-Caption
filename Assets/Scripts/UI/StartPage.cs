using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartPage : MonoBehaviour
{
    public InputField Port;
    public InputField Freqancy;
    public Dropdown Bone;

    public InputField PreasetName;
    public Dropdown PresetChoise;

    public Toggle[] Toggles;
    public GameObject Toggle;

    public Dropdown[] Dropdowns;
    public GameObject ListItem;
    // Start is called before the first frame update
    void Start()
    {
        Port.text = PlayerPrefs.GetString("Port","COM1");
        Freqancy.text = "" + PlayerPrefs.GetInt("Freqancy", 115200);
        Bone.value = PlayerPrefs.GetInt("Bone", 0);

        Toggles = new Toggle[(int)Skeleton.Indexs.Lenght];
        Dropdowns = new Dropdown[(int)Skeleton.Indexs.Lenght];

        for (int i = 0; i < (int)Skeleton.Indexs.Lenght; i++)
        {
            Toggles[i] = GameObject.Instantiate(Toggle, Toggle.transform.parent).GetComponent<Toggle>();
            Toggles[i].gameObject.transform.GetChild(1).GetComponent<Text>().text = ((Skeleton.Indexs)(i)).ToString();
            Toggles[i].isOn = PlayerPrefs.GetInt("IsContoleable"+i, 0) > 0;

            Dropdowns[i] = GameObject.Instantiate(ListItem, ListItem.transform.parent).transform.GetChild(1).gameObject.GetComponent<Dropdown>();
            Dropdowns[i].gameObject.transform.parent.GetChild(0).GetComponent<Text>().text = "  "+i;

            Dropdowns[i].options.Clear();
            for (int j = 0; j < (int)Skeleton.Indexs.Lenght; j++)
            {
                Dropdowns[i].options.Add(new Dropdown.OptionData(((Skeleton.Indexs)(j)).ToString()));
            }
            Dropdowns[i].value = PlayerPrefs.GetInt("NumberMap" + i, i);
        }

        GameObject.Destroy(Toggle);
        GameObject.Destroy(ListItem);

        UpdatePresetsList();

        PreasetName.text = PlayerPrefs.GetString("PresetName", "Fail");
    }

    private void UpdatePresetsList()
    {
        PresetChoise.options.Clear();
        for (int i = 0; i < PlayerPrefs.GetInt("PresetsCount", 0); i++)
        {
            PresetChoise.options.Add(new Dropdown.OptionData(PlayerPrefs.GetString("Preset"+i+"Name", "Fail")));
        }
    }

    public void PresetChouse(int c)
    {
        PreasetName.text = PlayerPrefs.GetString("Preset" + c + "Name", "Fail");
        Port.text = PlayerPrefs.GetString("Preset" + c + "Port", "Fail");
        Freqancy.text = PlayerPrefs.GetString("Preset" + c + "Freqancy", "0000");

    }

    public void PresetSaved()
    {
        int find = PlayerPrefs.GetInt("PresetsCount", 0);
        string presetName = PreasetName.text;
        for (int i = 0; i < PlayerPrefs.GetInt("PresetsCount", 0); i++)
        {
            if (PresetChoise.options[i].text == presetName)
            {
                find = i;
                PlayerPrefs.SetInt("PresetsCount", PlayerPrefs.GetInt("PresetsCount", 0) - 1);
                break;
            }
        }
        PlayerPrefs.SetString("Preset" + find + "Name", PreasetName.text);
        PlayerPrefs.SetString("Preset" + find + "Port", Port.text);
        PlayerPrefs.SetString("Preset" + find + "Freqancy", Freqancy.text);
        PlayerPrefs.SetInt("PresetsCount", PlayerPrefs.GetInt("PresetsCount", 0) + 1);

        PlayerPrefs.Save();
        UpdatePresetsList();
    }

    public void DeletePreset()
    {
        int find = -1;
        string presetName = PreasetName.text;
        for (int i = 0; i < PlayerPrefs.GetInt("PresetsCount", 0); i++)
        {
            if (PresetChoise.options[i].text == presetName)
            {
                find = i;
                break;
            }
        }
        if (find > -1)
        {
            for (int i = find+1; i < PlayerPrefs.GetInt("PresetsCount", 0); i++)
            {
                PlayerPrefs.SetString("Preset" + (i-1) + "Name", PlayerPrefs.GetString("Preset" + i + "Name", "Fail"));
                PlayerPrefs.SetString("Preset" + (i-1) + "Port", PlayerPrefs.GetString("Preset" + i + "Port", "Fail"));
                PlayerPrefs.SetString("Preset" + (i-1) + "Freqancy", PlayerPrefs.GetString("Preset" + i + "Freqancy", "0000"));
            }
            int c = PlayerPrefs.GetInt("PresetsCount", 0);
            PlayerPrefs.DeleteKey("Preset" + c + "Name");
            PlayerPrefs.DeleteKey("Preset" + c + "Port");
            PlayerPrefs.DeleteKey("Preset" + c + "Freqancy");
            PlayerPrefs.SetInt("PresetsCount", PlayerPrefs.GetInt("PresetsCount", 0) - 1);
            PlayerPrefs.Save();
        }

    }

    // Update is called once per frame
    public void Run()
    {
        PlayerPrefs.SetString("Port", Port.text);
        PlayerPrefs.SetInt("Freqancy", int.Parse(Freqancy.text));
        PlayerPrefs.SetInt("Bone", Bone.value);

        for (int i = 0; i < (int)Skeleton.Indexs.Lenght; i++)
        {
            PlayerPrefs.SetInt("IsContoleable" + i, Toggles[i].isOn ? 1 : 0 );
            PlayerPrefs.SetInt("NumberMap" + i, Dropdowns[i].value);
        }

        PlayerPrefs.SetString("PresetName", PreasetName.text);

        PlayerPrefs.Save();
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        Application.Quit();
    }

    
}

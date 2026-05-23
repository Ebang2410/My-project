using UnityEngine;

public class ResolutionGame : MonoBehaviour
{
    int height;
    int width;
    private void Start() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 240;
        height = Screen.height;
        width = Screen.width;
        
        if(!PlayerPrefs.HasKey("Graphisme"))
            MeduimResolution();
        else
        {
            switch (PlayerPrefs.GetString("Grasphime"))
            {
                case "High":
                    Highresolution();
                break;

                case "Meduim" :
                    MeduimResolution();
                break;

                case "Low" :
                    LowResolution();
                break;
                case "Ultra" :
                    Ultraresolution();
                break;
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Ultraresolution()
    {
        QualitySettings.SetQualityLevel(2);
        Resolution(1f);
        PlayerPrefs.SetString("Grasphime","High");
    }

    public void Highresolution()
    {
        QualitySettings.SetQualityLevel(2);
        Resolution(.75f);
        PlayerPrefs.SetString("Grasphime","High");
    }

    public void MeduimResolution()
    {
        QualitySettings.SetQualityLevel(1);
        Resolution(.75f);
        PlayerPrefs.SetString("Grasphime","Meduim");
    }

    public void LowResolution()
    {
        QualitySettings.SetQualityLevel(0);
        Resolution(.5f);
        PlayerPrefs.SetString("Grasphime","Low");
    }

    void Resolution(float niv)
    {
        if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            Screen.SetResolution( (int)(width * niv),(int)(height * niv),true);
    }
}

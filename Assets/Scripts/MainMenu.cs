using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour{
    
    public Button StartButton;
    public Button LoadButton;
    public Button OptionsButton;
    public Button QuitButton;

    private void Start(){
        // Set initial states
        var text_StartButton = StartButton.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        var text_LoadButton = LoadButton.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        var text_OptionsButton = OptionsButton.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        var text_QuitButton = QuitButton.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();

        text_StartButton.text = "StartButton varset";
        text_LoadButton.text = "LoadButton varset";
        text_OptionsButton.text = "OptionsButton varset";
        text_QuitButton.text = "QuitButton varset";

        // Add listeners for UI elements
        StartButton.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(text_StartButton));
        LoadButton.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(text_LoadButton));
        OptionsButton.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(text_OptionsButton));
        QuitButton.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(text_QuitButton));


        // myToggle.onValueChanged.AddListener(OnToggleValueChanged);
        // mySlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnButtonClick(TMP_Text pressed){
        pressed.text = "Button Clicked!";
    }
}

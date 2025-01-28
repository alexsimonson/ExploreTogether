using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITesting : MonoBehaviour{
    
    public Button button1;
    public Button button2;
    public Button button3;

    private void Start(){
        // Set initial states
        var text_button1 = button1.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        var text_button2 = button2.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        var text_button3 = button3.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();

        text_button1.text = "Changed Setting";
        text_button2.text = "UITesting.cs";
        text_button3.text = "Yay";
        // myImage.enabled = myToggle.isOn;

        // Add listeners for UI elements
        button1.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(text_button1));
        button2.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(text_button2));
        button3.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(text_button3));


        // myToggle.onValueChanged.AddListener(OnToggleValueChanged);
        // mySlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnButtonClick(TMP_Text pressed){
        pressed.text = "Button Clicked!";
    }
}

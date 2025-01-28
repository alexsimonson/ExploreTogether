using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuLogicUI : MonoBehaviour{

    [Header("Main Menu UI")]
    public Canvas MainMenuCanvas;
    public Button NewButton;
    public Button LoadButton;
    public Button OptionsButton;
    public Button QuitButton;

    [Header("New Character UI")]
    public Canvas NewCanvas;
    public TMP_InputField CharacterNameInput;
    public Toggle HardcoreToggle;
    public Button StartButton;
    public Button NewBackButton;

    [Header("Load Character UI")]
    public Canvas LoadCanvas;
    public Button LoadBackButton;

    [Header("Options UI")]
    public Canvas OptionsCanvas;
    public Slider VolumeSlider;
    public Toggle TestToggle;
    public Button OptionsBackButton;


    private void Start(){
        // start all canvas except MainMenu hidden
        SwitchCanvas(MainMenuCanvas);

        // Add listeners for Main Menu UI elements
        NewButton.GetComponent<Button>().onClick.AddListener(() => SwitchCanvas(NewCanvas));
        LoadButton.GetComponent<Button>().onClick.AddListener(() => SwitchCanvas(LoadCanvas));
        OptionsButton.GetComponent<Button>().onClick.AddListener(() => SwitchCanvas(OptionsCanvas));
        QuitButton.GetComponent<Button>().onClick.AddListener(QuitGame);

        // Add listeners for New Character UI elements
        StartButton.GetComponent<Button>().onClick.AddListener(InitializeNewStart);

        // Add listeners for Back Buttons in all UI elements
        NewBackButton.GetComponent<Button>().onClick.AddListener(() => SwitchCanvas(MainMenuCanvas));
        LoadBackButton.GetComponent<Button>().onClick.AddListener(() => SwitchCanvas(MainMenuCanvas));
        OptionsBackButton.GetComponent<Button>().onClick.AddListener(() => SwitchCanvas(MainMenuCanvas));

        void SwitchCanvas(Canvas visibleCanvas){
            // given all available canvas options, show the one that is passed in
            MainMenuCanvas.gameObject.SetActive(false);
            NewCanvas.gameObject.SetActive(false);
            LoadCanvas.gameObject.SetActive(false);
            OptionsCanvas.gameObject.SetActive(false);
            visibleCanvas.gameObject.SetActive(true);
        }

        void InitializeNewStart(){
            MakeNewCharacter();
            // StartGame();
        }

        bool MakeNewCharacter(){
            string san_input = SanitizeCharacterName(CharacterNameInput.text);
            if(san_input.Length < 1){
                Debug.Log("Character name too short.  Must be between 1 and 20 characters.")
                return false;
            }
            if(san_input.Length > 20){
                Debug.Log("Character name too long.  Must be between 1 and 20 characters.")
                return false;
            }
        }

        string SanitizeCharacterName(string raw_input){
            Debug.Log("Testing raw_input: " + raw_input);
            string san_input = raw_input.Trim();
            san_input = san_input.Replace("<", "&lt;").Replace(">", "&gt;"); // Escape HTML tags
            san_input = san_input.Replace(";", ""); // Optionally, remove semicolons (to prevent code injection)
            // Add any other rules
            return san_input;
        }

        void StartGame(CharacterData chosenCharacter){
            Debug.Log("This should launch the game with player data");
        }

        void QuitGame(){
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }

    private void OnButtonClick(TMP_Text pressed){
        pressed.text = "Button Clicked!";
    }
}

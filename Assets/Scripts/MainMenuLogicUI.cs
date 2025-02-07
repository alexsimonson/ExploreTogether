using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ExploreTogether{

    public class MainMenuLogicUI : MonoBehaviour{

        private string character_save_path;
        private List<Button> character_select_buttons;

        public Manager manager;

        [Header("Main Menu UI")]
        public RectTransform MainMenuPanel;
        public Button NewButton;
        public Button LoadButton;
        public Button OptionsButton;
        public Button QuitButton;

        [Header("New Character UI")]
        public RectTransform NewPanel;
        public TMP_InputField CharacterNameInput;
        public Toggle HardcoreToggle;
        public TMP_Dropdown GameModeDropdown;
        public Button StartButton;
        public Button NewBackButton;

        [Header("Load Character UI")]
        public RectTransform LoadPanel;
        public TMP_Text SelectedCharacterText;
        public Button LoadLoadButton;   // this name really is coding at its finest
        public Button LoadDeleteButton;
        public Button LoadBackButton;
        public Transform LoadCharacterContent;
        private string selected_character_name;

        [Header("Options UI")]
        public RectTransform OptionsPanel;
        public Slider VolumeSlider;
        public Toggle TestToggle;
        public Button OptionsBackButton;


        private void Awake(){
            manager = GameObject.Find("Manager").GetComponent<Manager>();
            // start all Panel except MainMenu hidden

            // Add listeners for Main Menu UI elements
            NewButton.GetComponent<Button>().onClick.AddListener(() => SwitchPanel(NewPanel));
            LoadButton.GetComponent<Button>().onClick.AddListener(HandleLoad);
            OptionsButton.GetComponent<Button>().onClick.AddListener(() => SwitchPanel(OptionsPanel));
            QuitButton.GetComponent<Button>().onClick.AddListener(QuitGame);

            // Add listeners for New Character UI elements
            StartButton.GetComponent<Button>().onClick.AddListener(InitializeNewStart);

            // Add listeners for Load Character UI elements
            LoadLoadButton.GetComponent<Button>().onClick.AddListener(() => AttemptLoadGame(selected_character_name));
            LoadDeleteButton.GetComponent<Button>().onClick.AddListener(() => {
                manager.file.DeleteCharacterData(selected_character_name);// need to delete button from view, should just re-render existing elements... 
                ReloadCharacters();
            });

            // Add listeners for Back Buttons in all UI elements
            NewBackButton.GetComponent<Button>().onClick.AddListener(() => SwitchPanel(MainMenuPanel));
            LoadBackButton.GetComponent<Button>().onClick.AddListener(() => SwitchPanel(MainMenuPanel));
            OptionsBackButton.GetComponent<Button>().onClick.AddListener(() => SwitchPanel(MainMenuPanel));
        }

        public void InitializeMainMenuPanels(){
            Debug.Log("Initializing main menu panels...");
            SwitchPanel(MainMenuPanel);
        }

        public void SwitchPanel(RectTransform visiblePanel){
            // given all available Panel options, show the one that is passed in
            MainMenuPanel.gameObject.SetActive(false);
            NewPanel.gameObject.SetActive(false);
            LoadPanel.gameObject.SetActive(false);
            OptionsPanel.gameObject.SetActive(false);
            // this allows us to turn all off with this function
            if(visiblePanel!=null){
                visiblePanel.gameObject.SetActive(true);
            }
        }

        void InitializeNewStart(){
            manager.chosen_character_data = MakeNewCharacter();
            if(manager.chosen_character_data==null){
                Debug.LogError("Error making new character");
            }else{
                StartGame(false);
            }
        }

        public CharacterData MakeNewCharacter(){
            string sanitized_name = SanitizeCharacterName(CharacterNameInput.text);
            if(sanitized_name==null){
                Debug.Log("Name already exists");
                return null;
            }
            if(sanitized_name.Length < 1){
                Debug.Log("Character name too short.  Must be between 1 and 20 characters.");
                return null;
            }
            if(sanitized_name.Length > 20){
                Debug.Log("Character name too long.  Must be between 1 and 20 characters.");
                return null;
            }
            // character name is ok
            CharacterData character_data = ScriptableObject.CreateInstance("CharacterData") as CharacterData;
            character_data.character_name = sanitized_name;
            character_data.hardcore = HardcoreToggle.isOn;
            character_data.game_mode = (GameMode.Mode)GameModeDropdown.value;
            character_data.health = 100;
            character_data.experience = 0;
            character_data.position = new Vector3(0, 1.5f, 0);  // initial starting position of generation

            // Convert the object to JSON
            if(manager.file.SaveCharacterData(character_data, true)==false){
                Debug.LogError("Refusing to overwrite existing save game data as new character.");
                return null;
            }
            return character_data;
        }

        string SanitizeCharacterName(string raw_input){
            Debug.Log("Testing raw_input: " + raw_input);
            string san_input = raw_input.Trim();
            san_input = san_input.Replace("<", "&lt;").Replace(">", "&gt;"); // Escape HTML tags
            san_input = san_input.Replace(";", ""); // Optionally, remove semicolons (to prevent code injection)
            // make sure character with name doesn't already exist
            if(manager.file.CheckNameExists(san_input)){
                return null;
            }
            return san_input;
        }

        void HandleLoad(){
            SwitchPanel(LoadPanel);
            ReloadCharacters();
        }

        // need a function to handle reloading the load characters list
        void ReloadCharacters(){
            ClearCharactersFromScrollView();
            RenderCharactersScrollView();
        }

        public void RenderCharactersScrollView(){
            AddCharactersToScrollView(manager.file.GetCharacterList(manager.file.GetSavesInDir()));
        }      

        void ClearCharactersFromScrollView(){
            foreach(Transform child in LoadCharacterContent){
                Destroy(child.gameObject);
            }
        }

        void AttemptLoadGame(string character_name){
            manager.chosen_character_data = manager.file.LoadCharacterData(character_name);
            if(manager.chosen_character_data==null){
                Debug.LogError("attempted to load data but nothing came back");
            }else{
                // setup player shit, then start game...
                // start looping through the manager.chosen_character_data for inventory/gear
                if(manager.chosen_character_data.inventory!=null){
                    bool import_inv_result = manager.player_inventory.ImportInventory(manager.chosen_character_data.inventory, true);
                }

                if(manager.chosen_character_data.gear!=null){
                    bool import_inv_result = manager.player_gear.ImportGear(manager.chosen_character_data.gear, true);
                }
                // game mode dependent stuff should happen within that class...
                StartGame(true);
            }
        }

        void UpdateSelectedCharacter(string character_name, Button _button_component){
            selected_character_name = character_name;
            SelectedCharacterText.text = selected_character_name;
            // set selected color differently
            foreach (Transform _character_select_button in LoadCharacterContent.transform){
                _character_select_button.gameObject.GetComponent<Image>().color = Color.blue;
            }
            GameObject.Find(character_name + "_select_button").GetComponent<Image>().color = Color.green;
        }

        // Method to add buttons dynamically
        void AddCharactersToScrollView(List<string> character_list){
            float buttonHeight = 40f;  // Height of each button
            float spacing = 10f;  // Spacing between buttons
            float yOffset = -30f;  // Initialize the Y offset for positioning
            character_select_buttons = new List<Button>();
            foreach(string character_name in character_list){
                Debug.Log("Add Button to list with charname: " + character_name);
                // Create a new button GameObject
                GameObject newButton = new GameObject(character_name + "_select_button");

                // Add the Button component to the GameObject
                Button buttonComponent = newButton.AddComponent<Button>();
                RectTransform nbrt = newButton.AddComponent<RectTransform>();
                Image nbImage = newButton.AddComponent<Image>();

                // Set the button as a child of the content panel FIRST

                // Add a Text component for the button's label
                GameObject buttonText = new GameObject("Text");
                RectTransform btrt = buttonText.AddComponent<RectTransform>();
                btrt.sizeDelta = new Vector2(160, buttonHeight);
                buttonText.transform.SetParent(newButton.transform, false);
                Text text = buttonText.AddComponent<Text>();
                text.text = character_name;
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");  // Use a built-in font
                text.alignment = TextAnchor.MiddleCenter;
                text.fontSize = 30;
                text.verticalOverflow = VerticalWrapMode.Overflow;

                // Style the button (set size, background color, etc.)
                nbrt.sizeDelta = new Vector2(160, buttonHeight);
                nbrt.anchorMin = new Vector2(0.5f, 1);  // Center horizontally, start from top
                nbrt.anchorMax = new Vector2(0.5f, 1);
                nbrt.pivot = new Vector2(0.5f, 1); // Pivot at top-center
                nbrt.anchoredPosition = new Vector2(0f, -yOffset);  // Positioning
                nbImage.color = Color.blue;  // Button background color
                // Set the button as a child of the content panel
                newButton.transform.SetParent(LoadCharacterContent, false);
                // Increase the Y offset to prevent overlapping (button height + spacing)
                yOffset += buttonHeight + spacing;
                character_select_buttons.Add(buttonComponent);
                buttonComponent.onClick.AddListener(() => UpdateSelectedCharacter(character_name, buttonComponent));
            }
            // adjust the size of scroll view
        }

        void StartGame(bool _isLoading){
            // eventually this should be changed so that we setup an enum or something for mode types, and use that for both the dropdown and this...
            // it's probably already setup...
            manager.lobby_mode = manager.chosen_character_data.game_mode;
            manager.game_mode = manager.LoadGameMode();
            manager.game_mode.SpawnMap(_isLoading);
            // manager.player.transform.position = chosenCharacterData.position;
            SwitchPanel(null);
        }

        void QuitGame(){
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                Debug.Log("Quit Game Unity Editor");
            #else
                Application.Quit();
                Debug.Log("Quitting application");
            #endif
        }

        private void OnButtonClick(TMP_Text pressed){
            pressed.text = "Button Clicked!";
        }
    }

}

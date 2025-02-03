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
            Debug.Log("Type of thing: " + MainMenuPanel.gameObject.name);
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

            // Add listeners for Back Buttons in all UI elements
            NewBackButton.GetComponent<Button>().onClick.AddListener(() => SwitchPanel(MainMenuPanel));
            LoadBackButton.GetComponent<Button>().onClick.AddListener(() => SwitchPanel(MainMenuPanel));
            OptionsBackButton.GetComponent<Button>().onClick.AddListener(() => SwitchPanel(MainMenuPanel));
        }

        public void InitializeMainMenuPanels(){
            Debug.Log("Initializing main menu panels...");
            SwitchPanel(MainMenuPanel);
        }

        void SwitchPanel(RectTransform visiblePanel){
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
            CharacterData new_character = MakeNewCharacter();
            if(new_character==null){
                Debug.LogError("Error making new character");
            }else{
                StartGame(new_character);
            }
        }

        CharacterData MakeNewCharacter(){
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
            character_data.name = sanitized_name;
            character_data.game_mode = (GameMode.Mode)GameModeDropdown.value;
            character_data.health = 100;
            character_data.experience = 0;
            character_data.position = new Vector3(0, 1.5f, 0);  // initial starting position of generation

            // Convert the object to JSON
            if(SaveCharacterData(character_data, true)==false){
                Debug.LogError("Refusing to overwrite existing save game data as new character.");
                return null;
            }
            return character_data;
        }

        CharacterData LoadCharacterData(string character_name){
            Debug.Log("LOADING CHARACTER NAME: " + character_name);
            string filePath = CreateCharacterSaveFileName(character_name, true);
            if (File.Exists(filePath)==false){
                Debug.LogError("No character data to load");
                return null;
            }
            string jsonData = File.ReadAllText(filePath);
            CharacterData loaded_data = ScriptableObject.CreateInstance<CharacterData>();
            JsonUtility.FromJsonOverwrite(jsonData, loaded_data);
            return loaded_data;
        }

        bool SaveCharacterData(CharacterData _character_data, bool isNew=false){
            string filePath = CreateCharacterSaveFileName(_character_data.name, true);
            if(File.Exists(filePath)==true && isNew){
                Debug.LogError("Save file already exists.  Preventing overwrite on new character.");
                return false;
            }
            // Convert the object to JSON
            string character_json_data = JsonUtility.ToJson(_character_data);
            // Write the JSON string to a file
            File.WriteAllText(filePath, character_json_data);
            return true;
        }

        string CreateCharacterSaveFileName(string character_name, bool fullPath=false){
            if(fullPath==true){
                return Application.persistentDataPath + "\\" + character_name + "_character_save.json";
            }else{
                return character_name + "_character_save.json";
            }
        }

        string SanitizeCharacterName(string raw_input){
            Debug.Log("Testing raw_input: " + raw_input);
            string san_input = raw_input.Trim();
            san_input = san_input.Replace("<", "&lt;").Replace(">", "&gt;"); // Escape HTML tags
            san_input = san_input.Replace(";", ""); // Optionally, remove semicolons (to prevent code injection)
            // make sure character with name doesn't already exist
            if(CheckNameExists(san_input)){
                return null;
            }
            return san_input;
        }

        void HandleLoad(){
            SwitchPanel(LoadPanel);
            AddCharactersToScrollView(GetCharacterList(GetSavesInDir()));
        }

        void AttemptLoadGame(string character_name){
            manager.chosen_character_data = LoadCharacterData(character_name);
            if(manager.chosen_character_data==null){
                Debug.LogError("attempted to load data but nothing came back");
            }else{
                // setup player shit, then start game...
                // start looping through the manager.chosen_character_data for inventory/gear
                if(manager.chosen_character_data.inventory!=null){
                    bool import_inv_result = manager.player_inventory.ImportInventory(manager.chosen_character_data.inventory);
                }

                if(manager.chosen_character_data.gear!=null){
                    bool import_inv_result = manager.player_gear.ImportGear(manager.chosen_character_data.gear);
                }
                // game mode dependent stuff should happen within that class...
                StartGame(manager.chosen_character_data);
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

        void StartGame(CharacterData chosenCharacterData){
            Debug.Log("This should launch the game with player data");
            Debug.Log("TESTING USE OF NAME: " + chosenCharacterData.name);
            Debug.Log("chosen char data: " + JsonUtility.ToJson(chosenCharacterData));
            manager.chosen_character_data = chosenCharacterData;
            // eventually this should be changed so that we setup an enum or something for mode types, and use that for both the dropdown and this...
            // it's probably already setup...
            manager.lobby_mode = chosenCharacterData.game_mode;
            manager.game_mode = manager.LoadGameMode();
            manager.game_mode.SpawnMap();
            manager.player.transform.position = chosenCharacterData.position;
            SwitchPanel(null);
        }

        void QuitGame(){
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                Debug.Log("TESTING ONE PLACE");
            #else
                Application.Quit();
                Debug.Log("TESTING ANOTHER PLACE");
            #endif
        }

        string[] GetSavesInDir(){
            var path = Application.persistentDataPath;
            if (Directory.Exists(path)==false){    
                // directory doesn't exist
                return Array.Empty<string>();
            }
            // Get all file paths in the directory
            string[] filePaths = Directory.GetFiles(path);

            if(filePaths==null || filePaths.Length==0){
                // no save files found
                return Array.Empty<string>();
            }

            // return all files found
            return filePaths;
        }

        // form character list from result of GetSavesInDir
        List<string> GetCharacterList(string[] character_save_paths){
            List<string> character_list = new List<string>();
            if(character_save_paths==null || character_save_paths.Length==0){
                // no save files found
                return new List<string>();
            }
            // Iterate through the file paths and print them to the console
            foreach (var filePath in character_save_paths){
                string found_name = filePath.Replace(Application.persistentDataPath + "\\", "").Replace("_character_save.json", "");
                // Debug.Log("Character name found: " + found_name);
                character_list.Add(found_name);
            }
            return character_list;
        }

        // check if character with name already exists in file dir
        bool CheckNameExists(string check_name){
            string[] filePaths = GetSavesInDir();

            if(filePaths==null || filePaths.Length==0){
                // no save files found
                return false;
            }else{
                // Iterate through the file paths and print them to the console
                foreach (var filePath in filePaths){
                    string found_name = ParseName(filePath);
                    // Debug.Log("Character name found: " + found_name);
                    if(found_name==check_name){
                        return true;
                    }
                }
            }
            return false;
        }

        string ParseName(string parse_this){
            return parse_this.Replace(Application.persistentDataPath + "\\", "").Replace("_character_save.json", "");
        }

        private void OnButtonClick(TMP_Text pressed){
            pressed.text = "Button Clicked!";
        }

        void OnApplicationQuit(){
            // this seems to work for editor and quit button... probably also works for X button
            Debug.Log("ON APPLICATION QUIT TESTING");
            manager.chosen_character_data.gear = manager.player_gear.ExportGear();
            manager.chosen_character_data.inventory = manager.player_inventory.ExportInventory();
            manager.chosen_character_data.dungeon_nodes = manager.map.GetComponent<Maze>().ExportMaze();
            // get latest character data and save
            SaveCharacterData(manager.chosen_character_data);
        }
    }

}

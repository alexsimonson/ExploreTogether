using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuLogicUI : MonoBehaviour{

    private string character_save_path;

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
    public Transform LoadCharacterContent;

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
        LoadButton.GetComponent<Button>().onClick.AddListener(HandleLoad);
        OptionsButton.GetComponent<Button>().onClick.AddListener(() => SwitchCanvas(OptionsCanvas));
        QuitButton.GetComponent<Button>().onClick.AddListener(QuitGame);

        // Add listeners for New Character UI elements
        StartButton.GetComponent<Button>().onClick.AddListener(InitializeNewStart);

        // Add listeners for Back Buttons in all UI elements
        NewBackButton.GetComponent<Button>().onClick.AddListener(() => SwitchCanvas(MainMenuCanvas));
        LoadBackButton.GetComponent<Button>().onClick.AddListener(() => SwitchCanvas(MainMenuCanvas));
        OptionsBackButton.GetComponent<Button>().onClick.AddListener(() => SwitchCanvas(MainMenuCanvas));

    }

    void SwitchCanvas(Canvas visibleCanvas){
        // given all available canvas options, show the one that is passed in
        MainMenuCanvas.gameObject.SetActive(false);
        NewCanvas.gameObject.SetActive(false);
        LoadCanvas.gameObject.SetActive(false);
        OptionsCanvas.gameObject.SetActive(false);
        visibleCanvas.gameObject.SetActive(true);
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
        character_data.health = 100;
        character_data.experience = 0;

        // Convert the object to JSON
        string character_json_data = JsonUtility.ToJson(character_data);
        character_save_path = Path.Combine(Application.persistentDataPath, CreateCharacterSaveFileName(sanitized_name));
            // Write the JSON string to a file
        File.WriteAllText(character_save_path, character_json_data);
        return character_data;
    }

    CharacterData LoadCharacterData(string character_name){
        string filePath = CreateCharacterSaveFileName(character_name, true);
        if (File.Exists(filePath)){
            string jsonData = File.ReadAllText(filePath);
            CharacterData loaded_data = ScriptableObject.CreateInstance<CharacterData>();
            JsonUtility.FromJsonOverwrite(jsonData, loaded_data);
            return loaded_data;
        }
        // error otherwise...
        Debug.LogError("No saved character data found.  Why are we calling this?");
        return null;
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
        SwitchCanvas(LoadCanvas);
        List<string> character_list = GetCharacterList(GetSavesInDir());
        AddCharactersToScrollView(character_list);
    }

    void AttemptLoadGame(string character_name){
        CharacterData loaded_data = LoadCharacterData(character_name);
        if(loaded_data==null){
            Debug.LogError("attempted to load data but nothing came back");
        }else{
            StartGame(loaded_data);
        }
    }

    // Method to add buttons dynamically
    void AddCharactersToScrollView(List<string> character_list){
        float buttonHeight = 40f;  // Height of each button
        float spacing = 10f;  // Spacing between buttons
        float yOffset = -110f;  // Initialize the Y offset for positioning
        foreach(string character_name in character_list){
            Debug.Log("Add Button to list with charname: " + character_name);
            // Create a new button GameObject
            GameObject newButton = new GameObject(character_name + "_select_button");

            // Add the Button component to the GameObject
            Button buttonComponent = newButton.AddComponent<Button>();
            RectTransform rt = newButton.AddComponent<RectTransform>();
            Image nbImage = newButton.AddComponent<Image>();

            // Add a Text component for the button's label
            GameObject buttonText = new GameObject("Text");
            buttonText.transform.SetParent(newButton.transform, false);
            Text text = buttonText.AddComponent<Text>();
            text.text = character_name;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");  // Use a built-in font
            text.alignment = TextAnchor.MiddleCenter;

            // Style the button (set size, background color, etc.)
            RectTransform rectTransform = newButton.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(160, buttonHeight);  // Adjust button size as needed
            nbImage.color = Color.green;  // Button background color

            // Set the button as a child of the content panel
            newButton.transform.SetParent(LoadCharacterContent, false);

            // Adjust the position of the button (manually setting the Y position)
            rectTransform.anchoredPosition = new Vector2(0f, -yOffset);

            // Increase the Y offset to prevent overlapping (button height + spacing)
            yOffset += buttonHeight + spacing;


            // Optionally add a click event listener to the button
            buttonComponent.onClick.AddListener(() => AttemptLoadGame(character_name));
        }
    }

    void StartGame(CharacterData chosenCharacterData){
        Debug.Log("This should launch the game with player data");
        Debug.Log("TESTING USE OF NAME: " + chosenCharacterData.name);
        Debug.Log("chosen char data: " + JsonUtility.ToJson(chosenCharacterData));
    }

    void QuitGame(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
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
}

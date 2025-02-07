using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

namespace ExploreTogether {
    public class FileHandler : ScriptableObject {

        public bool DeleteCharacterData(string _character_name){
            string filePath = CreateCharacterSaveFileName(_character_name, true);
            if(File.Exists(filePath)==false){
                Debug.LogError("Save file '" + filePath + "' doesn't exist.  Nothing to delete.");
                return false;
            }
            File.Delete(filePath);
            // should check filepath again if file exists...
            if(File.Exists(filePath)!=false){
                Debug.LogError("Failed to delete file.");
                return false;
            }
            return true;
        }

        public CharacterData LoadCharacterData(string character_name){
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

        public bool SaveCharacterData(CharacterData _character_data, bool isNew=false){
            string filePath = CreateCharacterSaveFileName(_character_data.character_name, true);
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

        public string[] GetSavesInDir(){
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

        public string CreateCharacterSaveFileName(string character_name, bool fullPath=false){
            if(fullPath==true){
                return Application.persistentDataPath + "\\" + character_name + "_character_save.json";
            }else{
                return character_name + "_character_save.json";
            }
        }

        // check if character with name already exists in file dir
        public bool CheckNameExists(string check_name){
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

        // form character list from result of GetSavesInDir
        public List<string> GetCharacterList(string[] character_save_paths){
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
    }
}

using Microsoft.Xna.Framework.Input;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using Windows.Storage;
using System.Diagnostics;

namespace LittleBattle.Model

{
    public class KeyMappingsManager
    {
        private const string FILENAME_DEFAULT = "default_key_mappings.json";
        private const string FILENAME_CUSTOM = "custom_key_mappings.json";

        public Keys MoveRight { get; set; }
        public Keys MoveLeft { get; set; }
        public Keys MoveUp { get; set; }
        public Keys MoveDown{ get; set; }
        public Keys Jump { get; set; }
        public Keys Attack { get; set; }


        public KeyMappingsManager() {
            MoveRight = Keys.D;
            MoveLeft = Keys.A;
            MoveUp = Keys.W;
            MoveDown = Keys.S;
            Jump = Keys.Space;
            Attack = Keys.M;
        }

        public void SaveCustomConfig(KeyMappingsManager customConfig)
        {
            SaveKeyMappings(customConfig,FILENAME_CUSTOM);
        }

        public async void LoadKeyMappings()
        {
            try
            {
                KeyMappingsManager keyMappings;

                if (File.Exists(Path.Combine(ApplicationData.Current.LocalFolder.Path, FILENAME_CUSTOM)))
                {
                    string jsonConfig = await GetJsonConfig();
                    keyMappings = JsonSerializer.Deserialize<KeyMappingsManager>(jsonConfig);
                    MoveRight = keyMappings.MoveRight;
                    MoveLeft = keyMappings.MoveLeft;
                    MoveUp = keyMappings.MoveUp;
                    MoveDown = keyMappings.MoveDown;
                    Jump = keyMappings.Jump;
                    Attack = keyMappings.Attack;
                    return;
                }

                keyMappings = new KeyMappingsManager();
                SaveKeyMappings(keyMappings,FILENAME_DEFAULT);
                
            }
            catch (Exception ex) { 
                Debug.WriteLine(ex.Message); ;
            }
            
        }
        public async void SaveKeyMappings(KeyMappingsManager keyMappingsManager,string filename)
        {
            try
            {
                String json = JsonSerializer.Serialize(keyMappingsManager);
                StorageFile file;

                if (File.Exists(Path.Combine(ApplicationData.Current.LocalFolder.Path, filename)) == false)
                {
                    file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename);
                    return;
                }

                file = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);           

                await FileIO.WriteTextAsync(file, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public async Task<String> GetJsonConfig()
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(FILENAME_DEFAULT);
                string jsonData = await FileIO.ReadTextAsync(file);
                return jsonData;
            }
            catch (FileNotFoundException ex)
            {
                Debug.WriteLine("Arquivo de configuração não encontrado. " + ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;    
            }

        }
    }
}

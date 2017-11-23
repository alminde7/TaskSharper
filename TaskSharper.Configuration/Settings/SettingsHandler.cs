using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TaskSharper.Configuration.Settings
{
    public class SettingsHandler<T> where T : new()
    {
        protected string FilePath;
        public async Task<T> Load()
        {
            // https://www.newtonsoft.com/json/help/html/DeserializeWithJsonSerializerFromFile.htm
            try
            {
                T model;
                using (StreamReader file = File.OpenText(FilePath))
                {
                    var jsonString = await file.ReadToEndAsync();
                    model = JsonConvert.DeserializeObject<T>(jsonString, new JsonSerializerSettings()
                    {
                        // https://stackoverflow.com/questions/29113063/json-net-why-does-it-add-to-list-instead-of-overwriting
                        // Settings this will replace all values in object instead of appending to it. 
                        // Needed because of lists.
                        ObjectCreationHandling = ObjectCreationHandling.Replace
                    });
                }
                return model;
            }
            catch (FileNotFoundException)
            {
                // File does not exist - create it. 
                Save(new T());
            }
            return new T();
        }

        public void Save(T obj)
        {
            // https://www.newtonsoft.com/json/help/html/SerializeWithJsonSerializerToFile.htm
            using (StreamWriter file = File.CreateText(FilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, obj);
            }
        }
    }
}
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
            try
            {
                T model;
                using (StreamReader file = File.OpenText(FilePath))
                {
                    var jsonString = await file.ReadToEndAsync();
                    model = JsonConvert.DeserializeObject<T>(jsonString);
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
            using (StreamWriter file = File.CreateText(FilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, obj);
            }
        }
    }
}
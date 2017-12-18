using System.IO;
using Newtonsoft.Json;

namespace TaskSharper.Configuration.Settings
{
    /// <summary>
    /// Handles load and save of settings files to disk.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SettingsHandler<T> where T : new()
    {
        /// <summary>
        /// IOLock used a mutex to control access to file system.
        /// </summary>
        private object IOLock = new object();

        /// <summary>
        /// Path to file, including the name of the file.
        /// </summary>
        protected string FilePath;

        /// <summary>
        /// Load settings from disk using FilePath. 
        /// </summary>
        /// <returns>Return settings object</returns>
        public T Load()
        {
            // https://www.newtonsoft.com/json/help/html/DeserializeWithJsonSerializerFromFile.htm
            try
            {
                T model;

                lock (IOLock)
                {
                    using (StreamReader file = File.OpenText(FilePath))
                    {
                        var jsonString = file.ReadToEnd();
                        model = JsonConvert.DeserializeObject<T>(jsonString, new JsonSerializerSettings()
                        {
                            // https://stackoverflow.com/questions/29113063/json-net-why-does-it-add-to-list-instead-of-overwriting
                            // Settings this will replace all values in object instead of appending to it. 
                            // Needed because of lists.
                            ObjectCreationHandling = ObjectCreationHandling.Replace
                        });
                    }
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

        /// <summary>
        /// Save settings to JSON file on disk
        /// </summary>
        /// <param name="obj">The settings object</param>
        public void Save(T obj)
        {
            // https://www.newtonsoft.com/json/help/html/SerializeWithJsonSerializerToFile.htm
            lock (IOLock)
            {
                using (StreamWriter file = File.CreateText(FilePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, obj);
                }
            }
        }
    }
}
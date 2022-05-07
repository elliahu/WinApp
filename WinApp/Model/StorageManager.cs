using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WinApp.Model
{
    public class StorageManager
    {
        public static readonly Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        private static readonly string prefix = "appdoc-";
        private static readonly string suffix = ".json";

        private static readonly string eventPrefix = "appev-";

        public static async Task Save(Document data)
        {
            Windows.Storage.StorageFile file = await storageFolder.CreateFileAsync(prefix + data.DocumentId.ToString().ToLower() + suffix, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(file, JsonSerializer.Serialize(data));
        }

        public static async Task<Document> Load(Guid id)
        {
            Windows.Storage.StorageFile file = await storageFolder.GetFileAsync(prefix + id.ToString().ToLower() + suffix);
            var document = JsonSerializer.Deserialize<Document>(await Windows.Storage.FileIO.ReadTextAsync(file));
            return document;
        }

        public static async Task<List<Document>> LoadAll()
        {
            List<Document> dataset = new List<Document>();
            var list = await storageFolder.GetFilesAsync();
            foreach (var file in list.Where(f => f.Name.StartsWith(prefix)))
            {
                var document = JsonSerializer.Deserialize<Document>(await Windows.Storage.FileIO.ReadTextAsync(file));
                dataset.Add(document);
            }
            return dataset;
        }

        public static async Task Delete(Guid id)
        {
            Windows.Storage.StorageFile file = await storageFolder.GetFileAsync(prefix + id.ToString().ToLower() + suffix);
            await file.DeleteAsync();
        }

        public static async Task SaveEvent(CustomEvent data){
            Windows.Storage.StorageFile file = await storageFolder.CreateFileAsync(eventPrefix + data.EventId.ToString().ToLower() + suffix, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(file, JsonSerializer.Serialize(data));
        }

        public static async Task<CustomEvent> LoadEvent(Guid id)
        {
            Windows.Storage.StorageFile file = await storageFolder.GetFileAsync(eventPrefix + id.ToString().ToLower() + suffix);
            var c = JsonSerializer.Deserialize<CustomEvent>(await Windows.Storage.FileIO.ReadTextAsync(file));
            return c;
        }

        public static async Task DeleteEvent(Guid id)
        {
            Windows.Storage.StorageFile file = await storageFolder.GetFileAsync(eventPrefix + id.ToString().ToLower() + suffix);
            await file.DeleteAsync();
        }

        public static async Task<List<CustomEvent>> LoadAllEvents()
        {
            List<CustomEvent> dataset = new List<CustomEvent>();
            var list = await storageFolder.GetFilesAsync();
            foreach (var file in list.Where(f => f.Name.StartsWith(eventPrefix)))
            {
                var document = JsonSerializer.Deserialize<CustomEvent>(await Windows.Storage.FileIO.ReadTextAsync(file));
                dataset.Add(document);
            }
            return dataset;
        }

    }
}

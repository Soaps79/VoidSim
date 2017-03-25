using System.IO;
using System.Linq;
using Assets.Scripts.WorldMaterials;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.WorldMaterials.Editor
{
    public class InventoryEditorViewModel : MonoBehaviour
    {
        public string Filename;
        public Inventory.ProductEntryInfo[] Products;

        // Use this for initialization
        public void SerializeAll()
        {
            var collection = SerializeProducts();
            SaveCollectionToFile(Filename, collection);
        }

        private string SerializeProducts()
        {
            return JsonConvert.SerializeObject(new Inventory.InventoryInfo {
                Name = Filename, Products = Products.ToList()}, Formatting.Indented);
        }

        private void SaveCollectionToFile(string name, string json)
        {
            using (FileStream fs = File.Open(string.Format("Assets/Resources/{0}.json", name), FileMode.Create, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(json);
            }
        }
    }
}

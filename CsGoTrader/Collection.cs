using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CsGoTrader
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Collection
    {
        [JsonProperty]
        public List<Skin> jsonSkins;

        private Dictionary<CollectionGrade, CollectionGradeSkins> skins;
        [JsonProperty]
        public string name;

        public Collection(string name, List<Skin> jsonSkins)
        {
            this.name = name;
            this.jsonSkins = jsonSkins;
        }

        public Collection()
        {
            this.jsonSkins = new List<Skin>();
        }

        public static Collection readCollection(string name, List<Tuple<TextBox, TextBox, TextBox, ComboBox>> controlsList)
        {
            var collection = new Collection();

            collection.name = name;
            foreach(var row in controlsList)
            {
                double minFloatValue = double.Parse(row.Item2.Text);
                double maxFloatValue = double.Parse(row.Item3.Text);
                ComboBoxItem selectedItem = (ComboBoxItem)row.Item4.SelectedItem;
                string selectedValue = (string)selectedItem.Content;
                CollectionGrade collectionGrade = (CollectionGrade)Enum.Parse(typeof(CollectionGrade), selectedValue);

                collection.jsonSkins.Add(new Skin(row.Item1.Text, minFloatValue, maxFloatValue, collectionGrade));
            }

            return collection;
        }
    }
}

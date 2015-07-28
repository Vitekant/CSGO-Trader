using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CsGoTrader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, Collection> collections;

        public MainWindow()
        {
            InitializeComponent();
            InitializeProgram();

        }

        private void InitializeProgram()
        {
            collections = new Dictionary<string, Collection>();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Skin skin = new Skin("AK-47 | Aquamarine Revenge");
            //SteamMarket.getPrices();
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SaveCollectionButton_Click(object sender, RoutedEventArgs e)
        {
            var controlsList = new List<Tuple<TextBox, TextBox, TextBox, ComboBox>>();
            var row = new Tuple<TextBox, TextBox, TextBox, ComboBox>(SkinName1, MinFloatValue1, MaxFloatValue1, CollectionGradeComboBox1);
            controlsList.Add(row);

            var collection = Collection.readCollection(CollectionName.Text, controlsList);
            if (collections.ContainsKey(collection.name))
            {
                collections.Add(collection.name, collection);
            }
            else
            {
                collections[collection.name] = collection;
            }

            RefreshCollectionsBox();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (FileStream fs = File.Open(@"collections.json", FileMode.OpenOrCreate))
            using (StreamWriter sw = new StreamWriter(fs))
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.Formatting = Formatting.Indented;

                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(jw, collections.Values);
            }
            //string output = JsonConvert.SerializeObject(collections.Values);

            //JsonTextReader reader = new JsonTextReader(new StringReader(output));
            //var collection = JsonConvert.DeserializeObject<List<Collection>>(output); 
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(@"collections.json"))
            {
                using (StreamReader file = File.OpenText(@"collections.json"))
                {

                    JsonSerializer serializer = new JsonSerializer();
                    var collectionsList = (List<Collection>)serializer.Deserialize(file, typeof(List<Collection>));
                    foreach (var collection in collectionsList)
                    {
                        if (collections.ContainsKey(collection.name))
                        {
                            collections.Add(collection.name, collection);
                        }
                        else
                        {
                            collections[collection.name] = collection;
                        }
                    }
                }
            }

            RefreshCollectionsBox();
        }

        private void RefreshCollectionsBox()
        {
            CollectionsBox.Items.Clear();
            foreach(var collection in collections)
            {
                var item = new ListBoxItem();
                item.Content = collection.Key;
                CollectionsBox.Items.Add(item);
            }
        }
    }
}

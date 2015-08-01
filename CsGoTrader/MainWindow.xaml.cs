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

        private List<Tuple<TextBox, TextBox, TextBox, ComboBox>> skinsControlsList;

        public MainWindow()
        {
            InitializeComponent();
            InitializeProgram();

        }

        private void InitializeProgram()
        {
            collections = new Dictionary<string, Collection>();
            skinsControlsList = new List<Tuple<TextBox, TextBox, TextBox, ComboBox>>()
            {
                new Tuple<TextBox, TextBox, TextBox, ComboBox>(SkinName1, MinFloatValue1, MaxFloatValue1, CollectionGradeComboBox1)
            };
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SaveCollectionButton_Click(object sender, RoutedEventArgs e)
        {
            var collection = Collection.readCollection(CollectionName.Text, skinsControlsList);
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
                        if (!collections.ContainsKey(collection.name))
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

        private void AddRowButtonClick(object sender, RoutedEventArgs e)
        {
            AddRow();
        }

        private void AddRow()
        {
            var rowNumber = SkinsGrid.RowDefinitions.Count;

            if (rowNumber > 16)
            {
                return;
            }

            var row = new RowDefinition();
            row.Height = new GridLength(30, GridUnitType.Pixel);
            SkinsGrid.RowDefinitions.Add(row);

            var nameBox = new TextBox();
            nameBox.Height = 20;
            nameBox.Width = 240;
            nameBox.Margin = new Thickness(10, 5, 0, 0);
            nameBox.VerticalAlignment = VerticalAlignment.Top;
            nameBox.HorizontalAlignment = HorizontalAlignment.Left;

            var minFloatValueBox = new TextBox();
            minFloatValueBox.Height = 20;
            minFloatValueBox.Width = 30;
            minFloatValueBox.Margin = new Thickness(5, 5, 5, 0);
            minFloatValueBox.VerticalAlignment = VerticalAlignment.Top;
            minFloatValueBox.HorizontalAlignment = HorizontalAlignment.Left;
            minFloatValueBox.Text = "0";

            var maxFloatValueBox = new TextBox();
            maxFloatValueBox.Height = 20;
            maxFloatValueBox.Width = 30;
            maxFloatValueBox.Margin = new Thickness(5, 5, 5, 0);
            maxFloatValueBox.VerticalAlignment = VerticalAlignment.Top;
            maxFloatValueBox.HorizontalAlignment = HorizontalAlignment.Left;
            maxFloatValueBox.Text = "1.0";

            var gradeComboBox = new ComboBox();
            gradeComboBox.Height = 20;
            gradeComboBox.Width = 120;
            gradeComboBox.Margin = new Thickness(5, 5, 0, 0);
            gradeComboBox.VerticalAlignment = VerticalAlignment.Top;
            gradeComboBox.HorizontalAlignment = HorizontalAlignment.Left;
            fillGadeBox(gradeComboBox);
            gradeComboBox.SelectedIndex = 1;

            SkinsGrid.Children.Add(nameBox);
            Grid.SetRow(nameBox, rowNumber);
            Grid.SetColumn(nameBox, 0);

            SkinsGrid.Children.Add(minFloatValueBox);
            Grid.SetRow(minFloatValueBox, rowNumber);
            Grid.SetColumn(minFloatValueBox, 1);

            SkinsGrid.Children.Add(maxFloatValueBox);
            Grid.SetRow(maxFloatValueBox, rowNumber);
            Grid.SetColumn(maxFloatValueBox, 2);

            SkinsGrid.Children.Add(gradeComboBox);
            Grid.SetRow(gradeComboBox, rowNumber);
            Grid.SetColumn(gradeComboBox, 3);

            skinsControlsList.Add(new Tuple<TextBox, TextBox, TextBox, ComboBox>(nameBox, minFloatValueBox, maxFloatValueBox, gradeComboBox));

        }

        private void fillGadeBox(ComboBox gradeComboBox)
        {
            gradeComboBox.FontSize = 9;

            foreach (CollectionGrade quality in Enum.GetValues(typeof(CollectionGrade)))
            {
                gradeComboBox.Items.Add(new ComboBoxItem() { Content = quality.ToString() });
            }
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            var collection = collections[(string)((ListBoxItem)CollectionsBox.SelectedItem).Content];

            OptimizationEngine.checkCollection(collection);
        }

        private void displayCollection(Collection collection)
        {
            while (collection.jsonSkins.Count < skinsControlsList.Count)
            {
                removeRow();
            }

            while (collection.jsonSkins.Count > skinsControlsList.Count)
            {
                AddRow();
            }

            CollectionName.Text = collection.name;

            int k = 0;
            foreach(var row in collection.jsonSkins)
            {
                skinsControlsList.ElementAt(k).Item1.Text = row.name;
                skinsControlsList.ElementAt(k).Item2.Text = row.minFloatValue.ToString();
                skinsControlsList.ElementAt(k).Item3.Text = row.maxFloatValue.ToString();
                skinsControlsList.ElementAt(k).Item4.SelectedIndex = (int)row.collectionGrade - 1;
                k++;
            }
        }

        private void removeRow()
        {
            if(SkinsGrid.RowDefinitions.Count == 1)
            {
                return;
            }

            var controlsToRemove = skinsControlsList.Last();
            var lastRow = SkinsGrid.RowDefinitions.Last();
            SkinsGrid.Children.Remove(controlsToRemove.Item1);
            SkinsGrid.Children.Remove(controlsToRemove.Item2);
            SkinsGrid.Children.Remove(controlsToRemove.Item3);
            SkinsGrid.Children.Remove(controlsToRemove.Item4);

            SkinsGrid.RowDefinitions.Remove(lastRow);
            skinsControlsList.Remove(controlsToRemove);
        }

        private void RemoveRowButtonClick(object sender, RoutedEventArgs e)
        {
            removeRow();
        }

        private void CollectionsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox box = (ListBox)sender;
            if((ListBoxItem)box.SelectedItem == null)
            {
                return;
            }

            var collection = collections[(string)((ListBoxItem)box.SelectedItem).Content];

            displayCollection(collection);
        }
    }
}

using System.Windows;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using Newtonsoft.Json;

namespace DJHCP
{
    public class Custom
    {
        public class Song
        {
            public class SongDetails
            {
                public string name { get; set; } = string.Empty;
                public string artist { get; set; } = string.Empty;
                public string album { get; set; } = string.Empty;
                public string genre { get; set; } = string.Empty;
                public int year { get; set; } = 0;
            }
            public SongDetails first;
            public SongDetails second;
            public string songId { get; set; } = string.Empty;
            public string dj { get; set; } = string.Empty;
            public string charter { get; set; } = string.Empty;
            public double song_length { get; set; } = 0;
            public double preview_start_time { get; set; } = 0;
            public double preview_end_time { get; set; } = 0;
        }
        public class Difficulty
        {
            public class DeckSpeed
            {
                public double deckspeed_beginner { get; set; } = 0;
                public double deckspeed_easy { get; set; } = 0;
                public double deckspeed_medium { get; set; } = 0;
                public double deckspeed_hard { get; set; } = 0;
                public double deckspeed_expert { get; set; } = 0;

            }
            public class Complexity
            {
                public double track_complexity { get; set; } = 0;
                public double tap_complexity { get; set; } = 0;
                public double cross_complexity { get; set; } = 0;
                public double scratch_complexity { get; set; } = 0;

            }
            public double bpm { get; set; } = 0;
            public DeckSpeed deck_speed;
            public Complexity complexity;
        }

        public class Extra
        {
            public class Megamix
            {
                public string playlist_track { get; set; } = string.Empty;
                public string megamix_name { get; set; } = string.Empty;
                public bool megamix_has_intro { get; set; } = false;
                public double megamix_highway_offset { get; set; } = 0;
                public bool megamix_transitions { get; set; } = false;
            }
            public class Id
            {
                public string id_name { get; set; } = string.Empty;
                public string id_artist { get; set; } = string.Empty;
                public string id_name2 { get; set; } = string.Empty;
                public string id_artist2 { get; set; } = string.Empty;
            }
            public Megamix megamix;
            public Id id;
            public double env_start_time { get; set; } = 0;
            public bool battle_music { get; set; } = false;
            public bool guitar_mix { get; set; } = false;
            public bool menu_music { get; set; } = false;
            public string additional_xml { get; set; } = string.Empty;
        }

        public Song song;
        public Difficulty difficulty;
        public Extra extra;
    }

    public partial class MainWindow : Window
    {
        public static List<XmlNode> nodes = new List<XmlNode>();
        public static List<XmlNode> removedNodes = new List<XmlNode>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void saveRemoved()
        {
            if (removedNodes.Count > 0)
            {
                string t = string.Empty;
                t += "<TrackList>";
                foreach (XmlNode n in removedNodes)
                {
                    t += "<Track ";
                    foreach (XmlAttribute attribute in n.Attributes)
                    {
                        t += attribute.Name;
                        t += "=\"";
                        t += attribute.Value;
                        t += "\" ";
                    }
                    t += ">";
                    t += n.InnerXml;
                    t += "</Track>";
                }
                t += "</TrackList>";
                File.WriteAllText("removedTracks.xml", t);
            }

        }

        private void LoadXml(XmlNode root)
        {
            TrackListing.ItemsSource = null;
            XmlNodeList list = root.ChildNodes;
            for (int i = 0; i < list.Count; ++i)
            {
                XmlNode node = list.Item(i);
                if (node.Name == "Track")
                {
                    nodes.Add(node);
                    
                }
            }
            TrackListing.ItemsSource = nodes;
        }

        private void LoadJson(string path)
        {
            string jsonText = File.ReadAllText(path);
            Custom custom = JsonConvert.DeserializeObject<Custom>(jsonText);
            string xml = "<Track ingame=\"true\" ondisc=\"true\" selectableinfem=\"yes\">";

            xml += "<IDTag>" + custom.song.songId + "</IDTag>";
            xml += "<LeaderboardId />";
            xml += "<IsTutorialTrack>0</IsTutorialTrack>";
            xml += "<HasVocalMarkup>0</HasVocalMarkup>";
            xml += "<BPM>" + custom.difficulty.bpm + "</BPM>";
            xml += @"<FolderLocation>AUDIO\Audiotracks\" + custom.song.songId + "</FolderLocation>";

            xml += "<DeckSpeedMultiplier Difficulty=\"0\">" + custom.difficulty.deck_speed.deckspeed_beginner + "</DeckSpeedMultiplier>";
            xml += "<DeckSpeedMultiplier Difficulty=\"1\">" + custom.difficulty.deck_speed.deckspeed_easy + "</DeckSpeedMultiplier>";
            xml += "<DeckSpeedMultiplier Difficulty=\"2\">" + custom.difficulty.deck_speed.deckspeed_medium + "</DeckSpeedMultiplier>";
            xml += "<DeckSpeedMultiplier Difficulty=\"3\">" + custom.difficulty.deck_speed.deckspeed_hard + "</DeckSpeedMultiplier>";
            xml += "<DeckSpeedMultiplier Difficulty=\"4\">" + custom.difficulty.deck_speed.deckspeed_expert + "</DeckSpeedMultiplier>";
            xml += "<PreviewLoopPointStartInBars>" + custom.song.preview_start_time + "</PreviewLoopPointStartInBars>";
            xml += "<PreviewLoopPointEndInBars>" + custom.song.preview_end_time + "</PreviewLoopPointEndInBars>";

            xml += "<TrackComplexity>" + custom.difficulty.complexity.track_complexity + "</TrackComplexity>";
            xml += "<TapComplexity>" + custom.difficulty.complexity.tap_complexity + "</TapComplexity>";
            xml += "<CrossfadeComplexity>" + custom.difficulty.complexity.cross_complexity + "</CrossfadeComplexity>";
            xml += "<ScratchComplexity>" + custom.difficulty.complexity.scratch_complexity + "</ScratchComplexity>";

            xml += "<MixArtist>" + custom.extra.id.id_artist + "</MixArtist>";
            if (custom.extra.id.id_artist2 != string.Empty)
            {
                xml += "<MixArtist>" + custom.extra.id.id_artist2 + "</MixArtist>";
            }

            xml += "<MixName>" + custom.extra.id.id_name + "</MixName>";
            if (custom.extra.id.id_name2 != string.Empty)
            {
                xml += "<MixName>" + custom.extra.id.id_name2 + "</MixName>";
            }

            xml += "<TrackDuration>" + custom.song.song_length + "</TrackDuration>";
            xml += custom.extra.additional_xml;
            xml += "</Track>";


            XmlDocument document = new XmlDocument();

            try
            {
                document.LoadXml(xml);
                XmlNode node = document.DocumentElement;

                TrackListing.ItemsSource = null;
                nodes.Add(node);
                TrackListing.ItemsSource = nodes;
                System.Windows.MessageBox.Show("Successfully added song to list", "Successfully added song to list");
            }
            catch (System.Xml.XmlException error)
            {
                System.Windows.MessageBox.Show("Error while adding song :" + error.Message,
                    "Xml error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void TrackListing_AddingNewItem(object sender, System.Windows.Controls.AddingNewItemEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                XmlDocument xml = new XmlDocument();
                try
                {
                    xml.Load(dialog.FileName);
                    LoadXml(xml.DocumentElement);
                }
                catch (System.Xml.XmlException)
                {
                    System.Windows.MessageBox.Show("Cannot read file as Xml.\nMake sure the file is a valid Xml file",
                        "Xml error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        private void TrackListing_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(TrackListing.SelectedIndex != -1)
            {
                proprieties.Items.Clear();
                XmlNode selected = nodes[TrackListing.SelectedIndex];
                if(selected.SelectNodes("MixArtist").Count > 0)
                {
                    string s = "Artist 1:" + selected.SelectNodes("MixArtist")[0].InnerText;
                    proprieties.Items.Add(new Label().Text = s);
                }
                else
                {
                    string s = "Artist 1: !MISSING!";
                    proprieties.Items.Add(new Label().Text = s);
                }

                if (selected.SelectNodes("MixName").Count > 0)
                {
                    string s2 = "Name 1:" + selected.SelectNodes("MixName")[0].InnerText;
                    proprieties.Items.Add(new Label().Text = s2);
                }
                else
                {
                    string s2 = "Name 1: !MISSING!"; 
                    proprieties.Items.Add(new Label().Text = s2);
                }
                if (selected.SelectNodes("MixArtist").Count > 1)
                {
                    string s3 = "Artist 2:" + selected.SelectNodes("MixArtist")[1].InnerText;
                    proprieties.Items.Add(new Label().Text = s3);
                }
                else
                {
                    string s3 = "Artist 2: !MISSING!";
                    proprieties.Items.Add(new Label().Text = s3);
                }
                if (selected.SelectNodes("MixName").Count > 1)
                {
                    string s4 = "Name 2: " + selected.SelectNodes("MixName")[1].InnerText;
                    proprieties.Items.Add(new Label().Text = s4);
                }
                else
                {
                    string s4 = "Name 2: !MISSING";
                    proprieties.Items.Add(new Label().Text = s4);
                }
                if(selected.SelectSingleNode("MixHeadlineDJName") != null)
                {
                    string s5 = "Mixer Dj Name:" + selected.SelectSingleNode("MixHeadlineDJName").InnerText;
                    proprieties.Items.Add(new Label().Text = s5);

                }
                else
                {
                    string s5 = "Mixer Dj Name:!MISSING!";
                    proprieties.Items.Add(new Label().Text = s5);
                }

                if (selected.SelectSingleNode("MixHeadline") != null)
                {
                    string s5 = "Mixer:" + selected.SelectSingleNode("MixHeadline").InnerText;
                    proprieties.Items.Add(new Label().Text = s5);
                }
                else
                {
                    string s5 = "Mixer: !MISSING!";
                    proprieties.Items.Add(new Label().Text = s5);
                }

                if(selected.SelectSingleNode("BPM") != null)
                {
                    string s6 = "BPM: " + selected.SelectSingleNode("BPM").InnerText;
                    proprieties.Items.Add(new Label().Text = s6);
                }
                else
                {
                    string s6 = "BPM: !MISSING!";
                    proprieties.Items.Add(new Label().Text = s6);
                }

                if(selected.SelectSingleNode("TrackComplexity") != null)
                {
                    string s7 = "Track Complexity: " + selected.SelectSingleNode("TrackComplexity").InnerText;
                    proprieties.Items.Add(new Label().Text = s7);
                }
                else
                {
                    string s7 = "Track Complexity: !MISSING!" ;
                    proprieties.Items.Add(new Label().Text = s7);
                }

                if (selected.SelectSingleNode("TapComplexity") != null)
                {
                    string s8 = "Tap Complexity: " + selected.SelectSingleNode("TapComplexity").InnerText;
                    proprieties.Items.Add(new Label().Text = s8);
                }
                else
                {
                    string s8 = "Tap Complexity: !MISSING!" ;
                    proprieties.Items.Add(new Label().Text = s8);
                }

                if(selected.SelectSingleNode("CrossfadeComplexity") != null)
                {
                    string s9 = "Crossfade Complexity: " + selected.SelectSingleNode("CrossfadeComplexity").InnerText;
                    proprieties.Items.Add(new Label().Text = s9);
                }
                else
                {
                    string s9 = "Crossfade Complexity: !MISSING!";
                    proprieties.Items.Add(new Label().Text = s9);
                }
                if (selected.SelectSingleNode("ScratchComplexity") != null)
                {
                    string s10 = "Scratch Complexity: " + selected.SelectSingleNode("ScratchComplexity").InnerText;
                    proprieties.Items.Add(new Label().Text = s10);
                }
                else
                {
                    string s10 = "Scratch Complexity: !MISSING!";
                    proprieties.Items.Add(new Label().Text = s10);
                }
            }
        }

        private void addCustomButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadJson(dialog.FileName);
            }
        }

        private void UpdateFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save tracklisting.xml";
            saveFileDialog.Filter = "xml file|*.xml";
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string s = string.Empty;
                s += "<TrackList>";
                foreach (XmlNode n in nodes)
                {
                    s += "<Track ";
                    foreach (XmlAttribute attribute in n.Attributes)
                    {
                        s += attribute.Name;
                        s += "=\"";
                        s += attribute.Value;
                        s += "\" ";
                    }
                    s += ">";
                    s += n.InnerXml;
                    s += "</Track>";
                }
                s += "</TrackList>";
                File.WriteAllText(saveFileDialog.FileName, s);
            }

            saveRemoved();
        }

        private void Convert(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                XmlDocument xml = new XmlDocument();
                try
                {
                    xml.Load(dialog.FileName);

                    //convert
                    Custom conversion = new Custom();
                    conversion.song.first.name = xml.SelectNodes("MixName")[0].InnerText;

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Title = "Save song.json";
                    saveFileDialog.Filter = "json file|*.json";
                    if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        File.WriteAllText(saveFileDialog.FileName, JsonConvert.SerializeObject(conversion));
                    }

                }
                catch (System.Xml.XmlException)
                {
                    System.Windows.MessageBox.Show("Cannot read file as Xml.\nMake sure the file is a valid Xml file",
                        "Xml error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Button_Drop(object sender, System.Windows.DragEventArgs e)
        {
            string[] paths = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(paths[0]);

                //convert
                Custom conversion = new Custom();
                conversion.song.first.name = xml.SelectNodes("MixName")[0].InnerText;

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Save song.json";
                saveFileDialog.Filter = "json file|*.json";
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, JsonConvert.SerializeObject(conversion));
                }

            }
            catch (System.Xml.XmlException)
            {
                System.Windows.MessageBox.Show("Cannot read file as Xml.\nMake sure the file is a valid Xml file",
                    "Xml error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Drop_1(object sender, System.Windows.DragEventArgs e)
        {
            string[] paths = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
            XmlDocument xml = new XmlDocument();
            xml.Load(paths[0]);
            LoadXml(xml.DocumentElement);
        }

        private void addCustomButton_Drop(object sender, System.Windows.DragEventArgs e)
        {
            string[] paths = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
            LoadJson(paths[0]);
        }

        private void Tracklisting_confirmRemove(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Delete)
            {
                string main = "Do you want to delete the selected entries?\nThis action is PERMANENT";
                string title = "Are you sure about that?";
                if(System.Windows.MessageBox.Show(main,title,MessageBoxButton.YesNo,MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    System.Collections.ArrayList arrayList = new System.Collections.ArrayList(TrackListing.SelectedItems);
                    
                    for(int i = arrayList.Count-1; i >= 0; i--)
                    {
                        int index = TrackListing.Items.IndexOf(arrayList[i]);
                        TrackListing.ItemsSource = null;
                        XmlNode removed = nodes[index];
                        nodes.RemoveAt(index);
                        removedNodes.Add(removed);
                        TrackListing.ItemsSource = nodes;
                    }
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            saveRemoved();
        }
    }
}

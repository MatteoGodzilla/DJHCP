using System.Windows;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.IO;
using Newtonsoft.Json;
using WK.Libraries.BetterFolderBrowserNS;

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
        public static List<XmlNode> totalNodes = new List<XmlNode>();
        public static List<XmlNode> visibleNodes = new List<XmlNode>();
        public string baseFolder = string.Empty;
        public List<string> textFiles = new List<string>();
        public Dictionary<string, string> tracStrings = new Dictionary<string, string>();
        public bool edited = false;

        

        public MainWindow()
        {
            InitializeComponent();
        }

        private int compareIds(XmlNode one, XmlNode two)
        {
            try
            {
                string oneId = one.SelectSingleNode("IDTag").InnerText;
                string twoId = two.SelectSingleNode("IDTag").InnerText;
                return oneId.CompareTo(twoId);
            }
            catch
            {
                return 0;
            }
        }

        private void updateList()
        {
            try
            {
                TrackListing.ItemsSource = null;
                visibleNodes.Clear();
                proprieties.Items.Clear();

                totalNodes.Sort(compareIds);

                List<string> textKeysFound = new List<string>();
                List<string> textNameFound = new List<string>();

                foreach (var name in tracStrings.Values)
                {
                    if (name.ToLower().Contains(SearchBar.Text.ToLower()))
                    {
                        textNameFound.Add(name);
                    }
                }

                foreach (var key in tracStrings.Keys)
                {
                    foreach (string name in textNameFound)
                    {
                        if (tracStrings[key] == name && !textKeysFound.Contains(key))
                        {
                            textKeysFound.Add(key);
                        }
                    }
                }

                foreach (XmlNode x in totalNodes)
                {
                    string id = x.SelectSingleNode("IDTag").InnerText;
                    if (id.ToLower().Contains(SearchBar.Text.ToLower()) && !visibleNodes.Contains(x))
                    {
                        visibleNodes.Add(x);
                    }

                    XmlNodeList nameList = x.SelectNodes("MixName");

                    foreach (XmlNode name in nameList)
                    {
                        foreach (string tag in textKeysFound)
                        {
                            if (name.InnerText == tag && !visibleNodes.Contains(x)) visibleNodes.Add(x);
                        }
                    }

                    XmlNodeList artistList = x.SelectNodes("MixArtist");

                    foreach (XmlNode artist in artistList)
                    {
                        foreach (string tag in textKeysFound)
                        {
                            if (artist.InnerText == tag && !visibleNodes.Contains(x)) visibleNodes.Add(x);
                        }
                    }
                }
                TrackListing.ItemsSource = visibleNodes;
            }
            catch
            {
                System.Windows.MessageBox.Show("Error updating list");
            }
        }

        /*
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
        */

        private void LoadXml(XmlNode root)
        {
            try
            {
                TrackListing.ItemsSource = null;
                XmlNodeList list = root.ChildNodes;
                for (int i = 0; i < list.Count; ++i)
                {
                    XmlNode node = list.Item(i);
                    if (node.Name == "Track")
                    {
                        bool present = false;
                        foreach (XmlNode x in totalNodes)
                        {
                            string customId = node.SelectSingleNode("IDTag").InnerText;
                            if (x.SelectSingleNode("IDTag").InnerText == customId)
                            {
                                present = true;
                                string message = "A song with the id " + customId + " is already present. Skipping entry";
                                System.Windows.MessageBox.Show(message, "message");
                            }
                        }
                        if (!present)
                        {
                            totalNodes.Add(node);
                        }
                    }
                }
                updateList();
            }
            catch (System.Xml.XPath.XPathException exception)
            {
                System.Windows.MessageBox.Show(exception.Message); 
            }
            
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
                totalNodes.Add(node);
                updateList();
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

        private void AddCustom(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Open 'info for tracklisting.xml'";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                XmlDocument xml = new XmlDocument();
                try
                {
                    xml.Load(dialog.FileName);
                    LoadXml(xml.DocumentElement);

                    string namesPath = dialog.FileName + @"\..\info for trac.csv";
                    StreamReader reader = new StreamReader(namesPath);

                    string line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("//")) continue;
                        if (string.IsNullOrEmpty(line)) continue;
                        string[] content;
                        content = line.Split(',');
                        if (!tracStrings.ContainsKey(content[0]))
                        {
                            tracStrings.Add(content[0], content[1]);
                        }
                    }

                    string dir = new List<string>(Directory.EnumerateDirectories(dialog.FileName + @"\..\"))[0];

                    List<string> tree = new List<string>(dir.Split('\\'));

                    string dirName = tree[tree.Count - 1];

                    DirectoryInfo files = new DirectoryInfo(dir);

                    System.IO.Directory.CreateDirectory(baseFolder + @"\AUDIO\Audiotracks\" + dirName);

                    var data = files.EnumerateFiles();

                    foreach (FileInfo f in data)
                    {
                        string destinationPath = baseFolder + @"\AUDIO\Audiotracks\" + dirName + "\\" + f.Name;
                        System.IO.File.Copy(f.FullName, destinationPath);
                    }

                }
                catch (XmlException error)
                {
                    string message = "ERROR: cannot load xml file.\nMake sure the file you selected is a valid song info file\n";
                    message += error.Message;
                    System.Windows.MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (PathTooLongException error)
                {
                    string message = "ERROR: Selected Path is too long.\n";
                    message += error.Message;
                    System.Windows.MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (IOException error)
                {
                    string message = "Warning: custom's files already present in base folder. Skipping\n";
                    message += error.Message;
                    System.Windows.MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch
                {
                    string message = "Error: Unknown error";
                    System.Windows.MessageBox.Show(message, "Parsing error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        private void TrackListing_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (TrackListing.SelectedIndex != -1)
            {
                proprieties.Items.Clear();
                XmlNode selected = visibleNodes[TrackListing.SelectedIndex];
                if (selected.SelectNodes("MixArtist").Count > 0)
                {
                    string id = selected.SelectNodes("MixArtist")[0].InnerText;
                    string s;
                    if (tracStrings.ContainsKey(id))
                    {
                        s = "Artist 1: " + tracStrings[id];
                    }
                    else
                    {
                        s = "Artist 1:" + id;
                    }
                    proprieties.Items.Add(new Label().Text = s);
                }
                else
                {
                    string s = "Artist 1: !MISSING!";
                    proprieties.Items.Add(new Label().Text = s);
                }

                if (selected.SelectNodes("MixName").Count > 0)
                {
                    string id = selected.SelectNodes("MixName")[0].InnerText;
                    string s;
                    if (tracStrings.ContainsKey(id))
                    {
                        s = "Name 1:" + tracStrings[id];
                    }
                    else
                    {
                        s = "Name 1:" + id;
                    }
                    proprieties.Items.Add(new Label().Text = s);
                }
                else
                {
                    string s2 = "Name 1: !MISSING!";
                    proprieties.Items.Add(new Label().Text = s2);
                }
                if (selected.SelectNodes("MixArtist").Count > 1)
                {
                    string id = selected.SelectNodes("MixArtist")[1].InnerText;
                    string s;
                    if (tracStrings.ContainsKey(id))
                    {
                        s = "Artist 2:" + tracStrings[id];
                    }
                    else
                    {
                        s = "Artist 2:" + id;
                    }
                    proprieties.Items.Add(new Label().Text = s);
                }
                else
                {
                    string s3 = "Artist 2: !MISSING!";
                    proprieties.Items.Add(new Label().Text = s3);
                }
                if (selected.SelectNodes("MixName").Count > 1)
                {
                    string id = selected.SelectNodes("MixName")[1].InnerText;
                    string s;
                    if (tracStrings.ContainsKey(id))
                    {
                        s = "Name 2: " + tracStrings[id];
                    }
                    else
                    {
                        s = "Name 2: " + id;
                    }
                    proprieties.Items.Add(new Label().Text = s);
                }
                else
                {
                    string s4 = "Name 2: !MISSING";
                    proprieties.Items.Add(new Label().Text = s4);
                }
                if (selected.SelectSingleNode("MixHeadlineDJName") != null)
                {
                    string id = selected.SelectSingleNode("MixHeadlineDJName").InnerText;
                    string s;
                    if (tracStrings.ContainsKey(id))
                    {
                        s = "Mixer Dj Name:" + tracStrings[id];
                    }
                    else
                    {
                        s = "Mixer Dj Name:" + id;
                    }
                    proprieties.Items.Add(new Label().Text = s);

                }
                else
                {
                    string s5 = "Mixer Dj Name:!MISSING!";
                    proprieties.Items.Add(new Label().Text = s5);
                }

                if (selected.SelectSingleNode("MixHeadline") != null)
                {
                    string id = selected.SelectSingleNode("MixHeadline").InnerText;
                    string s;
                    if (tracStrings.ContainsKey(id))
                    {
                        s = "Mixer:" + tracStrings[id];
                    }
                    else
                    {
                        s = "Mixer:" + id;
                    }
                    proprieties.Items.Add(new Label().Text = s);
                }
                else
                {
                    string s5 = "Mixer: !MISSING!";
                    proprieties.Items.Add(new Label().Text = s5);
                }

                if (selected.SelectSingleNode("BPM") != null)
                {
                    string s6 = "BPM: " + selected.SelectSingleNode("BPM").InnerText;
                    proprieties.Items.Add(new Label().Text = s6);
                }
                else
                {
                    string s6 = "BPM: !MISSING!";
                    proprieties.Items.Add(new Label().Text = s6);
                }

                if (selected.SelectSingleNode("TrackComplexity") != null)
                {
                    string s7 = "Track Complexity: " + selected.SelectSingleNode("TrackComplexity").InnerText;
                    proprieties.Items.Add(new Label().Text = s7);
                }
                else
                {
                    string s7 = "Track Complexity: !MISSING!";
                    proprieties.Items.Add(new Label().Text = s7);
                }

                if (selected.SelectSingleNode("TapComplexity") != null)
                {
                    string s8 = "Tap Complexity: " + selected.SelectSingleNode("TapComplexity").InnerText;
                    proprieties.Items.Add(new Label().Text = s8);
                }
                else
                {
                    string s8 = "Tap Complexity: !MISSING!";
                    proprieties.Items.Add(new Label().Text = s8);
                }

                if (selected.SelectSingleNode("CrossfadeComplexity") != null)
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

        private void UpdateFile(object sender, RoutedEventArgs e)
        {
            try
            {
                if (baseFolder != string.Empty)
                {

                    string path = baseFolder + @"\AUDIO\Audiotracks\tracklisting.xml";
                    string s = string.Empty;
                    s += "<TrackList>";
                    foreach (XmlNode n in totalNodes)
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
                    File.WriteAllText(path, s);

                    //saveRemoved();

                    path = baseFolder + @"\Text\TRAC\";
                    s = string.Empty;
                    List<string> valueList = new List<string>(tracStrings.Values);
                    for (int i = 0; i < tracStrings.Count; ++i)
                    {
                        s += valueList[i] + "\0";
                    }

                    foreach (string p in textFiles)
                    {
                        File.WriteAllText(p, s);
                    }

                    s = string.Empty;
                    List<string> keysList = new List<string>(tracStrings.Keys);
                    for (int i = 0; i < tracStrings.Count; ++i)
                    {
                        s += keysList[i] + "\r\n";
                    }
                    File.WriteAllText(path + "TRACID.txt", s);

                    System.Windows.MessageBox.Show("Done updating files", "Finished");
                    edited = false;
                }
            }
            catch
            {
                string message = "Error: could not update files";
                System.Windows.MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        /*
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
            catch (XmlException error)
            {
                string message = "ERROR: cannot load xml file.\nMake sure the file you selected is a valid song info file\n";
                message += error.Message;
                System.Windows.MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (PathTooLongException error)
            {
                string message = "ERROR: Selected Path is too long.\n";
                message += error.Message;
                System.Windows.MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException error)
            {
                string message = "ERROR: Cannot open/read selected file.\n";
                message += error.Message;
                System.Windows.MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                string message = "Error: Unhandled Exception when loading custom";
                System.Windows.MessageBox.Show(message, "Parsing error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        */

        private void AddCustomButtonDrop(object sender, System.Windows.DragEventArgs e)
        {
            string[] paths = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);

            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(paths[0]);
                LoadXml(xml.DocumentElement);

                string namesPath = paths[0] + @"\..\info for trac.csv";
                StreamReader reader = new StreamReader(namesPath);

                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("//")) continue;
                    if (string.IsNullOrEmpty(line)) continue;
                    string[] content;
                    content = line.Split(',');
                    if (!tracStrings.ContainsKey(content[0]))
                    {
                        tracStrings.Add(content[0], content[1]);
                    }
                }

                string dir = new List<string>(Directory.EnumerateDirectories(paths[0] + @"\..\"))[0];

                List<string> tree = new List<string>(dir.Split('\\'));

                string dirName = tree[tree.Count - 1];

                DirectoryInfo files = new DirectoryInfo(dir);

                System.IO.Directory.CreateDirectory(baseFolder + @"\AUDIO\Audiotracks\" + dirName);

                var data = files.EnumerateFiles();

                foreach (FileInfo f in data)
                {
                    string destinationPath = baseFolder + @"\AUDIO\Audiotracks\" + dirName + "\\" + f.Name;
                    System.IO.File.Copy(f.FullName, destinationPath);
                }
                edited = true;

                System.Windows.MessageBox.Show("Successfully copied files into game", "Copied successfully");
            }
            catch (XmlException error)
            {
                string message = "ERROR: cannot load xml file.\nMake sure the file you selected is a valid song info file\n";
                message += error.Message;
                System.Windows.MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (PathTooLongException error)
            {
                string message = "ERROR: Selected Path is too long.\n";
                message += error.Message;
                System.Windows.MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException error)
            {
                string message = "Warning: custom's files already present in base folder. Skipping\n";
                message += error.Message;
                System.Windows.MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch
            {
                string message = "Error: Unhandled Exception when adding custom";
                System.Windows.MessageBox.Show(message, "Parsing error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Tracklisting_confirmRemove(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Delete)
            {
                string main = "Do you want to delete the selected entries?\nThis action is PERMANENT";
                string title = "Are you sure about that?";
                if (System.Windows.MessageBox.Show(main, title, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    System.Collections.ArrayList arrayList = new System.Collections.ArrayList(TrackListing.SelectedItems);

                    for (int i = arrayList.Count - 1; i >= 0; i--)
                    {
                        int index = TrackListing.Items.IndexOf(arrayList[i]);
                        TrackListing.ItemsSource = null;
                        XmlNode removed = totalNodes[index];
                        totalNodes.RemoveAt(index);
                        
                        updateList();

                    }
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //saveRemoved();
            if (edited)
            {
                string text = "You have changes not applied to the files.\nDo you really want to close?";
                if (System.Windows.MessageBox.Show(text, "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning)
                    == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void OpenExtractedFiles(object sender, RoutedEventArgs e)
        {
            BetterFolderBrowser folderSelect = new BetterFolderBrowser();
            folderSelect.Title = "Open the WII/PS3/X360 folder";
            if (folderSelect.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string tracklistingPath = folderSelect.SelectedPath + @"\AUDIO\Audiotracks\tracklisting.xml";
                XmlDocument document = new XmlDocument();
                try
                {
                    document.Load(tracklistingPath);
                    LoadXml(document.DocumentElement);
                    baseFolder = folderSelect.SelectedPath;
                    baseFolderLabel.Content = baseFolder;

                    string trackStringFolderPath = folderSelect.SelectedPath + @"\Text\TRAC";

                    DirectoryInfo info = new DirectoryInfo(trackStringFolderPath);

                    FileInfo[] files = info.GetFiles();
                    foreach (FileInfo f in files)
                    {
                        if (f.Name == "TRACID.txt") continue;
                        textFiles.Add(f.FullName);
                    }

                    string[] ids = File.ReadAllText(trackStringFolderPath + @"\TRACID.txt").Split('\n');
                    string[] values = File.ReadAllText(trackStringFolderPath + @"\TRACE.txt").Split('\0');

                    for (int i = 0; i < ids.Length; ++i)
                    {
                        ids[i] = ids[i].Trim('\r');
                    }

                    for (int i = 0; i < ids.Length; ++i)
                    {
                        if (!tracStrings.ContainsKey(ids[i]) && ids[i] != string.Empty)
                        {
                            tracStrings.Add(ids[i], values[i]);
                        }
                    }
                    edited = true;
                }
                catch (XmlException error)
                {
                    string message = "ERROR: cannot load xml file.\nMake sure the file you selected is a valid song info file\n";
                    message += error.Message;
                    System.Windows.MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (PathTooLongException error)
                {
                    string message = "ERROR: Selected Path is too long.\n";
                    message += error.Message;
                    System.Windows.MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (IOException error)
                {
                    string message = "ERROR: Cannot open/read selected file.\n";
                    message += error.Message;
                    System.Windows.MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch
                {
                    string message = "ERROR: Unhandled Exception when opening extracted files";
                    System.Windows.MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SearchChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            updateList();
        }

        private void EditTrackData_Click(object sender, RoutedEventArgs e)
        {
            tracStringWindow window = new tracStringWindow(tracStrings);
            window.Owner = this;

            window.EditedTracStrings += ReceivedData;
            window.ShowDialog();
        }

        private void ReceivedData(object obj, List<Entry> entries)
        {
            TrackListing.ItemsSource = null;
            tracStrings.Clear();
            foreach(var entry in entries)
            {
                tracStrings.Add(entry.m_id, entry.m_value);
            }
            updateList();
        }
    }
}

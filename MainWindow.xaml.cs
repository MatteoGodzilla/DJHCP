using System.Windows;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.IO;
using WK.Libraries.BetterFolderBrowserNS;
using WK.Libraries.BetterFolderBrowserNS.Helpers;
using WPFCustomMessageBox;
using System.Text;

namespace DJHCP
{
    public partial class MainWindow : Window
    {
        public static List<XmlNode> totalNodes = new List<XmlNode>();
        public static List<XmlNode> visibleNodes = new List<XmlNode>();
        public string baseFolder = string.Empty;
        public List<string> textFiles = new List<string>();
        public Dictionary<string, string> tracStrings = new Dictionary<string, string>();
        public bool overwriteOne = false;
        public bool overwriteAll = false;

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
                        string customId = node.SelectSingleNode("IDTag").InnerText;
                        XmlNode needsRemoving = node;
                        foreach (XmlNode x in totalNodes)
                        {
                            if (x.SelectSingleNode("IDTag").InnerText == customId)
                            {
                                present = true;
                                if (overwriteAll)
                                {
                                    needsRemoving = x;
                                } else
                                {
                                    //Ask to overwrite

                                    string message = "A song with the id " + customId + " is already present. Overwrite the data?";
                                    MessageBoxResult res = CustomMessageBox.ShowYesNoCancel(message, "ID CONFLICT", "Overwrite All", "Overwrite just this one", "Ignore");
                                    //YES = REPLACE ALL, NO = REPLACE SINGLE, CANCEL = IGNORE
                                    if (res == MessageBoxResult.Yes)
                                    {
                                        //overwrite all
                                        overwriteAll = true;
                                        needsRemoving = x;
                                    } else if (res == MessageBoxResult.No)
                                    {
                                        overwriteOne = true;
                                        needsRemoving = x;
                                    }
                                }

                            }
                        }
                        if (overwriteOne || overwriteAll)
                        {
                            totalNodes.Remove(needsRemoving);
                            totalNodes.Add(node);
                        }
                        else if (!present)
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

        private void AddCustom(object sender, RoutedEventArgs e)
        {
            BetterFolderBrowserDialog dialog = new BetterFolderBrowserDialog();
            dialog.Title = "Select root of customs:";
            dialog.AllowMultiselect = true;
            if (dialog.ShowDialog())
            {
                overwriteAll = false;
                foreach(string path in dialog.FileNames)
                {
                    overwriteOne = false;
                    addData(path);
                }
                UpdateFile(null, null);
            }

        }

        private void CopyFolderRecursive(string source, string destination)
        {
            DirectoryInfo sourceFolder = new DirectoryInfo(source);
            Directory.CreateDirectory(Path.Combine(destination,sourceFolder.Name));

            foreach(FileInfo file in sourceFolder.GetFiles())
            {
                if(File.Exists(Path.Combine(destination, sourceFolder.Name, file.Name))){
                    if (overwriteOne)
                    {
                        file.CopyTo(Path.Combine(destination, sourceFolder.Name, file.Name), true);
                    }
                } else {
                    file.CopyTo(Path.Combine(destination, sourceFolder.Name, file.Name));
                }
                
            }

            foreach(DirectoryInfo sub in sourceFolder.GetDirectories())
            {
                CopyFolderRecursive(Path.Combine(source,sub.Name), Path.Combine(destination,sourceFolder.Name));
            }
        }

        private void addData(string path)
        {
            string customRoot = path;

            if (Directory.Exists(Path.Combine(path, "DJH2")))
            {
                //choose internal djh2 folder
                customRoot = Path.Combine(path, "DJH2");
            }

            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(customRoot + @"\Info For Tracklisting.xml");
                LoadXml(xml.DocumentElement);

                foreach (string fullDirPath in Directory.EnumerateDirectories(customRoot))
                {
                    CopyFolderRecursive(fullDirPath, baseFolder + @"\AUDIO\Audiotracks\");
                }
            }
            catch (XmlException error)
            {
                string message = "Error: cannot load xml file.\nMake sure the file you selected is a valid song info file\n";
                message += error.Message;
                System.Windows.MessageBox.Show(message, "Cannot load xml file", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch(DirectoryNotFoundException error)
            {
                string title = "Cannot find directory";
                string message = "Error: cannot find direcory.\n";
                message += error.Message;
                System.Windows.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                string message = "Error: Unhandled exception in addData()";
                System.Windows.MessageBox.Show(message, "Unknown error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                string namesPath = customRoot + @"\info for trac.csv";
                StreamReader reader = new StreamReader(namesPath);
                
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("//")) continue;
                    if (string.IsNullOrEmpty(line)) continue;

                    List<string> content = new List<string> { "" };
                    bool insideQuotes = false;
                    char lastChar = '\0';
                    foreach (char c in line)
                    {
                        if(c == '"')
                        {
                            insideQuotes = !insideQuotes;
                            if (lastChar == '"')
                            {
                                content[content.Count - 1] += '"';
                                lastChar = '\0';
                            } else
                            {
                                lastChar = c;
                            }
                        }
                        else if(c == ',' && !insideQuotes)
                        {
                            content.Add("");
                            lastChar = c;
                        } else
                        {
                            content[content.Count - 1] += c;
                            lastChar = c;
                        }
                    }

                    if(content.Count > 1)
                    {
                        if (tracStrings.ContainsKey(content[0]))
                        {
                            if (overwriteOne || overwriteAll)
                            {
                                tracStrings[content[0]] = content[1];
                            }
                        }
                        else
                        {
                            tracStrings.Add(content[0], content[1]);
                        }
                    }
                }
            }
            catch (IOException error)
            {
                string message = "Warning: \"info for trac.csv\" file not found\n";
                message += error.Message;
                System.Windows.MessageBox.Show(message, "File not found", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                    XmlDocument doc = new XmlDocument();
                    
                    XmlNode root = doc.CreateElement("TrackList");
                    doc.AppendChild(root);

                    for(int i = 0; i < totalNodes.Count; ++i)
                    {
                        XmlNode clone = totalNodes[i].Clone();
                        root.AppendChild(doc.ImportNode(clone, true));
                    }

                    StringBuilder stringBuilder = new StringBuilder();
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true,
                        IndentChars = "  ",
                        OmitXmlDeclaration = true
                    };

                    using (XmlWriter writer = XmlWriter.Create(stringBuilder, settings))
                    {
                        doc.WriteTo(writer);
                    };

                    File.WriteAllText(path, stringBuilder.ToString());

                    path = baseFolder + @"\Text\TRAC\";
                    string s = string.Empty;
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

                    if (baseFolder.EndsWith("PS3")) RenameInside(baseFolder);
                }
            }
            catch
            {
                string message = "Error: could not update files";
                System.Windows.MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

                        var node = (XmlNode)arrayList[i];
                                                    
                        if (node.SelectSingleNode("FolderLocation") != null)
                        {
                            string folder = "\\" + node.SelectSingleNode("FolderLocation").InnerText;
                            try
                            {
                                Directory.Delete(baseFolder + folder, true);
                            } catch (IOException exception)
                            {
                                System.Windows.MessageBox.Show(exception.Message);
                            }
                        }
                        totalNodes.RemoveAt(index);
                        UpdateFile(null, null);
                        updateList();
                    }
                }
            }
            else if (e.Key == System.Windows.Input.Key.Space)
            {
                XmlNode entry = visibleNodes[TrackListing.Items.IndexOf(TrackListing.SelectedItem)];
                string path = baseFolder + "\\" + entry.SelectSingleNode("FolderLocation").InnerText;
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = "explorer.exe";
                processStartInfo.Arguments = path;
                Process.Start(processStartInfo);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }

        private void OpenExtractedFilesButtonClick(object sender, RoutedEventArgs e)
        {
            BetterFolderBrowser folderSelect = new BetterFolderBrowser();
            folderSelect.Title = "Open the WII/PS3/X360 folder";
            if (folderSelect.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadExtractedFromPath(folderSelect.SelectedFolder);
            }
        }

        private void LoadExtractedFromPath(string path)
        {
            string tracklistingPath = path + @"\AUDIO\Audiotracks\tracklisting.xml";
            XmlDocument document = new XmlDocument();
            try
            {
                //backup
                File.Copy(Path.Combine(path, "AUDIO", "Audiotracks", "tracklisting.xml"), Path.Combine(path, "AUDIO", "Audiotracks", "tracklisting-DJHCP-BAK.xml"),true);
                CopyFolderRecursive(Path.Combine(path, "Text", "TRAC"), Path.Combine(path, "Text", "TRAC-BAK"));
                
                document.Load(tracklistingPath);
                LoadXml(document.DocumentElement);
                baseFolder = path;

                baseFolderLabel.Content = baseFolder;

                string trackStringFolderPath = path + @"\Text\TRAC";

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
            }
            catch (XmlException error)
            {
                string message = "ERROR: cannot load xml file.\nMake sure the file you selected is a valid song info file\n";
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
            foreach (var entry in entries)
            {
                tracStrings.Add(entry.m_id, entry.m_value);
            }
            updateList();
        }

        private void TracklistingGenerator_Open(object sender, RoutedEventArgs e)
        {
            TracklistingGenerator window = new TracklistingGenerator();
            window.Show();
        }

        private void ToUpper_Click(object sender, RoutedEventArgs e)
        {
            RenameInside(baseFolder);
            System.Windows.MessageBox.Show("Done Renaming Files", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RenameInside(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            FileInfo[] files = info.GetFiles();
            DirectoryInfo[] directories = info.GetDirectories();

            foreach(FileInfo f in files)
            {
                if(f.Name != f.Name.ToUpper())
                    File.Move(Path.Combine(path, f.Name), Path.Combine(path,f.Name.ToUpper()));
            }

            foreach(DirectoryInfo directory in directories)
            {
                if(directory.Name != directory.Name.ToUpper())
                {
                    Directory.Move(Path.Combine(path, directory.Name), Path.Combine(path, directory.Name + "temp"));
                    Directory.Move(Path.Combine(path, directory.Name + "temp"), Path.Combine(path, directory.Name.ToUpper()));
                }
                RenameInside(Path.Combine(path, directory.Name.ToUpper()));
                //Path.Combine(path, directory.Name.ToUpper()) is the new path
            }
        }
    }
}

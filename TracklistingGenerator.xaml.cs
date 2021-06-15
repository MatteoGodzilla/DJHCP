using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml;
using WK.Libraries.BetterFolderBrowserNS;

namespace DJHCP
{
    public partial class TracklistingGenerator : Window
    {
        public TracklistingGenerator()
        {
            InitializeComponent();
        }

        private void Generate(object sender, RoutedEventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlDocument doc = new XmlDocument();

            List<string> ids = new List<string>();
            List<string> data = new List<string>();

            bool notEnoughParameters = false;
            bool invalidParameters = false;

            XmlNode trackList = doc.CreateElement("TrackList");
            doc.AppendChild(trackList);
            XmlNode track = doc.CreateElement("Track");
            trackList.AppendChild(track);

            XmlAttribute ingame = doc.CreateAttribute("ingame");
            ingame.Value = "true";

            XmlAttribute ondisc = doc.CreateAttribute("ondisc");
            ondisc.Value = "true";

            XmlAttribute selectableinfem = doc.CreateAttribute("selectableinfem");
            selectableinfem.Value = "yes";

            track.Attributes.Append(ingame);
            track.Attributes.Append(ondisc);
            track.Attributes.Append(selectableinfem);

            if (tbID.Text != string.Empty)
            {
                XmlNode id = doc.CreateElement("IDTag");
                string trackID = generateID(tbID.Text);
                id.InnerText = trackID;
                track.AppendChild(id);

                //location folder
                XmlNode folder = doc.CreateElement("FolderLocation");
                folder.InnerText = @"AUDIO\Audiotracks\" + trackID;
                track.AppendChild(folder);
            }
            else notEnoughParameters = true;

            if (tbBPM.Text != string.Empty)
            {
                try
                {
                    XmlNode bpm = doc.CreateElement("BPM");
                    int BPM = int.Parse(tbBPM.Text);
                    bpm.InnerText = BPM.ToString();
                    track.AppendChild(bpm);

                    //deckspeed
                    XmlNode speedBeginner = doc.CreateElement("DeckSpeedMultiplier");
                    XmlNode speedEasy = doc.CreateElement("DeckSpeedMultiplier");
                    XmlNode speedMedium = doc.CreateElement("DeckSpeedMultiplier");
                    XmlNode speedHard = doc.CreateElement("DeckSpeedMultiplier");
                    XmlNode speedExpert = doc.CreateElement("DeckSpeedMultiplier");

                    XmlAttribute attrBeginner = doc.CreateAttribute("Difficulty");
                    XmlAttribute attrEasy = doc.CreateAttribute("Difficulty");
                    XmlAttribute attrMedium = doc.CreateAttribute("Difficulty");
                    XmlAttribute attrHard = doc.CreateAttribute("Difficulty");
                    XmlAttribute attrExpert = doc.CreateAttribute("Difficulty");

                    attrBeginner.Value = "0";
                    attrEasy.Value = "1";
                    attrMedium.Value = "2";
                    attrHard.Value = "3";
                    attrExpert.Value = "4";

                    speedBeginner.Attributes.Append(attrBeginner);
                    speedEasy.Attributes.Append(attrEasy);
                    speedMedium.Attributes.Append(attrMedium);
                    speedHard.Attributes.Append(attrHard);
                    speedExpert.Attributes.Append(attrExpert);

                    if(tbDeckSpeed.Text == string.Empty)
                    {
                        speedBeginner.InnerText = (240.0 / BPM).ToString();
                        speedEasy.InnerText = (240.0 / BPM).ToString();
                        speedMedium.InnerText = (250.0 / BPM).ToString();
                        speedHard.InnerText = (300.0 / BPM).ToString();
                        speedExpert.InnerText = (400.0 / BPM).ToString();
                    } else {
                        int speed = int.Parse(tbDeckSpeed.Text);
                        speedBeginner.InnerText = (speed / BPM).ToString();
                        speedEasy.InnerText = (speed / BPM).ToString();
                        speedMedium.InnerText = (speed / BPM).ToString();
                        speedHard.InnerText = (speed / BPM).ToString();
                        speedExpert.InnerText = (speed / BPM).ToString();
                    }

                    track.AppendChild(speedBeginner);
                    track.AppendChild(speedEasy);
                    track.AppendChild(speedMedium);
                    track.AppendChild(speedHard);
                    track.AppendChild(speedExpert);
                }
                catch
                {
                    invalidParameters = true;
                }
            }
            else notEnoughParameters = true;

            if (tbA1.Text != string.Empty)
            {
                XmlNode node = doc.CreateElement("MixArtist");
                string id = generateID(tbA1.Text);
                ids.Add(id);
                data.Add(sanitize(tbA1.Text));
                node.InnerText = id;
                track.AppendChild(node);
            }
            else notEnoughParameters = true;

            if (tbA2.Text != string.Empty)
            {
                XmlNode node = doc.CreateElement("MixArtist");
                string id = generateID(tbA2.Text);
                ids.Add(id);
                data.Add(sanitize(tbA2.Text));
                node.InnerText = id;
                track.AppendChild(node);
            }

            if (tbS1.Text != string.Empty)
            {
                XmlNode node = doc.CreateElement("MixName");
                string id = generateID(tbS1.Text);
                ids.Add(id);
                data.Add(sanitize(tbS1.Text));
                node.InnerText = id;
                track.AppendChild(node);
            }
            else notEnoughParameters = true;

            if (tbS2.Text != string.Empty)
            {
                XmlNode node = doc.CreateElement("MixName");
                string id = generateID(tbS2.Text);
                ids.Add(id);
                data.Add(sanitize(tbS2.Text));
                node.InnerText = id;
                track.AppendChild(node);
            }

            if (tbMixDJ.Text != string.Empty)
            {
                XmlNode node = doc.CreateElement("MixHeadlineDJName");
                string id = generateID(tbMixDJ.Text);
                ids.Add(id);
                data.Add(sanitize(tbMixDJ.Text));
                node.InnerText = id;
                track.AppendChild(node);
            }
            else notEnoughParameters = true;

            if (tbMixHeadline.Text != string.Empty)
            {
                XmlNode node = doc.CreateElement("MixHeadline");
                string id = generateID(tbMixHeadline.Text);
                ids.Add(id);
                data.Add(sanitize(tbMixHeadline.Text));
                node.InnerText = id;
                track.AppendChild(node);
            }

            if (checkVocal.IsChecked == true)
            {
                XmlNode node = doc.CreateElement("HasVocalMarkup");
                node.InnerText = "1";
                track.AppendChild(node);
            }

            if (checkMenu.IsChecked == true)
            {
                XmlNode node = doc.CreateElement("IsMenuMusic");
                node.InnerText = "1";
                track.AppendChild(node);
            }

            if (tbGeneralDiff.Text != string.Empty)
            {
                try
                {
                    XmlNode diff = doc.CreateElement("TrackComplexity");
                    diff.InnerText = int.Parse(tbGeneralDiff.Text).ToString();
                    track.AppendChild(diff);
                }
                catch
                {
                    invalidParameters = true;
                }
            } 
            else notEnoughParameters = true;

            if (tbTapDiff.Text != string.Empty)
            {
                try
                {
                    XmlNode diff = doc.CreateElement("TapComplexity");
                    diff.InnerText = int.Parse(tbTapDiff.Text).ToString();
                    track.AppendChild(diff);
                }
                catch
                {
                    invalidParameters = true;
                }
            }
            else notEnoughParameters = true;

            if (tbScratchDiff.Text != string.Empty)
            {
                try
                {
                    XmlNode diff = doc.CreateElement("ScratchComplexity");
                    diff.InnerText = int.Parse(tbScratchDiff.Text).ToString();
                    track.AppendChild(diff);
                }
                catch
                {
                    invalidParameters = true;
                }
            }
            else notEnoughParameters = true;

            if (tbCrossfadeDiff.Text != string.Empty)
            {
                try
                {
                    XmlNode diff = doc.CreateElement("CrossfadeComplexity");
                    diff.InnerText = int.Parse(tbCrossfadeDiff.Text).ToString();
                    track.AppendChild(diff);
                }
                catch
                {
                    invalidParameters = true;
                }
            }
            else notEnoughParameters = true;

            if (tbSongDuration.Text != string.Empty)
            {
                try
                {
                    XmlNode length = doc.CreateElement("TrackDuration");
                    length.InnerText = int.Parse(tbSongDuration.Text).ToString();
                    track.AppendChild(length);
                }
                catch
                {
                    invalidParameters = true;
                }
            }

            if (invalidParameters)
            {
                string title = "Format error";
                string message = "Cannot generate valid xml from parameters. Make sure they are the correct type.";
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (notEnoughParameters)
            {
                string title = "Not enough parameters";
                string message = "All fields with a * need to be set for a valid xml file";
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
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

                tbXML.Text = stringBuilder.ToString();

                string csv = string.Empty;
                for (int i = 0; i < ids.Count; ++i)
                {
                    csv += ids[i] + "," + data[i] + "\r\n";
                }

                tbCSV.Text = csv;
            }
        }

        public void GenerateFolders(object sender, RoutedEventArgs args)
        {
            if(tbXML.Text != "XML Here" && tbCSV.Text != "CSV Here")
            {
                BetterFolderBrowser browser = new BetterFolderBrowser();
                browser.Title = "Select a folder";
                if (browser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        string basePath = browser.SelectedFolder;
                        Directory.CreateDirectory(Path.Combine(basePath, "DJH2"));
                        Directory.CreateDirectory(Path.Combine(basePath, "DJH2", generateID(tbID.Text)));

                        File.WriteAllText(Path.Combine(basePath, "DJH2", "Info For Tracklisting.xml"), tbXML.Text);
                        File.WriteAllText(Path.Combine(basePath, "DJH2", "Info For TRAC.csv"), tbCSV.Text);
                    }
                    catch
                    {
                        //general
                    }
                }
            } else
            {
                MessageBox.Show("Please generate a valid xml entry first.", "Please generate", MessageBoxButton.OK,MessageBoxImage.Information);
            }
        }

        private string generateID(string value)
        {
            return value.ToUpper().Replace(' ', '_').Replace(",", string.Empty).Replace("\"",string.Empty);
        }

        private string sanitize(string value)
        {
            string res = value.ToUpper();
            if (res.Contains("\""))
            {
                res = res.Replace("\"", "\"\"");
            }
            if (res.Contains(",")) {
                res = "\"" + res + "\"";
            }
            return res;
        }
    }
}

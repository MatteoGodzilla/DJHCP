using System.Collections.Generic;
using System.IO;
using System.Text;
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
                id.InnerText = generateID(tbID.Text);
                track.AppendChild(id);
            }
            else notEnoughParameters = true;

            if (tbBPM.Text != string.Empty)
            {
                try
                {
                    XmlNode bpm = doc.CreateElement("BPM");
                    bpm.InnerText = int.Parse(tbBPM.Text).ToString();
                    track.AppendChild(bpm);
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
                string val = tbS1.Text.ToUpper();
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
                string val = tbS2.Text.ToUpper();
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
                    catch (IOException e)
                    {
                        //IO Exception
                    }
                    catch
                    {
                        //geneeral
                    }


                }
            } else
            {
                MessageBox.Show("Please generate a valid xml entry first.", "Please generate", MessageBoxButton.OK,MessageBoxImage.Information);
            }
        }

        private string generateID(string value)
        {
            string prefix = "DJHCP_";
            return prefix + value.ToUpper().Replace(' ', '_').Replace(",", string.Empty).Replace("\"",string.Empty);
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

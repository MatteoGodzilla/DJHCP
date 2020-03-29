using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DJHCP
{
    /// <summary>
    /// Interaction logic for tracStringWindow.xaml
    /// </summary>
    /// 

    public class Entry : IEditableObject
    {
        //public event PropertyChangedEventHandler PropertyChanged;
        public Entry(string id, string value)
        {
            m_id = id;
            m_value = value;
        }
        public string m_id { get; set; } = string.Empty;
        public string m_value { get; set; } = string.Empty;

        private string m_oldId { get; set; } = string.Empty;

        private string m_oldValue { get; set; } = string.Empty;

        void IEditableObject.BeginEdit()
        {
            m_oldId = m_id;
            m_oldValue = m_value;
        }

        void IEditableObject.CancelEdit()
        {
            m_id = m_oldId;
            m_value = m_oldValue;
        }

        void IEditableObject.EndEdit()
        {
            m_oldId = string.Empty;
            m_oldValue = string.Empty;
        }
    }

    public partial class tracStringWindow : Window
    {
        public delegate void EditedTracStringsHandler(object obj, List<Entry> list);

        public event EditedTracStringsHandler EditedTracStrings;

        public List<Entry> data = new List<Entry>();
        public List<Entry> visibleEntries;
        public tracStringWindow(Dictionary<string,string> list)
        {
            InitializeComponent();
            foreach (var key in list.Keys)
            {
                data.Add(new Entry(key, list[key]));
            }
            visibleEntries = new List<Entry>(data);
            Grid.ItemsSource = visibleEntries;
        }
        private void DialogClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            EditedTracStrings?.Invoke(this, data);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Grid.ItemsSource = null;
            visibleEntries.Clear();

            foreach(var entry in data)
            {
                if (entry.m_id.ToUpper().Contains(SearchBox.Text.ToUpper()))
                {
                    visibleEntries.Add(entry);
                }
                else if (entry.m_value.ToUpper().Contains(SearchBox.Text.ToUpper()))
                {
                    visibleEntries.Add(entry);
                }
            }
            Grid.ItemsSource = visibleEntries;
        }
    }
}

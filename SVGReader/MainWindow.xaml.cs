using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;
using System;

namespace SVGReader
{
    public class tabItemClass
    {
        public string fileName { get; set; }
        public string tabID { get; set; }
        public NavPage navPage { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<tabItemClass> tabItems { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            tabItems = new ObservableCollection<tabItemClass>();
            AddNewTab();
            DataContext = this;
            TabsControl.ItemsSource = tabItems;
        }

        private void AddNewTab()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter += "Scalable Vector Graphics | *.svg";
            if (openFile.ShowDialog() != null)
            {
                FileManager.FileSourceList.Add(FileManager.GetNextID(), openFile.FileName);
                tabItems.Add(new tabItemClass
                {
                    fileName = Path.GetFileName(openFile.FileName),
                    tabID = Guid.NewGuid().ToString(),
                    navPage = new NavPage()
                }) ;
                TabsControl.ItemsSource = tabItems;
                TabsControl.SelectedIndex = tabItems.Count - 1;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                //TabsControl.Items.Remove(item);
                int selectedTabIndex = TabsControl.SelectedIndex;
                int index = tabItems.IndexOf(tabItems.Where(x => x.tabID == button.Tag.ToString()).FirstOrDefault());
                if (selectedTabIndex == index)
                {
                    if (selectedTabIndex == (tabItems.Count - 1))
                    {
                        tabItems.RemoveAt(index);
                        TabsControl.SelectedIndex = (tabItems.Count - 1);
                    }
                    else
                    {
                        tabItems.RemoveAt(index);
                        TabsControl.SelectedIndex = selectedTabIndex;
                    }
                }
            }
        }

        private void AddTabButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewTab();
        }
    }
}

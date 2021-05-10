using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using SVGReader.RSA;

namespace SVGReader
{
    public class tabItemClass 
    {
        public string fileName { get; set; }
        public string tabID { get; set; }
        public NavPage navPage { get; set; }
        public bool IsOneTab { get; set; }
    
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<tabItemClass> _tabItems;
        public ObservableCollection<tabItemClass> tabItems 
        {
            get { return _tabItems; }
            set { _tabItems = value; NotifyPropertyChanged("tabItems"); }
        }
        public MainWindow()
        {
            InitializeComponent();
            tabItems = new ObservableCollection<tabItemClass>();
            AddNewTab();
            DataContext = this;
            NotifyPropertyChanged("tabItems");
            RSAKeyGenerator.GenerateKeyPair(1024);
            //TabsControl.ItemsSource = tabItems;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
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
                    navPage = new NavPage(),
                    IsOneTab = true
                }) ;
                if (tabItems.Count >= 2)
                {
                    foreach(tabItemClass tab in tabItems)
                    {
                        tab.IsOneTab = false;
                    }
                }
                NotifyPropertyChanged("tabItems");
                //TabsControl.ItemsSource = tabItems;
                TabsControl.SelectedIndex = tabItems.Count - 1;
            }
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                int selectedTabIndex = TabsControl.SelectedIndex;
                int index = tabItems.IndexOf(tabItems.Where(x => x.tabID == button.Tag.ToString()).FirstOrDefault());
                if (selectedTabIndex == index)
                {
                    if (selectedTabIndex == (tabItems.Count - 1))
                    {
                        tabItems.RemoveAt(index);
                        TabsControl.SelectedIndex = (tabItems.Count - 1);
                        if (tabItems.Count == 1)
                        {
                            tabItems[0].IsOneTab = true;
                        }
                    }
                    else
                    {
                        tabItems.RemoveAt(index);
                        TabsControl.SelectedIndex = selectedTabIndex;
                        if (tabItems.Count == 1)
                        {
                            tabItems[0].IsOneTab = true;
                        }
                    }
                }
                else { tabItems.RemoveAt(index); }
                NotifyPropertyChanged("tabItems");
            }
        }

        private void AddTabButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewTab();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Rectangle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            this.DragMove();
        }
    }
}

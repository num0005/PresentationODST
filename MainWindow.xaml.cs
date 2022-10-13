﻿using System;
using System.Collections.Generic;
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
using System.Diagnostics;
using Microsoft.Win32;
using Xceed.Wpf.AvalonDock.Layout;
using PresentationODST.Controls;
using PresentationODST.Controls.LayoutDocuments;
using PresentationODST.Utilities;


namespace PresentationODST
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Main_Window;
        public static int NewTagCount = 1;
        public static Dialogs.TagGroupSelector GroupSelector;

        public MainWindow()
        {
            InitializeComponent();
            Main_Window = this;
            if (!InitializeProject())
            {
                MessageBox.Show("Please navigate to your ODSTEK install folder.", "Startup", MessageBoxButton.OK);
                Utilities.Path.SetODSTEKPath();
            }
        }

        public bool InitializeProject()
        {
            if (Properties.Settings.Default.ODSTEKPath.Length <= 0)
                return false;
            else
            {
                Bungie.ManagedBlamSystem.InitializeProject(Bungie.InitializationType.TagsOnly, Properties.Settings.Default.ODSTEKPath);
                return true;
            }
        }

        private void CommandBinding_Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                //InitialDirectory = Properties.Settings.Default.ODSTEKPath
            };
            if (ofd.ShowDialog() == true)
            {
                if (!ofd.FileName.Contains(Properties.Settings.Default.ODSTEKPath + @"\tags"))
                {
                    MessageBox.Show("You tried to open a tag outside of your working directory. Bad!", "Oops...");
                }
                else
                {
                    ManagedBlam.Tags.OpenTag(ofd.FileName);
                }
            }
        }

        private void Preferences_Click(object sender, RoutedEventArgs e)
        {
            LayoutDocument PreferencesTab = new LayoutDocument
            {
                Title = "Preferences",
                Content = new Preferences()
            };
            LayoutDocumentPane ldp = TagDock.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            ldp.Children.Add(PreferencesTab);
            ldp.SelectedContentIndex = ldp.IndexOfChild(PreferencesTab);
        }

        private void CommandBinding_New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ManagedBlam.Tags.NewTag();
        }

        private void CommandBinding_Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (TagDocuments.Children[TagDocuments.SelectedContentIndex].Content.GetType() != typeof(TagView)) return;
            TagView SaveTagView = (TagView)TagDocuments.Children[TagDocuments.SelectedContentIndex].Content;
            SaveTagView.Save();
        }

        private void SaveFileAs_Click(object sender, RoutedEventArgs e)
        {
            if (TagDocuments.Children[TagDocuments.SelectedContentIndex].Content.GetType() != typeof(TagView)) return;
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = Utilities.Path.ODSTEKTagsPath
            };
            if (sfd.ShowDialog() == true)
            {
                string[] SavePath = Utilities.Path.GetTagsRelativePath(sfd.FileName).Split('.');
                TagView SaveAsTagView = (TagView)TagDocuments.Children[TagDocuments.SelectedContentIndex].Content;
                SaveAsTagView.SaveAs(Bungie.Tags.TagPath.FromPathAndExtension(SavePath[0], SavePath[1]));
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Uri iconUri = new Uri("pack://application:,,,/PresentationODST.ico", UriKind.RelativeOrAbsolute); //make sure your path is correct, and the icon set as Resource
            this.Icon = BitmapFrame.Create(iconUri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

            //etc.
        }
    }
}

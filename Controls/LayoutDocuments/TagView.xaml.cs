﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PresentationODST.Controls.TagFieldControls;

namespace PresentationODST.Controls.LayoutDocuments
{
    /// <summary>
    /// Interaction logic for TagView.xaml
    /// </summary>
    public partial class TagView : UserControl
    {
        public Bungie.Tags.TagFile TagFile;

        public TagView()
        {
            InitializeComponent();
            DataContext = this;
            if (Properties.Settings.Default.EasterEggs)
            {
                Random rand = new Random();
                TagGrid.Background = new SolidColorBrush(Color.FromArgb(255, (byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255)));
            }
        }

        public void Save()
        {
            TagFile.Save();
        }

        public void SaveAs(Bungie.Tags.TagPath path)
        {
            TagFile.SaveAs(path);
        }

        // There is much that could be improved here but it's somewhat functional for now
        private void TagSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!TagSearchBox.IsLoaded) return;
            // Return the TagGrid back to it's prior state if the search is removed
            if (TagSearchBox.Text.Length <= 0)
            {
                foreach (UIElement element in TagGrid.Children)
                {
                    if (element is ITagFieldControlBase tfcb)
                    {
                        if (!Utilities.WPF.ExpertModeVisibility(tfcb.GetTagField())) continue;
                        element.Visibility = Visibility.Visible;
                        TagGrid.RowDefinitions[Grid.GetRow(element)].MaxHeight = double.PositiveInfinity; // changing the maxheight in this method is a hack to get around rows not having a visisbility property
                    }
                }
            }

            foreach (UIElement element in TagGrid.Children) 
            {
                TagGrid.RowDefinitions[Grid.GetRow(element)].MaxHeight = 0;
                element.Visibility = Visibility.Collapsed;
                if (element is ITagFieldControlBase tfcb)
                {
                    if (!tfcb.GetTagField().FieldName.Contains(TagSearchBox.Text))
                    {
                        if (element is TagFieldBlockControl tfbc)
                        {
                            if (tfbc.ElementGrid.Children.Count > 0)
                            {
                                foreach (UIElement tf in tfbc.ElementGrid.Children)
                                {
                                    SeachTags(tf, element);
                                }
                            } 
                        }
                    }
                    else
                    {
                        if (!Utilities.WPF.ExpertModeVisibility(tfcb.GetTagField())) continue; // Do not search hidden expert mode fields
                        element.Visibility = Visibility.Visible;
                        TagGrid.RowDefinitions[Grid.GetRow(element)].MaxHeight = double.PositiveInfinity;
                    }
                }
            }
        }

        private void SeachTags(UIElement element, UIElement rootElement)
        {
            if (element is ITagFieldControlBase tfcb)
            {
                if (!tfcb.GetTagField().FieldName.Contains(TagSearchBox.Text))
                {
                    if (element is TagFieldBlockControl tfbc)
                    {
                        if (tfbc.ElementGrid.Children.Count > 0)
                        {
                            foreach (UIElement tf in tfbc.ElementGrid.Children)
                            {
                                SeachTags(tf, rootElement);
                            }
                        }
                    }
                }
                else
                {
                    rootElement.Visibility = Visibility.Visible;
                    TagGrid.RowDefinitions[Grid.GetRow(rootElement)].MaxHeight = double.PositiveInfinity;
                }
            }
        }
    }
}

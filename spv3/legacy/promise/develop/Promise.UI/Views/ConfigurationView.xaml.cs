﻿using System.Windows;
using System.Windows.Controls;
using Promise.Library.Video;
using Promise.UI.Controller;

namespace Promise.UI.Views
{
    /// <summary>
    ///     Interaction logic for ConfigView.xaml
    /// </summary>
    public partial class ConfigurationView : Window
    {
        private readonly ConfigurationController _configurationController = new ConfigurationController();

        public ConfigurationView()
        {
            InitializeComponent();
            DataContext = _configurationController;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _configurationController.SaveConfiguration();
            Close();
        }

        private void ResolutionsComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ResolutionsComboBox.ItemsSource = new VideoResolution().GameVideoResolutions;
        }

        private void RefreshRatesComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshRatesComboBox.ItemsSource = new VideoRefreshRate().GameRefreshRates;
        }

        private void AdaptersComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            AdaptersComboBox.ItemsSource = new VideoAdapter().AdapterNamesList;
        }

        private void AdaptersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _configurationController.SelectedAdapterIndex = AdaptersComboBox.SelectedIndex;
        }

        private void ResolutionsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _configurationController.SelectedVideoResolution = ResolutionsComboBox.SelectedItem as VideoResolution;
        }

        private void RefreshRatesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _configurationController.SelectedVideoRefreshRate = RefreshRatesComboBox.SelectedItem as VideoRefreshRate;
        }
    }
}
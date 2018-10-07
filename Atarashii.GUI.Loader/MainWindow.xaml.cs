﻿using System.ComponentModel;
using System.Windows;

namespace Atarashii.GUI.Loader
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Main _main;

        public MainWindow()
        {
            InitializeComponent();
            _main = (Main) DataContext;
            BaseApplication.Initialise(this, _main);
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            _main.Load();
        }

        private void Browse(object sender, RoutedEventArgs e)
        {
            _main.HcePath = BaseApplication.PickFile("HCE Executable|haloce.exe");
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            BaseApplication.Exit();
        }
    }
}
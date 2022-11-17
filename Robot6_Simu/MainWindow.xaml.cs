﻿using Robot6_Simu.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Media=System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Shaps=System.Windows.Shapes;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System.Drawing;
using System.Drawing.Drawing2D;
using Utilities;
using Robot6_Simu.Controls;
using System.Threading;

namespace Robot6_Simu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ThemeSettingsViewModel? tsv;
        int themeId = 0;
        List<Swatch> Swatches;
        Home home;

        public MainWindow()
        {
            InitializeComponent();
            tsv = new ThemeSettingsViewModel();
            Swatches = tsv.Swatches.ToList();
            Closing += MainWindow_Closing;
            //Loaded += MainWindow_Loaded;
            themeId = Properties.Settings.Default.ThemeId;
            Swatches = tsv.Swatches.ToList();
            tsv.ApplyPrimaryCommand.Execute(Swatches[themeId]);
            var mvm = DataContext as MainWindowViewModel;
            Home? c = mvm?.MenuItems[0].Content as Home;
            home = c;
            AddHotKeys();
            DataContext = new MainWindowViewModel();
        }
        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.ThemeId = themeId;
            GC.Collect();
            Application.Current.Shutdown();
        }



        private void AddHotKeys()
        {
            try
            {
                RoutedCommand Settings1 = new RoutedCommand();
                Settings1.InputGestures.Add(new KeyGesture(Key.Q, ModifierKeys.Control));
                CommandBindings.Add(new CommandBinding(Settings1, My_first_event_handler));

                RoutedCommand Settings2 = new RoutedCommand();
                Settings2.InputGestures.Add(new KeyGesture(Key.A, ModifierKeys.Control));
                CommandBindings.Add(new CommandBinding(Settings2, My_second_event_handler));
                RoutedCommand Settings3 = new RoutedCommand();
                Settings3.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Control));
                CommandBindings.Add(new CommandBinding(Settings3, My_third_event_handler));
                RoutedCommand Settings6 = new RoutedCommand();
                //Settings6.InputGestures.Add(new KeyGesture(Key.M, ModifierKeys.Control));
                //CommandBindings.Add(new CommandBinding(Settings6, My_event_handler6));

            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }
        }
        private void My_first_event_handler(object sender, ExecutedRoutedEventArgs e)
        {
            themeId++;
            themeId = themeId >= Swatches.Count ? 0 : themeId;
            tsv?.ApplyPrimaryCommand.Execute(Swatches[themeId]);
            var mvm = DataContext as MainWindowViewModel;
            Home? c = mvm?.MenuItems[0].Content as Home;
            home = c;
            //home?.Render();
        }

        private void My_second_event_handler(object sender, ExecutedRoutedEventArgs e)
        {
            themeId--;
            themeId = themeId < 0 ? Swatches.Count - 1 : themeId;
            tsv?.ApplyPrimaryCommand.Execute(Swatches[themeId]);
            var mvm = DataContext as MainWindowViewModel;
            Home? c = mvm?.MenuItems[0].Content as Home;
            home = c;
            //home?.Render();
        }
        //private void My_event_handler6(object sender, ExecutedRoutedEventArgs e)
        //{
        //    if (PCIDataViewModel.Simu)
        //        PCIDataViewModel.Simu = false;
        //    else
        //        PCIDataViewModel.Simu = true;
        //}
        private void My_third_event_handler(object sender, ExecutedRoutedEventArgs e)
        {
            if (tsv?.IsDarkTheme ?? false)
                tsv.IsDarkTheme = false;
            else
                tsv!.IsDarkTheme = true;
            var mvm = DataContext as MainWindowViewModel;
            Home? c = mvm?.MenuItems[0].Content as Home;
            home = c;
            //home?.Render();
            //var paletteHelper = new PaletteHelper();
            //var theme = paletteHelper.GetTheme();

            //theme.SetBaseTheme(theme.GetBaseTheme() == BaseTheme.Light ? Theme.Dark : Theme.Light);
            //paletteHelper.SetTheme(theme);
        }
        private void OnSelectedItemChanged(object sender, DependencyPropertyChangedEventArgs e) => MainScrollViewer.ScrollToHome();

    }
}

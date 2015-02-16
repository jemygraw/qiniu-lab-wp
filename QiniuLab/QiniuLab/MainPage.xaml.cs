﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using QiniuLab.Resources;
using QiniuLab.ViewModels;

namespace QiniuLab
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void SimpleUploadLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selected = SimpleUploadLongListSelector.SelectedItem;
            if (selected == null)
            {
                return;
            }

            ItemViewModel ivm = SimpleUploadLongListSelector.SelectedItem as ItemViewModel;
            Int32 id = ivm.ID;
            string name = ivm.Name;
            string navUrl = "";
            switch (id)
            {
                case 0:
                    navUrl = string.Format("/Controls/Upload/SimpleUploadWithoutKey.xaml?selectedItem={0}", ivm.ID); break;
            }
            if (
                !string.IsNullOrWhiteSpace(navUrl))
            {
                NavigationService.Navigate(new Uri(navUrl, UriKind.Relative));
            }
            this.SimpleUploadLongListSelector.SelectedItem = null;
        }

        private void AdvancedUploadLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
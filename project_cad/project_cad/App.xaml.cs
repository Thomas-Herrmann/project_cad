﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using project_cad.Services;
using project_cad.Views;

namespace project_cad
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            //DependencyService.Register<MockDataStore>(); TODO: add actual datastore
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

﻿using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using ContosoInsurance.Models;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.WindowsAzure.MobileServices.Files;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using ContosoInsurance.Helpers;
using System.Threading;
using ContosoInsurance.Converters;
using ContosoInsurance.ViewModels;

namespace ContosoInsurance.Views
{
	public partial class PartyContactViewiOS : ContentPage
	{
        private ClaimViewModel claimViewModel;
        public PartyContactViewiOS (ClaimViewModel cv)
		{
            Title = "Contoso Insurance";
            InitializeComponent();

            claimViewModel = cv;
            BindingContext = claimViewModel;
            InitBottomToolBar();

            if (claimViewModel.OtherPartyMobilePhone.Length > 0)
            {
                partyPhoneNumber.Text = claimViewModel.OtherPartyMobilePhone;
            }
            partyPhoneNumber.TextChanged += PartyPhoneNumber_TextChanged;
        }

        private void PartyPhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            Entry number = (Entry)sender;
            if (Regex.IsMatch(this.partyPhoneNumber.Text, @"^(\d{3})$"))
            {
                number.Text += "-";
            }
            else if (Regex.IsMatch(this.partyPhoneNumber.Text, @"^(\d{3}-)\d{3}$"))
            {
                number.Text += "-";
            }
        }

        public void InitBottomToolBar()
        {
            var nextTapGestureRecognizer = new TapGestureRecognizer();
            nextTapGestureRecognizer.Tapped += NextButton_Clicked;
            bottomView.NextImage.GestureRecognizers.Add(nextTapGestureRecognizer);

            bottomView.PreviousImage.IsVisible = true;
            var backTapGestureRecognizer = new TapGestureRecognizer();
            backTapGestureRecognizer.Tapped += PreviousButton_Clicked;
            bottomView.PreviousImage.GestureRecognizers.Add(backTapGestureRecognizer);
        }

        #region User Common Actions

        private async void PreviousButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(true);
            return;
        }

        private async void NextButton_Clicked(object sender, EventArgs e)
        {                
            if (Regex.IsMatch(this.partyPhoneNumber.Text, @"^(\d{3}-)\d{3}-\d{4}$"))
            {
               if(claimViewModel.OtherPartyMobilePhone != partyPhoneNumber.Text)
                {
                    await claimViewModel.UpdateClaimPartyMobileAsync(partyPhoneNumber.Text);
                }
                var nextPage = new IncidentDetailViewiOS(claimViewModel);
                await Navigation.PushAsync(nextPage, true);
                NavigationPage.SetHasBackButton(nextPage, false);
            }
            else
            {
                await DisplayAlert("Error", "You entered an invalid phone number. Please enter the phone number in the XXX-XXX-XXXX format.", "Close");
            }
        }

        public async void MenuClicked(object sender, EventArgs e)
        {
            this.menuList.IsVisible = !this.menuList.IsVisible;
        }

        public async void LogoutBtn_Tapped(object sender, EventArgs e)
        {
            this.menuList.IsVisible = false;
            await MobileServiceHelper.msInstance.DoLogOutAsync();
            await Navigation.PopToRootAsync(true);
        }

        public async void SettingsBtn_Tapped(object sender, EventArgs e)
        {
            this.menuList.IsVisible = false;
            var settingsView = new SettingsViewiOS();
            NavigationPage.SetHasBackButton(settingsView, false);
            await Navigation.PushAsync(settingsView, false);
        }

        #endregion
    }
}

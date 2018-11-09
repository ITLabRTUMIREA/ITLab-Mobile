﻿using Http_Post.Extensions.Responses.Event;
using Http_Post.Services;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using Xamarin.Forms;
using System.Collections.Generic;
using Http_Post.Controls;
using Models.PublicAPI.Responses.Event;

namespace Http_Post.Pages
{
    public partial class OneEventPage : ContentPage
    {
        private readonly HttpClient client = HttpClientFactory.HttpClient;
        private readonly Guid eventId;
        private EventViewExtended OneEvent { get; set; }

        public OneEventPage(Guid id)
        {
            eventId = id;
            Show();
            ChangeToolBar();
        }

        private void Init()
        {
            BindingContext = OneEvent; // necessarilly before 'InitializeComponent'
            InitializeComponent();

            UpdateLanguage();
            AddShifts();
        }

        private async void Show()
        {
            try
            {
                var response = await client.GetStringAsync($"event/{eventId}");
                var receivedData = JsonConvert.DeserializeObject<OneObjectResponse<EventViewExtended>>(response);

                if (receivedData.StatusCode != ResponseStatusCode.OK)
                    throw new Exception($"error {receivedData.StatusCode}");
                OneEvent = receivedData.Data;

                Init(); // init binding content 

            } catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        void AddShifts()
        {
            stackLayout.Children.Add(new StackShiftView(OneEvent.Shifts).stackLayout);
        }

        void UpdateLanguage()
        {
            Title = OneEvent.Title;
            lblShifts.Text = Res.Resource.Shifts + ":";
            lblBeginTime.Text = OneEvent.Shifts[0].BeginTime.ToLocalTime().ToString("dd MMMM, yyyy. HH:mm");
            lblEndTime.Text = OneEvent.Shifts[OneEvent.Shifts.Count - 1].EndTime.ToLocalTime().ToString("dd MMMM, yyyy. HH:mm");
        }

        void ChangeToolBar()
        {
            if (!GetRight())
                return;

            var itemChange = new ToolBar.ToolBarItems().Item(null, 1, ToolbarItemOrder.Primary, "ChangingPencil.png");
            itemChange.Clicked += async (s, e) =>
            {
                await Navigation.PushAsync(new CreateEventPage(OneEvent));
            };
            ToolbarItems.Add(itemChange);
        }

        bool GetRight()
        {
            string whatToCheck = "CanEditEvent";
            foreach (var item in CurrentUserIdFactory.UserRoles)
                if (item.Equals(whatToCheck))
                    return true;
            return false;
        }
    }
}
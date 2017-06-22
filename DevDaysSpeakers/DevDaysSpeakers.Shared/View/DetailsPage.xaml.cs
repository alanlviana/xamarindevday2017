using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using DevDaysSpeakers.Model;
using DevDaysSpeakers.Services;
using Plugin.TextToSpeech;

using DevDaysSpeakers.ViewModel;

namespace DevDaysSpeakers.View
{
    public partial class DetailsPage : ContentPage
    {
        Speaker speaker;
        SpeakersViewModel vm;
        public DetailsPage(Speaker speaker, SpeakersViewModel viewModel)
        {
            InitializeComponent();
            
            //Set local instance of speaker and set BindingContext
            this.speaker = speaker;
            this.vm = viewModel;
            BindingContext = this.speaker;
            
            ButtonSpeake.Clicked += ButtonSpeake_Clicked;
            ButtonWebsite.Clicked += ButtonWebsite_Clicked;
            ButtonAnalyze.Clicked += ButtonAnalyze_ClickedAsync;
            ButtonSave.Clicked += ButtonSave_Clicked;
        }

        private async void ButtonSave_Clicked(object sender, EventArgs e)
        {
            speaker.Title = EntryTitle.Text;
            await vm.UpdateSpeaker(speaker);
            await Navigation.PopAsync();
        }

        private async void ButtonAnalyze_ClickedAsync(object sender, EventArgs e)
        {
            // Don't Working
            var level = await EmotionService.GetAverageHappinessScoreAsync(this.speaker.Avatar);
            await DisplayAlert("Happiness Level", EmotionService.GetHappinessMessage(level),"OK");
        }

        private void ButtonWebsite_Clicked(object sender, EventArgs e)
        {
            if (speaker.Website.StartsWith("http"))
            {
                Device.OpenUri(new Uri(speaker.Website));
            }
        }

        protected override void OnDisappearing()
        {
            ButtonSpeake.Clicked -= ButtonSpeake_Clicked;
            ButtonWebsite.Clicked -= ButtonWebsite_Clicked;
        }

        private void ButtonSpeake_Clicked(object sender, EventArgs e)
        {
            CrossTextToSpeech.Current.Speak(this.speaker.Description);
        }
    }
}

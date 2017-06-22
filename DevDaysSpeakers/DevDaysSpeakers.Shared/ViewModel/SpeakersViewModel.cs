using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using DevDaysSpeakers.Model;
using DevDaysSpeakers.Services;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace DevDaysSpeakers.ViewModel
{
    public class SpeakersViewModel : INotifyPropertyChanged
    {

        public Command GetSpeakersCommand { get; set; }

        private bool busy;

        public bool IsBusy
        {
            get { return busy; }
            set {
                busy = value;
                OnPropertyChanged();
                GetSpeakersCommand.ChangeCanExecute();
            }
        }
        public ObservableCollection<Model.Speaker> Speakers { get; set; }

        public SpeakersViewModel()
        {
            this.Speakers = new ObservableCollection<Speaker>();
            this.GetSpeakersCommand = new Command(async () => await this.GetSpeakers(),
                                                        () => !IsBusy);
        }

        private async Task GetSpeakers()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                using (var client = new HttpClient())
                {
                    //var json = await client.GetStringAsync("https://demo4404797.mockable.io/speakers");
                    //var itens = JsonConvert.DeserializeObject<List<Speaker>>(json);

                    var service = DependencyService.Get<AzureService>();
                    var itens = await service.GetSpeakers();

                    Speakers.Clear();
                    foreach(var item in itens)
                    {
                        Speakers.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task UpdateSpeaker(Speaker speaker)
        {
            try
            {
                IsBusy = true;
                var service = DependencyService.Get<AzureService>();
                await service.UpdateSpeaker(speaker);
                await GetSpeakers();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var changed = PropertyChanged;
            if(changed == null)
            {
                return;
            }

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Net;

namespace MedicamentStore
{
    public class AddClientViewModel : MainWindowViewModel
    {
        // Define the custom event
        public TextEntryViewModel Name { get; set; }

        public TextEntryViewModel Adresse { get; set; }

        public ICommand SaveCommand { get; set; }
        public ICommand ClosedCommand { get; set; }
        public Window Window { get; }

        public AddClientViewModel(Window window) : base(window)
        {
            Name = new TextEntryViewModel
            {
                Label = "Nom"

            };
            Adresse = new TextEntryViewModel
            {
                Label= "adresse",
                height = 120
            };

            SaveCommand = new RelayCommand(async () => await Save());
            ClosedCommand = new RelayCommand(async () => await CloseWindow());
            Window = window;
        }

        private async Task CloseWindow()
        {
            Window.Close();
            await Task.Delay(1);
        }

        private async Task Save()
        {
            var result = await IoC.ClientManager.InsertClientAsync(new Client
            {
                Name = Name.EnteredText,
                Adresse = Adresse.EnteredText,
            });
            if (result.Successful)
            {
                await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Succes,
                 "Client ajouté"));
            }
            else
            {
                await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Error,
                 result.ErrorMessage));
                return;
            }
        }
    }
}

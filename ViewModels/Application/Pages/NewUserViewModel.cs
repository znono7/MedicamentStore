using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MedicamentStore
{
    public class NewUserViewModel : BaseViewModel
    {
        /// <summary>
        /// The current users name
        /// </summary>
        public TextEntryViewModel Name { get; set; }

        /// <summary>
        /// The current users username
        /// </summary>
        public TextEntryViewModel Username { get; set; }

        /// <summary>
        /// The current users password
        /// </summary>
        public PasswordEntryViewModel Password { get; set; }
        public PasswordEntryViewModel NewPassword { get; set; }

        /// <summary>
        /// The current users email
        /// </summary>
        public TextEntryViewModel Prenom { get; set; }


        public ICommand ReturnCommand {  get; set; }
        public ICommand SaveCommand {  get; set; }

        public NewUserViewModel()
        {
            ReturnCommand = new RelayCommand(async () => await BackToUsersPage());
            SaveCommand = new RelayCommand(async () => await Save());
            Name = new TextEntryViewModel() { Label = "Nom" };
            Prenom = new TextEntryViewModel() { Label = "Prenom" };
            Username = new TextEntryViewModel() { Label = "nom d'utilisateur" };
            Password = new PasswordEntryViewModel() { Label= "Mot de passe" };
            NewPassword = new PasswordEntryViewModel() { Label = "Confirmez le mot de passe" };
        }

        public async Task Save()
        {
            if (Password.Password.Unsecure() != NewPassword.Password.Unsecure())
            {
                await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Warning,
                    "Mot de passe incompatible"));
                return;
            }

           var Result = await IoC.UserManager.InsertUserAsync(new User
           {
               Nom = Name.EnteredText,
               Prenom = Prenom.EnteredText,
               UserName = Username.EnteredText,
               Password = Password.Password.Unsecure(),
               Type = "NORMALE"
           });

            if (!Result.Successful)
            {
                await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Error,
                   Result.ErrorMessage));
                return;
            }
            else
            {
                await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Succes,
                  "Utilisateur ajouté"));
            }
           
        }

        public async Task BackToUsersPage()
        {
            IoC.Application.GoToPage(ApplicationPage.Users);
            await Task.Delay(5);
        }
    }
}

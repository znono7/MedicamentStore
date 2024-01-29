using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace MedicamentStore
{
    public class ParemetresViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// The text to show while loading text
        /// </summary>
        private string mLoadingText = "...";

        #endregion

        #region Public Properties
        public int Id { get; set; }
        /// <summary>
        /// The current users name
        /// </summary>
        public TextEntryEditedViewModel Nom { get; set; }

        /// <summary>
        /// The current users username
        /// </summary>
        public TextEntryEditedViewModel Prenom { get; set; }

        /// <summary>
        /// The current users password
        /// </summary>
        public PasswordEntryEditedViewModel Password { get; set; }

        /// <summary>
        /// The current users email
        /// </summary>
        public TextEntryEditedViewModel UserName { get; set; }



        #endregion

        #region Transactional Properties

        /// <summary>
        /// Indicates if the first name is current being saved
        /// </summary>
        public bool FirstNameIsSaving { get; set; }

        /// <summary>
        /// Indicates if the last name is current being saved
        /// </summary>
        public bool LastNameIsSaving { get; set; }

        /// <summary>
        /// Indicates if the username is current being saved
        /// </summary>
        public bool UsernameIsSaving { get; set; }

  
        /// <summary>
        /// Indicates if the password is current being changed
        /// </summary>
        public bool PasswordIsChanging { get; set; }

        #endregion

        #region Public Commands



        /// <summary>
        /// The command to clear the users data from the view model
        /// </summary>
        public ICommand ClearUserDataCommand { get; set; }

       

        /// <summary>
        /// Saves the current first name to the server
        /// </summary>
        public ICommand SaveNomCommand { get; set; }

        /// <summary>
        /// Saves the current last name to the server
        /// </summary>
        public ICommand SavePrenomCommand { get; set; }

        /// <summary>
        /// Saves the current username to the server
        /// </summary>
        public ICommand SaveUsernameCommand { get; set; }

       

        #endregion
        public ParemetresViewModel()
        {
            Nom = new TextEntryEditedViewModel 
            { 
                Label = "Nom",
                OriginalText = mLoadingText,
                CommitAction = SaveNomAsync
               
            };
            Prenom = new TextEntryEditedViewModel 
            { 
                Label = "Prenom",
                OriginalText = mLoadingText,
                CommitAction = SavePrenomAsync
                
            };
            UserName = new TextEntryEditedViewModel 
            { 
                Label = "Nom d'utilisateur",
                OriginalText = mLoadingText,
                CommitAction = SaveUserNomAsync
            };
            Password = new PasswordEntryEditedViewModel 
            { 
                Label = "Mot de Passe", 
                FakePassword = "********" ,
                CommitAction = SavePasswordAsync
            };

            SaveNomCommand = new RelayCommand(async () => await SaveNomAsync());
            SavePrenomCommand = new RelayCommand(async () => await SavePrenomAsync());
            SaveUsernameCommand = new RelayCommand(async () => await SaveUserNomAsync());
            
        }

        private async Task<bool> SavePasswordAsync()
        {
            if (PasswordIsChanging)
                return false;
            try
            {
                PasswordIsChanging = true;
                // Make sure the user has entered the same password
                if (Password.NewPassword.Unsecure() != Password.ConfirmPassword.Unsecure())
                {
                    // Display error
                    await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel
                    (
                        
                          NotificationType.Warning,
                         "Le Nouveau mot de passe et la confirmation du mot de passe doivent correspondre"
                    ));

                    // Return fail
                    return false;
                }
                var result = await IoC.UserManager.UpdateUserPasswordAsync(new UpdateUserPassword
                {
                    Id = Id,
                    CurrentPassword = Password.CurrentPassword.Unsecure(),
                    NewPassword = Password.ConfirmPassword.Unsecure()
                });

                if (result.Successful)
                {
                    await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Succes, "Le Mot de Pass a été modifié avec succès"));

                    return true;
                }
                await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Error, result.ErrorMessage));

                return false;
            }
           finally 
            {
                PasswordIsChanging = false;
                
            }
        }

        private async Task<bool> SaveUserNomAsync()
        {
            if (UsernameIsSaving)
                return false;
            try
            {
                UsernameIsSaving = true;
                var result = await IoC.UserManager.UpdateUserProfilAsync(new UpdateUserProfile
                {
                    Id = Id,
                    UserName = UserName.OriginalText
                });
                if (result.Successful)
                {
                    await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Succes, "Le Nom d'utilisateur a été modifié avec succès"));
                    return true;
                }
                else
                {
                    await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Error, result.ErrorMessage));
                    return false;

                }


            }
            finally
            {

                UsernameIsSaving = false;
            }
        }

        private async Task<bool> SavePrenomAsync()
        {
            if (LastNameIsSaving)
                return false;
            try
            {
                LastNameIsSaving = true;
                var result = await IoC.UserManager.UpdateUserProfilAsync(new UpdateUserProfile
                {
                    Id = Id,
                    Prenom = Prenom.OriginalText
                });
                if (result.Successful)
                {
                    await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Succes, "Le Prenom a été modifié avec succès"));
                    return true;
                }
                else
                {
                    await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Error, result.ErrorMessage));
                    return false;

                }


            }
            finally
            {

                LastNameIsSaving = false;
            }
        }

        private async Task<bool> SaveNomAsync()
        {
            if (FirstNameIsSaving)
                return false;
            try
            {
                FirstNameIsSaving = true;
                var result = await IoC.UserManager.UpdateUserProfilAsync(new UpdateUserProfile
                {
                    Id = Id,
                    Nom = Nom.OriginalText
                });
                if (result.Successful)
                {
                    await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Succes, "Le Nom a été modifié avec succès"));
                    return true;
                }
                else
                {
                    await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Error, result.ErrorMessage));
                    return false;

                }


            }
            finally 
            {

                FirstNameIsSaving = false;
            }

        }

        /// <summary>
        /// Loads the settings from the local data store and binds them 
        /// to this view model
        /// </summary>
        /// <returns></returns>
        public async Task UpdateValuesFromDbAsync(LoginUserResult storedCredentials)
        {

            Id = storedCredentials.Id;
            // Set first name
            Nom.OriginalText = storedCredentials?.Nom;

            // Set last name
            Prenom.OriginalText = storedCredentials?.Prenom;

            // Set username
            UserName.OriginalText = storedCredentials?.UserName;

            await Task.Delay(10);
        }

    }
}

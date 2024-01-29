using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
    public class UsersViewModel : BaseViewModel
    {


        private ObservableCollection<UserCredentials> _users;

        public ObservableCollection<UserCredentials> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        public string NbrUser { get; set; } = "Aucun utilisateur";
        public ICommand? NewUser {  get; set; } 
        public ICommand? DeleteCommand {  get; set; }
        public UsersViewModel() 
        {
            FetchUsersFromDatabase().Wait();

            NewUser = new RelayCommand(async () => await NewUserAsync());
            DeleteCommand = new RelayParameterizedCommand(async (param) => await DeleteUser(param));
        }

        public async Task DeleteUser(object param)
        {
            if (param is int userId)
            {
                var c = new ConfirmBoxDialogViewModel
                {
                    Title = "Confirmer la Suppression",
                    Message = "Sont sûrs du Processus de Suppression ?"
                };
                await IoC.ConfirmBox.ShowMessage(c);
                if (c.IsConfirmed)
                {
                  
                    var result = await IoC.UserManager.DeleteUserAsync(userId);
                    if (result.Successful)
                    {
                        var userToRemove = Users.FirstOrDefault(u => u.Id == userId);
                        if (userToRemove != null)
                        {
                            Users.Remove(userToRemove);
                        }
                        string s = "";
                        if (Users.Count == 1)
                        {
                            s = "Utilisateur";
                        }
                        else
                        {
                            s = "Utilisateurs";
                        }
                        NbrUser = $"{Users.Count} {s}";
                        await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Succes, "L'utilisateur a été supprimé"));
                    }
                    else
                    {
                        await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Succes, result.ErrorMessage));

                    }
                }  
            }
        }

        public async Task NewUserAsync()
        {
            IoC.Application.GoToPage(ApplicationPage.NewUser);
            await Task.Delay(1);
        }

        public async Task FetchUsersFromDatabase()
        {
            var result = await IoC.UserManager.GetUsersAsync();

           if (result.Any() )
           {
                Users = new ObservableCollection<UserCredentials>(result);
                string s = "";
                if(result.Count == 1)
                {
                    s = "Utilisateur";
                }
                else
                {
                    s = "Utilisateurs";
                }
                NbrUser = $"{result.Count} {s}";
            }
            else
            {
                return;
            }
            
        }
    }
}

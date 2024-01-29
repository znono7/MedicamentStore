using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace MedicamentStore
{
    public class LoginViewModel: BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The email of the user
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// A flag indicating if the login command is running
        /// </summary>
        public bool LoginIsRunning { get; set; } = false;

       

        #endregion

        #region Commands

        /// <summary>
        /// The command to login
        /// </summary>
        public ICommand LoginCommand { get; set; }



        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public LoginViewModel()
        {
            // Create commands
            LoginCommand = new RelayParameterizedCommand(async (parameter) => await LoginAsync(parameter));

        }

        #endregion

        /// <summary>
        /// Attempts to log the user in
        /// </summary>
        /// <param name="parameter">The <see cref="SecureString"/> passed in from the view for the users password</param>
        /// <returns></returns>
        public async Task LoginAsync(object parameter)
        {
                IoC.Application.GoToPage(ApplicationPage.Home);
            return;

            //if (LoginIsRunning)
            //{
            //    return;
            //}

            //try
            //{
            //    LoginIsRunning = true;
            //    var Result = await IoC.UserAuth.GetUserByUsernameAsync(new LoginCredentials
            //    {
            //        UserName = UserName,
            //        Password = (parameter as IHavePassword).SecurePassword.Unsecure()

            //});
            //    if (!Result.Successful)
            //    {
            //        await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Error, Result.ErrorMessage));
            //        return;
            //    }

            //    await Task.Delay(1000);
            //    IoC.Application.GoToPage(ApplicationPage.Home);
                
            //    await IoC.Settings.UpdateValuesFromDbAsync(Result.Response);


            //}
            //catch 
            //{
            //}
            //finally 
            //{
            //    LoginIsRunning = false;
            //}
            
           
           
        }
    }
}

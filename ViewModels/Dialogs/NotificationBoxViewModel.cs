using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
   public class NotificationBoxViewModel : BaseDialogViewModel
    {

        /// <summary>
        /// The message to display
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The background color of the Box in ARGB value
        /// </summary>
        public string PrimaryBackground { get; set; }

        /// <summary>
        /// The Line color of the Box in ARGB value
        /// </summary>
        public string SecendaryBackground { get; set; }

        public  NotificationType NotificationType { get; set; }

        public IconType IconType { get; set; }

        public NotificationBoxViewModel(NotificationType notificationType , string message)
        {
            SetType(notificationType);
            Message = message;
        }

        public void SetType(NotificationType notificationType)
        {
            switch (notificationType)
            {
                case NotificationType.Info:
                    IconType = IconType.Info;
                    PrimaryBackground = "E0EEFB";
                    SecendaryBackground = "62ACED";
                    Title = "Information";
                    break;
                case NotificationType.Warning:
                    IconType = IconType.Warning;
                    PrimaryBackground = "FFF1DB";
                    SecendaryBackground = "FFB74D";
                    Title = "Avertissement";

                    break;
                case NotificationType.Error:
                    IconType = IconType.Error;
                    PrimaryBackground = "FFE7E0";
                    SecendaryBackground = "FF8964";
                    Title = "Erreur";

                    break;
                case NotificationType.Succes:
                    IconType = IconType.Succes;
                    PrimaryBackground = "E6F4E7";
                    SecendaryBackground = "82C785";
                    Title = "Succès";
                    break;
                default:
                    break;
            }
        }
    }
}

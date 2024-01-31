using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MedicamentStore
{
    /// <summary>
    /// Interaction logic for MouvementPage.xaml
    /// </summary>
    public partial class MouvementPage : BasePage<MouvementViewModel>
    {
        private bool isExpanded = false;
        public MouvementPage()
        {
            InitializeComponent();
        }
        public MouvementPage(MouvementViewModel viewModel) : base(viewModel) 
        {
            InitializeComponent();
        }

        private void expandButton_Click(object sender, RoutedEventArgs e)
        {
            isExpanded = !isExpanded;

            DoubleAnimation rotateAnimation = new DoubleAnimation
            {
                To = isExpanded ? 180 : 0,
                Duration = TimeSpan.FromSeconds(0.3)
            };

            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);

            //if (isExpanded)
            //{
            //    popup.IsOpen = true;
            //}
            //else
            //{
            //    popup.IsOpen = false;
            //}
        }
    }
}

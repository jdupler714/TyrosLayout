using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TyrosLayout
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
            
            // Define the StackPanel
            StackPanel myStackPanel = new StackPanel();
            myStackPanel.HorizontalAlignment = HorizontalAlignment.Left;
            myStackPanel.VerticalAlignment = VerticalAlignment.Top;

            // Define child content
            Button myButton1 = new Button();
            myButton1.Content = "Button 1";
            Button myButton2 = new Button();
            myButton2.Content = "Button 2";
            Button myButton3 = new Button();
            myButton3.Content = "Button 3";

            // Add child elements to the parent StackPanel
            myStackPanel.Children.Add(myButton1);
            myStackPanel.Children.Add(myButton2);
            myStackPanel.Children.Add(myButton3);

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string content = (sender as Button).Content.ToString();
            // Tab Logic
            if (content.Equals("Home"))
            {
                Home home = new Home();
                this.NavigationService.Navigate(home);
            }  
            else if (content.Equals("Upload"))
            {
                Upload upload = new Upload();
                this.NavigationService.Navigate(upload);
            }
            else
            {
                Pending pending = new Pending();
                this.NavigationService.Navigate(pending);
            }
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace Draw
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
            copyright.Text = "Copyright © " + DateTime.Now.Year + " Chromium Labs" + "\nAll rights reserved.";
        }

        private void btnContact_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "";
            emailComposeTask.Body = "";
            emailComposeTask.To = "hovokhc@outlook.com";

            emailComposeTask.Show();

        }

        private void btnRate_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();

            marketplaceReviewTask.Show();

        }
    }
}
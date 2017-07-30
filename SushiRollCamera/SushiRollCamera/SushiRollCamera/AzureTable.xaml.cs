using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.WindowsAzure.MobileServices;
using SushiRollCamera.DataModels;

namespace SushiRollCamera
{
    public partial class AzureTable : ContentPage
    {
        MobileServiceClient client = AzureManager.AzureManagerInstance.AzureClient;

        public AzureTable()
        {
            InitializeComponent();
        }

        async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            List<NotSushiRollModel> notSushiRollInformation = await AzureManager.AzureManagerInstance.GetSushiRollInformation();

            SushiRollList.ItemsSource = notSushiRollInformation;
        }
    }
}
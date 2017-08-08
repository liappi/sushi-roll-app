using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using SushiRollCamera.DataModels;

namespace SushiRollCamera
{
    public class AzureManager
    {

        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<NotSushiRollModel> notSushiRollTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("https://mysushiroll.azurewebsites.net");
            this.notSushiRollTable = this.client.GetTable<NotSushiRollModel>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }

        public async Task<List<NotSushiRollModel>> GetSushiRollInformation()
        {
            return await this.notSushiRollTable.ToListAsync();
        }

        public async Task PostSushiRollInformation(NotSushiRollModel notSushiRollModel)
        {
            await this.notSushiRollTable.InsertAsync(notSushiRollModel);
        }

    }
}

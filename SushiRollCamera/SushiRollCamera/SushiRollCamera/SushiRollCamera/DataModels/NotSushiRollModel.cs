using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

//Model class to represent the tables in the backend
namespace SushiRollCamera.DataModels
{
    public class NotSushiRollModel
    {
        [JsonProperty(PropertyName = "Id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "Tag")]
        public string Tag { get; set; }

        
    }
}
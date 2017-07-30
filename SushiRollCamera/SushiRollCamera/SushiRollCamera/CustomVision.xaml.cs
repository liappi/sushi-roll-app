using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SushiRollCamera.Model;
using Xamarin.Forms;
using Plugin.Geolocator;
using SushiRollCamera.DataModels;
using Newtonsoft.Json.Linq;

namespace SushiRollCamera
{
    public partial class CustomVision : ContentPage
    {
        public CustomVision()
        {
            InitializeComponent();
        }

        private async void loadCamera(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            //Checks if we have permission to use the camera
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "OH NOO there is no camera available.", "OK");
                return;
            }

            //Takes a photo
            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,  //Resizes photo to 50% of the original
                Directory = "Sample",
                Name = $"{DateTime.UtcNow}.jpg"
            });

            if (file == null)
                return;

            //This displays the image
            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });

            //Gives the response of whether the photo taken is of a sushi roll or not
            await MakePredictionRequest(file);

            //Gives the coordinates at which the photo was taken
            await postLocationAsync();
        }

        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }

        async Task MakePredictionRequest(MediaFile file)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Prediction-Key", "3593f17c5d574fa99b6d645b95472851");

            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/bc8db77b-6ff0-45bb-a32d-9fa42bf3bb42/image?iterationId=c93a008e-a546-45fc-ad2b-de36091f2c5f";

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);

                using (var content = new ByteArrayContent(byteData))
                {

                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response = await client.PostAsync(url, content);


                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();

                        EvaluationModel responseModel = JsonConvert.DeserializeObject<EvaluationModel>(responseString);

                        double max = responseModel.Predictions.Max(m => m.Probability);

                        TagLabel.Text = (max >= 0.5) ? "sushi roll" : "not sushi roll";

                    } else
                    {
                        TagLabel.Text = "Not working";
                    }

                    //Get rid of file once we have finished using it
                    file.Dispose();
                }
            }

        async Task postLocationAsync()
        {

            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;

            var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));

            NotSushiRollModel model = new NotSushiRollModel()
            {
                Longitude = (float)position.Longitude,
                Latitude = (float)position.Latitude            
            };

            await AzureManager.AzureManagerInstance.PostSushiRollInformation(model);
        }
    }
}
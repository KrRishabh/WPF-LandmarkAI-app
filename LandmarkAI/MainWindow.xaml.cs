using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LandmarkAI.Classes;
using Newtonsoft.Json;

namespace LandmarkAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (.jpg, .png)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*";


            if (dialog.ShowDialog()==true)
            {
                string fileName = dialog.FileName;
                selectedImage.Source = new BitmapImage(new Uri(fileName));
                MakePredictionAsync(fileName);
            }
        }

        private async void MakePredictionAsync(string fileName)
        {
            string url = "https://centralindia.api.cognitive.microsoft.com/customvision/v3.0/Prediction/1fabecdd-f0c3-4b9f-8a21-3e7a31319c0f/classify/iterations/LandmarkIteration1/image";
            string predictionKey = "6ae346cd8b2d4a7da87e8731d8e7c655";
            string contentType = "application/octet-stream";
            var file = File.ReadAllBytes(fileName);
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Prediction-Key", predictionKey);
                using(var content = new ByteArrayContent(file))
                {
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                    var response = await client.PostAsync(url, content);
                    string responseString = await response.Content.ReadAsStringAsync();
                    List<Prediction> predictions = (JsonConvert.DeserializeObject<CustomVision>(responseString)).Predictions;
                    ListViewPredictions.ItemsSource = predictions;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebAPIClient
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        //private static readonly string URL = "http://localhost:61606/";
        private static string URL = "https://apiinfominusmatic20200819111917.azurewebsites.net/";
        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
        }

        private static async Task RunAsync()
        {
            var billete2 =  InfoBilletePost();
            var billete1 = await InfoBilleteGet();        
        }

        private static async Task<Billete> InfoBilleteGet()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            var streamTask = client.GetStreamAsync(URL + @"api/Minusmatic?bille_image=C:\Users\pmoya\Desktop\Imagenes\billete100usdolar.jpg");

            var billete = await JsonSerializer.DeserializeAsync<Billete>(await streamTask);

            return billete;
        }

        private static Billete InfoBilletePost()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            string path = @"C:\Users\pmoya\Desktop\Imagenes\billete100usdolar.jpg";
            var binaryData = File.ReadAllBytes(path);
            string base64String = Convert.ToBase64String(binaryData);

           // DataFileFormat dataFile = new DataFileFormat { formatExt = "png", contentEncode64 = base64String };

            String jsonParams = @"{
                                   'formatExt':'jpg',
                                   'contentEncode64':'"+ base64String + @"'
                                  }";

            var content = new StringContent(jsonParams, Encoding.UTF8, "application/json");

            var response =  client.PostAsync(URL + "api/Minusmatic", content).Result;

            string streamTask= response.Content.ReadAsStringAsync().Result;

            var billete =  JsonSerializer.Deserialize<Billete>(streamTask);

            return billete;
        }
    }

    #region Clases Modelos Requeridos

    public class Billete
    {
        public string[] datos { get; set; }
    }

    public class DataFileFormat
    {
        public string formatExt { get; set; }
        public string contentEncode64 { get; set; }
    }  

    #endregion
}

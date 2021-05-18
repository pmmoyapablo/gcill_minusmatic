using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        private static readonly HttpClient client = new HttpClient();
        private string URL = "http://localhost:61606/";
       //private string URL = "https://apiinfominusmatic20200819111917.azurewebsites.net/";
        [TestMethod]
        public async Task TestMethodGet()
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

                var streamTask = client.GetStreamAsync(URL + @"api/Minusmatic?bille_image=D:\home\TempImagen\billete50milcop.jpg");

                var contributor = await JsonSerializer.DeserializeAsync<Billete>(await streamTask);

                Assert.IsTrue(contributor != null);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

        [TestMethod]
        public void TestMethodPost()
        {
            try {

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

                string path = @"C:\Users\pmoya\Desktop\billete50milcop.jpg";
                var binaryData = File.ReadAllBytes(path);
                string base64String = Convert.ToBase64String(binaryData);

                // DataFileFormat dataFile = new DataFileFormat { formatExt = "png", contentEncode64 = base64String };
                // String jsonParams = JsonConvert.SerializeObject(dataFile);
                String jsonParams = @"{
                                   'formatExt':'jpg',
                                   'contentEncode64':'" + base64String + @"'
                                  }";

                var content = new StringContent(jsonParams, Encoding.UTF8, "application/json");

                var response = client.PostAsync( URL + "api/Minusmatic", content).Result;

                string streamTask = response.Content.ReadAsStringAsync().Result;

                var contributor = JsonSerializer.Deserialize<Billete>(streamTask);

                Assert.IsTrue(contributor != null);

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
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

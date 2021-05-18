using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using ClienteMinusmatic.Properties;
using System.Data;

namespace ClienteMinusmatic
{
    public partial class Form1 : Form
    {    
        private string Billete_image = null;
        private  string URL = ""; 

        public Form1()
        {
          this.URL = Settings.Default.URL;
          InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                this.openFileDialog1.Filter = "Archivo de Imagen JPG|*jpg|Archivo de Imagen PNG|*png|Archivo PDF|*pdf|Archivo de Imagen TIF|*tif|Archivo de Imagen GIF|*gif";   //,*jpeg,*jpe,*jfif,*tif,*tiff,*gif,*bmp
                this.openFileDialog1.Title = "Seleccione el archivo de RIF";

                if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    this.tbxDatos.Text = "";

                    Billete_image = this.openFileDialog1.FileName;
                    this.label1.Text = Billete_image;

                    this.LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Información del Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
      
        private void LoadData()
        {
            try
            {
                Billete billete = null;
                ClientHttpREST clientHttp = new ClientHttpREST(URL);
                var binaryData = File.ReadAllBytes(Billete_image);
                FileInfo fileInfo = new FileInfo(Billete_image);
                string ext = fileInfo.Extension.Replace(".", "");
                string base64String = Convert.ToBase64String(binaryData);

                DataFileFormat dataFile = new DataFileFormat { formatExt = ext, contentEncode64 = base64String };
                String jsonParams = JsonConvert.SerializeObject(dataFile);
                string jsonOut = "";

                String response = clientHttp.PostObjetc("api/Minusmatic", jsonParams, out jsonOut);

                if (response == "OK")
                {
                    billete = TypeModel<Billete>.DeserializeInObject(jsonOut);
                }
                else
                {
                    MessageBox.Show("No se pudo procesar el requerimiento.", "Información del Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (billete.datos != null)
                {
                   foreach (string data in billete.datos)
                   {
                      this.tbxDatos.Text += data + "\r\n";
                   }             
                }
                else
                {
                    MessageBox.Show("No se obtuvieron datos.", "Información del Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Información del Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

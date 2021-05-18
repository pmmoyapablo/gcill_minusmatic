using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Tesseract;
using ApiInfoMinusmatic.Models;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Drawing.Imaging;
using GhostscriptSharp;
using System.Threading.Tasks;

namespace ApiInfoMinusmatic.Controllers
{
    public class MinusmaticController : ApiController
    {
        // GET: api/Minusmatic
        public IEnumerable<string> Get()
        {
            return new string[] { "Api", "Info Billete" };
        }

        // GET: api/Minusmatic?bille_image=C:\Users\pmoya\Desktop\billete100usdolar.jpg
        [ResponseType(typeof(Billete))]
        public IHttpActionResult Get(string bille_image)
        {
            try
            {
                Billete billete = null;
               
                if (bille_image == null || bille_image == "")
                {
                      return BadRequest();
                }

                FileInfo fileInfo = new FileInfo(bille_image);

                if (fileInfo.Extension == ".pdf")
                {  
                    bille_image = this.ConverFormatToImagen(bille_image);
                }

                string dataOut = this.GetTextOfImagen(bille_image);

               
                if (dataOut != null && dataOut != "")
                {
                    billete = this.GetInfoBillete(dataOut);                                             
                }

                return Ok(billete);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/Billete
        [ResponseType(typeof(Billete))]
        public IHttpActionResult Post(DataFileFormat dataFile)
        {
            try
            {
                //string pathFolder = @"D:\home\TempImagen";
                 string pathFolder = @"C:\TempImagen";

                // Determine whether the directory Not Exists.
                if (!Directory.Exists(pathFolder))
                {
                    // Try to create the directory.
                    Directory.CreateDirectory(pathFolder);
                }
             
                if (dataFile == null)
                {
                    return BadRequest();

                }else if (dataFile.contentEncode64 == null && dataFile.formatExt == null)
                {
                    return BadRequest("Formato y contenido indefinidos");
                }

                byte[] byteVector = System.Convert.FromBase64String(dataFile.contentEncode64);

                string cacheId = this.GenerarStringAletory(10);

                string bille_image = pathFolder + @"\tempFile_"+ cacheId +"." + dataFile.formatExt;
                string rif_pdf = null;
                File.WriteAllBytes(bille_image, byteVector);
                
                FileInfo fileInfo = new FileInfo(bille_image);

                if (fileInfo.Extension == ".pdf")
                {
                   rif_pdf = bille_image;
                   bille_image = this.ConverFormatToImagen(bille_image);
                   File.Delete(rif_pdf);
                }

                string dataOut = this.GetTextOfImagen(bille_image);

                Billete billete = null;

                if (dataOut != null && dataOut != "")
                {
                    billete = this.GetInfoBillete(dataOut);
                }

                File.Delete(bille_image);

                return Ok(billete);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #region Métodos Privados

        /// <summary>
        /// Establece la Información de un Contribuyente.
        /// </summary>
        /// <param name="pDataText">Texto con los datos</param>
        /// <returns>Devuelve un Objeto (Billete)</returns>
        private Billete GetInfoBillete(string pDataText)
        {
            string[] lines = pDataText.Split('\n');

            Billete billete = new Billete();
            List<String> datos = new List<String>();

            foreach (string line in lines)
            {
                if (line != "")
                { datos.Add(line); }          
            }

            billete.datos = datos.ToArray();

            return billete;
        }

        /// <summary>
        /// Lee una Imagen y la convierte en Texto Plano.
        /// </summary>
        /// <param name="bille_image">Path to file Imagen for convertetion</param>
        /// <returns>Devuelve el Texto contenido de la Imgaen (string)</returns>
        private string GetTextOfImagen(string bille_image)
        {   
            string textEnd = null;
            System.Web.UI.Page pgTemp = new System.Web.UI.Page();
            string tessdataDir = pgTemp.Server.MapPath(@"~/tessdata");

            using (var engine = new TesseractEngine(tessdataDir, "eng", EngineMode.Default))
            {
                // have to load Pix via a bitmap since Pix doesn't support loading a stream.
                using (var image = new System.Drawing.Bitmap(bille_image))
                {
                    using (var pix = PixConverter.ToPix(image))
                    {
                        using (var page = engine.Process(pix))
                        {
                            string meanConfidence = String.Format("{0:P}", page.GetMeanConfidence());
                            textEnd = page.GetText();
                        }
                    }
                }
            }

            return textEnd;
        }

        /// <summary>
        /// Convertetion PDF to image.
        /// </summary>
        /// <param name="pFilePDF">Path to file PDF for convertetion</param>
        /// <returns>Devuelve el Path de el archivo de Imgaen (string)</returns>
        private string ConverFormatToImagen(string pFilePDF)
        {
            FileInfo fileInfo = new FileInfo(pFilePDF);
            string nameOnly = fileInfo.Name.Replace(fileInfo.Extension, "");
            string pathOut = fileInfo.DirectoryName + "\\" + nameOnly + "Out.jpeg";

            this.CreatPDF(pFilePDF, pathOut, GhostscriptSharp.Settings.GhostscriptDevices.jpeg, GhostscriptSharp.Settings.GhostscriptPageSizes.letter, 200, 200);

            return pathOut;
        }

        /// <summary>
        /// Convertetion PDF to image.
        /// </summary>
        /// <param name="Path">Path to file for convertetion</param>
        /// <param name="PDFfile">Book name on HDD</param>
        /// <param name="Devise">Select one of the formats, jpg</param>
        /// <param name="PageFormat">Select one of page formats, like A4</param>
        /// <param name="qualityX"> Select quality, 200X200 ~ 1200X1900</param>
        /// <param name="qualityY">Select quality, 200X200 ~ 1200X1900</param>
        private void CreatPDF(string Path, string OutPath, GhostscriptSharp.Settings.GhostscriptDevices Devise, GhostscriptSharp.Settings.GhostscriptPageSizes PageFormat, int qualityX, int qualityY)
        {
            try
            {
                GhostscriptSharp.GhostscriptSettings SettingsForConvert = new GhostscriptSharp.GhostscriptSettings();

                SettingsForConvert.Device = Devise;

                GhostscriptSharp.Settings.GhostscriptPageSize pageSize = new GhostscriptSharp.Settings.GhostscriptPageSize();
                pageSize.Native = PageFormat;
                SettingsForConvert.Size = pageSize;

                SettingsForConvert.Resolution = new System.Drawing.Size(qualityX, qualityY);

                GhostscriptSharp.GhostscriptWrapper.GenerateOutput(Path, OutPath, SettingsForConvert); // here you could set path and name for out put file.
            }catch(Exception ex)
            {
                throw ex;
            }
        }

        private string GenerarStringAletory(int longitud)
        {
            string password = string.Empty;
            string[] letras = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "ñ", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
                                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};
            Random EleccionAleatoria = new Random();

            for (int i = 0; i < longitud; i++)
            {
                int LetraAleatoria = EleccionAleatoria.Next(0, 100);
                int NumeroAleatorio = EleccionAleatoria.Next(0, 9);

                if (LetraAleatoria < letras.Length)
                {
                    password += letras[LetraAleatoria];
                }
                else
                {
                    password += NumeroAleatorio.ToString();
                }
            }

            if (!password.Any(c => char.IsUpper(c)))
            {
                string letterUppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                char letter = letterUppers[EleccionAleatoria.Next(0, letterUppers.Length - 1)];
                password += letter.ToString();
            }

            return password;
        }

        #endregion
    }
}

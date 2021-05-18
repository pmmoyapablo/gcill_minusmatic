using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace ClienteMinusmatic
{
    class ClientHttpREST
    {
        private string URL = null;
        private string DATA = null;
        private string Token = null;

        public ClientHttpREST(string pUri)
        {
            this.URL = pUri;
        }
        public ClientHttpREST(string pUri, string pToken)
        {
            this.URL = pUri;
            this.Token = pToken;
        }

        public String GetToken(string pPath, string pData)
        {
            try
            {
                DATA = pData;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL + pPath);
                request.ContentType = "application/json";
                request.Method = "POST";
                request.ContentLength = DATA.Length;
                StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                requestWriter.Write(DATA);
                requestWriter.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    reader.Close();

                    return json;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public String GetObjetcsAll(string pPath)
        {

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL + pPath);
                request.Headers.Add("Authorization", Token);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    reader.Close();

                    return json;
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public String GetObjetc(string pPath, string pId)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL + pPath + "/" + pId);
                request.Headers.Add("Authorization", Token);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    reader.Close();

                    return json;
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public String PostObjetc(string pPath, string pData)
        {
            try
            {
                DATA = pData;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL + pPath);
                request.Headers.Add("Authorization", Token);
                request.ContentType = "application/json";
                request.Method = "POST";
                request.ContentLength = DATA.Length;
                StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                requestWriter.Write(DATA);
                requestWriter.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())

                    return response.StatusCode.ToString();

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public String PostObjetc(string pPath, string pData, out string pObjet)
        {
            try
            { 
                DATA = pData;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL + pPath);
                request.Headers.Add("Authorization", Token);
                request.ContentType = "application/json";
                request.Method = "POST";
                request.ContentLength = DATA.Length;
                StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                requestWriter.Write(DATA);
                requestWriter.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    pObjet = reader.ReadToEnd();
                    reader.Close();

                    return response.StatusCode.ToString();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public String PutObjetc(string pPath, string pId, string pData)
        {
            try
            {
                DATA = pData;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL + pPath + "/" + pId);
                request.Headers.Add("Authorization", Token);
                request.ContentType = "application/json";
                request.Method = "PUT";
                request.ContentLength = DATA.Length;
                StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                requestWriter.Write(DATA);
                requestWriter.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())

                    return response.StatusCode.ToString();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public String DeleteObjetc(string pPath, string pId)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL + pPath + "/" + pId);
                request.Headers.Add("Authorization", Token);
                request.Method = "DELETE";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())

                    return response.StatusCode.ToString();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}


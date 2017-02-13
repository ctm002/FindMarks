using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace TrackMarks.Web
{
    /// <summary>
    /// Summary description for WSConnectInapi
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WSMarks : WebService
    {

        [WebMethod]
        public void FindMarksByRange(string start, string end)
        {
            try
            {
                Random rnd = new Random();
                int tiempoDeConsulta = 5000;

                List<CabeceraSolicitud> solicitudesInapi = new List<CabeceraSolicitud>();
                var oJS = new JavaScriptSerializer();

                string id = "";
                string hash = "";
                string cookie = "";
                int min = 30000; //30 segundos
                int max = 50000; // 50 segundos

                int contadorFilas = 1;
                GetIDAndHash(ref id, ref hash, ref cookie);
                Debug.Print(id + ' ' + hash);

                int de = int.Parse(start);
                int hasta = int.Parse(end);

                for (int registro = de; registro <= hasta; registro++)
                {
                    string marcaJson = FindMarks(cookie, id, hash, registro.ToString());
                    dynamic objReturn = oJS.DeserializeObject(marcaJson);
                    string d = objReturn["d"];
                    if (d.Contains("ErrorMessage"))
                    {
                        Debug.Print("Nro de Registro:" + registro + ",d:" + d + ",id:" + id + ",hash:" + hash);
                        tiempoDeConsulta = rnd.Next(min, max);
                        Thread.Sleep(tiempoDeConsulta);
                        GetIDAndHash(ref id, ref hash, ref cookie);

                        marcaJson = FindMarks(cookie, id, hash, registro.ToString());
                        objReturn = oJS.DeserializeObject(marcaJson);
                        d = objReturn["d"];
                    }

                    CabeceraMarca objCabeceraMarca = oJS.Deserialize<CabeceraMarca>(objReturn["d"]);
                    if (objCabeceraMarca != null && objCabeceraMarca.Marcas.Count > 0)
                    {
                        DetalleMarca detalle = objCabeceraMarca.Marcas[0];
                        Debug.Print(contadorFilas + " " + string.Join(" ", detalle.cell));

                        string returnHash = objCabeceraMarca.Hash;
                        string[] datos = objCabeceraMarca.Marcas[0].cell.ToArray();
                        string nroSolicitud = datos[0];
                        string nroRegistro = datos[1];
                        string clase = datos[2];
                        string signo = datos[3];
                        string titular = datos[4];

                        string marcaDetalleJson = FindSolicitud(cookie, id, hash, nroSolicitud);
                        objReturn = oJS.DeserializeObject(marcaDetalleJson);
                        d = objReturn["d"];
                        while (d.Contains("ErrorMessage"))
                        {
                            Debug.Print("Nro de Solicitud:" + nroSolicitud + ",d:" + d + ",id:" + id + ",hash:" + hash);
                            tiempoDeConsulta = rnd.Next(min, max);
                            Thread.Sleep(tiempoDeConsulta);

                            //Generando nuevos hash e id 
                            GetIDAndHash(ref id, ref hash, ref cookie);
                            marcaDetalleJson = FindSolicitud(cookie, id, hash, nroSolicitud);
                            objReturn = oJS.DeserializeObject(marcaDetalleJson);
                            d = objReturn["d"];
                        }

                        CabeceraSolicitud objCabeceraSolicitud = oJS.Deserialize<CabeceraSolicitud>(objReturn["d"]);
                        string solicitudJSON = oJS.Serialize(objCabeceraSolicitud);
                        Debug.Print(solicitudJSON);

                        objCabeceraSolicitud.Marca.NumeroSolicitud = nroSolicitud;
                        solicitudesInapi.Add(objCabeceraSolicitud);
                        hash = objCabeceraSolicitud.Hash;

                    }
                    contadorFilas += 1;
                    tiempoDeConsulta = rnd.Next(min, max);
                    Thread.Sleep(tiempoDeConsulta);
                }

                string solicitudesJSON = oJS.Serialize(solicitudesInapi);
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.Flush();
                Context.Response.Write(solicitudesJSON);
            }
            catch (Exception ex)
            {
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.AddHeader("content-length", ex.Message.Length.ToString());
                Context.Response.Flush();
                Context.Response.Write(ex.Message);
            }
        }

        private void GetIDAndHash(ref string pID, ref string pHash, ref string cookie)
        {
            string URL = "http://200.55.216.86:8080/Marca/BuscarMarca.aspx";
            HttpWebRequest theWebRequest = (HttpWebRequest)WebRequest.Create(URL);
            theWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";
            theWebRequest.Method = "GET";
            theWebRequest.Referer = URL;
            theWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            theWebRequest.CookieContainer = new CookieContainer();
            HttpWebResponse theWebResponse = (HttpWebResponse)theWebRequest.GetResponse();
            cookie = "ASP.NET_SessionId=" + theWebResponse.Cookies["ASP.NET_SessionId"].Value;

            Stream dataStream = theWebResponse.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string contentHtml = reader.ReadToEnd();
            string pattern = @"^\s*setHash\s*\(\s*['""]([0-9a-f]*)['""]\s*,\s*['""]([0-9]*)['""]\s*\)\s*;";
            Regex regExp = new Regex(pattern);
            Match m = Regex.Match(contentHtml, pattern, RegexOptions.Multiline);
            if (m.Success)
            {
                pHash = m.Groups[1].Value;
                pID = m.Groups[2].Value;
            }
        }

        private string FindMarks(string pCookie, string pIDW, string pHash, string pNroRegistro)
        {
            string URL = "http://200.55.216.86:8080/Marca/BuscarMarca.aspx/FindMarcas";
            HttpWebRequest theWebRequest = (HttpWebRequest)WebRequest.Create(URL);
            theWebRequest.Method = "POST";
            theWebRequest.ContentType = "application/json; charset=UTF-8";
            theWebRequest.Headers["Cookie"] = pCookie;
            theWebRequest.Referer = "http://200.55.216.86:8080/Marca/BuscarMarca.aspx";
            using (var writer = theWebRequest.GetRequestStream())
            {
                ConsultaMarca objParametros = new ConsultaMarca();
                objParametros.IDW = pIDW;
                objParametros.Hash = pHash;
                objParametros.LastNumSol = 0;
                objParametros.param2 = pNroRegistro;

                JavaScriptSerializer oJS = new JavaScriptSerializer();
                string formData = oJS.Serialize(objParametros);
                var data = Encoding.UTF8.GetBytes(formData);
                writer.Write(data, 0, data.Length);
            }

            HttpWebResponse theWebResponse = (HttpWebResponse)theWebRequest.GetResponse();
            Stream dataStream = theWebResponse.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string returnValue = reader.ReadToEnd();
            return returnValue;
        }

        private string FindSolicitud(string pCookie, string pIDW, string pHash, string pNroSolicitud)
        {
            string URL = "http://200.55.216.86:8080/Marca/BuscarMarca.aspx/FindMarcaByNumeroSolicitud";
            HttpWebRequest theWebRequest = (HttpWebRequest)WebRequest.Create(URL);
            theWebRequest.Method = "POST";
            theWebRequest.ContentType = "application/json; charset=UTF-8";
            theWebRequest.Headers["Cookie"] = pCookie;
            theWebRequest.Referer = "http://200.55.216.86:8080/Marca/BuscarMarca.aspx";
            using (var writer = theWebRequest.GetRequestStream())
            {
                ConsultaSolicitud objParametros = new ConsultaSolicitud();
                objParametros.IDW = pIDW;
                objParametros.Hash = pHash;
                objParametros.numeroSolicitud = pNroSolicitud;

                JavaScriptSerializer oJS = new JavaScriptSerializer();
                string formData = oJS.Serialize(objParametros);
                var data = Encoding.UTF8.GetBytes(formData);
                writer.Write(data, 0, data.Length);
            }

            HttpWebResponse theWebResponse = (HttpWebResponse)theWebRequest.GetResponse();
            Stream dataStream = theWebResponse.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            return reader.ReadToEnd();
        }
    }
}



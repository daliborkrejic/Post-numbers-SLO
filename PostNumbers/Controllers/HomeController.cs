using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;

namespace PostNumbers.Controllers
{
    public class DataObject
    {
        public string kraj { get; set; }
        public string postSt { get; set; }
    }

    public class HomeController : Controller
    {
        private const string URL = "http://sandbox.lavbic.net/OIS/api/kraji";
        private static string urlParameters = "?api_key=123";

        static string fileName = "export_csv.txt";
        static string OUTPUT_FILE = @"D:\1 Dokumenti\Visual Studio 2013\Projects\PostNumbers\" + fileName;
        
        public ActionResult Index()
        {
            ViewBag.Title = "Poštne številke in kraji";
            string tabela = "";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var dataObjects = response.Content.ReadAsAsync<IEnumerable<DataObject>>().Result;
                foreach (var d in dataObjects)
                {
                    // PHP
                    //string temp = d.postSt + " => " + "\"" + d.kraj + "\"" + ", ";

                    // CSV
                    string temp = d.postSt + "\t" + d.kraj + "\n";
                    tabela = tabela + temp;
                }
                

                printInFile(tabela, OUTPUT_FILE);
                ViewBag.Tabela = tabela;
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return View();
        }

        private static void printInFile(string output, string fileNameLoc, bool printConsole = true)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileNameLoc, true))
                {
                    if (printConsole)
                    {
                        Console.WriteLine(output);
                    }
                    file.WriteLine(output);
                }
            }
            catch (ObjectDisposedException)
            {
                string error = "Pisanje v datoteko exc: ObjectDisposedException.";
                Console.WriteLine(error);
            }
            catch (IndexOutOfRangeException)
            {
                string error = "Pisanje v datoteko exc: IndexOutOfRangeException.";
                Console.WriteLine(error);
            }
        }
    }

}

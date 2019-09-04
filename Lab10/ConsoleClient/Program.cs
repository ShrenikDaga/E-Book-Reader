using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ConsoleClient
{
    class Program
    {
        public HttpClient client { get; set; }

        private string baseUrl_;

        Program(string url)
        {
            baseUrl_ = url;
            client = new HttpClient();
        }
        //----< upload file >--------------------------------------

        public async Task<HttpResponseMessage> SendFile(string fileSpec, string imagePath,string categoryid,string numPages)
        {
            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            byte[] data = File.ReadAllBytes(fileSpec);
            ByteArrayContent bytes = new ByteArrayContent(data);

            byte[] image = File.ReadAllBytes(imagePath);
            ByteArrayContent imageContent = new ByteArrayContent(image);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
            
            byte[] pages = Encoding.ASCII.GetBytes(numPages);
            ByteArrayContent author = new ByteArrayContent(pages);

            byte[] category = Encoding.ASCII.GetBytes(categoryid);
            ByteArrayContent catid = new ByteArrayContent(category);


            string fileName = Path.GetFileName(fileSpec);
            string imageName = Path.GetFileName(imagePath);

            multiContent.Add(bytes, "files", fileName);
            multiContent.Add(author,"Pages");
            multiContent.Add(catid, "CategoryID");
            multiContent.Add(imageContent, "image", imageName);


            return await client.PostAsync(baseUrl_, multiContent);
        }

        /// <summary>
        /// ////////////////////////////////////////////////////////
        /// ////////////////////////////////////////////////////////

        public async Task<HttpResponseMessage> ReplaceFile(string fileSpec, string imagePath, string bookidd)
        {
            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            byte[] data = File.ReadAllBytes(fileSpec);
            ByteArrayContent bytes = new ByteArrayContent(data);

            byte[] image = File.ReadAllBytes(imagePath);
            ByteArrayContent imageContent = new ByteArrayContent(image);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            byte[] pages = Encoding.ASCII.GetBytes("Replace");
            ByteArrayContent author = new ByteArrayContent(pages);

            byte[] category = Encoding.ASCII.GetBytes(bookidd);
            ByteArrayContent bookid = new ByteArrayContent(category);


            string fileName = Path.GetFileName(fileSpec);
            string imageName = Path.GetFileName(imagePath);

            multiContent.Add(bytes, "files", fileName);
            multiContent.Add(author, "Type");
            multiContent.Add(bookid, "BookID");
            multiContent.Add(imageContent, "image", imageName);


            return await client.PostAsync(baseUrl_, multiContent);
        }

        /// </summary>
        /// <returns></returns>
        //----< get list of files in server FileStorage >----------

        //public async Task<IEnumerable<string>> GetFileList()
        //{
        //    HttpResponseMessage resp = await client.GetAsync(baseUrl_);
        //    var files = new List<string>();
        //    if (resp.IsSuccessStatusCode)
        //    {
        //        var json = await resp.Content.ReadAsStringAsync();
        //        JArray jArr = (JArray)JsonConvert.DeserializeObject(json);
        //        foreach (var item in jArr)
        //            files.Add(item.ToString());
        //    }
        //    return files;
        //}

        public async Task<IEnumerable<string>> GetFileList()
        {
            HttpResponseMessage resp = await client.GetAsync(baseUrl_);
            var files = new List<string>();
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                JArray jArr = (JArray)JsonConvert.DeserializeObject(json);
                foreach (var item in jArr)
                    files.Add(item.ToString());
            }
            return files;
        }

        //---------------------------------------------------------
        public async Task<IEnumerable<string>> GetBookList()
        {
            HttpResponseMessage resp = await client.GetAsync(baseUrl_+"/1/1");
            var files = new List<string>();
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                JArray jArr = (JArray)JsonConvert.DeserializeObject(json);
                foreach (var item in jArr)
                    files.Add(item.ToString());
            }
            return files;
        }

        //----< download the id-th file >--------------------------

        public async Task<HttpResponseMessage> GetFile(int id)
        {
            return await client.GetAsync(baseUrl_ + "/" + id.ToString());
        }
        //----< delete the id-th file from FileStorage >-----------

        public async Task<HttpResponseMessage> DeleteFile(int id)
        {
            return await client.DeleteAsync(baseUrl_ + "/" + id.ToString());
        }
        //----< usage message shown if command line invalid >------

        static void showUsage()
        {
            Console.Write("\n  Command line syntax error: expected usage:\n");
            Console.Write("\n    http[s]://machine:port /option [filespec]\n\n");
        }
        //----< validate the command line >------------------------

        static bool parseCommandLine(string[] args)
        {
            if (args.Length < 2)
            {
                showUsage();
                return false;
            }
            if (args[0].Substring(0, 4) != "http")
            {
                showUsage();
                return false;
            }
            if (args[1][0] != '/')
            {
                showUsage();
                return false;
            }
            return true;
        }
        //----< display command line arguments >-------------------

        static void showCommandLine(string[] args)
        {
            string arg0 = args[0];
            string arg1 = args[1];
            string arg2;
            if (args.Length == 3)
                arg2 = args[2];
            else
                arg2 = "";
            Console.Write("\n  CommandLine: {0} {1} {2}", arg0, arg1, arg2);
        }

        static string checkInput(string input)
        {
            string argsVal = "";
            if (input.Equals("1"))
            {
                argsVal = "/up";
            }
            if (input.Equals("2"))
            {
                argsVal = "/fl";
            }
            if (input.Equals("3"))
            {
                argsVal = "Quit";
            }
            return argsVal;
        }

        static void Main(string[] args)
        {
            string input="";
            while (input != "3")
            {
                Console.Write("\n Console Client");
                Console.Write("\n =====================================================\n");
                Console.Write("\n  Choose an option from the below list to Proceed");
                Console.Write("\n     1> Upload a book");
                Console.Write("\n     2> Replace a book");
                Console.Write("\n     3> Exit");
                Console.Write("\n =====================================================\n");

                
                input = Console.ReadLine();
                string newArgs = checkInput(input);
                Console.WriteLine(input);

                string url = "https://localhost:44333/api/Files";
                Program client = new Program(url);

                //Console.Write("\n  sending request to {0}\n", url);

                switch (newArgs)
                {
                    case "/fl":
                        Task<IEnumerable<string>> tfl = client.GetBookList();
                        var resultfl = tfl.Result;
                        Console.Write("List of available books:");
                        foreach (var item in resultfl)
                        {
                            Console.Write("\n  {0}", item);
                        }
                        Console.Write("\n Enter the Book number for which you want to replace these files: ");
                        string book = Console.ReadLine();
                        Console.Write("\n  Enter the name of the file: ");
                        string rfilename = Console.ReadLine();
                        Console.Write("\n  Enter the name of the image: ");
                        string rimageName = Console.ReadLine();
                        bool correctbook = false;

                        foreach (var item in resultfl)
                        {
                            if (item.Contains(book))
                                correctbook = true;
                        }


                        try
                        {
                            if (correctbook)
                            {
                                Task<HttpResponseMessage> trpl = client.ReplaceFile(rfilename, rimageName, book);
                                Console.Write(trpl.Result);
                            }
                            else
                            {
                                Console.Write("Invalid Category entered.");
                            }

                        }
                        catch (Exception)
                        {
                            Console.Write("Something went wrong. Please check if all the file names were correctly entered.");
                        }



                        break;
                    case "/up":

                        Task<IEnumerable<string>> CatList = client.GetFileList();
                        var resultcat = CatList.Result;
                        Console.Write("List of available categories:");
                        foreach (var item in resultcat)
                        {
                            Console.Write("\n  {0}", item);
                        }
                        Console.Write("\n Enter the Category number in which you want to add this book: ");
                        string category = Console.ReadLine();
                        Console.Write("\n  Enter the name of the file: ");
                        string filename = Console.ReadLine();
                        Console.Write("\n  Enter the number of pages: ");
                        string pages = Console.ReadLine();
                        Console.Write("\n  Enter the name of the image: ");
                        string imageName = Console.ReadLine();
                        bool correctcategory = false;

                        foreach (var item in resultcat)
                        {
                            if (item.Contains(category))
                                correctcategory = true;
                        }


                        try
                        {
                            if (correctcategory)
                            {
                                Task<HttpResponseMessage> tup = client.SendFile(filename, imageName, category, pages);
                                Console.Write(tup.Result);
                            }
                            else
                            {
                                Console.Write("Invalid Category entered.");
                            }

                        }
                        catch (Exception)
                        {
                            Console.Write("Something went wrong. Please check if all the file names were correctly entered.");
                        }

                        break;
                    case "/dn":
                        int id = Int32.Parse(args[2]);
                        Task<HttpResponseMessage> tdn = client.GetFile(id);
                        Console.Write(tdn.Result);
                        break;
                    case "Quit":
                        // ToDo
                        break;
                }
            }

            

            Console.WriteLine("\n Thank you. Press Key to exit: ");
            Console.ReadKey();
        }
    }
}

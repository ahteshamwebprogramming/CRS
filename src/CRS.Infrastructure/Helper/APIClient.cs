using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace CRS.Infrastructure.Helper;

public class APIClient
{
    public static string UploadFilesToRemoteUrl(string url, NameValueCollection simpleParams, NameValueCollection fileParams)
    {
        // Strings to be used as boundries for the multipart request
        string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
        byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n" + "--" + boundary + "\r\n");
        byte[] lastBoundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n" + "--" + boundary + "--");

        // Form field multipart header template
        string formdataTemplate = "\r\n" + "--" + boundary + "\r\n" + "Content-Disposition: form-data; name=\"{0}\";" + "\r\n" + "\r\n" + "{1}";

        // File field multipart header template
        string headerTemplate = "Content-Disposition: form-data; name=\"{0}\";filename=\"{1}\"" + "\r\n" + "Content-Type: application/octet-stream" + "\r\n" + "\r\n";

        // Memory stream to buffer the data to be sent over http connection
        Stream memStream = new System.IO.MemoryStream();

        // Create the multipart web request to be sent
        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

        // Set the various headers for the http request
        httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;
        httpWebRequest.Method = "POST";
        httpWebRequest.KeepAlive = true;
        httpWebRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;

        // Fix for the issue in .Net library
        System.Net.ServicePointManager.Expect100Continue = false;

        // Append the non-file parameters to the memory stream
        foreach (string key in simpleParams.Keys)
        {
            string formitem = string.Format(formdataTemplate, key, simpleParams[key]);
            byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
            memStream.Write(formitembytes, 0, formitembytes.Length);
        }

        if (fileParams.Keys.Count > 0)
        {
            memStream.Write(boundarybytes, 0, boundarybytes.Length);

            // Append the file parameters
            int fileParamIndex = 0;
            foreach (string key in fileParams.Keys)
            {
                // Header for the File part of form data
                string header = string.Format(headerTemplate, key, fileParams[key]);
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);

                memStream.Write(headerbytes, 0, headerbytes.Length);

                // Dump the file content to the memory stream
                FileStream fileStream = new FileStream(fileParams[key], FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[1024];

                int bytesRead = 0;

                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStream.Write(buffer, 0, bytesRead);
                }

                fileStream.Close();

                // If there are more files to be sent, write the intermediate boundry
                if (fileParamIndex++ < (fileParams.Count - 1))
                {
                    memStream.Write(boundarybytes, 0, boundarybytes.Length);
                }
            }
        }

        // Write the last part of POST request
        memStream.Write(lastBoundarybytes, 0, lastBoundarybytes.Length);

        // Set the content length for the POST request
        httpWebRequest.ContentLength = memStream.Length;

        // Reposition the memory stream pointer to start reading from the begining
        memStream.Position = 0;

        // Get the memmory stream into a buffer to be sent over http
        byte[] tempBuffer = new byte[memStream.Length];
        memStream.Read(tempBuffer, 0, tempBuffer.Length);
        memStream.Close();

        // Send the data to server
        Stream requestStream = httpWebRequest.GetRequestStream();
        requestStream.Write(tempBuffer, 0, tempBuffer.Length);
        requestStream.Close();

        // Get the response from Server
        WebResponse webResponse2 = httpWebRequest.GetResponse();
        Stream stream2 = webResponse2.GetResponseStream();
        StreamReader reader2 = new StreamReader(stream2);
        string str = reader2.ReadToEnd();
        webResponse2.Close();

        httpWebRequest = null;
        webResponse2 = null;

        return str;
    }
}


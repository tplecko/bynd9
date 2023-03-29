var url = "http://192.168.2.142:23432/updater/post";

var httpRequest = (HttpWebRequest)WebRequest.Create(url);
httpRequest.Method = "POST";

httpRequest.Headers["API-key"] = "AAAAAAAAAAAAAAA";
httpRequest.ContentType = "application/json";

var data = @"{
  ""IP"": ""10.0.0.25"",
  ""DeviceID"": ""Device1""
}";

using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
{
    streamWriter.Write(data);
}

var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
{
    var result = streamReader.ReadToEnd();
}

Console.WriteLine(httpResponse.StatusCode);
using System.Linq;

namespace GsaGHTests.Helper
{
  internal static class GsaFile
  {
    internal static string Steel_Design_Simple
    {
      get
      {
        if (_Steel_Design_Simple == "")
          _Steel_Design_Simple = DownloadFile("https://samples.oasys-software.com/gsa/10.1/Steel/Steel_Design_Simple.gwb");
        return _Steel_Design_Simple;
      } 
    }
    private static string _Steel_Design_Simple = "";

    private static string DownloadFile(string url)
    {
      string path = System.IO.Path.GetTempPath();
      string fileName = url.Split('/').Last();
      //create webclient and download example file:
      System.Net.WebClient webClient = new System.Net.WebClient();
      webClient.DownloadFile(url, path + fileName);
      return path + fileName;
    }
  }
}

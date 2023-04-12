using GsaAPI;

namespace GsaGH {
  /// <summary>
  /// Class to hold Titles used in Grasshopper GSA file.
  /// </summary>
  public static class Titles {
    public static string Calculation { get; set; }
    public static string Initials { get; set; }
    public static string JobNumber { get; set; }
    public static string Notes { get; set; }
    public static string SubTitle { get; set; }
    public static string Title { get; set; }

    private static bool s_calculationByuser;
    private static bool s_initialsByuser;
    private static bool s_jobnumberByuser;
    private static bool s_notesByuser;
    private static bool s_subtitleByuser;
    private static bool s_titleByuser;

    public static void GetTitlesFromGsa(Model model) {
      GsaAPI.Titles titles = model.Titles();
      if (!s_calculationByuser) {
        Titles.Calculation = titles.Calculation;
      }

      if (!s_initialsByuser) {
        Titles.Initials = titles.Initials;
      }

      if (!s_jobnumberByuser) {
        Titles.JobNumber = titles.JobNumber;
      }

      if (!s_notesByuser) {
        Titles.Notes = titles.Notes;
      }

      if (!s_subtitleByuser) {
        Titles.SubTitle = titles.SubTitle;
      }

      if (!s_titleByuser) {
        Titles.Title = titles.Title;
      }
    }

    public static void SetCalculation(string calculation) {
      Titles.Calculation = calculation;
      s_calculationByuser = true;
    }

    public static void SetInitials(string initials) {
      Titles.Initials = initials;
      s_initialsByuser = true;
    }

    public static void SetJobNumber(string jobnumber) {
      Titles.JobNumber = jobnumber;
      s_jobnumberByuser = true;
    }

    public static void SetNotes(string notes) {
      Titles.Notes = notes;
      s_notesByuser = true;
    }

    public static void SetSubTitle(string subtitle) {
      Titles.SubTitle = subtitle;
      s_subtitleByuser = true;
    }

    public static void SetTitle(string title) {
      Titles.Title = title;
      s_titleByuser = true;
    }
  }
}

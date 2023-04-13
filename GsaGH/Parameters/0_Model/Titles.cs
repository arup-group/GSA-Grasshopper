using GsaAPI;

namespace GsaGH {
  /// <summary>
  ///   Class to hold Titles used in Grasshopper GSA file.
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
        Calculation = titles.Calculation;
      }

      if (!s_initialsByuser) {
        Initials = titles.Initials;
      }

      if (!s_jobnumberByuser) {
        JobNumber = titles.JobNumber;
      }

      if (!s_notesByuser) {
        Notes = titles.Notes;
      }

      if (!s_subtitleByuser) {
        SubTitle = titles.SubTitle;
      }

      if (!s_titleByuser) {
        Title = titles.Title;
      }
    }

    public static void SetCalculation(string calculation) {
      Calculation = calculation;
      s_calculationByuser = true;
    }

    public static void SetInitials(string initials) {
      Initials = initials;
      s_initialsByuser = true;
    }

    public static void SetJobNumber(string jobnumber) {
      JobNumber = jobnumber;
      s_jobnumberByuser = true;
    }

    public static void SetNotes(string notes) {
      Notes = notes;
      s_notesByuser = true;
    }

    public static void SetSubTitle(string subtitle) {
      SubTitle = subtitle;
      s_subtitleByuser = true;
    }

    public static void SetTitle(string title) {
      Title = title;
      s_titleByuser = true;
    }
  }
}

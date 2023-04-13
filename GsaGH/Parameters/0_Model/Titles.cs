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

    private static bool calculationByuser;
    private static bool initialsByuser;
    private static bool jobnumberByuser;
    private static bool notesByuser;
    private static bool subtitleByuser;
    private static bool titleByuser;

    public static void GetTitlesFromGsa(Model model) {
      GsaAPI.Titles titles = model.Titles();
      if (!calculationByuser) {
        Calculation = titles.Calculation;
      }

      if (!initialsByuser) {
        Initials = titles.Initials;
      }

      if (!jobnumberByuser) {
        JobNumber = titles.JobNumber;
      }

      if (!notesByuser) {
        Notes = titles.Notes;
      }

      if (!subtitleByuser) {
        SubTitle = titles.SubTitle;
      }

      if (!titleByuser) {
        Title = titles.Title;
      }
    }

    public static void SetCalculation(string calculation) {
      Calculation = calculation;
      calculationByuser = true;
    }

    public static void SetInitials(string initials) {
      Initials = initials;
      initialsByuser = true;
    }

    public static void SetJobNumber(string jobnumber) {
      JobNumber = jobnumber;
      jobnumberByuser = true;
    }

    public static void SetNotes(string notes) {
      Notes = notes;
      notesByuser = true;
    }

    public static void SetSubTitle(string subtitle) {
      SubTitle = subtitle;
      subtitleByuser = true;
    }

    public static void SetTitle(string title) {
      Title = title;
      titleByuser = true;
    }
  }
}

using GsaAPI;

namespace GsaGH {

  /// <summary>
  /// Class to hold Titles used in Grasshopper GSA file.
  /// </summary>
  public static class Titles {

    #region Properties + Fields
    public static string Calculation {
      get { return s_calculation; }
      set { s_calculation = value; }
    }

    public static string Initials {
      get { return s_initials; }
      set { s_initials = value; }
    }

    public static string JobNumber {
      get { return s_jobnumber; }
      set { s_jobnumber = value; }
    }

    public static string Notes {
      get { return s_notes; }
      set { s_notes = value; }
    }

    public static string SubTitle {
      get { return s_subtitle; }
      set { s_subtitle = value; }
    }

    public static string Title {
      get { return s_title; }
      set { s_title = value; }
    }

    private static string s_calculation;
    private static bool s_calculationByuser;
    private static string s_initials;
    private static bool s_initialsByuser;
    private static string s_jobnumber;
    private static bool s_jobnumberByuser;
    private static string s_notes;
    private static bool s_notesByuser;
    private static string s_subtitle;
    private static bool s_subtitleByuser;
    private static string s_title;
    private static bool s_titleByuser;
    #endregion Properties + Fields

    #region Public Methods
    public static void GetTitlesFromGsa(Model model) {
      GsaAPI.Titles titles = model.Titles();
      if (!s_calculationByuser)
        Titles.Calculation = titles.Calculation;
      if (!s_initialsByuser)
        Titles.Initials = titles.Initials;
      if (!s_jobnumberByuser)
        Titles.JobNumber = titles.JobNumber;
      if (!s_notesByuser)
        Titles.Notes = titles.Notes;
      if (!s_subtitleByuser)
        Titles.SubTitle = titles.SubTitle;
      if (!s_titleByuser)
        Titles.Title = titles.Title;
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

    #endregion Public Methods
  }
}

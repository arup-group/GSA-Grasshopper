using GsaAPI;

namespace GsaGH
{
    /// <summary>
    /// Class to hold Titles used in Grasshopper GSA file. 
    /// </summary>
    public static class Titles
    {
        public static string Calculation
        {
            get { return m_calculation; }
            set { m_calculation = value; }
        }
        public static string Initials
        {
            get { return m_initials; }
            set { m_initials = value; }
        }
        public static string JobNumber
        {
            get { return m_jobnumber; }
            set { m_jobnumber = value; }
        }
        public static string Notes
        {
            get { return m_notes; }
            set { m_notes = value; }
        }
        public static string SubTitle
        {
            get { return m_subtitle; }
            set { m_subtitle = value; }
        }
        public static string Title
        {
            get { return m_title; }
            set { m_title = value; }
        }
        #region fields
        private static string m_calculation;
        private static bool m_calculation_byuser = false;
        private static string m_initials;
        private static bool m_initials_byuser = false;
        private static string m_jobnumber;
        private static bool m_jobnumber_byuser = false;
        private static string m_notes;
        private static bool m_notes_byuser = false;
        private static string m_subtitle;
        private static bool m_subtitle_byuser = false;
        private static string m_title;
        private static bool m_title_byuser = false;
        #endregion

        public static void SetTitlesInGSA(Model model)
        {
            GsaAPI.Titles titles = model.Titles();
            titles.Calculation = Titles.Calculation;
            titles.Initials = Titles.Initials;
            titles.JobNumber = Titles.JobNumber;
            titles.Notes = Titles.Notes;
            titles.SubTitle = Titles.SubTitle;
            titles.Title = Titles.Title;
            //model.Titles() = titles; GsaAPI needs to be updated to allow setting titles
        }
        public static void GetTitlesFromGSA(Model model)
        {
            GsaAPI.Titles titles = model.Titles();
            if (!m_calculation_byuser)
                Titles.Calculation = titles.Calculation;
            if (!m_initials_byuser)
                Titles.Initials = titles.Initials;
            if (!m_jobnumber_byuser)
                Titles.JobNumber = titles.JobNumber;
            if (!m_notes_byuser)
                Titles.Notes = titles.Notes;
            if (!m_subtitle_byuser)
                Titles.SubTitle = titles.SubTitle;
            if (!m_title_byuser)
                Titles.Title = titles.Title;
        }

        public static void SetCalculation(string calculation)
        {
            Titles.Calculation = calculation;
            m_calculation_byuser = true;
        }
        public static void SetInitials(string initials)
        {
            Titles.Initials = initials;
            m_initials_byuser = true;
        }
        public static void SetJobNumber(string jobnumber)
        {
            Titles.JobNumber = jobnumber;
            m_jobnumber_byuser = true;
        }
        public static void SetNotes(string notes)
        {
            Titles.Notes = notes;
            m_notes_byuser = true;
        }
        public static void SetSubTitle(string subtitle)
        {
            Titles.SubTitle = subtitle;
            m_subtitle_byuser = true;
        }
        public static void SetTitle(string title)
        {
            Titles.Title = title;
            m_title_byuser = true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GsaAPI;
using Rhino;

namespace GhSA.Util
{
    /// <summary>
    /// Class to hold Titles used in Grasshopper GSA file. 
    /// </summary>
    public static class GsaTitles
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
            titles.Calculation = GsaTitles.Calculation;
            titles.Initials = GsaTitles.Initials;
            titles.JobNumber = GsaTitles.JobNumber;
            titles.Notes = GsaTitles.Notes;
            titles.SubTitle = GsaTitles.SubTitle;
            titles.Title = GsaTitles.Title;
            //model.Titles() = titles; GsaAPI needs to be updated to allow setting titles
        }
        public static void GetTitlesFromGSA(Model model)
        {
            GsaAPI.Titles titles = model.Titles();
            if (!m_calculation_byuser)
                GsaTitles.Calculation = titles.Calculation;
            if (!m_initials_byuser)
                GsaTitles.Initials = titles.Initials;
            if (!m_jobnumber_byuser)
                GsaTitles.JobNumber = titles.JobNumber;
            if (!m_notes_byuser)
                GsaTitles.Notes = titles.Notes;
            if (!m_subtitle_byuser)
                GsaTitles.SubTitle = titles.SubTitle;
            if (!m_title_byuser)
                GsaTitles.Title = titles.Title;
        }

        public static void SetCalculation(string calculation)
        {
            GsaTitles.Calculation = calculation;
            m_calculation_byuser = true;
        }
        public static void SetInitials(string initials)
        {
            GsaTitles.Initials = initials;
            m_initials_byuser = true;
        }
        public static void SetJobNumber(string jobnumber)
        {
            GsaTitles.JobNumber = jobnumber;
            m_jobnumber_byuser = true;
        }
        public static void SetNotes(string notes)
        {
            GsaTitles.Notes = notes;
            m_notes_byuser = true;
        }
        public static void SetSubTitle(string subtitle)
        {
            GsaTitles.SubTitle = subtitle;
            m_subtitle_byuser = true;
        }
        public static void SetTitle(string title)
        {
            GsaTitles.Title = title;
            m_title_byuser = true;
        }
    }
}

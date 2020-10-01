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
        private static string m_initials;
        private static string m_jobnumber;
        private static string m_notes;
        private static string m_subtitle;
        private static string m_title;
        #endregion

        public static void SetTitles(Model model)
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
        public static void GetTitles(Model model)
        {
            GsaAPI.Titles titles = model.Titles();
            GsaTitles.Calculation = titles.Calculation;
            GsaTitles.Initials = titles.Initials;
            GsaTitles.JobNumber = titles.JobNumber;
            GsaTitles.Notes = titles.Notes;
            GsaTitles.SubTitle = titles.SubTitle;
            GsaTitles.Title = titles.Title;
            //model.Titles() = titles;
        }
    }
}

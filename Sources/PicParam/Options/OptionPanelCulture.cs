#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Collections.ObjectModel;
using System.IO;
using System.Globalization;
using System.Reflection;
using System.Resources;
#endregion

namespace PicParam
{
    public partial class OptionPanelCulture : GLib.Options.OptionsPanel
    {
        public OptionPanelCulture()
        {
            InitializeComponent();

            // -- fill cbLanguages
            int iSel = -1, i = -1;
            ReadOnlyCollection<CultureInfo> cultureInfos = GetAvailableCultures();
            foreach (CultureInfo ci in cultureInfos)
            {
                cbLanguages.Items.Add(new ComboCultureWrapper(ci));
                ++i;
                if (ci.DisplayName == CultureInfo.CurrentCulture.DisplayName)
                    iSel = i;
            }
            cbLanguages.SelectedIndex = iSel;
            // --
        }

        #region Helpers
        private static ReadOnlyCollection<CultureInfo> GetAvailableCultures()
        {
            List<CultureInfo> list = new List<CultureInfo>();

            string startupDir = Application.StartupPath;
            Assembly asm = Assembly.GetEntryAssembly();

            CultureInfo neutralCulture = CultureInfo.InvariantCulture;
            if (asm != null)
            {
                NeutralResourcesLanguageAttribute attr = Attribute.GetCustomAttribute(asm, typeof(NeutralResourcesLanguageAttribute)) as NeutralResourcesLanguageAttribute;
                if (attr != null)
                    neutralCulture = CultureInfo.GetCultureInfo(attr.CultureName);
            }
            // -- rather than neutral culture choose en-us
            neutralCulture = CultureInfo.GetCultureInfo("en-US");
            list.Add(neutralCulture);

            if (asm != null)
            {
                string baseName = asm.GetName().Name;
                foreach (string dir in Directory.GetDirectories(startupDir))
                {
                    // Check that the directory name is a valid culture
                    DirectoryInfo dirinfo = new DirectoryInfo(dir);
                    CultureInfo tCulture = null;
                    try
                    {
                        tCulture = CultureInfo.GetCultureInfo(dirinfo.Name);
                    }
                    // Not a valid culture : skip that directory
                    catch (ArgumentException)
                    {
                        continue;
                    }

                    // Check that the directory contains satellite assemblies
                    if (dirinfo.GetFiles(baseName + ".resources.dll").Length > 0)
                    {
                        list.Add(tCulture);
                    }

                }
            }
            return list.AsReadOnly();
        }
        #endregion

        #region Handlers
        private void onComboSelectionChanged(object sender, EventArgs e)
        {
            // culture
            ComboCultureWrapper cCultWrapper = cbLanguages.SelectedItem as ComboCultureWrapper;
            if (null != cCultWrapper)
                Properties.Settings.Default.CultureToUse = cCultWrapper.ToCultureInfoString();

            // tells the user that the application must restart
            this.ApplicationMustRestart = true;
        }
        #endregion

        #region ComboCultureWrapper
        internal class ComboCultureWrapper
        {
            public ComboCultureWrapper(CultureInfo ci)
            {
                _ci = ci;
            }
            public override string ToString()
            {
                return _ci.DisplayName;
            }
            public string ToCultureInfoString()
            {
                return _ci.ToString();
            }
            private CultureInfo _ci;
        }
        #endregion
    }
}

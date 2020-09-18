using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration;

namespace GLib.Options
{
    [ToolboxItem(true)]
    public partial class OptionsForm : Form
    {

        #region Fields
        private Dictionary<string, object> _ChangedSettings;

        private Color _BoxBackgroundColor = SystemColors.ControlLight;

        private bool _FirstLoad = true;
        private int _CategoryTreeWidth;

        private bool _Saving;
        private bool _ApplyAlwaysEnabled;

        [Browsable(true)]
        [Category("Action")]
        [Description("This event occurs when the form options must be saved (click on Apply or OK form buttons)")]
        public event EventHandler OptionsSaving;

        [Browsable(true)]
        [Category("Action")]
        [Description("This event occurs when the form options have been saved")]
        public event EventHandler OptionsSaved;

        [Browsable(true)]
        [Category("Action")]
        [Description("This event occurs when the form items must be reset (click on Cancel form button)")]
        public event EventHandler ResetForm;

        #endregion

        #region Properties
        public override Size MinimumSize
        {
            get
            {
                Size msz = base.MinimumSize;
                int w = OptionsFormSplit.Panel2MinSize + OptionsFormSplit.SplitterWidth + OptionsFormSplit.SplitterDistance + 3;

                if (base.MinimumSize.Width < w)
                {
                    msz.Width = w;
                }

                return msz;
            }
            set
            {
                base.MinimumSize = value;
            }
        }

        [Browsable(false)]
        [Category("Options Form")]
        [Description("The panels loaded in the OptionsForm")]
        public OptionsPanelList Panels { get; } = new OptionsPanelList();

        [Browsable(false)]
        [Category("Options Form")]
        [Description("The ApplicationSettingsBase object to use for options synchronization")]
        public ApplicationSettingsBase AppSettings { get; set; }

        [Browsable(false)]
        [Category("Options Form")]
        [Description("The Settings object to use for options synchronization")]
        public SettingsPropertyCollection Settings { get; }

        [Category("Options Form")]
        [Description("Images used in the category tree")]
        public ImageList CategoryImages
        {
            get
            {
                return CatTree.ImageList;
            }
            set
            {
                CatTree.ImageList = value;
            }
        }


        [Category("Options Form")]
        [Description("Indicates the width of the Category Tree Box")]
        public int CategoryTreeWidth
        {
            get
            {
                if (_FirstLoad && _CategoryTreeWidth > 0)
                {
                    OptionsFormSplit.SplitterDistance = _CategoryTreeWidth;
                    return _CategoryTreeWidth;
                }

                return OptionsFormSplit.SplitterDistance;
            }
            set
            {
                OptionsFormSplit.SplitterDistance = value;
                _CategoryTreeWidth = value;
            }
        }

        [Category("Options Form")]
        [Description("This is the Text for the category header box")]
        public String CategoryHeaderText
        {
            get
            {
                return CatHeader.Text;
            }
            set
            {
                CatHeader.Text = value;
            }
        }

        [Category("Options Form")]
        [Description("This is the Text for the OK button")]
        public String OkButtonText
        {
            get
            {
                return OKBtn.Text;
            }
            set
            {
                OKBtn.Text = value;
            }
        }

        [Category("Options Form")]
        [Description("This is the Text for the Apply button")]
        public String ApplyButtonText
        {
            get
            {
                return ApplyBtn.Text;
            }
            set
            {
                ApplyBtn.Text = value;
            }
        }

        [Category("Options Form")]
        [Description("This is the Text for the Cancel button")]
        public String CancelButtonText
        {
            get
            {
                return CancelBtn.Text;
            }
            set
            {
                CancelBtn.Text = value;
            }
        }

        [Category("Options Form")]
        [Description("This is the Text for the Application Must Restart label box")]
        public String AppRestartText
        {
            get
            {
                return AppRestartLabel.Text;
            }
            set
            {
                AppRestartLabel.Text = value;
            }
        }

        [Category("Options Form Colors")]
        [Description("Indicates the background color for each box")]
        public Color BoxBackColor
        {
            get
            {
               return _BoxBackgroundColor;
            }
            set
            {
                if (value != null && _BoxBackgroundColor != value)
                {
                    _BoxBackgroundColor = value;

                    CatHeader.BackColor = _BoxBackgroundColor;
                    CatDescr.BackColor = _BoxBackgroundColor;
                    OptionsPanelPath.BackColor = _BoxBackgroundColor;
                    OptionDescrLabel.BackColor = _BoxBackgroundColor;
                    AppRestartLabel.BackColor = _BoxBackgroundColor;
                }
            }
        }

        [Category("Options Form Colors")]
        [Description("Indicates the foreground color for the options panel path box")]
        public Color OptionsPathForeColor
        {
            get
            {
                return OptionsPanelPath.ForeColor;
            }
            set
            {
                OptionsPanelPath.ForeColor = value;
            }
        }

        [Category("Options Form Colors")]
        [Description("Indicates the foreground color for the Application Must Restart label box")]
        public Color AppRestartForeColor
        {
            get
            {
                return AppRestartLabel.ForeColor;
            }
            set
            {
                AppRestartLabel.ForeColor = value;
            }
        }

        [Category("Options Form Descriptions")]
        [Description("This is the default Text for the description box when no description is available")]
        public string OptionsNoDescription { get; set; } = string.Empty;

        [Category("Options Form Descriptions")]
        [Description("This is the description for the Category Header box when the mouse hover it")]
        public string CategoryHeaderDescription
        {
            get
            {
                return CatHeader.AccessibleDescription;
            }
            set
            {
                CatHeader.AccessibleDescription = value;
            }
        }

        [Category("Options Form Descriptions")]
        [Description("This is the description for the Options Panel Path box when the mouse hover it")]
        public string OptionsPanelPathDescription
        {
            get
            {
                return OptionsPanelPath.AccessibleDescription;
            }
            set
            {
                OptionsPanelPath.AccessibleDescription = value;
            }
        }

        [Category("Options Form Descriptions")]
        [Description("This is the description for the Category Tree box when the mouse hover it")]
        public string CategoryTreeDescription
        {
            get
            {
                return CatTree.AccessibleDescription;
            }
            set
            {
                CatTree.AccessibleDescription = value;
            }
        }

        [Category("Options Form Descriptions")]
        [Description("This is the description for the Category Description box when the mouse hover it")]
        public string CategoryDescrDescription
        {
            get
            {
                return CatDescr.AccessibleDescription;
            }
            set
            {
                CatDescr.AccessibleDescription = value;
            }
        }

        [Category("Options Form Descriptions")]
        [Description("This is the description for the Option Description box when the mouse hover it")]
        public string OptionDescrDescription
        {
            get
            {
                return OptionDescrLabel.AccessibleDescription;
            }
            set
            {
                OptionDescrLabel.AccessibleDescription = value;
            }
        }

        [Category("Options Form Descriptions")]
        [Description("This is the description for the Application Must Restart box when the mouse hover it")]
        public string ApplicationRestartDescription
        {
            get
            {
                return AppRestartLabel.AccessibleDescription;
            }
            set
            {
                AppRestartLabel.AccessibleDescription = value;
            }
        }

        [Category("Options Form Descriptions")]
        [Description("This is the description for the OK Form Button when the mouse hover it")]
        public string OkButtonDescription
        {
            get
            {
                return OKBtn.AccessibleDescription;
            }
            set
            {
                OKBtn.AccessibleDescription = value;
            }
        }

        [Category("Options Form Descriptions")]
        [Description("This is the description for the Apply Form Button when the mouse hover it")]
        public string ApplyButtonDescription
        {
            get
            {
                return ApplyBtn.AccessibleDescription;
            }
            set
            {
                ApplyBtn.AccessibleDescription = value;
            }
        }

        [Category("Options Form Descriptions")]
        [Description("This is the description for the Cancel Form Button when the mouse hover it")]
        public string CancelButtonDescription
        {
            get
            {
                return CancelBtn.AccessibleDescription;
            }
            set
            {
                CancelBtn.AccessibleDescription = value;
            }
        }

        [Category("Options Form Flags")]
        [Description("Indicates if the Application Must Restart box must appear")]
        public bool ApplicationMustRestart
        {
            get
            {
                return !OptDescrSplit.Panel2Collapsed;
            }
            set
            {
                OptDescrSplit.Panel2Collapsed = !value;
            }
        }

        [Category("Options Form Flags")]
        [Description("Indicates if the Category Header box must be visible")]
        public bool ShowCategoryHeader
        {
            get
            {
                return CatHeaderPanel.Visible;
            }
            set
            {
                CatHeaderPanel.Visible = value;
            }
        }

        [Category("Options Form Flags")]
        [Description("Indicates if the Category Description box must be visible")]
        public bool ShowCategoryDescription
        {
            get
            {
                return CatDescrPanel.Visible;
            }
            set
            {
                CatDescrPanel.Visible = value;
            }
        }

        [Category("Options Form Flags")]
        [Description("Indicates if the Options Description box must be visible")]
        public bool ShowOptionsDescription
        {
            get
            {
                return !OptionsSplitContainer.Panel2Collapsed;
            }
            set
            {
                OptionsSplitContainer.Panel2Collapsed = !value;
            }
        }

        [Category("Options Form Flags")]
        [Description("Indicates if first Panel must be selected when selection is on a parent Category.")]
        public bool SelectFirstPanel { get; set; }


        [Category("Options Form Flags")]
        [Description("Indicates if the Options Panel Path box must be visible")]
        public bool ShowOptionsPanelPath
        {
            get
            {
                return OptionsPanelPath.Visible;
            }
            set
            {
                OptionsPanelPath.Visible = value;
            }
        }

        [Category("Options Form Flags")]
        [Description("Indicates if the Splitter capacity is enabled for the Options Form")]
        public bool EnableFormSplitter
        {
            get
            {
                return !OptionsFormSplit.IsSplitterFixed;
            }
            set
            {
                OptionsFormSplit.IsSplitterFixed = !value;
            }
        }

        [Category("Options Form Flags")]
        [Description("Indicates if the Options form will automatically save the Application Settings")]
        public bool AutomaticSaveSettings { get; set; } = true;

        [Category("Options Form Flags")]
        [Description("Indicates if the Apply button must be always enabled (and not only when an option change)")]
        public bool ApplyAlwaysEnabled
        {
            get
            {
                return _ApplyAlwaysEnabled;
            }
            set
            {
                _ApplyAlwaysEnabled = value;

                if (_ApplyAlwaysEnabled)
                {
                    ApplyBtn.Enabled = true;
                }
            }
        }

        [Category("Options Form Flags")]
        [Description("Indicates if the Apply button must be always enabled (and not only when an option change)")]
        public bool SaveAndCloseOnReturn
        {
            get
            {
                return AcceptButton != null;
            }
            set
            {
                if (value)
                {
                    AcceptButton = OKBtn;
                }
                else
                {
                    AcceptButton = null;
                }
            }
        }

        [Browsable(false)]
        [Category("Options Form Flags")]
        [Description("Indicates if the OK button was clicked on form close.")]
        public bool OkClicked { get; set; }

        #endregion

        #region Construction

        public OptionsForm() : this(new Settings())
        {
           
        }

        public OptionsForm(ApplicationSettingsBase settings)
        {
            InitializeComponent();

            BoxBackColor = SystemColors.ControlLight;
            OptionsNoDescription = "Description.";

            Panels.PanelAdded += new OptionsPanelEventHandler(_Panels_PanelAdded);

            ApplyBtn.Enabled = _ApplyAlwaysEnabled;

            AppSettings = settings;
            Settings = new SettingsPropertyCollection();
            _ChangedSettings = new Dictionary<String, Object>();

            AppSettings.SettingChanging += new SettingChangingEventHandler(_AppSettings_SettingChanging);
        }

        #endregion

        #region Methods
        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (_CategoryTreeWidth > 0 && _FirstLoad)
            {
                OptionsFormSplit.Dock = DockStyle.None;
                OptionsFormSplit.Dock = DockStyle.Fill;
                OptionsFormSplit.SplitterDistance = _CategoryTreeWidth;
                _FirstLoad = false;
            }
        }
        void _Panels_PanelAdded(object sender, OptionsPanelEventArgs e)
        {
            AddPanel(e.Panel);
        }
        private void AddPanel(OptionsPanel panel)
        {
            string optionCategory = panel.CategoryPath;
            string displayName = panel.DisplayName;

            string[][] lpaths = GetPaths(optionCategory);
            string[] paths = lpaths[0];
            string[] labs = lpaths[1];

            TreeNode pnode = null;
            TreeNode nnode = null;

            if (paths.Length > 1)
            {
                TreeNode[] search = CatTree.Nodes.Find(paths[0], false);

                if (search != null && search.Length > 0)
                {
                    pnode = search[0];
                }
                else
                {
                    CatTree.Nodes.Add(paths[0], labs[0], paths[0], paths[0]);
                    pnode = CatTree.Nodes[CatTree.Nodes.Count - 1];
                }

                int i = 1;
                int sub = paths.Length - 1;
                for (; i < sub; i++)
                {
                    search = pnode.Nodes.Find(paths[i], false);

                    if (search != null && search.Length > 0)
                    {
                        pnode = search[0];
                    }
                    else
                    {
                        pnode.Nodes.Add(paths[i], labs[i], String.Join("\\", paths, 0, i + 1), String.Join("\\", paths, 0, i + 1));
                        pnode = pnode.Nodes[pnode.Nodes.Count - 1];
                    }
                }

                if (i < sub)
                {
                    pnode = null;
                }
            }

            if (pnode != null)
            {
                nnode = new TreeNode(displayName)
                {
                    Name = optionCategory,
                    ImageKey = String.Join("\\", paths),
                    SelectedImageKey = String.Join("\\", paths)
                };
                pnode.Nodes.Add(nnode);

                if (panel != null)
                {
                    panel.OptionsChanged += new EventHandler(panel_OptionsChanged);
                    panel.Dock = DockStyle.Fill;
                    panel.PanelAdded(this);

                    EnableDescrControl(panel);
                }
            }
            else if (paths.Length == 1)
            {
                nnode = new TreeNode(displayName)
                {
                    Name = optionCategory
                };
                CatTree.Nodes.Add(nnode);

                if (panel != null)
                {
                    panel.OptionsChanged += new EventHandler(panel_OptionsChanged);
                    panel.Dock = DockStyle.Fill;
                    panel.PanelAdded(this);

                    EnableDescrControl(panel);
                }
            }
        }

        private string[][] GetPaths(String path)
        {
            string[][] ret;

            string[] p = path.Split(new String[] { CatTree.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);

            ret = new string[][] { new String[p.Length], new String[p.Length] };

            int i1, i2;
            for (int i = 0; i < p.Length; i++)
            {
                String sp = p[i];

                if ((i1 = sp.IndexOf("{\"")) > -1
                    && (i2 = sp.IndexOf("\"}")) > -1)
                {
                    ret[0][i] = sp.Substring(0, i1);
                    ret[1][i] = sp.Substring(i1 + 2, i2 - (i1 + 2));
                }
                else
                {
                    ret[0][i] = sp;
                    ret[1][i] = sp;
                }
            }

            return ret;
        }

        void panel_OptionsChanged(object sender, EventArgs e)
        {
            if (!_ApplyAlwaysEnabled)
            {
                ApplyBtn.Enabled = true;
            }
        }

        private void EnableDescrControl(Control ctrl)
        {
            ctrl.MouseEnter += new EventHandler(MouseEnterDescr);
            ctrl.MouseLeave += new EventHandler(MouseLeaveDescr);

            foreach (Control ctrln in ctrl.Controls)
            {
                EnableDescrControl(ctrln);
            }
        }

        public OptionsPanel SwitchPanel(String optionCategory)
        {
            try
            {
                OptionsPanelPath.Text = "";

                if (Panels.Count > 0)
                {
                    OptionsPanel pn = null;

                    for (int i = 0; i < Panels.Count; i++)
                    {
                        if (Panels[i].CategoryPath == optionCategory)
                        {
                            pn = Panels[i];
                            break;
                        }
                    }

                    if (pn != null)
                    {
                        string displayName = pn.DisplayName;

                        string[][] lpaths = GetPaths(optionCategory);
                        string[] paths = lpaths[0];

                        if (paths.Length > 1)
                        {
                            TreeNode[] search = CatTree.Nodes.Find(paths[0], false);

                            if (search != null && search.Length > 0)
                            {
                                OptionsPanelPath.Text += search[0].Text + " � ";

                                int i = 1;
                                int sub = paths.Length - 1;
                                for (; i < sub; i++)
                                {
                                    search = search[0].Nodes.Find(paths[i], false);

                                    if (search != null && search.Length > 0)
                                    {
                                        OptionsPanelPath.Text += search[0].Text + " � ";
                                    }
                                }

                            }
                        }

                        OptionsPanelPath.Text += displayName;

                        if (OptionPanelContainer.Controls.Count == 0 || !OptionPanelContainer.Controls[0].Equals(pn))
                        {
                            OptionPanelContainer.Controls.Clear();
                            OptionPanelContainer.Controls.Add(pn);

                            pn.Visible = true;
                        }

                        return pn;
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private void CatTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            GoToFirstPanel(e.Node);
        }

        private void GoToFirstPanel(TreeNode selectedNode)
        {
            if (selectedNode.Nodes != null && selectedNode.Nodes.Count > 0)
            {
                selectedNode.Expand();

                if (SelectFirstPanel)
                {
                    selectedNode.TreeView.SelectedNode = selectedNode.Nodes[0];
                }
                else
                {
                    GoToFirstPanel(selectedNode.Nodes[0]);
                }
            }
            else
            {
                OptionsPanel pn = SwitchPanel(selectedNode.Name);

                if (pn != null && !string.IsNullOrEmpty(pn.AccessibleDescription))
                {
                    CatDescr.Text = pn.AccessibleDescription;
                }
                else
                {
                    CatDescr.Text = OptionsNoDescription;
                }
            }
        }

        void _AppSettings_SettingChanging(object sender, SettingChangingEventArgs e)
        {
            if (!_Saving)
            {
                try
                {
                    SettingsProperty value = Settings[e.SettingName];
                    object sval = AppSettings[e.SettingName];

                    if (value != null && sval != null)
                    {
                        if (!sval.Equals(e.NewValue))
                        {
                            try
                            {
                                if (!_ChangedSettings.ContainsKey(e.SettingName))
                                {
                                    _ChangedSettings.Add(e.SettingName, value.DefaultValue);
                                }
                            }
                            catch
                            {
                                _ChangedSettings.Add(e.SettingName, value.DefaultValue);
                            }
                        }
                        else
                        {
                            try
                            {
                                _ChangedSettings.Remove(e.SettingName);
                            }
                            catch
                            {
                            }
                        }

                        if (!_ApplyAlwaysEnabled)
                        {
                            if (_ChangedSettings.Count > 0)
                            {
                                ApplyBtn.Enabled = true;
                            }
                            else
                            {
                                ApplyBtn.Enabled = false;
                            }
                        }

                        value.DefaultValue = e.NewValue;
                        e.Cancel = true;
                    }
                }
                catch
                {
                }
            }
        }

        public void OnSaveOptions()
        {
            _Saving = true;

            OptionsSaving?.Invoke(this, EventArgs.Empty);

            foreach (SettingsProperty sett in Settings)
            {
                try
                {
                    AppSettings[sett.Name] = sett.DefaultValue;
                }
                catch
                {
                }
            }

            if (AutomaticSaveSettings)
            {
                AppSettings.Save();
                AppSettings.Reload();

                OptionsSaved?.Invoke(this, EventArgs.Empty);
            }

            _Saving = false;
        }

        public void OnResetOptions()
        {
            ResetForm?.Invoke(this, EventArgs.Empty);

            if (!_ApplyAlwaysEnabled)
            {
                ApplyBtn.Enabled = false;
            }
        }

        public void MouseEnterDescr(object sender, EventArgs e)
        {
            Control ctrl = (Control)sender;

            if (ctrl != null && !string.IsNullOrEmpty(ctrl.AccessibleDescription))
            {
                OptionDescrLabel.Text = ctrl.AccessibleDescription;
            }
            else
            {
                OptionDescrLabel.Text = OptionsNoDescription;
            }
        }

        public void MouseLeaveDescr(object sender, EventArgs e)
        {
            OptionDescrLabel.Text = OptionsNoDescription;
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            OkClicked = true;
        }

        private void ApplyBtn_Click(object sender, EventArgs e)
        {
            OnSaveOptions();

            if (!_ApplyAlwaysEnabled)
            {
                ApplyBtn.Enabled = false;
            }
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            OkClicked = false;
        }

        private void OptionsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //e.Cancel = true;

            Visible = false;

            if (OkClicked)
            {
                OnSaveOptions();
            }
            else
            {
                OnResetOptions();
            }
        }
        #endregion
    }

    internal sealed class Settings : ApplicationSettingsBase
    {
    }
}
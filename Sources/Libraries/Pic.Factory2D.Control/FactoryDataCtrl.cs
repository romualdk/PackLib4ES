﻿#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using log4net;
#endregion

namespace Pic.Factory2D.Control
{
    public partial class FactoryDataCtrl : UserControl
    {
        #region Constructor
        public FactoryDataCtrl()
        {
            InitializeComponent();
        }
        #endregion

        #region Paint event handlers
        private void PbCut_Paint(object sender, PaintEventArgs e)
        {
            Size sz = pbCut.Size;
            e.Graphics.DrawLine( new Pen(Color.Red), new Point(0, sz.Height / 2), new Point(sz.Width / 2, sz.Height / 2) );
        }
        private void PbFold_Paint(object sender, PaintEventArgs e)
        {
            Size sz = pbFold.Size;
            e.Graphics.DrawLine(new Pen(Color.Blue), new Point(0, sz.Height / 2), new Point(sz.Width / 2, sz.Height / 2));
        }
        #endregion

        #region Public properties
        public PicFactory Factory
        {
            set { _factory = value; Refresh(); }
        }
        #endregion

        #region Factory update handling
        public override void Refresh()
        {
            try
            {
                // create entities
                if (Parent is IEntitySupplier)
                    _entitySupplier = this.Parent as IEntitySupplier;
                if (null != _entitySupplier)
                {
                    // clear factory
                    _factory.Clear();
                    // update factory
                    _entitySupplier.CreateEntities(_factory);
                }

                // get selected tab and compute data accordingly
                switch (tabControlData.SelectedIndex)
                {
                    case 0:
                        // compute length
                        PicVisitorDieCutLength visitorLengthes = new PicVisitorDieCutLength();
                        _factory.ProcessVisitor(visitorLengthes);
                        // update controls
                        double lengthCut = 0.0, lengthFold = 0.0;
                        if (visitorLengthes.Lengths.ContainsKey(PicGraphics.LT.LT_CUT))
                            lengthCut = visitorLengthes.Lengths[PicGraphics.LT.LT_CUT];

                        lblValueLengthCut.Text = UnitSystem.Instance.CumulativeLength(lengthCut); //string.Format(": {0:0.###} m", lengthCut / 1000.0);
                        if (visitorLengthes.Lengths.ContainsKey(PicGraphics.LT.LT_CREASING))
                            lengthFold = visitorLengthes.Lengths[PicGraphics.LT.LT_CREASING];
                        lblValueLengthFold.Text = UnitSystem.Instance.CumulativeLength(lengthFold); //string.Format(": {0:0.###} m", lengthFold / 1000.0);
                        lblValueLengthTotal.Text = UnitSystem.Instance.CumulativeLength(lengthCut + lengthFold); //string.Format(": {0:0.###} m", (lengthCut + lengthFold) / 1000.0);
                        break;
                    case 1:
                        // compute bounding box
                        Box2D bbox = Tools.BoundingBox(_factory, 0.0);
                        // update controls
                        lblValueLength.Text = string.Format(": {0:0.#} {1}", bbox.Width, UnitSystem.Instance.UnitLength);
                        lblValueWidth.Text = string.Format(": {0:0.#} {1}", bbox.Height, UnitSystem.Instance.UnitLength);
                        break;
                    case 2:
                        // compute area
                        try
                        {
                            PicToolArea picToolArea = new PicToolArea();
                            _factory.ProcessTool(picToolArea);
                            lblAreaValue.Text = UnitSystem.Instance.Area(picToolArea.Area);//string.Format(": {0:0.###} m²", (picToolArea.Area * 1.0E-06));

                        lblNameFormat.Visible = lblValueFormat.Visible = _factory.HasCardboardFormat;
                        lblNameEfficiency.Visible = lblValueEfficiency.Visible = _factory.HasCardboardFormat;

                        if (_factory.HasCardboardFormat)
                        {
                            lblValueFormat.Text = string.Format(": {0:0.#} x {1:0.#}", _factory.Format.Width, _factory.Format.Height);
                            lblValueEfficiency.Text = string.Format(": {0:0.#} %", 100.0 * picToolArea.Area / (_factory.Format.Width * _factory.Format.Height));
                        }
                       }
                        catch (PicToolTooLongException /*ex*/)
                        {
                            lblAreaValue.Text = Properties.Resources.ID_ABANDONPROCESSING;
                            lblNameEfficiency.Visible = false;
                            lblValueEfficiency.Visible = false;
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region Private data members
        protected static readonly ILog _log = LogManager.GetLogger(typeof(FactoryDataCtrl));
        private IEntitySupplier _entitySupplier;
        private Pic.Factory2D.PicFactory _factory = new PicFactory();

        public delegate void onTabChanged(int currentIndex);
        public event onTabChanged TabChanged;
        #endregion

        /// <summary>
        /// call TabChanged 
        /// </summary>
        private void OnTabControlSelectedIndexChanged(object sender, EventArgs e)
        {
            Refresh();
            TabChanged?.Invoke(tabControlData.SelectedIndex);
        }
    }
}

#region Using directives
using System;
using System.Collections.Generic;

using Sharp3D.Math.Core;
using IxMilia.Dxf;
using IxMilia.Dxf.Entities;
using log4net;
#endregion

namespace Pic.Factory2D
{
    public class PicLoaderDXF : IDisposable
    {
        #region Constructor
        public PicLoaderDXF(PicFactory factory)
        {
            Factory = factory;
            // clear before loading data
            Factory.Clear();
        }
        #endregion

        #region IDisposable override
        public void Dispose()
        {
        }
        #endregion

        #region Loading
        public bool Load(System.IO.Stream stream)
        {
            try
            {
                return Load( DxfFile.Load(stream) );
            }
            catch (Exception ex)
            {
                _log.Warn($"Libray IxMilia.Dxf failed to load DXF stream : {ex.Message}");
                return false;
            }
        }
        public bool Load(string filePath)
        {
            try
            {
                return Load( DxfFile.Load(filePath) );
            }
            catch (Exception ex)
            {
                _log.Warn($"Libray IxMilia.Dxf failed to load DXF file {filePath} : {ex.Message}");
                return false;
            }
        }

        private bool Load(DxfFile dxfFile)
        {
            foreach (var lineType in dxfFile.LineTypes)
            { }

            foreach (var entity in dxfFile.Entities)
            {
                try
                {
                    switch (entity.EntityType)
                    {
                        case DxfEntityType.Line: ProcessDxfLine(entity as DxfLine); break;
                        case DxfEntityType.Arc: ProcessDxfArc(entity as DxfArc); break;
                        case DxfEntityType.Circle: ProcessDxfCircle(entity as DxfCircle); break;
                        case DxfEntityType.Polyline: ProcessDxfPolyLine(entity as DxfPolyline); break;
                        case DxfEntityType.LwPolyline: ProcessDxfLwPolyline(entity as DxfLwPolyline); break;
                        default:
                            _log.Warn($"Skipping entity of type {entity.EntityType}");
                            break;
                    }
                }
                catch (Exception /*ex*/)
                {
                    _log.Warn($"Failed to process {entity.EntityType}");
                }
            }
            return true;
        }
        #endregion

        #region Processing methods
        void ProcessDxfPoint(DxfPoint dxfPoint)
        {
            PicPoint picPoint = Factory.AddPoint(
                PicGraphics.LT.LT_CUT, 0, 0,
                new Vector2D(dxfPoint.X, dxfPoint.Y)
                );
        }
        void ProcessDxfLine(DxfLine dxfLine)
        {
            PicSegment picSegment = Factory.AddSegment(
                DxfColor2PicLT(dxfLine), 0, 0
                , new Vector2D(dxfLine.P1.X, dxfLine.P1.Y)
                , new Vector2D(dxfLine.P2.X, dxfLine.P2.Y)
                );
            picSegment.Group = DxfLayer2PicGrp(dxfLine.Layer);
        }
        void ProcessDxfLwPolyline(DxfLwPolyline dxfPolyline)
        {
            if (dxfPolyline.Vertices.Count < 2) return;

            int iVertexCount = 0;
            Vector2D startPoint = new Vector2D();
            foreach (var vertex in dxfPolyline.Vertices)
            {
                Vector2D endPoint = new Vector2D(vertex.X, vertex.Y);
                if (iVertexCount > 0)
                {
                    PicSegment picSegment = Factory.AddSegment(
                        DxfColor2PicLT(dxfPolyline), 0, 0
                        , startPoint
                        , endPoint);
                    picSegment.Group = DxfLayer2PicGrp(dxfPolyline.Layer);
                }

                startPoint = endPoint;
                ++iVertexCount;
            }

            PicSegment endSegment = Factory.AddSegment(
                    DxfColor2PicLT(dxfPolyline), 0, 0
                    , startPoint
                    , new Vector2D(dxfPolyline.Vertices[0].X, dxfPolyline.Vertices[0].Y)
                );
            endSegment.Group = DxfLayer2PicGrp(dxfPolyline.Layer);
        }
        void ProcessDxfPolyLine(DxfPolyline dxfPolyLine)
        {
            if (!dxfPolyLine.ContainsVertices) return;

            int iVertexCount = 0;
            Vector2D startPoint = new Vector2D();
            foreach (var vertex in dxfPolyLine.Vertices)
            {
                Vector2D endPoint = new Vector2D(vertex.Location.X, vertex.Location.Y);
                if (iVertexCount > 0)
                {
                    PicSegment picSegment = Factory.AddSegment(
                        DxfColor2PicLT(dxfPolyLine), 0, 0
                        , startPoint
                        , endPoint);
                }

                startPoint = endPoint;
                ++iVertexCount;
            }
        }
        void ProcessDxfArc(DxfArc dxfArc)
        {
            
            double angle0 = dxfArc.StartAngle;
            double angle1 = dxfArc.EndAngle;
            double angle = dxfArc.EndAngle - dxfArc.StartAngle;
            if (angle > 0.0)
            {
                PicArc picArc = Factory.AddArc(
                    DxfColor2PicLT(dxfArc), DxfLayer2PicGrp(dxfArc.Layer), 0
                    , new Vector2D(dxfArc.Center.X, dxfArc.Center.Y)
                    , dxfArc.Radius
                    , angle0, angle1
                );
            }
            else
            {
                while (angle < 0)
                    angle += 360.0;
                while (angle > 360.0)
                    angle -= 360.0;


                angle0 += angle;
                angle1 -= angle;
                while (angle0 < 0.0)
                {
                    angle0 += 360.0;
                    angle1 += 360.0;
                }
                while (angle0 >= 360.0)
                    angle0 -= 360.0;




                PicArc picArc = Factory.AddArc(
                    DxfColor2PicLT(dxfArc), DxfLayer2PicGrp(dxfArc.Layer), 0
                    , new Vector2D(dxfArc.Center.X, dxfArc.Center.Y)
                    , dxfArc.Radius
                    , angle0, angle1
                    );
            }
        }
        void ProcessDxfCircle(DxfCircle dxfCircle)
        {
            PicArc picArc = Factory.AddArc(
                DxfColor2PicLT(dxfCircle), DxfLayer2PicGrp(dxfCircle.Layer), 0
                , new Vector2D(dxfCircle.Center.X, dxfCircle.Center.Y)
                , dxfCircle.Radius
                , 0.0, 360.0
                );
        }
        #endregion

        #region Helpers
        public PicGraphics.LT DxfColor2PicLT(DxfEntity entity)
        {
            DxfColor dxfColor = entity.Color;
            if (dxfColor.IsByLayer)
            {
                string layerName = entity.Layer;
                if (!LineTypeDictionary.ContainsKey(layerName))
                {
                    PicGraphics.LT lt = PicGraphics.LT.LT_CUT;
                    if (string.Equals(layerName, "CUTLINES", StringComparison.CurrentCultureIgnoreCase))
                        lt = PicGraphics.LT.LT_CUT;
                    else if (string.Equals(layerName, "CREASELINES", StringComparison.CurrentCultureIgnoreCase))
                        lt = PicGraphics.LT.LT_CREASING;
                    else if (string.Equals(layerName, "GLUEPERFORATIONS", StringComparison.CurrentCultureIgnoreCase))
                        lt = PicGraphics.LT.LT_PERFOCREASING;
                    else if (string.Equals(layerName, "0", StringComparison.CurrentCultureIgnoreCase))
                        lt = PicGraphics.LT.LT_CUT;
                    else if (string.Equals(layerName, "1", StringComparison.CurrentCultureIgnoreCase))
                        lt = PicGraphics.LT.LT_CREASING;
                    else if (string.Equals(layerName, "10", StringComparison.CurrentCultureIgnoreCase))
                        lt = PicGraphics.LT.LT_CUT;
                    else if (string.Equals(layerName, "20", StringComparison.CurrentCultureIgnoreCase))
                        lt = PicGraphics.LT.LT_CREASING;
                    else
                        lt = PicGraphics.LT.LT_CUT;

                    LineTypeDictionary.Add(layerName, lt);
                }
                return LineTypeDictionary[layerName];
            }
            else if (dxfColor.IsIndex)
            {
                string csIndex = dxfColor.Index.ToString();
                if (!LineTypeDictionary.ContainsKey(csIndex))
                {
                    PicGraphics.LT lt = PicGraphics.LT.LT_CUT;
                    if (csIndex == "0")
                        lt = PicGraphics.LT.LT_CREASING;
                    else if (csIndex == "1")
                        lt = PicGraphics.LT.LT_CUT;
                    else if (csIndex == "2")
                        lt = PicGraphics.LT.LT_CREASING;
                    else if (csIndex == "3")
                        lt = PicGraphics.LT.LT_CREASING;
                    else
                        lt = PicGraphics.LT.LT_CUT;

                    LineTypeDictionary.Add(csIndex, lt);
                }
                return LineTypeDictionary[csIndex];
            }
            else if (dxfColor.IsByBlock)
            {
                return PicGraphics.LT.LT_CUT;
            }
            else
                return PicGraphics.LT.LT_CUT;
        }
        short DxfLayer2PicGrp(string layerName)
        {
            if (!Layer2GrpDictionary.ContainsKey(layerName))
                Layer2GrpDictionary.Add(layerName, 0);
            return Layer2GrpDictionary[layerName];
        }
        #endregion

        #region Private properties
        private PicFactory Factory { get; set; }
        private Dictionary<string, PicGraphics.LT> LineTypeDictionary { get; set; } = new Dictionary<string, PicGraphics.LT>();
        private Dictionary<string, short> Layer2GrpDictionary { get; set; } = new Dictionary<string, short>();
        #endregion

        #region Members
        private static ILog _log = LogManager.GetLogger(typeof(PicLoaderDXF));
        #endregion
    }
}

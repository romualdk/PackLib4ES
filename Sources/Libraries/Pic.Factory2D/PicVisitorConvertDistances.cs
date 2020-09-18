#region Using directives
#endregion

namespace Pic.Factory2D
{
    public class PicVisitorConvertDistances : PicFactoryVisitor
    {
        public PicVisitorConvertDistances(double multiplicator)
        {
            Multiplicator = multiplicator;
        }

        public override void ProcessEntity(PicEntity entity)
        {
            if (entity is PicPoint point)
                FactoryOut.AddPoint(point.LineType, point.Group, point.Layer, Multiplicator * point.Coord);
            else if (entity is PicSegment seg)
                FactoryOut.AddSegment(seg.LineType, seg.Group, seg.Layer, Multiplicator * seg.Pt0, Multiplicator * seg.Pt1);
            else if (entity is PicArc arc)
                FactoryOut.AddArc(arc.LineType, arc.Group, arc.Layer, Multiplicator * arc.Center, Multiplicator * arc.Radius, arc.AngleBeg, arc.AngleEnd);
            else if (entity is PicNurb nurb)
                FactoryOut.AddNurb(nurb.LineType, nurb.Group, nurb.Layer);
            else if (entity is PicCotationDistance cotDist)
            {
                PicCotation.CotType eCotType = PicCotation.CotType.COT_DISTANCE; ;
                if (entity is PicCotationDistance) eCotType = PicCotation.CotType.COT_DISTANCE;
                else if (entity is PicCotationHorizontal) eCotType = PicCotation.CotType.COT_HORIZONTAL;
                else if (entity is PicCotationVertical) eCotType = PicCotation.CotType.COT_VERTICAL;
                FactoryOut.AddCotation(eCotType, cotDist.Group, cotDist.Layer, Multiplicator * cotDist.Pt0, Multiplicator * cotDist.Pt1, Multiplicator * cotDist.Offset, cotDist.Text, 1);
            }
        }
        public override void Finish() {}
        public override void Initialize(PicFactory factory) {}

        public PicFactory FactoryOut { get; private set; } = new PicFactory();
        public double Multiplicator { get; set; } = 1.0;
    }

    public class PicVisitorConvertDistancesRef : PicFactoryVisitor
    {
        public PicVisitorConvertDistancesRef(double multiplicator)
        {
            Multiplicator = multiplicator;
        }

        public override void ProcessEntity(PicEntity entity)
        {
            if (entity is PicPoint point)
                FactoryOut.AddPoint(point.LineType, point.Group, point.Layer, Multiplicator * point.Coord);
            else if (entity is PicSegment seg)
                FactoryOut.AddSegment(seg.LineType, seg.Group, seg.Layer, Multiplicator * seg.Pt0, Multiplicator * seg.Pt1);
            else if (entity is PicArc arc)
                FactoryOut.AddArc(arc.LineType, arc.Group, arc.Layer, Multiplicator * arc.Center, Multiplicator * arc.Radius, arc.AngleBeg, arc.AngleEnd);
            else if (entity is PicNurb nurb)
                FactoryOut.AddNurb(nurb.LineType, nurb.Group, nurb.Layer);
            else if (entity is PicCotationDistance cotDist)
            {
                PicCotation.CotType eCotType = PicCotation.CotType.COT_DISTANCE; ;
                if (entity is PicCotationDistance) eCotType = PicCotation.CotType.COT_DISTANCE;
                else if (entity is PicCotationHorizontal) eCotType = PicCotation.CotType.COT_HORIZONTAL;
                else if (entity is PicCotationVertical) eCotType = PicCotation.CotType.COT_VERTICAL;
                FactoryOut.AddCotation(eCotType, cotDist.Group, cotDist.Layer, Multiplicator * cotDist.Pt0, Multiplicator * cotDist.Pt1, Multiplicator * cotDist.Offset, cotDist.Text, 1);
            }
        }
        public override void Finish() { }
        public override void Initialize(PicFactory factory) { }

        public PicFactory FactoryOut { get; private set; } = new PicFactory();
        public double Multiplicator { get; set; } = 1.0;
    }
}

#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using Sharp3D.Math.Core;

using Pic.Plugin;
using Pic.Factory2D;
#endregion

namespace Pic.Plugin
{
    public class Tools : IDisposable
    {
        #region Data members
        public static readonly int FontSize = 10;
        public static readonly Color FontColor = Color.White;
        public static readonly Color BackgroundColor = Color.Black;
        #endregion

        #region Constructors
        /// <summary>
        /// Instantiates the <see cref="Tools"/> class
        /// </summary>
        /// <param name="filepath">Component file path</param>
        public Tools(string filepath, IComponentSearchMethod searchMethod)
        {
            ComponentLoader loader = new ComponentLoader();
            loader.SearchMethod = searchMethod;
            Component = loader.LoadComponent(filepath);
        }

        public Tools(Guid compGuid, IComponentSearchMethod searchMethod)
        {
            ComponentLoader loader = new ComponentLoader();
            loader.SearchMethod = searchMethod;
            Component = loader.LoadComponent(compGuid);
        }
        /// <summary>
        /// Instantiates the <see cref="Tools"/> class
        /// </summary>
        /// <param name="component">An instance of the <see cref="Component"/> class.</param>
        public Tools(Component component, IComponentSearchMethod searchMethod)
        {
            Component = component;
        }
        #endregion

        #region Public properties
        internal Component Component { get; private set; }
        public bool ReflexionX { get; set; } = false;
        public bool ReflexionY { get; set; } = false;
        public bool ShowCotations { get; set; } = true;
        public string SourceCode
        {
            get
            {
                if (null == Component)
                    throw new PluginException("No plugin loaded");
                return Component.SourceCode;
            }
        }
        public Guid Guid
        {
            get
            {
                if (null == Component)
                    throw new PluginException("No plugin loaded");
                return Component.Guid;
            }
        }
        public string Name
        {
            get
            {
                if (null == Component)
                    throw new PluginException("No plugin loaded");
                return Component.Name;
            }            
        }
        public string Description
        {
            get
            {
                if (null == Component)
                    throw new PluginException("No plugin loaded");
                return Component.Description;
            }            
        }

        public string Version
        {
            get
            {
                if (null == Component)
                    throw new PluginException("No plugin loaded");
                return Component.Version;
            }
        }
        #endregion

        #region Load method
        /// <summary>
        /// Loads a plugin from a given file path
        /// </summary>
        /// <param name="filepath">Plugin file path</param>
        private void LoadPluginFromFile(string filepath)
        {
            ComponentLoader loader = new ComponentLoader();            
            Component = loader.LoadComponent(filepath);
        }
        #endregion

        #region Tool methods
        /// <summary>
        /// Generate an image from plugin with default parameters
        /// </summary>
        /// <param name="size"></param>
        /// <param name="filter"></param>
        /// <param name="bmp"></param>
        public void GenerateImage(Size size, out Image bmp)
        {
            // check if plugin was instantiated
            if (null == Component)
                throw new Exception("No valid plugin instance loaded!");

            // instantiate factory
            PicFactory factory = new PicFactory();
            // generate entities
            ParameterStack stack = Component.BuildParameterStack(null);
            Component.CreateFactoryEntities(factory, stack);
            // apply any needed transformation
            if (ReflexionX) factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionX));
            if (ReflexionY) factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionY));
            // draw image
            PicGraphicsImage picImage = new PicGraphicsImage(size, Factory2D.Tools.BoundingBox(factory, 0.05));
            factory.Draw(picImage, ShowCotations ? PicFilter.FilterNone : !PicFilter.FilterCotation);
            bmp = picImage.Bitmap;
        }

        public void GenerateImage(Size size, ParameterStack stack, out Image bmp)
        {
            // check if plugin was instantiated
            if (null == Component)
                throw new Exception("No valid plugin instance loaded!");

            // instantiate factory
            PicFactory factory = new PicFactory();
            // generate entities
            Component.CreateFactoryEntities(factory, stack);
            // apply any needed transformation
            if (ReflexionX) factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionX));
            if (ReflexionY) factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionY));
            // draw image
            PicGraphicsImage picImage = new PicGraphicsImage(size, Factory2D.Tools.BoundingBox(factory, 0.05));
            factory.Draw(picImage, ShowCotations ? PicFilter.FilterNone : !PicFilter.FilterCotation);

            bmp = picImage.Bitmap;
        }

        public byte[] GenerateImage(Size size, ParameterStack stack, ImageFormat format)
        {
            GenerateImage(size, stack, out Image bmp);
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, format);
            return ms.GetBuffer();
        }

        public byte[] GenerateExportFile(string fileFormat, ParameterStack stack)
        {
            // build factory
            PicFactory factory = new PicFactory();
            Component.CreateFactoryEntities(factory, stack);
            if (ReflexionX) factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionX));
            if (ReflexionY) factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionY));

            // instantiate filter
            PicFilter filter = (ShowCotations ? PicFilter.FilterNone : !PicFilter.FilterCotation) & PicFilter.FilterNoZeroEntities;

            // get bounding box
            PicVisitorBoundingBox visitorBoundingBox = new PicVisitorBoundingBox();
            factory.ProcessVisitor(visitorBoundingBox, filter);
            Box2D box = visitorBoundingBox.Box;
            // add margins : 5 % of the smallest value among height 
            box.AddMarginRatio(0.05);
            // get file content
            if ("des" == fileFormat)
            {
                PicVisitorDesOutput visitor = new PicVisitorDesOutput
                {
                    Author = "treeDiM"
                };
                // process visitor
                factory.ProcessVisitor(visitor, filter);
                return visitor.GetResultByteArray();
            } 
            else if ("dxf" == fileFormat)
            {
                PicVisitorOutput visitor = new PicVisitorDxfOutput();
                visitor.Author = "treeDiM";
                // process visitor
                factory.ProcessVisitor(visitor, filter);
                return visitor.GetResultByteArray();

            }
            #if PDFSharp
            else if ("pdf" == fileFormat)
            {
                PicGraphicsPdf graphics = new PicGraphicsPdf(box)
                {
                    Author = "treeDiM",
                    Title = "Pdf Export"
                };
                // draw
                factory.Draw(graphics, filter);
                return graphics.GetResultByteArray();
            }
            #endif
            else
                throw new Exception("Invalid file format : " + fileFormat);
        }

        public void Dimensions(ParameterStack stack, out double width, out double height, out double lengthCut, out double lengthFold)
        {
            // build factory
            PicFactory factory = new PicFactory();
            Component.CreateFactoryEntities(factory, stack);
            if (ReflexionX) factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionX));
            if (ReflexionY) factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionY));
            // get bounding box
            PicVisitorBoundingBox visitorBoundingBox = new PicVisitorBoundingBox();
            factory.ProcessVisitor(visitorBoundingBox, ShowCotations ? PicFilter.FilterNone : !PicFilter.FilterCotation);
            Box2D box = visitorBoundingBox.Box;
            width = box.Width;
            height = box.Height;
            // get length
            PicVisitorDieCutLength visitorLength = new PicVisitorDieCutLength();
            factory.ProcessVisitor(visitorLength, PicFilter.FilterNone);
            lengthCut = visitorLength.Lengths.ContainsKey(PicGraphics.LT.LT_CUT) ? visitorLength.Lengths[PicGraphics.LT.LT_CUT] : 0.0;
            lengthFold = visitorLength.Lengths.ContainsKey(PicGraphics.LT.LT_CREASING) ? visitorLength.Lengths[PicGraphics.LT.LT_CREASING] : 0.0;
        }

        public string GetInsertionCode()
        {
            ParameterStack stackIn = Component.BuildParameterStack(null);
            string csCode = string.Empty;
            csCode += "\n";
            csCode += "\t\t{ // " + Component.Name + "\n";
            csCode += "\t\t\tIPlugin pluginIn = Host.GetPluginByGuid(\"" + Component.Guid.ToString()+ "\");\n";
            csCode += "\t\t\tParameterStack stackIn = Host.GetInitializedParameterStack(pluginIn);\n";
            foreach (Parameter param in stackIn)
            {
                // double
                if (param is ParameterDouble paramDouble)
                    csCode += "\t\t\tstackIn.SetDoubleParameter(\"" + paramDouble.Name + "\"," + paramDouble.ValueDefault.ToString() + ");\t\t// " + paramDouble.Description + "\n";

                // bool
                if (param is ParameterBool paramBool)
                    csCode += "\t\t\tstackIn.SetBoolParameter(\"" + paramBool.Name + "\"," + (paramBool.ValueDefault ? "true" : "false") + ");\t\t// " + paramBool.Description + "\n";

                // int
                if (param is ParameterInt paramInt)
                    csCode += "\t\t\tstackIn.SetIntParameter(\"" + paramInt.Name + "\"," + paramInt.ValueDefault.ToString() + ");\t\t// " + paramInt.Description + "\n";

                // multi
                if (param is ParameterMulti paramMulti)
                    csCode += "\t\t\tstackIn.SetMultiParameter(\"" + paramMulti.Name + "\"," + paramMulti.Value.ToString() + ");\t\t// " + paramMulti.Description + "\n";
            }
            csCode += "\t\t\tbool reflectionX = false, reflectionY = false;\n";
            csCode += "\t\t\tTransform2D transfReflect = (reflectionY ? Transform2D.ReflectionY : Transform2D.Identity) * (reflectionX ? Transform2D.ReflectionX : Transform2D.Identity);\n";
            csCode += "\t\t\tpluginIn.CreateFactoryEntities(fTemp, stackIn,\n";
			csCode += "\t\t\t\t Transform2D.Translation(new Vector2D(0.0, 0.0))\n";
			csCode += "\t\t\t\t *Transform2D.Rotation(0.0)\n";
			csCode += "\t\t\t\t *transfReflect);\n";
            csCode += "\t\t}\n";
            return csCode;
        }
#endregion

#region Static methods
        /// <summary>
        /// Annotates an image with specified string
        /// </summary>
        /// <param name="image">Image that will be annotated</param>
        /// <param name="annotation">Annotation</param>
        public static void Annotate(Image image, string annotation)
        {
            // graphics
            Graphics grph = Graphics.FromImage(image);
            Font tfont = new Font("Arial", FontSize);
            Brush tbrush = new SolidBrush(FontColor);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Far;
            sf.LineAlignment = StringAlignment.Far;
            Size txtSize = grph.MeasureString(annotation, tfont).ToSize();
            grph.FillRectangle(new SolidBrush(BackgroundColor), new Rectangle(image.Width - txtSize.Width - 2, image.Height - txtSize.Height - 2, txtSize.Width + 2, txtSize.Height + 2));
            grph.DrawString(annotation, tfont, tbrush, new PointF(image.Width - 2, image.Height - 2), sf);
        }
#endregion

#region IDisposable implementation
        public void Dispose()
        {
        }
#endregion
    }
}

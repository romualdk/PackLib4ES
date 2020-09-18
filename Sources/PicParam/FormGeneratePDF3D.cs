#region Using directives
using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Sharp3D.Math.Core;

using System.IO;
using System.Threading;
using System.Diagnostics;
#endregion

namespace PicParam
{
    public partial class FormGeneratePDF3D : Form
    {
        #region Constructor
        public FormGeneratePDF3D(DockTreeView form)
        {
            InitializeComponent();
            formTreeView = form;
        }
        #endregion

        #region Properties
        public string OutputFilePath
        {
            set { fileSelectOutput.FileName = value; }
            get { return fileSelectOutput.FileName; }
        }
        #endregion

        #region Load override
        protected override void OnLoad(EventArgs e)
        {
 	        base.OnLoad(e);
            fileSelectPdfTemplate.FileName = Path.Combine( Path.GetDirectoryName(formTreeView.Pic3DExporterPath), "DefaultTemplate.pdf" );
            OnFileNameChanged(this, null);
        }
        #endregion

        #region Event handlers
        private void OnFileNameChanged(object sender, EventArgs e)
        {
            try
            {
                bnGenerate.Enabled =
                    File.Exists(fileSelectPdfTemplate.FileName)
                    && Directory.Exists(Path.GetDirectoryName(fileSelectOutput.FileName));
            }
            catch (Exception)
            { 
            }
        }
        void OutputHandler(object sender, DataReceivedEventArgs e)
        {
            Trace.WriteLine(e.Data);
            BeginInvoke(new MethodInvoker(() =>
            {
                richTextBoxOutput.AppendText(e.Data + Environment.NewLine);

            }));
        }
        private void OnGenerate(object sender, EventArgs e)
        {
            try
            {
                richTextBoxOutput.Clear();

                if (null == formTreeView) return;
                string pdfFile = fileSelectOutput.FileName;
                string desFile = Path.ChangeExtension(pdfFile, "des");
                string des3File = Path.ChangeExtension(pdfFile, "des3");
                string xmlFile = Path.ChangeExtension(pdfFile, "xml");
                string xmlResultFile = Path.ChangeExtension(pdfFile, "result.xml");
                string u3dFile = Path.ChangeExtension(pdfFile, "u3d");
                string defaultPdfTemplate = fileSelectPdfTemplate.FileName;

                // generate des file
                formTreeView.Export(desFile);
                // #### job file
                Pic3DExporter.Job job = new Pic3DExporter.Job();
                // **** FILES BEGIN ****
                job.Pathes.Add(new Pic3DExporter.PathItem() { pathID = "FID-1", path = defaultPdfTemplate, type = Pic3DExporter.pathType.FILE });
                job.Pathes.Add(new Pic3DExporter.PathItem() { pathID = "FID-2", path = desFile, type = Pic3DExporter.pathType.FILE });
                job.Pathes.Add(new Pic3DExporter.PathItem() { pathID = "FID-3", path = des3File, type = Pic3DExporter.pathType.FILE });
                job.Pathes.Add(new Pic3DExporter.PathItem() { pathID = "FID-4", path = u3dFile, type = Pic3DExporter.pathType.FILE });
                job.Pathes.Add(new Pic3DExporter.PathItem() { pathID = "FID-5", path = pdfFile, type = Pic3DExporter.pathType.FILE });

                int fid = 5;
                foreach (string filePath in filePathes)
                {
                    job.Pathes.Add(new Pic3DExporter.PathItem() { pathID = string.Format("FID-{0}", ++fid), path = filePath, type = Pic3DExporter.pathType.FILE });
                }
                // **** FILES END ****
                // **** TASKS BEGIN ****
                // DES -> DES3
                Pic3DExporter.task_2D_TO_DES3 task_2D_to_DES3 = new Pic3DExporter.task_2D_TO_DES3() { id = "TID-1" };
                task_2D_to_DES3.Inputs.Add(new Pic3DExporter.PathRef() { pathID = "FID-2", role = "input des", deleteAfterUsing = true });
                task_2D_to_DES3.Outputs.Add(new Pic3DExporter.PathRef() { pathID = "FID-3", role = "output des3", deleteAfterUsing = false });
                task_2D_to_DES3.autoparameters.thicknessSpecified = true;
                task_2D_to_DES3.autoparameters.thickness = (float)thickness;
                task_2D_to_DES3.autoparameters.foldPositionSpecified = true;
                task_2D_to_DES3.autoparameters.foldPosition = (float)0.5;
                task_2D_to_DES3.autoparameters.pointRef.Add((float)v.X);
                task_2D_to_DES3.autoparameters.pointRef.Add((float)v.Y);
                fid = 5;
                foreach (string filePath in filePathes)
                {
                    task_2D_to_DES3.autoparameters.modelFiles.Add(new Pic3DExporter.PathRef() { pathID = string.Format("FID-{0}", ++fid), role = "model files", deleteAfterUsing = false });
                }
                job.Tasks.Add(task_2D_to_DES3);
                // DES3 -> U3D
                Pic3DExporter.task_DES3_TO_U3D task_DES3_to_U3D = new Pic3DExporter.task_DES3_TO_U3D() { id = "TID-2" };
                task_DES3_to_U3D.Dependencies = "TID-1";
                task_DES3_to_U3D.Inputs.Add(new Pic3DExporter.PathRef() { pathID = "FID-3", role = "input des3", deleteAfterUsing = true });
                task_DES3_to_U3D.Outputs.Add(new Pic3DExporter.PathRef() { pathID = "FID-4", role = "output u3d", deleteAfterUsing = false });
                task_DES3_to_U3D.Parameters.Material.opacitySpecified = true;
                task_DES3_to_U3D.Parameters.Material.opacity = 1.0F;
                task_DES3_to_U3D.Parameters.Material.reflectivitySpecified = true;
                task_DES3_to_U3D.Parameters.Material.reflectivity = 0.0F;
                task_DES3_to_U3D.Parameters.Qualities.meshDefaultSpecified = true;
                task_DES3_to_U3D.Parameters.Qualities.meshDefault = 1000;
                task_DES3_to_U3D.Parameters.Qualities.meshPositionSpecified = true;
                task_DES3_to_U3D.Parameters.Qualities.meshPosition = 1000;
                task_DES3_to_U3D.Parameters.Qualities.shaderQualitySpecified = true;
                task_DES3_to_U3D.Parameters.Qualities.shaderQuality = 1000;
                job.Tasks.Add(task_DES3_to_U3D);
                // U3D -> PDF
                float[] mat = { -0.768655F, -0.632503F, 0.0954455F, -0.220844F, 0.402444F, 0.888407F, -0.600332F, 0.661799F, -0.449025F, 1805.8F, -1990.7F, 1350.67F };
                List<float> viewMatrix = new List<float>(mat);
                float[] bColor = { 1.0f, 1.0f, 1.0f };
                List<float> backColor = new List<float>(bColor);

                Pic3DExporter.task_U3D_TO_PDF task_U3D_to_PDF = new Pic3DExporter.task_U3D_TO_PDF() { id = "TID-3" };
                task_U3D_to_PDF.Dependencies = "TID-2";
                task_U3D_to_PDF.Inputs.Add(new Pic3DExporter.PathRef() { pathID = "FID-4", role = "input u3d", deleteAfterUsing = true });
                task_U3D_to_PDF.Inputs.Add(new Pic3DExporter.PathRef() { pathID = "FID-1", role = "pdf template", deleteAfterUsing = false });
                task_U3D_to_PDF.Outputs.Add(new Pic3DExporter.PathRef() { pathID = "FID-5", role = "output pdf", deleteAfterUsing = false });
                task_U3D_to_PDF.pdfAnnotation.PageLayout.buttonPositionsSpecified = true;
                task_U3D_to_PDF.pdfAnnotation.PageLayout.buttonPositions = Pic3DExporter.RelativePosition.LEFT;
                task_U3D_to_PDF.pdfAnnotation.PageLayout.pageNumberSpecified = true;
                task_U3D_to_PDF.pdfAnnotation.PageLayout.pageNumber = 1;
                task_U3D_to_PDF.pdfAnnotation.PageLayout.position.Add(40);
                task_U3D_to_PDF.pdfAnnotation.PageLayout.position.Add(40);
                task_U3D_to_PDF.pdfAnnotation.PageLayout.dimensions.Add(760);
                task_U3D_to_PDF.pdfAnnotation.PageLayout.dimensions.Add(500);
                task_U3D_to_PDF.pdfAnnotation.ViewNodes.Add(new Pic3DExporter.viewNode() { name = "View_Step0", matrix = viewMatrix, backgroundColor = backColor, COSpecified = true, CO = 3000.0f, lightingScheme = "CAD" });
                job.Tasks.Add(task_U3D_to_PDF);
                // 
                // open PDF
                Pic3DExporter.task_OPEN_PDF_ADOBEREADER task_OpenPDF = new Pic3DExporter.task_OPEN_PDF_ADOBEREADER() { id = "TID-4" };
                task_OpenPDF.Dependencies = "TID-3";
                task_OpenPDF.Inputs.Add(new Pic3DExporter.PathRef() { pathID = "FID-5", role = "input pdf", deleteAfterUsing = false });
                job.Tasks.Add(task_OpenPDF);
                // **** TASKS END ****
                job.SaveToFile(xmlFile);

                // #### execute Pic3DExporter
                string exePath = formTreeView.Pic3DExporterPath;
                if (!File.Exists(exePath))
                {
                    MessageBox.Show(string.Format("File {0} could not be found!", exePath));
                    return;
                }

                var procExporter = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = exePath,
                        Arguments = string.Format(" -t \"{0}\"", xmlFile),
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = false                        
                    }
                };
                procExporter.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
                procExporter.Start();
                procExporter.BeginOutputReadLine();
                while (!procExporter.HasExited)
                    Application.DoEvents();
                Thread.Sleep(1000);
                // delete xml task file
                try
                {
                    if (!Properties.Settings.Default.ShowLogConsole)
                    {
                        File.Delete(xmlFile);
                        File.Delete(xmlResultFile);
                    }
                }
                catch (Exception) { }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Data members
        public List<string> filePathes = new List<string>();
        public double thickness = 0.0;
        public Vector2D v = Vector2D.Zero;
        private DockTreeView formTreeView;
        #endregion
    }
}

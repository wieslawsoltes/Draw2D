using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Draw2D.Presenters;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Tools;
using SkiaSharp;

namespace Draw2D.Export
{
    public class SkiaSvgConverter : ISvgConverter
    {
        internal static string FormatXml(string xml)
        {
            var sb = new StringBuilder();
            var element = XElement.Parse(xml);
            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = false;
            settings.Indent = true;
            settings.NewLineOnAttributes = false;

            using (var writer = XmlWriter.Create(sb, settings))
            {
                element.Save(writer);
            }

            return sb.ToString();
        }

        public string ConvertToSvgDocument(IToolContext context, IContainerView containerView)
        {
            using var stream = new MemoryStream();
            using (var wstream = new SKManagedWStream(stream))
            using (var writer = new SKXmlStreamWriter(wstream))
            using (var canvas = SKSvgCanvas.Create(SKRect.Create(0, 0, (int)containerView.Width, (int)containerView.Height), writer))
            {
                if (containerView.SelectionState?.Shapes?.Count > 0)
                {
                    using var skiaSelectedPresenter = new SkiaExportSelectedPresenter(context, containerView);
                    skiaSelectedPresenter.Draw(canvas, containerView.Width, containerView.Height, 0, 0, 1.0, 1.0);
                }
                else
                {
                    using var skiaContainerPresenter = new SkiaExportContainerPresenter(context, containerView);
                    skiaContainerPresenter.Draw(canvas, containerView.Width, containerView.Height, 0, 0, 1.0, 1.0);
                }
            }

            stream.Position = 0;
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var xml = reader.ReadToEnd();
            return FormatXml(xml);
        }
    }
}

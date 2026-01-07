using ComplaignManagementSystem.Data.Models;
using RazorLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.Business.EmailHandler
{
    public class EmailTemplateRenderer : IEmailTemplateRenderer
    {
        private readonly RazorLightEngine _engine;

        public EmailTemplateRenderer()
        {
            try
            {
                // Use assembly of the layer where templates exist
                var dataLayerAssembly = typeof(ComplaintMaster).Assembly; // Pick any class from Data Layer
                var templateRoot = Path.Combine(Path.GetDirectoryName(dataLayerAssembly.Location)!, "EmailTemplates");

                if (!Directory.Exists(templateRoot))
                {
                    throw new DirectoryNotFoundException($"EmailTemplates folder not found at: {templateRoot}");
                }

                _engine = new RazorLightEngineBuilder()
                    .UseFileSystemProject(templateRoot)
                    .UseMemoryCachingProvider()
                    .EnableDebugMode()
                    .Build();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to initialize RazorLight engine", ex);
            }
        }

        public async Task<string> RenderAsync(string templateName, object model)
        {

            try
            {
                return await _engine.CompileRenderAsync($"{templateName}.cshtml", model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}

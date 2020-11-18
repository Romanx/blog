using Statiq.App;
using Statiq.Common;
using Statiq.Core;

namespace Blog
{
    /// <summary>
    /// Extensions for the pipeline collections to make it easy to integrate into common pipelines
    /// </summary>
    public static class PipelineExtensions
    {
        /// <summary>
        /// A helper for adding reading time meta data to blog post pipelines.
        /// </summary>
        /// <param name="pipelineCollection"></param>
        /// <param name="wordsPerMinute"></param>
        /// <returns></returns>
        public static Bootstrapper AddReadingTimeMeta(this Bootstrapper bootstrapper)
        {
            return bootstrapper.ModifyPipeline(nameof(Statiq.Web.Pipelines.Inputs), pipeline =>
            {
                var ifMod = pipeline.ProcessModules[3] as ExecuteIf;
                var res = ifMod[0].InsertBeforeFirst<SetMetadata>(new ExecuteIf(Config.FromDocument(doc => doc.MediaTypeEquals(MediaTypes.Markdown)))
                {
                    new ReadingTimeModule()
                });
            });
        }
    }
}
using Statiq.Common;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Blog
{
    public class ReadingTimeModule : ParallelConfigModule<uint>
    {
        private static readonly Regex SpacesRegex = new(@"\s+", RegexOptions.Compiled | RegexOptions.Multiline);

        public ReadingTimeModule() : base(Config.FromSetting<uint>(ReadingTimeKeys.ReadingTimeWordsPerMinute), true)
        {
        }

        protected override async Task<IEnumerable<IDocument>> ExecuteConfigAsync(IDocument input, IExecutionContext context, uint wordsPerMinute)
        {
            var content = await input.GetContentStringAsync();
            var wordsPerSecond = wordsPerMinute / 60;

            return new[]
            {
                input.Clone(GetMetaData(content, wordsPerSecond)),
            };
        }

        private static Dictionary<string, object> GetMetaData(string content, uint wordsPerSecond)
        {
            uint words = (uint)SpacesRegex.Matches(content.Trim()).Count;

            var totalReadingTimeSeconds = words / wordsPerSecond;
            var minutes = (uint)Math.Floor(totalReadingTimeSeconds / 60m);
            var seconds = (uint)Math.Round(totalReadingTimeSeconds - (minutes * 60m));

            return new Dictionary<string, object> {
                { ReadingTimeKeys.ReadingTime, new ReadingTimeMeta(minutes, seconds, totalReadingTimeSeconds, words) }
            };
        }
    }
}
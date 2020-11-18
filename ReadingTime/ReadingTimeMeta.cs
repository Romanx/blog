namespace Blog
{
    /// <summary>
    /// The reading time meta for a document
    /// </summary>
    public class ReadingTimeMeta
    {
        /// <summary>
        /// How long to read the document in minutes
        /// </summary>
        public uint Minutes { get; }

        /// <summary>
        /// How long to read the document in seconds
        /// </summary>
        public uint Seconds { get; }

        /// <summary>
        /// How long in total for seconds
        /// </summary>
        public uint TotalInSeconds { get; }

        /// <summary>
        /// How many words in the document
        /// </summary>
        public uint Words { get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        /// <param name="totalInSeconds"></param>
        /// <param name="words"></param>
        public ReadingTimeMeta(uint minutes, uint seconds, uint totalInSeconds, uint words)
        {
            Minutes = minutes;
            Seconds = seconds;
            TotalInSeconds = totalInSeconds;
            Words = words;
        }
    }
}
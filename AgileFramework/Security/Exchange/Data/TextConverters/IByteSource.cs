namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    /// <summary>
    /// Interface definition for Byte Source.
    /// </summary>
    internal interface IByteSource
    {
        bool GetOutputChunk(out byte[] chunkBuffer, out int chunkOffset, out int chunkLength);

        void ReportOutput(int readCount);
    }
}

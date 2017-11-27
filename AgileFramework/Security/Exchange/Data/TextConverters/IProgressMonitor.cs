namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    /// <summary>
    /// Interface for classes which can report progress.
    /// </summary>
    internal interface IProgressMonitor
    {
        /// <summary>
        /// Report the progress of the current operation.
        /// </summary>
        void ReportProgress();
    }
}

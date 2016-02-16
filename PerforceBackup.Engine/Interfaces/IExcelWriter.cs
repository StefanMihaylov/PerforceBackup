namespace PerforceBackup.Engine.Interfaces
{
    using PerforceBackup.Engine.Models;

    public interface IExcelWriter : IService
    {
        string FullFileRoot { get; }

        void AddRow(CheckPointLogModel logData);

        void ConvertToPdf();

        void OpenExcel();

        void OpenPdf();        
    }
}

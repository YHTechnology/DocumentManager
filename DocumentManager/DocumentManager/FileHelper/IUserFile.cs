using System.Windows.Threading;
using System.ComponentModel;
using System.IO;
namespace FileHelper
{
    public static class Enums
    {
        public enum FileStates
        {
            Pending = 0,
            Uploading = 1,
            Finished = 2,
            Deleted = 3,
            Error = 4
        }
    }

    public interface IUserFile
    {
        string FileName { get; set; }
        Enums.FileStates State { get; set; }
        string StateString { get; }

        double FileSize { get; }
        Stream FileStream { get; set; }

        double BytesUploaded { get; set; }
        double BytesUploadedFinished { get; set; }

        float Percentage { get; set; }
        float PercentageFinished { get; set; }

        string ErrorMessage { get; set; }

        void Upload(string initParams, Dispatcher uiDispatcher);
        void CancelUpload();

        event PropertyChangedEventHandler PropertyChanged;
    }
}
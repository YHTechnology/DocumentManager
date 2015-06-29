using System;

namespace FileHelper
{
    public interface IFileUploader
    {
        void StartUpload(string initParams);
        void CancelUpload();

        event EventHandler UploadFinished;
    }
}
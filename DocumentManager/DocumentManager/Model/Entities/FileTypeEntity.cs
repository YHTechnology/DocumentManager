using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.DataAnnotations;

namespace DocumentManager.Model.Entities
{
    public class FileTypeEntity : NotifyPropertyChanged
    {
        private int fileTypeId;
        private string fileTypeName;

        public int FileTypeId
        {
            get { return fileTypeId; }
            set { if (fileTypeId != value) { fileTypeId = value; UpdateChanged("FileTypeId"); } }
        }

        [Required(ErrorMessage = "档案类型名不能为空")]
        public string FileTypeName
        {
            get { return fileTypeName; }
            set { if (fileTypeName != value) { fileTypeName = value; UpdateChanged("FileTypeName"); } }
        }


        public void Update()
        {
            this.FileTypeId = FileType.file_type_id;
            this.FileTypeName = FileType.file_type_name;
        }

        public void DUpdate()
        {
            FileType.file_type_id = FileTypeId;
            FileType.file_type_name = FileTypeName;
        }

        public void RaisALL()
        {
            UpdateChanged("FileTypeId");
            UpdateChanged("FileTypeName");
        }

        public string ToString()
        {
            return string.Format("FileTypeId:{0},FileTypeName:{1}"
                , FileTypeId, FileTypeName);
        }

        public DocumentManager.Web.Model.filetype FileType { get; set; }
    }
}

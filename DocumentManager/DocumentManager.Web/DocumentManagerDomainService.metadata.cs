
namespace DocumentManager.Web.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;


    // The MetadataTypeAttribute identifies actionMetadata as the class
    // that carries additional metadata for the action class.
    [MetadataTypeAttribute(typeof(action.actionMetadata))]
    public partial class action
    {

        // This class allows you to attach custom attributes to properties
        // of the action class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class actionMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private actionMetadata()
            {
            }

            public int action_id { get; set; }

            public string action_name { get; set; }

            public Nullable<int> supper_action_id { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies filetypeMetadata as the class
    // that carries additional metadata for the filetype class.
    [MetadataTypeAttribute(typeof(filetype.filetypeMetadata))]
    public partial class filetype
    {

        // This class allows you to attach custom attributes to properties
        // of the filetype class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class filetypeMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private filetypeMetadata()
            {
            }

            public int file_type_id { get; set; }

            public string file_type_name { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies systemlogMetadata as the class
    // that carries additional metadata for the systemlog class.
    [MetadataTypeAttribute(typeof(systemlog.systemlogMetadata))]
    public partial class systemlog
    {

        // This class allows you to attach custom attributes to properties
        // of the systemlog class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class systemlogMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private systemlogMetadata()
            {
            }

            public string system_log { get; set; }

            public int system_log_id { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies taxpayerMetadata as the class
    // that carries additional metadata for the taxpayer class.
    [MetadataTypeAttribute(typeof(taxpayer.taxpayerMetadata))]
    public partial class taxpayer
    {

        // This class allows you to attach custom attributes to properties
        // of the taxpayer class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class taxpayerMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private taxpayerMetadata()
            {
            }

            public string taxpayer_code { get; set; }

            public int taxpayer_id { get; set; }

            public string taxpayer_name { get; set; }

            public Nullable<int> taxpayer_type_id { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies taxpayerdocumentMetadata as the class
    // that carries additional metadata for the taxpayerdocument class.
    [MetadataTypeAttribute(typeof(taxpayerdocument.taxpayerdocumentMetadata))]
    public partial class taxpayerdocument
    {

        // This class allows you to attach custom attributes to properties
        // of the taxpayerdocument class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class taxpayerdocumentMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private taxpayerdocumentMetadata()
            {
            }

            public Nullable<long> taxpayer_document_bytes { get; set; }

            public string taxpayer_document_descript { get; set; }

            public int taxpayer_document_id { get; set; }

            public string taxpayer_document_name { get; set; }

            public Nullable<int> taxpayer_document_type_id { get; set; }

            public Nullable<int> taxpayer_id { get; set; }

            public Nullable<DateTime> taxpayer_update_time { get; set; }

            public Nullable<int> taxpayer_update_user_id { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies taxpayertypeMetadata as the class
    // that carries additional metadata for the taxpayertype class.
    [MetadataTypeAttribute(typeof(taxpayertype.taxpayertypeMetadata))]
    public partial class taxpayertype
    {

        // This class allows you to attach custom attributes to properties
        // of the taxpayertype class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class taxpayertypeMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private taxpayertypeMetadata()
            {
            }

            public int taxpayertype_id { get; set; }

            public string taxpayertype_name { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies userMetadata as the class
    // that carries additional metadata for the user class.
    [MetadataTypeAttribute(typeof(user.userMetadata))]
    public partial class user
    {

        // This class allows you to attach custom attributes to properties
        // of the user class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class userMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private userMetadata()
            {
            }

            public string user_cname { get; set; }

            public int user_id { get; set; }

            public string user_name { get; set; }

            public string user_password { get; set; }
        }
    }
}

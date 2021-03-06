﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Common
{
    [XmlRoot("FtpAcknowledgementMessage")]
    public class AckMessage
    {
        [XmlElement("FileStatus")]
        public string FileStatus { get; set; }
        [XmlElement("ErrorText")]
        public string ErrorText { get; set; }
        [XmlElement("AffectedResource")]
        public AffectedResource[] AffectedResources { get; set; }
    }

    public class AffectedResource
    {
        [XmlElement("ISRC")]
        public string ISRC { get; set; }
        [XmlElement("ProprietaryId")]
        public ProprietaryId[] Properties { get; set; }

        [XmlIgnore]
        public string Reference
        {
            get
            {
                var referenceProperty = Properties.Where(p => p.Namespace == "YOUTUBE:REFERENCE_ID").FirstOrDefault();
                if (referenceProperty != null)
                    return referenceProperty.Text;
                return null;
            }
        }
    }

    public class ProprietaryId
    {
        [XmlAttribute("Namespace")]
        public string Namespace { get; set; }
        [XmlText]
        public string Text { get; set; }
    }
}

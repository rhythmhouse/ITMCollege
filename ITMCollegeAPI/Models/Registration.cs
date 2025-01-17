﻿using System;
using System.Collections.Generic;

#nullable disable

namespace ITMCollegeAPI.Models
{
    public partial class Registration
    {
        public long RegistrationId { get; set; }
        public string RegNum { get; set; }
        public string Image { get; set; }
        public int? SpeSubjectId { get; set; }
        public int? OpSubjectId { get; set; }
        public string EmergencyName { get; set; }
        public string EmergencyAddress { get; set; }
        public string EmergencyPhone { get; set; }
    }
}

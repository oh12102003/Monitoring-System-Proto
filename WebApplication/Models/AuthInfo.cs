﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class AuthInfo
    {
        public string userName { get; set; }
        public string authGrade { get; set; }
        public string userId { get; set; }

        public string monitoring { get; set; }
        public string recipe { get; set; }
        public string management { get; set; }
    }
}
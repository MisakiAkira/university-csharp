﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    internal class JsonResult
    {
        public DateTime CreatedAt { get; set; }

        public string Author { get; set; }

        public HashSet<Student> Students { get; set; }
    }
}

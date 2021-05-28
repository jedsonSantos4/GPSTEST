﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Entities
{
    public class ValidResult<T> where T : new ()
        
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public T Value { get; set; }
    }
}

﻿using System.Collections.Generic;
using System.Runtime.InteropServices.ObjectiveC;

namespace CRMApp.Common
{
    public class JsonTemplate<T>
    {
        
        public string? StatusCode { get; set; }
        public string? Msg { get; set; }
        public T Content { get; set; }
    }
}

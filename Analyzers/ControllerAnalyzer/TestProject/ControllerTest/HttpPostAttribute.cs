﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerTest
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class HttpPostAttribute:Attribute
    {
    }
}

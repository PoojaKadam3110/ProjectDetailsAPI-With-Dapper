﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ILoggerService
    {
        void LogInformation(string message);
        void LogError(string message);
    }

}

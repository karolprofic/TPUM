using System;
using System.Collections.Generic;
using System.Text;
using Data.Interfaces;

namespace Data
{
    public abstract class DataAbstractApi
    {
        public static DataAbstractApi Create()
        {
            return new DataApi();
        }

        public abstract IElection GetElection();
    }
}

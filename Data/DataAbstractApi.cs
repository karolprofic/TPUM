using System;
using System.Collections.Generic;
using System.Text;
using Data.Interfaces;

namespace Data
{
    public abstract class DataAbstractAPI
    {
        public static DataAbstractAPI Create()
        {
            return new DataAPI();
        }

        public abstract IElection GetElection();
    }
}

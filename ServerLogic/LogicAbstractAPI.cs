using System;
using ServerData;
using ServerLogic.Interfaces;

namespace ServerLogic
{
    public abstract class LogicAbstractAPI
    {
        public DataAbstractAPI DataAbstractApi { get; private set; }

        public LogicAbstractAPI(DataAbstractAPI dataAbstractApi)
        {
            DataAbstractApi = dataAbstractApi;
        }

        public static LogicAbstractAPI Create(DataAbstractAPI dataApi = null)
        {
            if (dataApi == null)
            {
                return new LogicAPI(DataAbstractAPI.Create());
            }
            return new LogicAPI(dataApi);
        }

        public abstract IElectionSystem GetElectionSystem();
    }

}

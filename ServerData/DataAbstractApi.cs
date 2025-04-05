using ServerData.Interfaces;

namespace ServerData
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

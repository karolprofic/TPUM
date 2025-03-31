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

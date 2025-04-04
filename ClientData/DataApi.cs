using Data.Interfaces;

namespace Data
{
    internal class DataAPI : DataAbstractAPI
    {
        private readonly Election election;

        public DataAPI()
        {
            election = new Election();
        }

        public override IElection GetElection()
        {
            return election;
        }

    }
}
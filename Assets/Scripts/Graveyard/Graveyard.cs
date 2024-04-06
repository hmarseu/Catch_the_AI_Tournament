using System.Collections.Generic;
using YokaiNoMori.Interface;


namespace YokaiNoMori.General
{
    public class Graveyard
    {
        private List<IPawn> m_pawns;

        public List<IPawn> Pawns
        {
            get { return m_pawns; }
            private set { m_pawns = value; }
        }


        public void Init()
        {
            Pawns = new List<IPawn>();
        }


        public void AddToGraveyard(IPawn parachutableObject)
        {
            Pawns.Add(parachutableObject);
        }

        public void RemoveToGraveyard(IPawn parachutableObject)
        {
            Pawns.Remove(parachutableObject);
        }
    }
}

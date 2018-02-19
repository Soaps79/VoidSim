using System;
using Assets.WorldMaterials.Population;

namespace Assets.Placeables.Nodes
{
    public class Occupancy
    {
        private Person _person;
        public Person Person
        {
            get { return _person; }
            set
            {
                if (value == _person)
                    return;

                _person = value;
                CheckUpdate();
            }
        }

        private bool _isReserved;
        public bool IsReserved
        {
            get { return _isReserved; }
            set
            {
                if (value == _isReserved)
                    return;

                _isReserved = value;
                CheckUpdate();
            }
        }

        private void CheckUpdate()
        {
            if (OnUpdate != null)
                OnUpdate(this);
        }

        public bool IsOccupied { get { return _person != null; } }

        public Action<Occupancy> OnUpdate;
    }
}
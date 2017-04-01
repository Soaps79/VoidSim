using Assets.Scripts.WorldMaterials;

namespace Assets.Station
{
    // extremely placeholder, need to explore the placeable system as a whole
    public class Placeable : IEnergyConsumer
    {
        private Product _baseProduct;
        private int _energyConsumption;

        // extract energy info to child class?
        public EnergyConsumerNode EnergyConsumerNode { get; private set; }
        private void InitializeEnergyConsumer()
        {
            EnergyConsumerNode = new EnergyConsumerNode();
            EnergyConsumerNode.AmountConsumed = _energyConsumption;
        }

        public Placeable(int energyConsumption)
        {
            _energyConsumption = energyConsumption;
            InitializeEnergyConsumer();
        }
    }
}
namespace VoidSim.Console.Products
{
    // This object is essentially a service to the rest of the game whose role is to connect consumers with providers. 
    // The goods do not actually pass through here, Providers simply agree to send a specified amount to the Consumer. 
    // If this amount does not fully meet the requested amount, that amount will be lessened by the contracted amount, 
    // and the Hub will continue to try and find a Provider for the remainder.
    public class ProductTradingHub
    {
        
    }

    // An actor in the trading system
    // This object should eventually have information about 
    // location, diplomatic faction, whatever the hub can use to decide if its a fit
    // OR, does the hub ask the trader if he will willingly trade with the other?
    public class ProductTrader
    {
        public string Name;
    }

    // One trade consideration could be Distance. Distance can work such that 
    // if the distance is too far, the traders can buy fuel along the way (from us!)
    // Good/easy early game income, maybe a small production chain to produce it
    // Great material for a tutorial (you explored a planet nearby that has X! 
    // You are already producing Y in your base, build a factory to combine them to make fuel.)


    // ProductTraders will submit these requests to the hub when they either 
    // need goods (Consume), or they have excess good they want to trade (Provide)
    public enum ProductTransferAction
    {
        Provide, Consume
    }

    public class ProductRequest
    {
        public ProductTransferAction Action;
        public string ProductName;
        public int Amount;
    }
}
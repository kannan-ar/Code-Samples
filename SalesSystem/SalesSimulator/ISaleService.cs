namespace SalesSimulator
{
    using System.Threading.Tasks;
    
    using SalesSimulator.Models;

    public interface ISaleService
    {
        Task Buy(Purchase purchase);
        ModeOfPayment GetModeOfPayment();
    }
}
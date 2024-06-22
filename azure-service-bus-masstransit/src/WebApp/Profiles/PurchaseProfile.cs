using AutoMapper;
using Messaging.Lib.QueueMessages;
using WebApp.Models;

namespace WebApp.Profiles
{
    public class PurchaseProfile : Profile
    {
        public PurchaseProfile()
        {
            CreateMap<PurchaseOrder, PurchaseCreated>();
        }
    }
}

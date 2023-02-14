using Azure.Data.Tables;
using Functions.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functions.Services
{
    public interface ITableService
    {
        Task Insert<T>(string tableName, T entity) where T : AzureTableEntity;
    }
}

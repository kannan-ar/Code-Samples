using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functions.Modals
{
    public class Entity : AzureTableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override void SetPartitionAndRowKeys()
        {
            RowKey = Id.ToString();
            PartitionKey = Name;
        }
    }
}

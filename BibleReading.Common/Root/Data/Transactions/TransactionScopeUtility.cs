using System;
using System.Transactions;

using BibleReading.Common45.Properties;

namespace BibleReading.Common45.Root.Data.Transactions
{    
    public class TransactionScopeUtility
    {
        public static TransactionScope GetTransactionScope()
        {
            var options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadCommitted;
            options.Timeout = new TimeSpan(0, 0, Settings.Default.TransactionTimeout);

            return new TransactionScope(TransactionScopeOption.Required, options);
        }
    }
}

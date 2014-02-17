namespace SharpArch.RavenDb
{
    using System;
    using System.Transactions;

    using Raven.Client;

    using SharpArch.Domain.PersistenceSupport;

    public class TransactionManager : ITransactionManager
    {
        private readonly IDocumentSession session;

        private TransactionScope transaction;

        public TransactionManager(IDocumentSession session)
        {
            this.session = session;
        }

        public IDisposable BeginTransaction(string isolationLevel)
        {
            IsolationLevel transactionIsolationLevel;

            if (!IsolationLevel.TryParse(isolationLevel, false, out transactionIsolationLevel))
            {
                throw new ArgumentException(
                    string.Format("{0} is not a valid System.Transactions.IsolationLevel value", isolationLevel),
                    "isolationLevel");
            }

            var transactionOptions = new TransactionOptions { IsolationLevel = transactionIsolationLevel };

            return this.transaction ?? (this.transaction = new TransactionScope(TransactionScopeOption.Required, transactionOptions));
        }

        public void CommitTransaction()
        {
            this.session.SaveChanges();
            this.transaction.Complete();
            this.ClearTransaction();
        }

        public void RollbackTransaction()
        {
            this.ClearTransaction();
        }

        private void ClearTransaction()
        {
            this.transaction.Dispose();
            this.transaction = null;
        }
    }
}
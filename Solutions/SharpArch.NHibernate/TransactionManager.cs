using System.Data;

namespace SharpArch.NHibernate
{
    using System;

    using Domain;
    using Domain.PersistenceSupport;
    using global::NHibernate;

    /// <summary>
    /// Provides methods for managing NHibernate ADO.Net transactions, Beginning, Commiting, Rolling back.
    /// </summary>
    /// <remarks>
    /// Note that you shouldn't have to invoke this object very often.
    /// If you're using on of the the <c>TransactionAttribute</c> atrributes
    /// provided by SharpArch on your controller actions, then the transaction
    /// opening/committing will be taken care of for you.
    /// </remarks>
    public class TransactionManager : ITransactionManager
    {
        public TransactionManager(string factoryKey)
        {
            Check.Require(!string.IsNullOrEmpty(factoryKey), "factoryKey may not be null or empty");

            this.FactoryKey = factoryKey;
        }

        public string FactoryKey { get; set; }

        private ISession Session
        {
            get
            {
                return NHibernateSession.CurrentFor(this.FactoryKey);
            }
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns>
        /// The transaction instance.
        /// </returns>
        /// <exception cref="System.ArgumentException">isolationLevel</exception>
        public IDisposable BeginTransaction(string isolationLevel)
        {
            IsolationLevel transactionIsolationLevel;

            if (!IsolationLevel.TryParse(isolationLevel, false, out transactionIsolationLevel))
            {
                throw new ArgumentException(
                    string.Format("{0} is not a valid System.Data.IsolationLevel value", isolationLevel),
                    "isolationLevel");
            }

            return this.Session.BeginTransaction(transactionIsolationLevel);
        }

        /// <summary>
        /// Commits the transaction, saving all changes.
        /// </summary>
        public void CommitTransaction()
        {
            if (this.Session.Transaction != null && this.Session.Transaction.IsActive)
            {
                this.Session.Transaction.Commit();
            }
        }

        /// <summary>
        /// Rolls the transaction back, discarding any changes.
        /// </summary>
        public void RollbackTransaction()
        {
            if (this.Session.Transaction != null && this.Session.Transaction.IsActive)
            {
                this.Session.Transaction.Rollback();
            }
        }
    }
}
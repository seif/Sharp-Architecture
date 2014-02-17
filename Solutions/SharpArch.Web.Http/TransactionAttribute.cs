using System.Data;

namespace SharpArch.Web.Http
{
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    using SharpArch.Domain;
    using SharpArch.Domain.PersistenceSupport;

    /// <summary>
    /// An attribute that implies a transaction.
    /// </summary>
    public class TransactionAttribute : ActionFilterAttribute
    {
        string _isolationLevel = "Unspecified";

        /// <summary>
        /// Gets or sets the database context
        /// The value should be injected by the filter provider.
        /// </summary>
        public ITransactionManager TransactionManager { get; set; }

        /// <summary>
        /// Gets or sets the Isolation level to be used when creating the transaction.
        /// Default is IslocationLevel.Unspecified.
        /// </summary>
        public string IsolationLevel
        {
            get { return _isolationLevel; }
            set { _isolationLevel = value; }
        }

        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            Check.Require(this.TransactionManager != null, "TransactionManager was null, register an implementation of TransactionManager in the IoC container.");

            base.OnActionExecuting(actionContext);

            this.TransactionManager.BeginTransaction(this.IsolationLevel);
        }

        /// <summary>
        /// Occurs after the action method is invoked.
        /// </summary>
        /// <param name="actionExecutedContext">The action executed context.</param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);

            if (actionExecutedContext.Exception != null)
            {
                this.TransactionManager.RollbackTransaction();
            }
            else
            {
                this.TransactionManager.CommitTransaction();
            }
        }
    }
}

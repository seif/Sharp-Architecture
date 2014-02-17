using System.Data;
using Iesi.Collections;

namespace SharpArch.Web.Mvc
{
    using System.Web.Mvc;

    using SharpArch.Domain;
    using SharpArch.Domain.PersistenceSupport;

    public class TransactionAttribute : ActionFilterAttribute
    {
        string _isolationLevel = "Unspecified";

        /// <summary>
        /// Gets or sets the databse context
        /// The value should be injected by the filter provider.
        /// </summary>
        public ITransactionManager TransactionManager { get; set; }

        /// <summary>
        /// Gets or sets whether the transaction should rollback if model state is not valid.
        /// </summary>
        public bool RollbackOnModelStateError { get; set; }

        /// <summary>
        /// Gets or sets the Isolation level to be used when creating the transaction.
        /// Default is IslocationLevel.Unspecified.
        /// </summary>
        public string IsolationLevel
        {
            get { return _isolationLevel; }
            set { _isolationLevel = value; }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (((filterContext.Exception != null) && filterContext.ExceptionHandled) ||
                this.ShouldRollback(filterContext))
            {
                this.TransactionManager.RollbackTransaction();
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Check.Require(this.TransactionManager != null, "TransactionManager was null, register an implementation of TransactionManager in the IoC container.");

            this.TransactionManager.BeginTransaction(this.IsolationLevel);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            
            if (((filterContext.Exception != null) && (!filterContext.ExceptionHandled)) ||
                this.ShouldRollback(filterContext))
            {
                this.TransactionManager.RollbackTransaction();
            }
            else
            {
                this.TransactionManager.CommitTransaction();
            }
        }

        private bool ShouldRollback(ControllerContext filterContext)
        {
            return this.RollbackOnModelStateError && !filterContext.Controller.ViewData.ModelState.IsValid;
        }
    }
}
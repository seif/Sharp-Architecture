using System;
using Machine.Specifications;
using Moq;
using NHibernate;
using SharpArch.NHibernate;
using It = Machine.Specifications.It;

namespace SharpArch.Specifications.NHibernate
{
    [Subject(typeof(TransactionManager))]
    public class When_beginning_transaction_passing_invalid_isolation_level : context_with_mocked_session_factory
    {
        Establish context = () => { Subject = new TransactionManager(MockSessionFactoryKey); };

        Because of = () => CaughtException = Catch.Exception(() => Subject.BeginTransaction("SomeUnsupportedValue"));

        It should_throw_an_ArgumentException = () => CaughtException.ShouldBeOfType<ArgumentException>();

        It should_include_the_incorrect_value_in_the_exception_message =()=> CaughtException.Message.ShouldContain("SomeUnsupportedValue");

        protected static Exception CaughtException { get; set; }

        protected static TransactionManager Subject { get; set; }
    }

    [Subject(typeof(TransactionManager))]
    public class When_getting_the_session : context_with_mocked_session_factory
    {
        Establish context = () => { Subject = new TransactionManager(MockSessionFactoryKey); };

        Because of = () => Subject.CommitTransaction();

        It should_get_the_session_for_the_subjects_factory_key = () =>
            mockSessionStorage.Verify(x => x.GetSessionForKey(MockSessionFactoryKey));

        protected static TransactionManager Subject { get; set; }
    }

    [Subject(typeof(TransactionManager))]
    public class When_commiting_an_active_transaction : context_with_mocked_session_factory
    {
        Establish context = () =>
            {
                mockTransaction = new Mock<ITransaction>();
                mockSession.SetupGet(x => x.Transaction).Returns(mockTransaction.Object);
                mockTransaction.SetupGet(x => x.IsActive).Returns(true);
                Subject = new TransactionManager(MockSessionFactoryKey);
            };

        Because of = () => Subject.CommitTransaction();

        It should_check_if_transaction_is_currently_active = () =>
            mockTransaction.VerifyGet(x => x.IsActive);

        It should_commit_the_transaction = () =>
            mockTransaction.Verify(x => x.Commit(), Times.Once);

        protected static TransactionManager Subject { get; set; }
        protected static Mock<ITransaction> mockTransaction;
    }

    [Subject(typeof(TransactionManager))]
    public class When_commiting_an_inactive_transaction : context_with_mocked_session_factory
    {
        Establish context = () =>
        {
            mockTransaction = new Mock<ITransaction>();
            mockSession.SetupGet(x => x.Transaction).Returns(mockTransaction.Object);
            mockTransaction.SetupGet(x => x.IsActive).Returns(false);
            Subject = new TransactionManager(MockSessionFactoryKey);
        };

        Because of = () => Subject.CommitTransaction();

        It should_check_if_transaction_is_currently_active = () =>
            mockTransaction.VerifyGet(x => x.IsActive);

        It should_not_commit_the_transaction = () =>
            mockTransaction.Verify(x => x.Commit(), Times.Never);

        protected static TransactionManager Subject { get; set; }
        protected static Mock<ITransaction> mockTransaction;
    }

    [Subject(typeof(TransactionManager))]
    public class When_rolling_back_an_active_transaction : context_with_mocked_session_factory
    {
        Establish context = () =>
        {
            mockTransaction = new Mock<ITransaction>();
            mockSession.SetupGet(x => x.Transaction).Returns(mockTransaction.Object);
            mockTransaction.SetupGet(x => x.IsActive).Returns(true);
            Subject = new TransactionManager(MockSessionFactoryKey);
        };

        Because of = () => Subject.RollbackTransaction();

        It should_check_if_transaction_is_currently_active = () =>
            mockTransaction.VerifyGet(x => x.IsActive);

        It should_rollback_the_transaction = () =>
            mockTransaction.Verify(x => x.Rollback(), Times.Once);

        protected static TransactionManager Subject { get; set; }
        protected static Mock<ITransaction> mockTransaction;
    }

    [Subject(typeof(TransactionManager))]
    public class When_rolling_back_an_inactive_transaction : context_with_mocked_session_factory
    {
        Establish context = () =>
        {
            mockTransaction = new Mock<ITransaction>();
            mockSession.SetupGet(x => x.Transaction).Returns(mockTransaction.Object);
            mockTransaction.SetupGet(x => x.IsActive).Returns(false);
            Subject = new TransactionManager(MockSessionFactoryKey);
        };

        Because of = () => Subject.RollbackTransaction();

        It should_check_if_transaction_is_currently_active = () =>
            mockTransaction.VerifyGet(x => x.IsActive);

        It should_not_attempt_to_rollback_the_transaction = () =>
            mockTransaction.Verify(x => x.Rollback(), Times.Never);

        protected static TransactionManager Subject { get; set; }
        protected static Mock<ITransaction> mockTransaction;
    }
}
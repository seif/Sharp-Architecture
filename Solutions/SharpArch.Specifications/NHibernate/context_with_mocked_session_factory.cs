using Machine.Specifications;
using Moq;
using NHibernate;
using SharpArch.NHibernate;

namespace SharpArch.Specifications.NHibernate
{
    public class context_with_mocked_session_factory
    {
        private Establish context = () =>
            {
                NHibernateSession.Reset();
                mockSessionStorage = new Mock<ISessionStorage>();
                mockSession = new Mock<ISession>();
                mockSessionFactory = new Mock<ISessionFactory>();

                mockSessionStorage.Setup(x => x.GetSessionForKey(MockSessionFactoryKey)).Returns(mockSession.Object);
                mockSessionFactory.Setup(x => x.OpenSession()).Returns(mockSession.Object);
                mockSessionFactory.Setup(x => x.GetCurrentSession()).Returns(mockSession.Object);
                NHibernateSession.InitStorage(mockSessionStorage.Object);
                NHibernateSession.AddSessionFactory(MockSessionFactoryKey, mockSessionFactory.Object);
            };

        protected const string MockSessionFactoryKey = "mocked_session_factory";
        protected static Mock<ISessionStorage> mockSessionStorage;
        protected static Mock<ISessionFactory> mockSessionFactory;
        protected static Mock<ISession> mockSession;
    }
}
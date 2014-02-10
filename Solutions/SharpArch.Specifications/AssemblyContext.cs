using System.IO;

namespace SharpArch.Specifications
{
    using System;

    using Machine.Specifications;

    using global::SharpArch.Testing.NUnit.NHibernate;

    public class AssemblyContext : IAssemblyContext
    {
        public void OnAssemblyStart()
        {
            RepositoryTestsHelper.InitializeNHibernateSession(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Hibernate.cfg.xml"));
        }

        public void OnAssemblyComplete()
        {
            RepositoryTestsHelper.Shutdown();
        }
    }
}
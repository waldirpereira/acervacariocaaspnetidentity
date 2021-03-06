﻿using AutoMapper;
using Acerva.Modelo.Mapeamento;
using Acerva.Web.Models.CadastroRegionais;
using FluentNHibernate.Cfg;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;

namespace Acerva.Tests.Infra
{
    [TestClass]
    public static class TestBootstrapper
    {
        public static ISessionFactory SessionFactory { get; private set; }

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            XmlConfigurator.Configure();
            SessionFactory = ConfiguraSessionFactoryPersistencia();

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<CadastroRegionalMapperProfile>();
            });
        }

        private static ISessionFactory ConfiguraSessionFactoryPersistencia()
        {
            var sessionFactory = Fluently.Configure()
                .Mappings(
                    m => m.FluentMappings.AddFromAssemblyOf<RegionalClassMap>()
                )
                .BuildSessionFactory();

            return sessionFactory;
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            SessionFactory.Dispose();
            SessionFactory = null;
        }
    }
}
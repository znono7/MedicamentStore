using HashidsNet;
using MedicamentStore.ViewModels.Application;
using Ninject;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MedicamentStore
{
    /// <summary>
    /// The IoC container for our application
    /// </summary>
    public static class IoC
    {
        #region Public Properties

        /// <summary>
        /// The kernel for our IoC container
        /// </summary>
        public static IKernel Kernel { get; private set; } = new StandardKernel();

        /// <summary>
        /// A shortcut to access the <see cref="IUIManager"/>
        /// </summary>
        public static IUIManager UI => IoC.Get<IUIManager>();

        /// <summary>
        /// A shortcut to access the <see cref="IConfirmManager"/>
        /// </summary>
        public static IConfirmManager ConfirmBox => IoC.Get<IConfirmManager>();

        /// <summary>
        /// A shortcut to access the <see cref="IConfirmManager"/>
        /// </summary>
        public static INotificationManager NotificationBox => IoC.Get<INotificationManager>();

        /// <summary>
        /// A shortcut to access the <see cref="ApplicationViewModel"/>
        /// </summary>
        public static ApplicationViewModel Application => IoC.Get<ApplicationViewModel>();
       // public static StockHostViewModel Stocks => IoC.Get<StockHostViewModel>();


        /// <summary>
        /// A shortcut to access toe <see cref="IUserRepository"/> service
        /// </summary>
        public static IUserRepository UserManager => IoC.Get<IUserRepository>();
        public static IClientRepository ClientManager => IoC.Get<IClientRepository>();
        public static IMedicamentRepository MedicamentManager => IoC.Get<IMedicamentRepository>();

        public static IAuthenticationRepository UserAuth => IoC.Get<IAuthenticationRepository>();
        public static IStockRepository StockManager => IoC.Get<IStockRepository>();
        public static ISuppliesRepository SuppliesManager => IoC.Get<ISuppliesRepository>();
        public static IProduitRepository ProduitManager => IoC.Get<IProduitRepository>();
        public static IInvoiceRepository InvoiceManager => IoC.Get<IInvoiceRepository>();
        public static ITransactionRepository TransactionManager  => IoC.Get<ITransactionRepository>();
        public static IHashids HashidsManager    => IoC.Get<IHashids>();



        /// <summary>
        /// A shortcut to access the <see cref="SettingsViewModel"/>
        /// </summary>
        public static ParemetresViewModel Settings => IoC.Get<ParemetresViewModel>();

       

        /// 
        #endregion

        #region Construction

        /// <summary>
        /// Sets up the IoC container, binds all information required and is ready for use
        /// NOTE: Must be called as soon as your application starts up to ensure all 
        ///       services can be found
        /// </summary>
        public static void Setup()
        {
            // Bind all required view models
            BindViewModels();
        }

        /// <summary>
        /// Binds all singleton view models
        /// </summary>
        private static void BindViewModels()
        {
            // Bind to a single instance of Application view model
            Kernel.Bind<ApplicationViewModel>().ToConstant(new ApplicationViewModel());
            Kernel.Bind<ParemetresViewModel>().ToConstant(new ParemetresViewModel());
            //  Kernel.Bind<StockHostViewModel>().ToConstant(new  StockHostViewModel());
            string s = "E:\\WPF\\My Projects\\MedicamentStore\\DataBase\\StoreDB.db;Version = 3;";

            Kernel.Bind<SqliteDbConnection>().ToSelf().WithConstructorArgument("connectionString", $"Data Source ={s}");
                                                                           //"Data Source =|DataDirectory|\\DataBase\\StoreDB.db;Version = 3;");

            BindRepository<IUserRepository, UserRepository>("context");

            BindRepository<IAuthenticationRepository, AuthenticationRepository>("context");

            BindRepository<IClientRepository, ClientRepository>("context");

            BindRepository<IMedicamentRepository, MedicamentRepository>("context");

            BindRepository<IStockRepository, StockRepository>("context");

            BindRepository<ISuppliesRepository, SuppliesRepository>("context");

            BindRepository<IProduitRepository, ProduitRepository>("context");

            BindRepository<IInvoiceRepository, InvoiceRepository>("context");
            BindRepository<ITransactionRepository, TransactionRepository>("context");





            Kernel.Bind<IUIManager>().ToConstant(new UIManager());

            Kernel.Bind<IConfirmManager>().ToConstant(new ConfirmManager());

            Kernel.Bind<INotificationManager>().ToConstant(new NotificationManager());

            Kernel.Bind<IHashids>().ToConstant(new Hashids("Etablissement Public de Santé de  Proximité à Aflou",8));









        }

        #endregion

        /// <summary>
        /// Get's a service from the IoC, of the specified type
        /// </summary>
        /// <typeparam name="T">The type to get</typeparam>
        /// <returns></returns>
        public static T Get<T>()
        {
            return Kernel.Get<T>();
        }

        private static void BindRepository<TRepository, TImplementation>(string paramName)
                            where TRepository : class
                            where TImplementation : class, TRepository
        {
            Kernel.Bind<TRepository>().ToMethod(context =>
            {
                var dbContext = context.Kernel.Get<SqliteDbConnection>();
                return Activator.CreateInstance(typeof(TImplementation), dbContext) as TRepository;
            }).InScope(context => context.Parameters.SingleOrDefault(p => p.Name == paramName));
        }
    }
}

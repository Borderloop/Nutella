using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterGUI.DbContext;
using MySql.Data.MySqlClient;
using System.Data.Entity.Core.EntityClient;

namespace MasterGUI
{
    public class BorderContextWrapper
    {
        public BorderContext context;

        public BorderContextWrapper()
        {
            context = new BorderContext(BuildConnectionString());
        }

        private string BuildConnectionString()
        {
            MySqlConnectionStringBuilder providerConnStrBuilder = new MySqlConnectionStringBuilder();
            providerConnStrBuilder.AllowUserVariables = true;
            providerConnStrBuilder.AllowZeroDateTime = true;
            providerConnStrBuilder.ConvertZeroDateTime = true;
            providerConnStrBuilder.MaximumPoolSize = (uint)125;
            providerConnStrBuilder.Pooling = true;
            providerConnStrBuilder.Port = 3306;
            providerConnStrBuilder.Database = "";
            providerConnStrBuilder.Password = "";
            providerConnStrBuilder.Server = "";
            providerConnStrBuilder.UserID = "";

            EntityConnectionStringBuilder entityConnStrBuilder = new EntityConnectionStringBuilder();
            entityConnStrBuilder.Provider = "MySql.Data.MySqlClient";
            entityConnStrBuilder.ProviderConnectionString = providerConnStrBuilder.ToString();
            entityConnStrBuilder.Metadata = "res:// */BetsyContext.BetsyModel.csdl|res:// */BetsyContext.BetsyModel.ssdl|res:// */BetsyContext.BetsyModel.msl";

            return entityConnStrBuilder.ConnectionString;
        }
    }
}

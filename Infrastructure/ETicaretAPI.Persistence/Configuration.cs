using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence
{
    //appseting.js dosyasında veri tabanı bağlantımızı yazdık daha sonra
    //vertabanı bağ.okuyabilmek için bu sınıfı kullanarak sürekliveri tabanı bağlantısı yazmaktan kurtukduk
    static class Configuration
    {
      static public string ConnectionString
      { 
            get
            {
                ConfigurationManager configurationManager = new();
                configurationManager.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../Presentation/ETicaretAPI.API"));
                configurationManager.AddJsonFile("appsettings.json");
                return configurationManager.GetConnectionString("PostgreSQL");
            } 
      }
    }
}

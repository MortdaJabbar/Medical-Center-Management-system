namespace MCMSDAL
{
    public static class AppConfig
    {
        public static string ConnectionString
        { 
             
            
            get { return "Server=.; Database=MCMS; Integrated Security=True; TrustServerCertificate=True; Connect Timeout=30;";  }

           
        }
    }
}

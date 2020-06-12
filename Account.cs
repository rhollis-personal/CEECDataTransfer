namespace CeecDataTransfer
{
    public class Account
    {        
        #region info
        private string name ="mhamilton@carollo.com";
        private string password= "bhu8765tgb";
        private string privateKey= "912cb571-e670-4bac-8789-2d28ccd87965";        
        #endregion
                   
        public string PrivateKey
        {          
            get
            {
                return privateKey;
            }
        }

        public string Username
        {         
            get
            {
                return name;
            }
        }

        public string Password
        {           
            get
            {
                return password;
            }
        }

    }
}

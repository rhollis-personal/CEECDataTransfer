namespace CeecDataTransfer
{
    public class Accounts
    {

        public class AbsorbLMS
        {
            #region info
            private string name = "CEEC@carollo.com";            
            private string password = "Ap1s3sp3rf0rm!";            
            private string privateKey = "912cb571-e670-4bac-8789-2d28ccd87965";
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

        public class RedVectorLMS
        {
            #region info
            private string name = "Carollo!WS";
            private string password = "Givany!031617";
            #endregion
         
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
}

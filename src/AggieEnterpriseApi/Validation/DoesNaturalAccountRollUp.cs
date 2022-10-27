using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AggieEnterpriseApi.Validation
{
    public static class DoesNaturalAccountRollUp
    {
        public static bool NaturalAccount(IErpAccountRollup_ErpAccount? naturalAccount, string expectedCode)
        {
            if (naturalAccount == null)
            {
                return false;
            }
            if (naturalAccount.Code == expectedCode)
            {
                return false; //We only want to check the parents. If the found account is the same as the expected value, that isn't valid
            }

            if (naturalAccount.Parent?.Code == expectedCode)
            {
                return true;
            }
            if (naturalAccount.Parent?.Parent?.Code == expectedCode)
            {
                return true;
            }
            if (naturalAccount.Parent?.Parent?.Parent?.Code == expectedCode)
            {
                return true;
            }
            if (naturalAccount.Parent?.Parent?.Parent?.Parent?.Code == expectedCode)
            {
                return true;
            }
            if (naturalAccount.Parent?.Parent?.Parent?.Parent?.Parent?.Code == expectedCode)
            {
                return true;
            }
            if (naturalAccount.Parent?.Parent?.Parent?.Parent?.Parent?.Parent?.Code == expectedCode)
            {
                return true;
            }
            if (naturalAccount.Parent?.Parent?.Parent?.Parent?.Parent?.Parent?.Parent?.Code == expectedCode)
            {
                return true;
            }


            return false;
        }
    }
}

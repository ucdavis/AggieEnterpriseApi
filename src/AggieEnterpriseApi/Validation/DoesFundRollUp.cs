using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AggieEnterpriseApi.Validation
{
    public static class DoesFundRollUp
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fund"></param>
        /// <param name="level">HierarchyDepth</param>
        /// <param name="expectedCode"></param>
        /// <returns></returns>
        public static bool Fund(IFundParents_ErpFund? fund, int level, string expectedCode)
        {
            if (fund == null)
            {
                return false;
            }
            if (fund.HierarchyDepth == level && fund.Code == expectedCode)
            {
                return true;
            }
            if (fund.Parent == null)
            {
                return false;
            }

            if (fund.Parent.HierarchyDepth == level && fund.Parent.Code == expectedCode)
            {
                return true;
            }
            if (fund.Parent.Parent == null)
            {
                return false;
            }

            if (fund.Parent.Parent.HierarchyDepth == level && fund.Parent.Parent.Code == expectedCode)
            {
                return true;
            }
            if (fund.Parent.Parent.Parent == null)
            {
                return false;
            }

            if (fund.Parent.Parent.Parent.HierarchyDepth == level && fund.Parent.Parent.Parent.Code == expectedCode)
            {
                return true;
            }
            if (fund.Parent.Parent.Parent.Parent == null)
            {
                return false;
            }

            if (fund.Parent.Parent.Parent.Parent.HierarchyDepth == level && fund.Parent.Parent.Parent.Parent.Code == expectedCode)
            {
                return true;
            }
            if (fund.Parent.Parent.Parent.Parent.Parent == null)
            {
                return false;
            }
            if (fund.Parent.Parent.Parent.Parent.Parent.HierarchyDepth == level && fund.Parent.Parent.Parent.Parent.Parent.Code == expectedCode)
            {
                return true;
            }

            if (fund.Parent.Parent.Parent.Parent.Parent.Parent == null)
            {
                return false;
            }
            if (fund.Parent.Parent.Parent.Parent.Parent.Parent.HierarchyDepth == level && fund.Parent.Parent.Parent.Parent.Parent.Parent.Code == expectedCode)
            {
                return true;
            }

            if (fund.Parent.Parent.Parent.Parent.Parent.Parent.Parent == null)
            {
                return false;
            }
            if (fund.Parent.Parent.Parent.Parent.Parent.Parent.Parent.HierarchyDepth == level && fund.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Code == expectedCode)
            {
                return true;
            }


            return false;
        }
    }
}

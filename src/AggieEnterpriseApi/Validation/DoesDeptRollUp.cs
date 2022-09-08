using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AggieEnterpriseApi.Validation
{
    public static class DoesDeptRollUp
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fund"></param>
        /// <param name="level">HierarchyDepth</param>
        /// <param name="expectedCode"></param>
        /// <returns></returns>
        public static bool Dept(IDeptParents_ErpFinancialDepartment? dept, string expectedCode)
        {
            if (dept == null)
            {
                return false;
            }
            if(!dept.Enabled)
            {
                return false;
            }
            
            if (dept.Code == expectedCode)
            {
                return true;
            }
            if (dept.Parent == null)
            {
                return false;
            }

            if (dept.Parent.Code == expectedCode)
            {
                return true;
            }
            if (dept.Parent.Parent == null)
            {
                return false;
            }

            if (dept.Parent.Parent.Code == expectedCode)
            {
                return true;
            }
            if (dept.Parent.Parent.Parent == null)
            {
                return false;
            }

            if (dept.Parent.Parent.Parent.Code == expectedCode)
            {
                return true;
            }
            if (dept.Parent.Parent.Parent.Parent == null)
            {
                return false;
            }

            if (dept.Parent.Parent.Parent.Parent.Code == expectedCode)
            {
                return true;
            }
            if (dept.Parent.Parent.Parent.Parent.Parent == null)
            {
                return false;
            }
            if (dept.Parent.Parent.Parent.Parent.Parent.Code == expectedCode)
            {
                return true;
            }

            if (dept.Parent.Parent.Parent.Parent.Parent.Parent == null)
            {
                return false;
            }
            if (dept.Parent.Parent.Parent.Parent.Parent.Parent.Code == expectedCode)
            {
                return true;
            }

            if (dept.Parent.Parent.Parent.Parent.Parent.Parent.Parent == null)
            {
                return false;
            }
            if (dept.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Code == expectedCode)
            {
                return true;
            }


            return false;
        }
    }
}

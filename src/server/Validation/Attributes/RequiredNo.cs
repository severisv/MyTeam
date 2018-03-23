using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MyTeam.Resources;

namespace MyTeam
{
    public class RequiredNO : RequiredAttribute
    {
        public RequiredNO() : base()
        {
            ErrorMessage = "{0} " + Res.IsRequired.ToLower();
        }

    }
}

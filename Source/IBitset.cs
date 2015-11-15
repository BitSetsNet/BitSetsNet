using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Source
{
    public interface IBitset 
    {
        IBitset And(IBitset x);
        IBitset Or(IBitset y);
        IBitset Length();
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ActeursETFilms
{
    class FiltreAge
    {
        public int ActAge;
        
        public FiltreAge(int Age)
        {
            ActAge = Age;
        }

        public int actAge { get; set; }
            
        
        public bool filtr(object x)
        { 
        
            acteurs actr = (acteurs)x;  
            DateTime DtAct=  (DateTime)actr.datenaiss;
            int Years= DateTime.Now.Year-DtAct.Year;

            if(Years>ActAge)
                return true;
                return false;

        }
    }
}

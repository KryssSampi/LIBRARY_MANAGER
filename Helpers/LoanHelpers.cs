using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBBRARY_MANAGER.Helpers
{
    public static class LoanHelper
    {
        public static decimal GetPenaltyAmount(DateTime expectedReturn, DateTime actualReturn)
        {
            int daysLate = (actualReturn.Date - expectedReturn.Date).Days;
            if (daysLate <= 0) return 0;
            // Montant métier : 0.50€ par jour, plafond à 20€
            const decimal penaltyPerDay = 0.50m;
            const decimal maxPenalty = 20m;
            decimal penalty = daysLate * penaltyPerDay;
            return Math.Min(penalty, maxPenalty);
        }
    }
}

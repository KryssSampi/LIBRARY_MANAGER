using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBBRARY_MANAGER.Helpers
{
    public static class SubscriberHelper
    {
        public static DateTime GetMaxReturnDate(DateTime borrowDate, decimal fidelity)
        {
            // Base autorisée : 21 jours
            int baseDays = 21;
            // Coefficient multiplicateur
            decimal coef = 1.0m + 0.05m * Math.Max(0, fidelity - 1); // fidélité 1 => x1, fidélité 10 => x1.45
            int maxDays = (int)Math.Round(baseDays * coef);
            return borrowDate.AddDays(maxDays);
        }

        public static decimal DecreaseFidelity(decimal oldFidelity, int daysLate)
        {
            // pénalité : 0.5 par retard, +0.1 par jour de retard
            decimal penalty = 0.5m + 0.1m * daysLate;
            decimal newFidelity = Math.Max(0, oldFidelity - penalty);
            return newFidelity;
        }
    }
}

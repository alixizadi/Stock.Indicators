﻿using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // SIMPLE MOVING AVERAGE
        public static IEnumerable<CmfResult> GetCmf(IEnumerable<Quote> history, int lookbackPeriod = 20)
        {

            // clean quotes
            Cleaners.PrepareHistory(history);

            // check parameters
            ValidateCmf(history, lookbackPeriod);

            // initialize
            List<Quote> historyList = history.ToList();
            List<CmfResult> results = new List<CmfResult>();
            IEnumerable<AdlResult> adlResults = GetAdl(history);

            // roll through history
            foreach (AdlResult r in adlResults)
            {

                CmfResult result = new CmfResult
                {
                    Index = r.Index,
                    Date = r.Date,
                    MoneyFlowMultiplier = r.MoneyFlowMultiplier,
                    MoneyFlowVolume = r.MoneyFlowVolume
                };

                if (r.Index >= lookbackPeriod)
                {

                    decimal avgMfv = adlResults
                        .Where(x => x.Index > (r.Index - lookbackPeriod) && x.Index <= r.Index)
                        .Select(x => x.MoneyFlowVolume)
                        .ToArray()
                        .Average();

                    decimal avgVol = historyList
                        .Where(x => x.Index > (r.Index - lookbackPeriod) && x.Index <= r.Index)
                        .Select(x => x.Volume)
                        .ToArray()
                        .Average();

                    if (avgVol != 0)
                    {
                        result.Cmf = avgMfv / avgVol;
                    }
                }

                results.Add(result);
            }

            return results;
        }


        private static void ValidateCmf(IEnumerable<Quote> history, int lookbackPeriod)
        {

            // check parameters
            if (lookbackPeriod <= 0)
            {
                throw new BadParameterException("Lookback period must be greater than 0 for Chaikin Money Flow.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod + 1;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for Chaikin Money Flow.  " +
                        string.Format(englishCulture,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }

        }
    }

}

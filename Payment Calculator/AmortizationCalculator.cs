﻿using System.Collections.Generic;
using System.ComponentModel;

namespace PaymentCalculator
{
    public class AmortizationCalculator
    {
        // Inputs
        public decimal AssetCost { get; }

        public decimal DownPayment { get; }

        public decimal InterestRate { get; }

        // Calculated variables
        public decimal LoanAmount => AssetCost - DownPayment;

        // Outputs
        public decimal MonthlyPayment => Financial.PMT(RatePerPeriod, NumberOfPeriods, -LoanAmount);

        public int NumberOfPeriods => Years * PeriodsPerYear;
        public int PeriodsPerYear { get; }
        public decimal RatePerPeriod => InterestRate / PeriodsPerYear;
        public decimal TotalInterestPaid { get; private set; }
        public decimal TotalPaid => TotalInterestPaid + AssetCost;
        public int Years { get; }

        public AmortizationCalculator(decimal assetCost, decimal downPayment, decimal interestRate, int years, int periodsPerYear)
        {
            AssetCost = assetCost;
            DownPayment = downPayment;
            InterestRate = interestRate;
            Years = years;
            PeriodsPerYear = periodsPerYear;
        }

        /// <summary>
        /// Returns an amoritization table with columns for payment number, interest of the payment, principle of the payment, and balance left of the mortgage.
        /// </summary>
        /// <returns></returns>
        public List<SinglePaymentInformation> MakeTable()
        {
            TotalInterestPaid = 0;

            var table = new List<SinglePaymentInformation>();

            var row = new SinglePaymentInformation();

            for (int i = 1; i <= NumberOfPeriods; i++)
            {
                row = new SinglePaymentInformation()
                {
                    PeriodNumber = i,
                    InterestPayment = Financial.IPMT(RatePerPeriod, i, NumberOfPeriods, -LoanAmount),
                    PrincipalPayment = Financial.PPMT(RatePerPeriod, i, NumberOfPeriods, -LoanAmount),
                    BalanceLeft = Financial.IPMT(RatePerPeriod, i + 1, NumberOfPeriods, -LoanAmount) / RatePerPeriod,
                };

                table.Add(row);

                TotalInterestPaid += row.InterestPayment;
            }
            return table;
        }
    }

    /// <summary>
    /// DTO representing a breakdown of monthly payment information.
    /// </summary>
    public class SinglePaymentInformation
    {
        [DisplayName("Balance Remaining")]
        public decimal BalanceLeft { get; set; }

        [DisplayName("Interest")]
        public decimal InterestPayment { get; set; }

        [DisplayName("#")]
        public int PeriodNumber { get; set; }

        [DisplayName("Principal")]
        public decimal PrincipalPayment { get; set; }
    }
}
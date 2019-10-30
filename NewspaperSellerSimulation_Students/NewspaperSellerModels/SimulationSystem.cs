using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NewspaperSellerModels
{
    public class SimulationSystem
    {
        public SimulationSystem()
        {
            DayTypeDistributions = new List<DayTypeDistribution>();
            DemandDistributions = new List<DemandDistribution>();
            SimulationTable = new List<SimulationCase>();
            PerformanceMeasures = new PerformanceMeasures();
        }
        ///////////// INPUTS /////////////
        public int NumOfNewspapers { get; set; }
        public int NumOfRecords { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal ScrapPrice { get; set; }
        public decimal UnitProfit { get; set; }
        public List<DayTypeDistribution> DayTypeDistributions { get; set; }
        public List<DemandDistribution> DemandDistributions { get; set; }

        ///////////// OUTPUTS /////////////
        public List<SimulationCase> SimulationTable { get; set; }
        public PerformanceMeasures PerformanceMeasures { get; set; }

        public void startsimulation(string input)
        {
            SetInputValues(input);
            SetDayTypeComulativeDistribution();
            SetDemandComulativeDistribution();
            runSimulation();
            calcPerformanceMeasures();

        }
        void SetDayTypeComulativeDistribution()
        {
            Decimal CP = 0;
            int min = 1;
            for(int i=0;i<this.DayTypeDistributions.Count;i++)
            {
                CP += this.DayTypeDistributions[i].Probability;
                this.DayTypeDistributions[i].CummProbability = CP;
                this.DayTypeDistributions[i].MinRange = min;
                this.DayTypeDistributions[i].MaxRange = Decimal.ToInt32(CP*100);
                min = this.DayTypeDistributions[i].MaxRange + 1;

            }
        }
        void SetDemandComulativeDistribution()
        {
            Decimal GCP = 0;
            Decimal FCP = 0;
            Decimal PCP = 0;

            int Gmin = 1;
            int Fmin = 1;
            int Pmin = 1;

            bool GFlag = false;
            bool FFlag = false;
            bool PFlag = false;

            for (int i = 0; i < this.DemandDistributions.Count; i++)
            {
                GCP += this.DemandDistributions[i].DayTypeDistributions[0].Probability;
                FCP += this.DemandDistributions[i].DayTypeDistributions[1].Probability;
                PCP += this.DemandDistributions[i].DayTypeDistributions[2].Probability;

                this.DemandDistributions[i].DayTypeDistributions[0].CummProbability = GCP;
                this.DemandDistributions[i].DayTypeDistributions[1].CummProbability = FCP;
                this.DemandDistributions[i].DayTypeDistributions[2].CummProbability = PCP;



                if (GFlag == false)
                {
                    this.DemandDistributions[i].DayTypeDistributions[0].MinRange = Gmin;
                    this.DemandDistributions[i].DayTypeDistributions[0].MaxRange = Decimal.ToInt32(GCP * 100);
                    Gmin = this.DemandDistributions[i].DayTypeDistributions[0].MaxRange + 1;

                    if (GCP == 1)
                    {
                        GFlag = true;
                    }
                }
                

                if (FFlag == false)
                {
                    this.DemandDistributions[i].DayTypeDistributions[1].MinRange = Fmin;
                    this.DemandDistributions[i].DayTypeDistributions[1].MaxRange = Decimal.ToInt32(FCP * 100);
                    Fmin = this.DemandDistributions[i].DayTypeDistributions[1].MaxRange + 1;

                    if (FCP == 1)
                    {
                        FFlag = true;
                    }
                }
                

                if (PFlag == false)
                {
                    this.DemandDistributions[i].DayTypeDistributions[2].MinRange = Pmin;
                    this.DemandDistributions[i].DayTypeDistributions[2].MaxRange = Decimal.ToInt32(PCP * 100);
                    Pmin = this.DemandDistributions[i].DayTypeDistributions[2].MaxRange + 1;

                    if (PCP == 1)
                    {
                        PFlag = true;
                    }
                }
                
            }
        }
        void SetInputValues(string input)
        {
            string text = File.ReadAllText(input);
            string[] lines = text.Split('\n');
            this.NumOfNewspapers = Int32.Parse(lines[1]);
            this.NumOfRecords = Int32.Parse(lines[4]);
            this.PurchasePrice = Decimal.Parse(lines[7]);
            this.ScrapPrice = Decimal.Parse(lines[10]);
            this.SellingPrice = Decimal.Parse(lines[13]);
            string[] DTD = lines[16].Split(',');

                DayTypeDistribution tmp1 = new DayTypeDistribution();
                tmp1.DayType = Enums.DayType.Good;
                tmp1.Probability = Decimal.Parse(DTD[0]);
                this.DayTypeDistributions.Add(tmp1);

                DayTypeDistribution tmp2 = new DayTypeDistribution();
                tmp2.DayType = Enums.DayType.Fair;
                tmp2.Probability = Decimal.Parse(DTD[1]);
                this.DayTypeDistributions.Add(tmp2);

                DayTypeDistribution tmp3 = new DayTypeDistribution();
                tmp3.DayType = Enums.DayType.Poor;
                tmp3.Probability = Decimal.Parse(DTD[2]);
                this.DayTypeDistributions.Add(tmp3);
            
            for(int i=19;i<lines.Length;i++)
            {
                DemandDistribution D = new DemandDistribution();
                string[] DD = lines[i].Split(',');
                D.Demand = Int32.Parse(DD[0]);

                DayTypeDistribution D1 = new DayTypeDistribution();
                D1.DayType = Enums.DayType.Good;
                D1.Probability = Decimal.Parse(DD[1]);
                D.DayTypeDistributions.Add(D1);

                DayTypeDistribution D2 = new DayTypeDistribution();
                D2.DayType = Enums.DayType.Fair;
                D2.Probability = Decimal.Parse(DD[2]);
                D.DayTypeDistributions.Add(D2);

                DayTypeDistribution D3 = new DayTypeDistribution();
                D3.DayType = Enums.DayType.Poor;
                D3.Probability = Decimal.Parse(DD[3]);
                D.DayTypeDistributions.Add(D3);

                this.DemandDistributions.Add(D);
            }

        }
        ////////////////////////////////

        void runSimulation()
        {
            Random random = new Random();
            for (int i = 0; i < NumOfRecords; i++)
            {
                SimulationCase simCase = new SimulationCase();
                simCase.DayNo = i + 1;
                simCase.RandomNewsDayType = random.Next(1, 100);
                simCase.NewsDayType = getDayType(simCase.RandomNewsDayType);
                simCase.RandomDemand = random.Next(1, 100);
                simCase.Demand = getDemand(simCase.NewsDayType, simCase.RandomDemand);
                simCase.DailyCost = CalcDaiyCost();
                simCase.SalesProfit=calcDailyProfit(simCase.Demand);
                simCase.LostProfit = calcLostProfit(simCase.Demand);
                simCase.ScrapProfit = calcSalvageFromSaleOfScrap(simCase.Demand);
                simCase.DailyNetProfit =calcDailyNetProfit(simCase.SalesProfit,simCase.DailyCost, simCase.LostProfit, simCase.ScrapProfit);
                SimulationTable.Add(simCase);
            }
        }
        void calcPerformanceMeasures()
        {
            PerformanceMeasures.TotalCost = calcTotalCost();
            PerformanceMeasures.TotalLostProfit = calcTotalLostProfit();
            PerformanceMeasures.TotalNetProfit = calcTotalNetProfit();
            PerformanceMeasures.TotalScrapProfit = calcTotalScrapProfit();
            PerformanceMeasures.TotalSalesProfit = calcTotalSalesProfit();
            PerformanceMeasures.DaysWithMoreDemand = DaysWithMoreDemand ();
            PerformanceMeasures.DaysWithUnsoldPapers = DaysWithUnsoldPapers();

        }
        decimal calcTotalCost()
        {
            decimal sum = 0;
            for(int i=0;i<NumOfRecords;i++)
            {
                sum += SimulationTable[i].DailyCost;
            }
            return sum;
        }
        decimal calcTotalSalesProfit()
        {
            decimal sum = 0;
            for (int i = 0; i < NumOfRecords; i++)
            {
                sum += SimulationTable[i].SalesProfit;
            }
            return sum;
        }
        decimal calcTotalLostProfit()
        {
            decimal sum = 0;
            for (int i = 0; i < NumOfRecords; i++)
            {
                sum += SimulationTable[i].LostProfit;
            }
            return sum;
        }
        decimal calcTotalScrapProfit()
        {
            decimal sum = 0;
            for (int i = 0; i < NumOfRecords; i++)
            {
                sum += SimulationTable[i].ScrapProfit;
            }
            return sum;
        }
        decimal calcTotalNetProfit()
        {
            decimal sum = 0;
            for (int i = 0; i < NumOfRecords; i++)
            {
                sum += SimulationTable[i].DailyNetProfit;
            }
            return sum;
        }
        int DaysWithMoreDemand()
        {
            int counter = 0;
            for (int i = 0; i < NumOfRecords; i++)
            {
                if (SimulationTable[i].Demand > NumOfNewspapers)
                    counter++;
            }
            return counter;
        }
        int DaysWithUnsoldPapers()
        {
            int counter = 0;
            for (int i = 0; i < NumOfRecords; i++)
            {
                if (SimulationTable[i].Demand< NumOfNewspapers)
                    counter++;
            }
            return counter;
        }
        Enums.DayType getDayType(int randomNum)
        {
         
            int index = 0;
            for (int i = 0; i < DayTypeDistributions.Count; i++)
            {
                if(DayTypeDistributions[i].MinRange<=randomNum&&DayTypeDistributions[i].MaxRange>=randomNum)
                {
                    index = i;
                    break;
                }
            }
            
            return DayTypeDistributions[index].DayType;
           
        }

        int getDemand(Enums.DayType DT,int randomNum)
        {
          
            int index = (int)DT;
            for(int i=0;i<DemandDistributions.Count;i++)
            {
                if (DemandDistributions[i].DayTypeDistributions[index].MinRange <= randomNum && DemandDistributions[i].DayTypeDistributions[index].MaxRange >= randomNum)
                {
                    index = i;
                    break;
                }
            }
            return DemandDistributions[index].Demand;
        }

        decimal calcDailyProfit(int demand)
        {
            if (demand > NumOfNewspapers)
                demand = NumOfNewspapers;
            return SellingPrice * demand;
        }
        decimal CalcDaiyCost()
        {
            return PurchasePrice * NumOfNewspapers;
        }
        decimal calcLostProfit(int demand)
        {
           int ExcessDemand = demand - NumOfNewspapers;
            if (ExcessDemand < 0)
                return 0;
            return (ExcessDemand * SellingPrice) - (ExcessDemand * PurchasePrice);
        }

        decimal calcSalvageFromSaleOfScrap(int demand)
        {
            int scrap = NumOfNewspapers - demand;
            if (scrap < 0)
                return 0;
            return scrap * ScrapPrice;
        }

        decimal calcDailyNetProfit(decimal salesProfit,decimal dailyCost, decimal lostProfit,decimal salvageFromSale)
        {
            return (salesProfit - dailyCost - lostProfit + salvageFromSale);
        }

    

        
    }
}

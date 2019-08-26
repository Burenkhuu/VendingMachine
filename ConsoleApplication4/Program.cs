using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication4
{
    namespace VendingMachine
    {
        class VendingMachine
        {
            public Dictionary<int, int> Coins { get; set; } = new Dictionary<int, int>();
            public bool BuyAnItem(int change, Dictionary<int, int> payment)
            {
                //CalculateMinCoin(change, )
                return true;
            }
            public void AddCoins(Dictionary<int, int> coins)
            {                
                foreach (var coin in coins)
                {
                    int count;
                    if (Coins.TryGetValue(coin.Key, out count))
                    {
                        Coins[coin.Key] = count + coin.Value;
                    }
                    else
                    {
                        Coins.Add(coin.Key, coin.Value);
                    }
                }
            }
            public int? CalculateMinCoin(int amount, out int[] change)
            {
                int[] coins = Coins.Keys.ToArray();
                int[] limits = Coins.Values.ToArray();

                int[][] coinsUsed = new int[amount + 1][];
                for (int i = 0; i <= amount; ++i)
                {
                    coinsUsed[i] = new int[coins.Length];
                }

                int[] minCoins = new int[amount + 1];
                for (int i = 1; i <= amount; ++i)
                {
                    minCoins[i] = int.MaxValue - 1;
                }

                int[] limitsCopy = new int[limits.Length];
                limits.CopyTo(limitsCopy, 0);

                for (int i = 0; i < coins.Length; ++i)
                {
                    while (limitsCopy[i] > 0)
                    {
                        for (int j = amount; j >= 0; --j)
                        {
                            int currAmount = j + coins[i];
                            if (currAmount <= amount)
                            {
                                if (minCoins[currAmount] > minCoins[j] + 1)
                                {
                                    minCoins[currAmount] = minCoins[j] + 1;

                                    coinsUsed[j].CopyTo(coinsUsed[currAmount], 0);
                                    coinsUsed[currAmount][i] += 1;
                                }
                            }
                        }
                        limitsCopy[i] -= 1;
                    }
                }

                if (minCoins[amount] == int.MaxValue - 1)
                {
                    change = null;
                    return null;
                }
                change = coinsUsed[amount];
                return minCoins[amount];
            }
        }
        class Currency
        {
            public int Denomination { get; set; }
            public int Count { get; set; }

            public Currency(int denom, int count)
            {
                this.Denomination = denom;
                this.Count = count;
            }
        }
        class Program
        {            
            static void Main(string[] args)
            {                
                //Console.WriteLine(minCoins(new int[] { 1, 2, 4, 10, 25 }, new int[] { 1, 2, 4, 1, 10 }, 5, 100));
                int[] ans;
                VendingMachine vm = new VendingMachine();
                
                vm.AddCoins(new Dictionary<int, int> { { 2, 10 }, { 1, 5 } });
                vm.CalculateMinCoin(100, out ans);
                Console.WriteLine(string.Join(",", ans));
                //makeChangeLimitedCoins(new int[] { 1, 2, 4, 10, 25 }, new int[] { 1, 2, 4, 1, 10 }, 100);

                //coinChange(new int[] { 1, 2, 4, 10, 25 }, new int[] { 1, 2, 4, 1, 3 }, 75);
                Console.Read();
                Console.Read();
            }                        
        }
    }
}

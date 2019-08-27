using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendingMachine
{
    class Item
    {
        public string name { get; set; }
        public int price { get; set; }
        public int count { get; set; }
    }
    class VendingMachine
    {
        //Items: Key is item name, Value is item
        public Dictionary<string, Item> Items { get; set; } = new Dictionary<string, Item>();
        // Coins: Key is denomination, Value is number of coins
        public Dictionary<int, int> Coins { get; set; } = new Dictionary<int, int>();
        public bool BuyAnItem(string ItemName, Dictionary<int, int> payment)
        {
            if(!Items.ContainsKey(ItemName))
            {
                Console.WriteLine("Item not found");
                return false;
            }
            int paidAmount = 0;
            foreach (var pair in payment)
            {
                paidAmount = paidAmount + (pair.Key * pair.Value);
            }
            int change = paidAmount - Items[ItemName].price;
            if (change < 0)
            {
                Console.WriteLine("Payment is not enough!");
                return false;
            }
            Console.WriteLine("Change:" + change + "c");
            if (change == 0)
            {
                Console.WriteLine("No change");
            }
            Dictionary<int, int> changeCoins = CalculateMinCoin(change);
            if (changeCoins == null)
            {
                Console.WriteLine("Not able to change, please enter different amout!");
                return false;
            }                
            foreach (var pair in changeCoins)
            {
                Console.WriteLine(pair.Key + "c * " + pair.Value);
            }
            return RemoveItem(ItemName) && RemoveCoins(changeCoins) && AddCoins(payment);
        }
        public bool RemoveItem(string itemName)
        {
            Item item;
            if (Items.TryGetValue(itemName, out item))
            {
                Items[itemName].count = item.count - 1;
                if (Items[itemName].count == 0)
                    Items.Remove(itemName);
                return true;
            }
            else
            {
                Console.WriteLine("Item not found!");
                return false;
            }
        }
        public bool AddItem(Item itemToAdd)
        {
            Item item;
            try
            {
                if (Items.TryGetValue(itemToAdd.name, out item))
                {
                    Items[itemToAdd.name].count = item.count + itemToAdd.count;
                }
                else
                {
                    Items.Add(itemToAdd.name, itemToAdd);
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public bool AddCoins(Dictionary<int, int> coins)
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
            return true;
        }
        public bool RemoveCoins(Dictionary<int, int> coinsToRemove)
        {
            foreach (var coin in coinsToRemove)
            {
                int count;
                if (Coins.TryGetValue(coin.Key, out count))
                {
                    if (count < coin.Value)
                        return false;
                }
                else
                {
                    return false;
                }
            }
            foreach (var coin in coinsToRemove)
            {
                Coins[coin.Key] = Coins[coin.Key] - coin.Value;
                if (Coins[coin.Key] == 0)
                    Coins.Remove(coin.Key);
            }
            return true;
        }
        public Dictionary<int, int> CalculateMinCoin(int amount)
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
            Dictionary<int, int> change = new Dictionary<int, int>();
            if (minCoins[amount] == int.MaxValue - 1)
            {
                return null;
            }               
            for (int i = 0; i < coins.Length; i++)
            {
                if (coinsUsed[amount][i] > 0)
                    change.Add(coins[i], coinsUsed[amount][i]);
            }
            return change;
        }
        public void PrintItems()
        {
            Console.WriteLine();
            Console.WriteLine("Items:");
            foreach (var pair in Items)
            {
                Console.WriteLine("Name: " + pair.Key + " Price: " + pair.Value.price + " Available: "+ pair.Value.count);
            }                
        }
        public void PrintCoins()
        {
            Console.WriteLine();
            Console.WriteLine("Coins:");
            foreach (var pair in Coins)
            {
                Console.WriteLine("Denomination: " + pair.Key + " Count: " + pair.Value);
            }                
        }
    }
    class Program
    {            
        static void Main(string[] args)
        {
            //initializing VendingMachine
            VendingMachine vm = new VendingMachine();
                
            vm.AddCoins(new Dictionary<int, int> { { 1, 100 }, { 5, 50 }, { 10, 50 }, { 25, 50 }});

            vm.AddItem(new Item { name = "pepsi", price = 70, count = 10 });
            vm.AddItem(new Item { name = "coke", price = 85, count = 10 });
            vm.AddItem(new Item { name = "water", price = 90, count = 10 });
            Console.WriteLine("-----------------------INITIAL STATE---------------------------");
            vm.PrintCoins();
            vm.PrintItems();
            
            RunTest(vm, "pepsi", new Dictionary<int, int> { { 25, 3 } }, 1);
            RunTest(vm, "coke", new Dictionary<int, int> { { 25, 4 }, { 7, 3 } }, 2);
            RunTest(vm, "water", new Dictionary<int, int> { { 25, 3 }, { 10, 1 }, { 5, 1 } }, 3);
            RunTest(vm, "water", new Dictionary<int, int> { { 25, 3 }, { 10, 2 }, { 1, 2 } }, 4);
            RunTest(vm, "water", new Dictionary<int, int> { { 25, 3 } }, 5);
            RunTest(vm, "water", new Dictionary<int, int> { { 100000, 1 } }, 6);
            RunTest(vm, "apple", new Dictionary<int, int> { { 25, 1 } }, 7);            

            Console.WriteLine();
            Console.WriteLine("Press enter key to end");
            Console.Read();

        }

        static void RunTest(VendingMachine vm, string itemName, Dictionary<int, int> paid, int testID)
        {
            bool succeeded = false;
            Console.WriteLine("-----------------------test"+ testID + "---------------------------");
            StringBuilder str = new StringBuilder();
            foreach (var pair in paid)
            {
                str.Append(pair.Key + "c * " + pair.Value);
            }
            Console.WriteLine("Buying "+ itemName+". Paid "+str.ToString());
            succeeded = vm.BuyAnItem(itemName, paid);
            Console.WriteLine("Transation " + (succeeded ? "succeeded" : "failed"));
            vm.PrintCoins();
            vm.PrintItems();
        }
    }
}

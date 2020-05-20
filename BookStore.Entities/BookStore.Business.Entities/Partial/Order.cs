using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookStore.Business.Entities
{
    public partial class Order
    {
        public List<UsedStock> UpdateStockLevels()
        {
            List<Warehouse> warehouses = new List<Warehouse>();
            foreach (OrderItem lItem in this.OrderItems)
            {
                int total = 0;
                foreach (Stock lStock in lItem.Book.Stocks)
                {
                    if (!warehouses.Contains(lStock.Warehouse))
                    {
                        warehouses.Add(lStock.Warehouse);
                    }
                    total += (int)lStock.Quantity;
                }
                if (total < lItem.Quantity)
                {
                    throw (new InsufficientStockException());
                }
            }
            List<List<Warehouse>> lWarehouseSubsets = GetWarehouseCombinations(warehouses);
            List<Warehouse> min = warehouses;
            foreach (var subset in lWarehouseSubsets)
            {
                bool hasAllItems = true;
                foreach (OrderItem lItem in this.OrderItems)
                {
                    int quantityLeft = lItem.Quantity;
                    foreach (Warehouse lWarehouse in subset)
                    {
                        Stock lStock = lWarehouse.Stocks.Where(stock => stock.Book.Id == lItem.Book.Id).First();
                        quantityLeft -= (int)lStock.Quantity;
                        if (quantityLeft < 0)
                        {
                            break;
                        }
                    }
                    if (quantityLeft > 0)
                    {
                        hasAllItems = false;
                        break;
                    }
                }
                if (hasAllItems && subset.Count < min.Count)
                {
                    min = subset;
                }
            }
            List<UsedStock> lUsedStocks = new List<UsedStock>();
            foreach (OrderItem lItem in this.OrderItems)
            {
                int quantityLeft = lItem.Quantity;
                foreach (Warehouse lWarehouse in min)
                {
                    if (lWarehouse.Stocks.Where(stock => stock.Book.Id == lItem.Book.Id).Count() > 0)
                    {
                        Stock lStock = lWarehouse.Stocks.Where(stock => stock.Book.Id == lItem.Book.Id).First();
                        int difference = 0;
                        if (lStock.Quantity < quantityLeft)
                        {
                            quantityLeft -= (int)lStock.Quantity;
                            difference = (int)lStock.Quantity;
                            // lStock.Quantity = 0;
                        }
                        else
                        {
                            difference = quantityLeft;
                            quantityLeft = 0;
                            // lStock.Quantity -= quantityLeft;
                        }
                        if (quantityLeft < 0)
                        {
                            break;
                        }
                        UsedStock lUsedStock = new UsedStock()
                        {
                            OrderItem = lItem,
                            OrderItemId = lItem.Id,
                            Stock = lStock,
                            Quantity = difference
                        };
                        lUsedStocks.Add(lUsedStock);
                    }
                }
                if (quantityLeft > 0)
                {
                    throw (new InsufficientStockException());
                }
                
            }
            return lUsedStocks;
        }

        private List<List<Warehouse>> GetWarehouseCombinations(List<Warehouse> pWarehouses)
        {
            Warehouse lFirstElement = pWarehouses.First();
            List<List<Warehouse>> lCombinations = new List<List<Warehouse>>();
            if (pWarehouses.Count == 1)
            {
                List<Warehouse> newCombination = new List<Warehouse>();
                newCombination.Add(lFirstElement);
                lCombinations.Add(newCombination);
            }
            else
            {
                List<List<Warehouse>> subsetCombination = GetWarehouseCombinations(pWarehouses.GetRange(1, pWarehouses.Count - 1));
                foreach (var combination in subsetCombination)
                {
                    List<Warehouse> newCombination = new List<Warehouse>();
                    newCombination.Add(lFirstElement);
                    newCombination.Concat(combination);
                    lCombinations.Add(newCombination);
                }
            }
            return (lCombinations);
        }
    }
}

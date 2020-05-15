using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookStore.Business.Entities
{
    public partial class Order
    {
        public void UpdateStockLevels()
        {
            List<Warehouse> warehouses = new List<Warehouse>();
            foreach (OrderItem lItem in this.OrderItems)
            {
                foreach (Stock lStock in lItem.Book.Stocks)
                {
                    if (!warehouses.Contains(lStock.Warehouse))
                    {
                        warehouses.Add(lStock.Warehouse);
                    }
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
            foreach (OrderItem lItem in this.OrderItems)
            {
                int quantityLeft = lItem.Quantity;
                foreach (Warehouse lWarehouse in min)
                {
                    Stock lStock = lWarehouse.Stocks.Where(stock => stock.Book.Id == lItem.Book.Id).First();
                    if (lStock.Quantity < quantityLeft)
                    {
                        lStock.Quantity = 0;
                    } else
                    {
                        lStock.Quantity -= quantityLeft;
                    }
                    quantityLeft -= (int)lStock.Quantity;
                    if (quantityLeft < 0)
                    {
                        break;
                    }
                }
            }
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

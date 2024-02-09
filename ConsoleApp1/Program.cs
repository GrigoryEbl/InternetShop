using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    private static void Main()
    {
        Warehouse warehouse = new Warehouse();
        Shop shop = new Shop(warehouse);

        Good iPhone12 = new Good("IPhone 12");
        Good iPhone11 = new Good("IPhone 11");

        warehouse.Delive(iPhone12, 10);
        warehouse.Delive(iPhone11, 1);

        shop.ShowGoodsInWarehouse();

        Cart cart = shop.CreateCart();
        cart.AddGood(iPhone12, 4, warehouse);
        cart.AddGood(iPhone11, 3, warehouse);

        cart.ShowGoodsInCart();

        Console.WriteLine(cart.Order().Paylink);

        shop.ShowGoodsInWarehouse();

        cart.AddGood(iPhone12, 9, warehouse);
    }
}

class Cart
{
    private Dictionary<Good, uint> _goods = new Dictionary<Good, uint>();

    public IReadOnlyDictionary<Good, uint> Goods => _goods;

    public event Action<IReadOnlyDictionary<Good, uint>> Ordered;

    public void AddGood(Good good, uint count, Warehouse warehouse)
    {
        if (warehouse.Goods.TryGetValue(good, out uint value))
        {
            if (value >= count)
            {
                _goods.Add(good, count);
                Console.WriteLine($"В корзину добавлен: {good.Name}: {count}шт.");
            }
            else
                Console.WriteLine("Нет нужного количества товара на складе");
        }
    }

    public void ShowGoodsInCart()
    {
        Console.WriteLine("Корзина");

        foreach (var item in Goods)
            Console.WriteLine(item.Key.Name + ": " + item.Value);

        Console.WriteLine();
    }

    public Order Order()
    {
        if (Goods.Count == 0)
        {
            throw new InvalidOperationException("корзина пуста");
        }

        Ordered?.Invoke(Goods);
        _goods.Clear();
        return new Order("Заказ оформлен");
    }
}

class Warehouse
{
    private Dictionary<Good, uint> _goods = new Dictionary<Good, uint>();

    public IReadOnlyDictionary<Good, uint> Goods => _goods;

    public void Delive(Good good, uint count)
    {
        if (count > 0)
            _goods.Add(good, count);
        else
            Console.WriteLine("Кол-во не может быть 0");
    }

    public void RemoveGoods(IReadOnlyDictionary<Good, uint> goodsInCart)
    {
        Console.WriteLine("Удаление со склада");

        foreach (var good in _goods)
        {
            if (goodsInCart.ContainsKey(good.Key))
            {
                goodsInCart.TryGetValue(good.Key, out uint valueInCart);

                _goods[good.Key] -= valueInCart;

                Console.WriteLine($"Со склада удален: {good.Key.Name}: {valueInCart}шт.");
            }
        }
    }
}

class Good
{
    public Good(string name)
    {
        if (name.Length <= 0)
        {
            throw new InvalidOperationException("Нет названия товара");
        }

        Name = name;
    }

    public string Name { get; private set; }
}

class Shop
{
    private Warehouse _warehouse;

    public Shop(Warehouse warehouse)
    {
        if (warehouse != null)
            _warehouse = warehouse;
    }

    public Cart CreateCart()
    {
        Cart cart = new Cart();

        cart.Ordered += OnOrdered;

        return cart;
    }

    public void ShowGoodsInWarehouse()
    {
        Console.WriteLine("Склад");
        if (_warehouse != null)
            foreach (var item in _warehouse.Goods)
                Console.WriteLine(item.Key.Name + ": " + item.Value);
        else
            Console.WriteLine("Склад пуст");
        Console.WriteLine();
    }

    private void OnOrdered(IReadOnlyDictionary<Good, uint> goodsInCart)
    {
        _warehouse.RemoveGoods(goodsInCart);
    }
}

class Order
{
    public readonly string Paylink;

    public Order(string paylink)
    {
        Paylink = paylink;
    }
}
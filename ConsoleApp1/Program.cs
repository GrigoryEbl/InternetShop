using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;

class Program
{
    private static void Main()
    {
        Warehouse warehouse = new Warehouse();
        Shop shop = new Shop(warehouse);

        Good iPhone12 = new Good("IPhone 12");
        Good iPhone11 = new Good("IPhone 11");

        warehouse.Delive(iPhone12, 10);
        warehouse.Delive(iPhone11, 3);

        //Вывод всех товаров на складе с их остатком
        shop.ShowGoodsInWarehouse();

        Cart cart = shop.CreateCart();
        cart.AddGood(iPhone12, 4, warehouse);
        cart.AddGood(iPhone11, 2, warehouse); //при такой ситуации возникает ошибка так, как нет нужного количества товара на складе
        //Вывод всех товаров в корзине
        cart.ShowGoodsInCart();

        Console.WriteLine(cart.Order().Paylink);

        shop.ShowGoodsInWarehouse();

        //cart.AddGood(iPhone12, 9, warehouse); //Ошибка, после заказа со склада убираются заказанные товары
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
                Console.WriteLine($"В крзину добавлен: {good.Name}: {count}шт.");
            }
            else
                throw new InvalidOperationException("Превышено кол-во");
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
        //_goods.Clear();
        return new Order("Ordered!");
    }
}

class Warehouse
{
    private Dictionary<Good, uint> _goods = new Dictionary<Good, uint>();

    public IReadOnlyDictionary<Good, uint> Goods => _goods;

    public void Delive(Good good, uint count)
    {
        _goods.Add(good, count);
    }

    public void RemoveGoods(IReadOnlyDictionary<Good, uint> goodsInCart)
    {
        foreach (var good in _goods)
        {
            if (_goods.ContainsKey(goodsInCart[good.Key]))
            {
                goodsInCart.TryGetValue(good, out uint valueInCart);

                _goods[good] -= valueInCart;

                Console.WriteLine($"Со склада удален: {good.Name}: {valueInCart}шт.");
            }
        }
    }
}

class Good
{
    public Good(string name)
    {
        Name = name;
    }

    public string Name { get; private set; }
}

class Shop
{
    private Warehouse _warehouse;

    public Shop(Warehouse warehouse)
    {
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
        foreach (var item in _warehouse.Goods)
            Console.WriteLine(item.Key.Name + ": " + item.Value);
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
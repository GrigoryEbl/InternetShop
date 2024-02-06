using System;

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

        //Вывод всех товаров на складе с их остатком

        Cart cart = shop.Cart();
        cart.Add(iPhone12, 4);
        cart.Add(iPhone11, 3); //при такой ситуации возникает ошибка так, как нет нужного количества товара на складе

        //Вывод всех товаров в корзине

        Console.WriteLine(cart.Order().Paylink);

        cart.Add(iPhone12, 9); //Ошибка, после заказа со склада убираются заказанные товары
    }
}

class Cart
{
    private Dictionary<Good, uint> _goods = new Dictionary<Good, uint>();
    public IReadOnlyDictionary<Good, uint> Goods => _goods;

    public void Add(Good item, uint count)
    {
        _goods.Add(item, count);
    }
}

class Warehouse
{
    private Dictionary<Good, uint> _goods = new Dictionary<Good, uint>();

    public void Delive(Good item, uint count)
    {
        _goods.Add(item, count);
    }
}

class Good
{
    public string Name { get; private set; }

    public Good(string name)
    {
        Name = name;
    }
}

class Shop
{
    private Warehouse _warehouse;
    private Cart _cart;

    public Shop(Warehouse warehouse)
    {
        _warehouse = warehouse;
    }

    public Cart Cart()
    {
        return _cart = new Cart();
    }
}
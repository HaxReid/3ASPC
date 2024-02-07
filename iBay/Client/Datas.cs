namespace Client;

public static class Datas
{
        public static dynamic UserJson => new
        {
            Email = "user@user.com",
            Username = "user",
            Password = "user",
            Role = "user"
        };

        public static dynamic SellerJson => new
        {
            Email = "seller@seller.com",
            Username = "seller",
            Password = "seller",
            Role = "seller"
        };

        public static dynamic AdminJson => new
        {
            Email = "admin@admin.com",
            Username = "admin",
            Password = "admin",
            Role = "admin"
        };

        public static dynamic LoginAdmin => new
        {
            Email = "admin@admin.com",
            Password = "admin"
        };

        public static dynamic LoginUser => new
        {
            Email = "user@user.com",
            Password = "user"
        };

        public static dynamic LoginSeller => new
        {
            Email = "seller@seller.com",
            Password = "seller"
        };

        public static dynamic NewUserJson => new
        {
            Email = "newuser@newuser.com",
            Username = "newuser",
            Password = "newuser",
            Role = "user"
        };

        public static dynamic ChaiseJson => new
        {
            Name = "chaise",
            Image = "chaise.png",
            Type = "Meuble",
            Price = 10,
            Available = true,
            AddedTime = "2021-01-01",
            SellerId = 1
        };

        public static dynamic TabletteJson => new
        {
            Name = "tablette",
            Image = "tablette.png",
            Type = "Electronique",
            Price = 20,
            Available = false,
            AddedTime = "2021-01-10",
            SellerId = 1
        };

        public static dynamic UpdatedChaiseJson => new
        {
            Id = 1,
            Name = "chaise de bureau",
            Image = "chaise_de_bureau.png",
            Type = "Meuble",
            Price = 150,
            Available = true,
            AddedTime = "2021-01-02",
            SellerId = 1
        };
}
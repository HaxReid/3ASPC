using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Client
{
    class Program
    {
        private static HttpClient _httpClient = new HttpClient();
        private static string _baseUrl = $"http://localhost:8080"; 
        private static string _jwtToken; 

        static async Task Main(string[] args)
        {
            //Créer des Users
            await CreateUserAsync(Datas.UserJson);
            await CreateUserAsync(Datas.SellerJson);
            await CreateUserAsync(Datas.AdminJson);
            
            // Se connecter en tant qu'admin (pour tester facilement les endpoints)
            await LoginAsync(Datas.LoginAdmin);
            
            //Créer des Products
            await CreateProductAsync(Datas.ChaiseJson);
            await CreateProductAsync(Datas.TabletteJson);
            
            // Requêtes sur les endpoints Users
            await GetUsersAsync();
            await GetUserAsync(3);
            await UpdateUserAsync(1, Datas.NewUserJson);
            await GetUserAsync(1);
            await DeleteUserAsync(1);
            await GetUserAsync(1);
            
            // Requêtes sur les endpoints Products
            await GetProductsAsync("type", 10);
            await GetProductAsync(2);
            
            await UpdateProductAsync(1, Datas.UpdatedChaiseJson);
            await GetProductAsync(1);
            await DeleteProductAsync(1);
            await GetProductAsync(1);
            
            // Script pour tester les endpoints Cart
            
            await CreateProductAsync(Datas.ChaiseJson);
            await AddToCartAsync(2);
            await AddToCartAsync(3);
            await GetCartAsync();
            await RemoveFromCartAsync(2);
            await GetCartAsync();
            
        }
        
        // Fonctions pour utiliser les endpoints de l'API
        static async Task LoginAsync(object loginRequestJson)
        {
            var json = JsonConvert.SerializeObject(loginRequestJson);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/api/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<dynamic>(responseContent);

                if (responseData.token != null)
                {
                    _jwtToken = responseData.token;
                    Console.WriteLine("Connexion réussie. Token JWT obtenu.");
                }
                else
                {
                    Console.WriteLine("Échec de la connexion : jeton JWT non trouvé dans la réponse.");
                }
            }
            else
            {
                Console.WriteLine($"Échec de la connexion : {response.StatusCode}");
            }
        }

        static async Task CreateUserAsync(object userJson)
        {
            var json = JsonConvert.SerializeObject(userJson);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/api/users", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Utilisateur créé avec succès.");
            }
            else
            {
                Console.WriteLine($"Échec de la création de l'utilisateur: {response.StatusCode}");
            }
        }
        
        static async Task GetUsersAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/users");

            if (response.IsSuccessStatusCode)
            {
                var usersJson = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<dynamic>>(usersJson);

                Console.WriteLine("Liste des utilisateurs :");
                foreach (var user in users)
                {
                    Console.WriteLine($"ID: {user.id}, Nom d'utilisateur: {user.username}, Email: {user.email}, Rôle: {user.role}");
                }
            }
            else
            {
                Console.WriteLine($"Échec de la récupération des utilisateurs : {response.StatusCode}");
            }
        }

        static async Task GetUserAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/users/{id}");

            if (response.IsSuccessStatusCode)
            {
                var userJson = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<dynamic>(userJson);

                Console.WriteLine($"Utilisateur trouvé : ID: {user.id}, Nom d'utilisateur: {user.username}, Email: {user.email}, Rôle: {user.role}");
            }
            else
            {
                Console.WriteLine($"Échec de la récupération de l'utilisateur : {response.StatusCode}");
            }
        }

        static async Task UpdateUserAsync(int id, object updatedUserJson)
        {
            var json = JsonConvert.SerializeObject(updatedUserJson);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);

            var response = await _httpClient.PutAsync($"{_baseUrl}/api/users/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Utilisateur mis à jour avec succès.");
            }
            else
            {
                Console.WriteLine($"Échec de la mise à jour de l'utilisateur : {response.StatusCode}");
            }
        }

        static async Task DeleteUserAsync(int id)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);

            var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/users/{id}");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Utilisateur supprimé avec succès.");
            }
            else
            {
                Console.WriteLine($"Échec de la suppression de l'utilisateur : {response.StatusCode}");
            }
        }
        
        public static async Task GetProductsAsync(string sortBy, int limit = 10)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/products?sortBy={sortBy}&limit={limit}");

            if (response.IsSuccessStatusCode)
            {
                var productsJson = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<dynamic>>(productsJson);

                Console.WriteLine("Liste des produits :");
                foreach (var product in products)
                {
                    Console.WriteLine($"ID: {product.id}, Nom: {product.name}, Image:{product.image}, Type: {product.type}, Prix: {product.price}, Disponibilité: {product.available}, Date d'ajout: {product.addedTime}, ID du vendeur: {product.sellerId}");
                }
            }
            else
            {
                Console.WriteLine($"Échec de la récupération des produits : {response.StatusCode}");
            }
        }

        public static async Task GetProductAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/products/{id}");

            if (response.IsSuccessStatusCode)
            {
                var productJson = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<dynamic>(productJson);

                Console.WriteLine($"Produit trouvé : ID: {product.id}, Nom: {product.name}, Type: {product.type}, Prix: {product.price}");
            }
            else
            {
                Console.WriteLine($"Échec de la récupération du produit : {response.StatusCode}");
            }
        }

        public static async Task CreateProductAsync(object productJson)
        {
            var json = JsonConvert.SerializeObject(productJson);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);

            var response = await _httpClient.PostAsync($"{_baseUrl}/api/products", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Produit créé avec succès.");
            }
            else
            {
                Console.WriteLine($"Échec de la création du produit : {response.StatusCode}");
            }
        }

        public static async Task UpdateProductAsync(int id, object updatedProductJson)
        {
            var json = JsonConvert.SerializeObject(updatedProductJson);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);

            var response = await _httpClient.PutAsync($"{_baseUrl}/api/products/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Produit mis à jour avec succès.");
            }
            else
            {
                Console.WriteLine($"Échec de la mise à jour du produit : {response.StatusCode}");
            }
        }

        public static async Task DeleteProductAsync(int id)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);

            var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/products/{id}");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Produit supprimé avec succès.");
            }
            else
            {
                Console.WriteLine($"Échec de la suppression du produit : {response.StatusCode}");
            }
        }

        public static async Task AddToCartAsync(int productId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/cart/{productId}");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Produit ajouté au panier avec succès.");
            }
            else
            {
                Console.WriteLine($"Échec d'ajout du produit au panier : {response.StatusCode}");
            }
        }

        public static async Task RemoveFromCartAsync(int productId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_baseUrl}/api/cart/{productId}");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Produit supprimé du panier avec succès.");
            }
            else
            {
                Console.WriteLine($"Échec de suppression du produit du panier : {response.StatusCode}");
            }
        }

        public static async Task GetCartAsync()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/api/cart");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var cartItemsJson = await response.Content.ReadAsStringAsync();
                var cartItems = JsonConvert.DeserializeObject<List<dynamic>>(cartItemsJson);

                Console.WriteLine("Contenu du panier :");
                foreach (var item in cartItems)
                {
                    Console.WriteLine($"ID du produit: {item.productId}, ID de l'utilisateur: {item.userId}");
                }
            }
            else
            {
                Console.WriteLine($"Échec de la récupération du panier : {response.StatusCode}");
            }
        }
        
    } 
}

    

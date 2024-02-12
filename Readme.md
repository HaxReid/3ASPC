# 3ASPC

Ce guide explique comment déployer un projet de gestion d'utilisateurs et de produits en utilisant Docker. Le projet comprend une API backend développée avec ASP.NET Core et une application console en C# pour interagir avec l'API. Vous disposerez d'un fichier ZIP contenant le projet, que vous utiliserez pour déployer l'application à l'aide de Docker.

---

## Prérequis

Avant de commencer, assurez-vous d'avoir les outils suivants installés sur votre machine :

- Docker pour la gestion des conteneurs.
- .NET SDK pour compiler et exécuter le code.
- Un IDE ou un éditeur de texte, tel que Visual Studio, Visual Studio Code ou JetBrains Rider.

## Installation et Exécution

1. Téléchargement du Projet : Téléchargez et extrayez le fichier ZIP contenant le projet sur votre machine.

2. Accès au Répertoire du Projet : Accédez au répertoire extrait du projet à l'aide de la ligne de commande ou de l'explorateur de fichiers.

3. Construction et Lancement des Conteneurs : Utilisez Docker Compose pour construire les conteneurs Docker et démarrer l'application en exécutant la commande suivante dans le répertoire du projet :

```bash
   docker-compose up --build
```

Cette commande va créer et démarrer les conteneurs Docker pour le backend de l'API.

4. Accès à l'Application : Une fois les conteneurs démarrés, l'API sera accessible à l'adresse http://localhost:8080 ainsi que la documentation swagger en http://localhost:8080/swagger en plus d'un deuxième swagger en http://localhost:8081.

5. Lancement de l'Application Client : Ouvrez le projet de l'application console dans votre IDE (par exemple, Visual Studio ou Jetbrains Rider) et exécutez-le fichier program.cs pour interagir avec l'API.

## Fonctionnalités

- CRUD (Création, Lecture, Mise à jour, Suppression) des utilisateurs.
- Authentification des utilisateurs avec JWT.
- CRUD des produits avec autorisations basées sur les rôles.
- Ajout et suppression de produits dans le panier.

---
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIBBRARY_MANAGER.Model;
using LIBBRARY_MANAGER.UI.Common.Items.MainView;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.Items;

namespace LIBBRARY_MANAGER.UI.Modules.BookCatalog.Core.TestCase
{

        public static class BookCatalogSample
        {
            public static ObservableCollection<Book> GetSampleBooks() => new()
        {
            // 🧠 SCIENCE & SAVOIRS
            new Book
            {
                Title = "L’Univers dans une coquille de noix",
                Author = "Stephen Hawking",
                ISBN = "9780553802023",
                Category = nameof(Book.CategoryAllowed.Science),
                Description = "Un voyage fascinant à travers les mystères de l’espace, du temps et de la physique quantique.",
                PublishDate = new DateTime(2001, 3, 14),
                Quantity = 3,
                CoverUrl = "https://covers.openlibrary.org/b/id/240726-L.jpg"
            },
            new Book
            {
                Title = "Introduction à l’intelligence artificielle",
                Author = "François Chollet",
                ISBN = "9780134610993",
                Category = nameof(Book.CategoryAllowed.Informatique),
                Description = "Une exploration moderne des réseaux neuronaux et de l’apprentissage profond.",
                PublishDate = new DateTime(2018, 11, 12),
                Quantity = 5,
                CoverUrl = "https://covers.openlibrary.org/b/id/987654-L.jpg"
            },
            new Book
            {
                Title = "La Structure des Révolutions Scientifiques",
                Author = "Thomas S. Kuhn",
                ISBN = "9780226458083",
                Category = nameof(Book.CategoryAllowed.Philosophie),
                Description = "Une étude sur la dynamique des changements de paradigmes dans la science.",
                PublishDate = new DateTime(1962, 1, 1),
                Quantity = 2,
                CoverUrl = "https://covers.openlibrary.org/b/id/111223-L.jpg"
            },

            // 📚 LITTÉRATURE
            new Book
            {
                Title = "Le Petit Prince",
                Author = "Antoine de Saint-Exupéry",
                ISBN = "9782070612758",
                Category = nameof(Book.CategoryAllowed.Classique),
                Description = "Un conte poétique et philosophique sur l’amitié, l’amour et la perte.",
                PublishDate = new DateTime(1943, 4, 6),
                Quantity = 7,
                CoverUrl = "https://covers.openlibrary.org/b/id/8231856-L.jpg"
            },
            new Book
            {
                Title = "L’Étranger",
                Author = "Albert Camus",
                ISBN = "9782070360024",
                Category = nameof(Book.CategoryAllowed.Roman),
                Description = "Un roman existentiel sur l’absurde et la condition humaine.",
                PublishDate = new DateTime(1942, 5, 19),
                Quantity = 4,
                CoverUrl = "https://covers.openlibrary.org/b/id/7222246-L.jpg"
            },

            // 🌍 HISTOIRE & SOCIÉTÉ
            new Book
            {
                Title = "Sapiens : Une brève histoire de l’humanité",
                Author = "Yuval Noah Harari",
                ISBN = "9780099590088",
                Category = nameof(Book.CategoryAllowed.Histoire),
                Description = "Un panorama fascinant de l’évolution de l’espèce humaine et de ses structures sociales.",
                PublishDate = new DateTime(2011, 6, 4),
                Quantity = 5,
                CoverUrl = "https://covers.openlibrary.org/b/id/8128690-L.jpg"
            },
            new Book
            {
                Title = "Le Capital au XXIe siècle",
                Author = "Thomas Piketty",
                ISBN = "9782021082289",
                Category = nameof(Book.CategoryAllowed.Économie),
                Description = "Une analyse des inégalités économiques et de leur évolution historique.",
                PublishDate = new DateTime(2013, 8, 25),
                Quantity = 2,
                CoverUrl = "https://covers.openlibrary.org/b/id/7894561-L.jpg"
            },

            // 🎨 ARTS & CULTURE
            new Book
            {
                Title = "L’Histoire de l’Art",
                Author = "Ernst H. Gombrich",
                ISBN = "9780714832470",
                Category = nameof(Book.CategoryAllowed.Art),
                Description = "Un classique de la vulgarisation artistique, de la préhistoire à l’art moderne.",
                PublishDate = new DateTime(1995, 2, 12),
                Quantity = 3,
                CoverUrl = "https://covers.openlibrary.org/b/id/10435654-L.jpg"
            },

            // 🌱 NATURE & VIE PRATIQUE
            new Book
            {
                Title = "Le Guide du Jardinier Débutant",
                Author = "Jean Martin",
                ISBN = "9782761942102",
                Category = nameof(Book.CategoryAllowed.Jardinage),
                Description = "Tout ce qu’il faut savoir pour entretenir un jardin sain et florissant.",
                PublishDate = new DateTime(2020, 3, 22),
                Quantity = 6,
                CoverUrl = "https://covers.openlibrary.org/b/id/9326548-L.jpg"
            },
            new Book
            {
                Title = "Cuisine du Monde",
                Author = "Julie Andrieu",
                ISBN = "9782017087742",
                Category = nameof(Book.CategoryAllowed.Cuisine),
                Description = "Un tour du monde culinaire à travers 80 recettes emblématiques.",
                PublishDate = new DateTime(2019, 10, 15),
                Quantity = 8,
                CoverUrl = "https://covers.openlibrary.org/b/id/9873211-L.jpg"
            },

            // 👶 JEUNESSE & DIVERTISSEMENT
            new Book
            {
                Title = "Harry Potter à l’école des sorciers",
                Author = "J.K. Rowling",
                ISBN = "9780747532699",
                Category = nameof(Book.CategoryAllowed.Fantasy),
                Description = "Les débuts d’un jeune sorcier destiné à sauver le monde magique.",
                PublishDate = new DateTime(1997, 6, 26),
                Quantity = 10,
                CoverUrl = "https://covers.openlibrary.org/b/id/7984916-L.jpg"
            },
            new Book
            {
                Title = "One Piece - Tome 1",
                Author = "Eiichiro Oda",
                ISBN = "9782723433610",
                Category = nameof(Book.CategoryAllowed.Manga),
                Description = "Les aventures de Monkey D. Luffy à la recherche du One Piece.",
                PublishDate = new DateTime(1997, 7, 22),
                Quantity = 12,
                CoverUrl = "https://covers.openlibrary.org/b/id/9132478-L.jpg"
            },
            new Book
            {
                Title = "Les Aventures de Tintin : Le Secret de la Licorne",
                Author = "Hergé",
                ISBN = "9782203001122",
                Category = nameof(Book.CategoryAllowed.BandeDessinee),
                Description = "Un classique de la bande dessinée européenne.",
                PublishDate = new DateTime(1943, 1, 1),
                Quantity = 9,
                CoverUrl = "https://covers.openlibrary.org/b/id/1028346-L.jpg"
            },
            new Book
            {
                Title = "Shining",
                Author = "Stephen King",
                ISBN = "9780307743657",
                Category = nameof(Book.CategoryAllowed.Horreur),
                Description = "Un roman d’horreur psychologique se déroulant dans un hôtel isolé.",
                PublishDate = new DateTime(1977, 1, 28),
                Quantity = 4,
                CoverUrl = "https://covers.openlibrary.org/b/id/8673219-L.jpg"
            },
            new Book
            {
                Title = "Le Seigneur des Anneaux",
                Author = "J.R.R. Tolkien",
                ISBN = "9780261102385",
                Category = nameof(Book.CategoryAllowed.Fantasy)      ,
                Description = "Une épopée mythique sur la lutte entre le bien et le mal en Terre du Milieu.",
                PublishDate = new DateTime(1954, 7, 29),
                Quantity = 5,
                CoverUrl = "https://covers.openlibrary.org/b/id/8235082-L.jpg"
            }
        };

    }

    public class DemoFilters
    {
        public static ObservableCollection<FilterLabelViewModel> GetDemoList()
        {
            return new ObservableCollection<FilterLabelViewModel>
        {
            new FilterLabelViewModel
            {
                Id = "cat",
                Type = "Catégorie",
                Value = "Jeunesse"
            },
            new FilterLabelViewModel
            {
                Id = "date",
                Type = "Date d'édition",
                Value = "1992"
            },
            new FilterLabelViewModel
            {
                Id = "sort",
                Type = "Tri",
                Value = "Titre (A-Z)"
            }
        };
        }
    }
    public static class SubscriberCatalogSample
    {
        public static List<Subscriber> FakeSubscribers() => new()
{
    new Subscriber
    {
        Name_User = "Alice Dupont",
        Adresse_Mail = "alice.dupont@example.com",
        Fidelity = 1.20m,
        BirthDate = new DateTime(1995, 4, 12),
        Num_Telephone = "613-555-1234",
        Adresse = "120 Rue Laurier, Ottawa, ON"
    },
    new Subscriber
    {
        Name_User = "Bob Martin",
        Adresse_Mail = "bob.martin@example.com",
        Fidelity = 8.5m,
        BirthDate = new DateTime(1988, 9, 23),
        Num_Telephone = "343-555-5678",
        Adresse = "45 Rue King, Gatineau, QC"
    },
    new Subscriber
    {
        Name_User = "Charlie Lefebvre",
        Adresse_Mail = "charlie.lefebvre@example.com",
        Fidelity = 6.0m,
        BirthDate = new DateTime(1992, 11, 8),
        Num_Telephone = "819-555-9944",
        Adresse = "230 Avenue Rideau, Ottawa, ON"
    },
    new Subscriber
    {
        Name_User = "Diane Tremblay",
        Adresse_Mail = "diane.tremblay@example.com",
        Fidelity = 1.90m,
        BirthDate = new DateTime(1997, 2, 18),
        Num_Telephone = "613-555-6655",
        Adresse = "78 Rue Main, Orleans, ON"
    },
    new Subscriber
    {
        Name_User = "Émile Caron",
        Adresse_Mail = "emile.caron@example.com",
        Fidelity = 4.5m,
        BirthDate = new DateTime(2000, 6, 30),
        Num_Telephone = "343-555-4455",
        Adresse = "12 Boulevard Saint-Laurent, Ottawa, ON"
    },
    new Subscriber
    {
        Name_User = "Fatima Bouchard",
        Adresse_Mail = "fatima.bouchard@example.com",
        Fidelity = 2.10m,
        BirthDate = new DateTime(1993, 1, 25),
        Num_Telephone = "613-555-7788",
        Adresse = "90 Rue Dalhousie, Ottawa, ON"
    },
    new Subscriber
    {
        Name_User = "Gabriel Lavoie",
        Adresse_Mail = "gabriel.lavoie@example.com",
        Fidelity = 1.30m,
        BirthDate = new DateTime(1998, 5, 9),
        Num_Telephone = "343-555-3388",
        Adresse = "210 Rue Preston, Ottawa, ON"
    },
    new Subscriber
    {
        Name_User = "Hélène Girard",
        Adresse_Mail = "helene.girard@example.com",
        Fidelity = 3.00m,
        BirthDate = new DateTime(1991, 12, 5),
        Num_Telephone = "819-555-8811",
        Adresse = "35 Rue Bank, Ottawa, ON"
    },
    new Subscriber
    {
        Name_User = "Isaac Morel",
        Adresse_Mail = "isaac.morel@example.com",
        Fidelity = 7.5m,
        BirthDate = new DateTime(1990, 7, 20),
        Num_Telephone = "613-555-2200",
        Adresse = "64 Rue Kent, Ottawa, ON"
    },
    new Subscriber
    {
        Name_User = "Jade Roy",
        Adresse_Mail = "jade.roy@example.com",
        Fidelity = 1.60m,
        BirthDate = new DateTime(1999, 10, 14),
        Num_Telephone = "343-555-1177",
        Adresse = "18 Rue Somerset, Ottawa, ON"
    }
};


    }


}


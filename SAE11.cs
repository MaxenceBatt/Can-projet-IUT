using System;
using System.Collections.Generic;
using System.IO;

class SAE11
{

    public static int compteur=0;
    // Structure 

    struct Reservation
    {
        public string nom;
        public int idLiaison;
        public string date;
        public string heure;
        public string horodatage;

        public Reservation(string n, int id, string d, string h, string hd)
        {
            nom = n;
            idLiaison = id;
            date = d;
            heure = h;
            horodatage = hd;
        }
    }

    struct Passager
    {
        public string nom;
        public string prenom;
        public string codeCategorie;

        public Passager(string n, string p, string cC)
        {
            nom = n;
            prenom = p;
            codeCategorie = cC;
        }
    }

    struct Véhicule
    {
        public string codeCategorie;
        public int quantite;

        public Véhicule(string cC, int qte)
        {
            codeCategorie = cC;
            quantite = qte;
        }
    }

    // Main (Majoritairement utiliser pour appler les fonctions et créer le JSON)

    static void Main()
    {
        Console.Clear();        // Clear la console, soit avoir un affichage "neuf"
        AfficherBanniere();     // On appel la fonction qui affiche le logo de la CAN
        
        //Vérification de l'existence d'un fichier
        string fichier = "trajet.json";     
        if (!File.Exists(fichier))
        {
            Console.WriteLine("Fichier introuvable.");
            return;
        }

        //Début de la réservation
        Console.Write("Souhaitez-vous réserver une traversée ? (oui / non) : ");
        string reponse = LireOuiNon(); //Appeler la fonction oui/non pour avoir la réponse
        //Quitter si pas de réservation (inutile certes mais au moins c'est là)
        if (reponse == "non")
        {
            Console.WriteLine("À bientôt !");
            return;
        }
        //Sinon faire le reste du code ci-dessous
        File.WriteAllText("trajet.json", ""); // vide le fichier
        Console.Write("Sous quel nom sera la réservation ? : ");
        string nomClient = Console.ReadLine(); //On sauvegarde le nom du client pour la réservation

        Console.WriteLine("Veuillez désormais choisir un trajet parmis les trajets suivants (en saisissant le numéro associé) :");
        int choix = ChoisirTrajet();    //Affichage des différents trajets dispo 

        Reservation trajet;         // On créer une réservation qu'on appel trajet de type 'reservation'
        List<Passager> listePassagers = new List<Passager>();       //Création d'une liste de Passagers ou l'on va stocker chaque passager dans le cas ou l'utilisateur demande un retour
        List<Véhicule> listeVehicules = new List<Véhicule>();       //Création d'une liste de Véhicule ou l'on va stocker chaque véhicule dans le cas ou l'utilisateur demande un retour


        // Choix de la fonction en focntion du choix
        if (choix == 1) trajet = LorientGroix(nomClient);
        else if (choix == 2) trajet = GroixLorient(nomClient);
        else if (choix == 3) trajet = QuiberonLePalais(nomClient);
        else trajet = LePalaisQuiberon(nomClient);

        // Ajout du trajet dans le billet 
        string ligne=(  "[\n\t{\n\t\t\"reservation\": {\n\t\t\t\"nom\" : \""+trajet.nom+"\",\n\t\t\t\"idLiaison\" : "+trajet.idLiaison+",\n\t\t\t\"date\" : \""+trajet.date+"\",\n\t\t\t\"heure\" : \""+trajet.heure+"\",\n\t\t\t\"horodatage\" : \""+trajet.horodatage+"\"\n\t\t},\n\t\t\"passagers\": [");
        string nomDuFichier = "trajet.json";        //Nom du JSON dans lequel on va mettre les infos
        File.AppendAllText(nomDuFichier, ligne);

        // Passagers
        Console.WriteLine();
        Console.WriteLine("================================");
        int nbPassagers = LireEntier("Combien de passagers souhaitez-vous inscrire ? : ");
        while(nbPassagers < 1)      //Vérifier qu'il y a au moins un passager
        {
            nbPassagers = LireEntier("Réserver un voyage sans avoir de voyageur... Donne un vrai nombre de passager : ");
        }
        string lignePassagers;  //Création de la chaine de caractere dans laquelle sera écrit la partie passager du JSON
        Passager passager;
        for(int i = 1; i <= nbPassagers-1; i++)     //Pour chaque passager sauf le dernier (pour respecter l'écriture JSON)
        {
            lignePassagers = "";        //Reset la ligne 
            passager = infosPassagers();        //Récupérer les infos nécessaires
            listePassagers.Add(passager);       //Ajouter les infos dans la liste pour un éventuel retour
            //Création de la ligne JSON grâce aux infos récupérés
            lignePassagers=(  "\n\t\t\t{\n\t\t\t\t\"nom\" : \""+passager.nom+"\",\n\t\t\t\t\"prenom\" : \""+passager.prenom+"\",\n\t\t\t\t\"codeCategorie\" : \""+passager.codeCategorie+"\"\n\t\t\t},");    
            File.AppendAllText(nomDuFichier, lignePassagers);   //Ajout du passager en cours dans le JSON
            Console.WriteLine();
        }
        //Comme ce qu'il y a dans la boucle, sauf qu'il y a une virgule en moins à la toute fin
        lignePassagers = "";
        passager = infosPassagers();
        listePassagers.Add(passager);
        lignePassagers=(  "\n\t\t\t{\n\t\t\t\t\"nom\" : \""+passager.nom+"\",\n\t\t\t\t\"prenom\" : \""+passager.prenom+"\",\n\t\t\t\t\"codeCategorie\" : \""+passager.codeCategorie+"\"\n\t\t\t}\n\t\t],\n\t\t\"vehicules\": [");
        File.AppendAllText(nomDuFichier, lignePassagers);

        // Véhicules
        Console.WriteLine();
        Console.WriteLine("================================");
        afficherVéhicules();
        int nbVéhicule = LireEntier("Combien de catégories de véhicules souhaitez-vous embarquer avec vous ? : ");
        while(nbVéhicule < 0 || nbVéhicule > 11)       // Vérifier que l'utilisateur ne saisisse pas de nombre négatif ou supérieur a 11
        {
            if(nbVéhicule<0) nbVéhicule = LireEntier("Un nombre de véhicules négatifs. Hm. ok, et pour de vrai ? Combien de catégories ? : ");
            else nbVéhicule = LireEntier("Vous avez saisis trop de catégories, veuillez saisir un nombre valide : ");
        }
        string ligneVéhicules; //Création de la chaine de caractere dans laquelle sera écrit la partie véhicule du JSON
        Véhicule véhicule;
        for(int i = 1; i <= nbVéhicule-1; i++) // Pour chaque véhicule sauf le dernier
        {
            ligneVéhicules = "";        //reset la ligne
            véhicule = infosVéhicules();        //Récupérer les infos données par l'utilisateur
            listeVehicules.Add(véhicule);       //Ajouter dans la liste pour éventuel retour
            //ajout du contenu du JSON dans la ligne
            ligneVéhicules=(  "\n\t\t\t{\n\t\t\t\t\"codeCategorie\" : \""+véhicule.codeCategorie+"\",\n\t\t\t\t\"quantite\" : "+véhicule.quantite+"\n\t\t\t},");
            File.AppendAllText(nomDuFichier, ligneVéhicules);       //Ajouter le contenu JSON dans le fichier JSON
            Console.WriteLine();
        }
        string retour = "";
        if(nbVéhicule>0)//Dernier véhicule (vérifier quand même que l'utilisateur n'a pas saisi 0)
        {
            ligneVéhicules = "";
            véhicule = infosVéhicules();
            listeVehicules.Add(véhicule);
            Console.WriteLine();
            Console.Write("Veux-tu réserver un trajet retour ? (oui/non) : "); //Proposer un trajet retour (ici car sinon le JSON ne sera pas écrit correctement a cause de ma façon d'ajouter les infos)
            retour = LireOuiNon();
            if(retour == "non") // Si pas de retour demandé fermeture normale du JSON pour 1 réservation
            {
                ligneVéhicules=(  "\n\t\t\t{\n\t\t\t\t\"codeCategorie\" : \""+véhicule.codeCategorie+"\",\n\t\t\t\t\"quantite\" : "+véhicule.quantite+"\n\t\t\t}\n\t\t]\n\t}\n]\n");
                File.AppendAllText(nomDuFichier, ligneVéhicules);   
            }
            if(retour == "oui") // Si retour demandé éétape supplémentaire
            {
                Console.WriteLine("\n--- TRAJET RETOUR ---");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("⚠ Veuillez être logique dans vos choix. La date de votre trajet retour doit être après la date de votre trajet aller. La date que vous aviez selectionner pour l'aller était le "+trajet.date+" a "+trajet.heure+".");
                Console.ResetColor();
                Reservation trajetRetour;
                if (choix == 1) trajetRetour = GroixLorient(nomClient);     //Si l'utilisateur avait saisis comme trajet aller Lorient groix, trajet retour sera Groix Lorient
                else if (choix== 2) trajetRetour = LorientGroix(nomClient);     //Si l'utilisateur avait saisis comme trajet aller Groix Lorient, trajet retour sera Lorient Groix
                else if (choix == 3) trajetRetour = LePalaisQuiberon(nomClient);    //Si l'utilisateur avait saisis comme trajet aller Quiberon LePalais, trajet retour sera LePalais Quiberon
                else trajetRetour = QuiberonLePalais(nomClient);        //Si l'utilisateur avait saisis comme trajet aller LePalais Quiberon, trajet retour sera Quiberon LePalais

                // ÉCRITURE DU DERNIER VÉHICULE (sans virgule)
                File.AppendAllText(nomDuFichier,
                    "\n\t\t\t{\n\t\t\t\t\"codeCategorie\" : \""+véhicule.codeCategorie+"\",\n\t\t\t\t\"quantite\" : "+véhicule.quantite+"\n\t\t\t}\n\t\t]\n\t},\n"
                );
                EcrireTrajetJSON(
                    nomDuFichier,
                    trajetRetour,
                    listePassagers,
                    listeVehicules,
                    false
                );

                // Fermeture finale du tableau JSON
                File.AppendAllText(nomDuFichier, "]\n");
            }
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine("Veux-tu réserver un trajet retour ? (oui/non) : ");
            retour = LireOuiNon();      
            if(retour == "non")     // Si pas de retour finir normalement 
            {
                ligneVéhicules=(  "\n\t\t]\n\t}\n]\n");
                File.AppendAllText(nomDuFichier, ligneVéhicules);  
            }
            if(retour == "oui") // Si retour faire comme avant
            {
                Console.WriteLine("\n--- TRAJET RETOUR ---");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("⚠ Veuillez être logique dans vos choix. La date de votre trajet retour doit être après la date de votre trajet aller. La date que vous aviez selectionner pour l'aller était le "+trajet.date+" a "+trajet.heure+".");
                Console.ResetColor();
                Reservation trajetRetour;
                if (choix == 1) trajetRetour = GroixLorient(nomClient);
                else if (choix== 2) trajetRetour = LorientGroix(nomClient);
                else if (choix == 3) trajetRetour = LePalaisQuiberon(nomClient);
                else trajetRetour = QuiberonLePalais(nomClient);

                // Écriture du trajet retour avec LES MÊMES passagers et véhicules
                File.AppendAllText(nomDuFichier, "\n\t\t]\n\t},\n");
                EcrireTrajetJSON(
                    nomDuFichier,
                    trajetRetour,
                    listePassagers,
                    listeVehicules,
                    false
                );

                // Fermeture finale du tableau JSON
                File.AppendAllText(nomDuFichier, "]\n");
            }
        }      
    }

    // Fonctions utilitaires ( utilisés pour afficher des choses simples ou éviter la redondance (genre question oui / non ou les jours de la semaine))

    static void AfficherBanniere() // fonction qui affiche le logo de la CAN
    {
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine();
        Console.WriteLine("                                                 ");
        Console.WriteLine("            ██████╗  █████╗ ███╗   ██╗           ");
        Console.WriteLine("           ██╔════╝ ██╔══██╗████╗  ██║           ");
        Console.WriteLine("           ██║      ███████║██╔██╗ ██║           ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("           ██║      ██╔══██║██║╚██╗██║           ");
        Console.WriteLine("           ╚██████╗ ██║  ██║██║ ╚████║           ");
        Console.WriteLine("            ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═══╝           ");
        Console.ForegroundColor = ConsoleColor.Black;
        Console.WriteLine("       COMPAGNIE ALSACIENNE DE NAVIGATION        ");
        Console.ResetColor();
        Console.WriteLine();
    }

    static void afficherVéhicules()
    {
        string[] categories =           //Créer le tableau avec toutes les catégories de véhicules
        {
            "Trottinette électrique",
            "Vélo ou remorque à vélo",
            "Vélo électrique",
            "Vélo cargo ou tandem",
            "Deux-roues < 125 cm3",
            "Deux-roues > 125 cm3",
            "Voiture moins de 4 m",
            "Voiture de 4 m à 4.39 m",
            "Voiture de 4.40 m à 4.79 m",
            "Voiture 4.80 m et plus",
            "Camping-car - véhicule plus de 2.10 m de haut"
        };
        Console.WriteLine();
        Console.WriteLine("Voici la liste des véhicules autorisés.");
        Console.WriteLine("----------------------------------------------------");
        for (int i = 0; i < categories.Length-1; i++)       //Afficher tous les véhicules
        {
            Console.WriteLine($"{i + 1}. {categories[i]}");
            Console.WriteLine();
        }
        Console.WriteLine($"{11}. {categories[10]}");
        Console.WriteLine("----------------------------------------------------");
        Console.WriteLine();
    }

    static string LireOuiNon() // Vérifie les réponses au question a choix oui/non de l'utilisateur
    {
        string r = Console.ReadLine();
        while (r != "oui" && r != "non")    // Boucle tant que la réponse est pas 'oui' ou 'non'
        {
            Console.Write("Réponse invalide (oui / non) : ");
            r = Console.ReadLine();
        }
        return r;
    }

    static int LireEntier(string message)       //Fonction qui vérifie que l'utilisateur entre un int et pas autre chose
    {
        int valeur;
        Console.Write(message);
        while (!int.TryParse(Console.ReadLine(), out valeur))
        {
            Console.Write("Entrée invalide, veuillez entrer un nombre : ");
        }
        return valeur;
    }

    static string ChoisirHoraireAvecFleches(List<string> horaires)      //Permet un affichage clean pour choisir l'heure
    {
        int index = 0;
        ConsoleKey key;

        do //Faire tant que
        {
            Console.Clear();        //Nettoyer la console
            Console.WriteLine("Choisissez un horaire (↑ ↓ puis Entrée)\n");

            for (int i = 0; i < horaires.Count; i++)        // faire la liste des horaires
            {
                if (i == index)     //Si l'horaire est égal a l'index mettre le fond en bleu et rajouter une flèche
                {
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("> " + horaires[i]);
                    Console.ResetColor();
                }
                else        //Ssinon juste écrire l'horaire avec un espace devant
                {
                    Console.WriteLine("  " + horaires[i]);
                }
            }

            key = Console.ReadKey(true).Key;    //key c'est égal à la touche sur laquelle on appuie

            if (key == ConsoleKey.UpArrow)  //Si c'est flèche du haut indice -1 donc on monte
                index = (index - 1 + horaires.Count) % horaires.Count;

            if (key == ConsoleKey.DownArrow) // si c'est flèche du bas indice +1 donc on descend
                index = (index + 1) % horaires.Count;

        } while (key != ConsoleKey.Enter); //Tant qu'on a pas appuyer sur entrer

        return horaires[index];     //Renvoyer l'heure séléctionnée
    }


    static int ChoisirTrajet() // Fonction qui fait choisir le trajet
    {
        Console.WriteLine("1 - Lorient → Groix");
        Console.WriteLine("2 - Groix → Lorient");
        Console.WriteLine("3 - Quiberon → Le Palais");
        Console.WriteLine("4 - Le Palais → Quiberon");
        Console.Write("Choix : ");

        int choix;
        while (!int.TryParse(Console.ReadLine(), out choix) || choix < 1 || choix > 4) // Boucle si ID du trajet existe pas
        {
            Console.Write("Choix invalide : ");
        }
        return choix;
    }

    static string NomJour(int jour) //Les jours de la semaine pour un affichage plus joli beau
    {
        string[] jours = {
            "Vendredi", "Samedi", "Dimanche",
            "Lundi", "Mardi", "Mercredi", "Jeudi"
        };  

        return jours[jour % 7];
    }

    // Fonctions longues (Fonctions "principales", les plus gros blocs de code)

    static Reservation AfficherEtValiderTrajet(
        Dictionary<int, List<string>> planning,
        string nomTrajet,
        string depart,
        string arrivee,
        int idTrajet,
        string nomClient) // Renvoie une réservation et demande toutes les infos nécessaires à la création de cette réservation
    {
        Console.WriteLine("\nHoraires disponibles pour " + nomTrajet + " :");

        // Affiche chaque jour et chaque horaire par jour
        Console.WriteLine("==================================");
        ConsoleColor[] couleurs = new ConsoleColor[]
        {
            ConsoleColor.Cyan,
            ConsoleColor.Green,
            ConsoleColor.Yellow,
            ConsoleColor.Magenta,
            ConsoleColor.Blue,
            ConsoleColor.Red,
            ConsoleColor.DarkCyan,
            ConsoleColor.DarkYellow,
            ConsoleColor.Gray
        };

        foreach (KeyValuePair<int, List<string>> jour in planning)
        {
            string nomJour = NomJour(jour.Key);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(nomJour + " " + jour.Key + " :\t");

            int indexCouleur = 0; // on recommence à 0 pour chaque jour

            foreach (string h in jour.Value)
            {
                Console.ForegroundColor = couleurs[indexCouleur % couleurs.Length];
                Console.Write(h + "  ");
                indexCouleur++;
            }

            Console.WriteLine();
        }

        Console.ResetColor();
        Console.WriteLine("==================================");

        //pose des questions et vérifie que la réponse existe
        int jourChoisi = LireEntier("Choisissez un jour (en saisissant le nombre) : ");

        while (!planning.ContainsKey(jourChoisi)) //tant que jourChoisi ne se trouve pas dans planning (!planning.ContaineKey = planning.ContainKey == false)
        {
            jourChoisi = LireEntier("Jour invalide : ");
        }

        string heure = ChoisirHoraireAvecFleches(planning[jourChoisi]);


        string date = new DateTime(2025, 11, jourChoisi).ToString("yyyy-MM-dd");
        string horodatage = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        return new Reservation(nomClient, idTrajet, date, heure, horodatage);
    }

    static Passager infosPassagers()        //Récupère toutes les infos nécessaire pour un passager et renvoie le passager
    {
        Passager p;
        int age;
        compteur += 1;
        Console.Write("Quel est le nom du passager N°"+compteur+" : ");
        p.nom = Console.ReadLine();
        Console.Write("Quel est le prénom du passager N°"+compteur+" : ");
        p.prenom = Console.ReadLine();
        age = LireEntier("Quel est l'âge du passager N°"+compteur+" (taper un nombre négatif si c'est un animal): ");
        if(age>=26) p.codeCategorie = "adu26p";
        else if(age>=18) p.codeCategorie = "jeu1825";
        else if(age>=4) p.codeCategorie = "enf417";
        else if(age>=0) p.codeCategorie = "bebe";
        else p.codeCategorie = "ancomp";
        return p;
    }

    
    static Véhicule infosVéhicules()        //Récupère toutes les infos nécessaire pour une cat"gorie de véhicule et renvoie la catégorie
    {
        Véhicule v;
        int idCategorie = LireEntier("Quel est la categorie du/des véhicule (taper l'identifiant de la categorie) : ");
        while(idCategorie >11 || idCategorie < 1)
        {
            idCategorie = LireEntier("Identifiant invalide... Tapez-en un parmis les identifiants existant (Taper 0 pour rafficher la liste des catégories) : ");
            if(idCategorie == 0) afficherVéhicules();
        }
        if(idCategorie == 1) v.codeCategorie = "trot";
        else if(idCategorie == 2) v.codeCategorie = "velo";
        else if(idCategorie == 3) v.codeCategorie = "velelec";
        else if(idCategorie == 4) v.codeCategorie = "cartand";
        else if(idCategorie == 5) v.codeCategorie = "mobil";
        else if(idCategorie == 6) v.codeCategorie = "moto";
        else if(idCategorie == 7) v.codeCategorie = "cat1";
        else if(idCategorie == 8) v.codeCategorie = "cat2";
        else if(idCategorie == 9) v.codeCategorie = "cat3";
        else if(idCategorie == 10) v.codeCategorie = "cat4";
        else if(idCategorie == 11) v.codeCategorie = "camp";
        else v.codeCategorie = "???";

        v.quantite = LireEntier("Combien de véhicules sont dans cette catégorie ? : ");
        return v;
    }

    static void EcrireTrajetJSON(       
        string fichier,
        Reservation trajet,
        List<Passager> passagers,
        List<Véhicule> vehicules,
        bool virguleApres)
    {                                           //Créer les infos pour le retour dans le JSON
        File.AppendAllText(fichier,             //Ajoute la réservation
            "\t{\n\t\t\"reservation\": {\n" +
            "\t\t\t\"nom\" : \"" + trajet.nom + " - retour\",\n" +
            "\t\t\t\"idLiaison\" : " + trajet.idLiaison + ",\n" +
            "\t\t\t\"date\" : \"" + trajet.date + "\",\n" +
            "\t\t\t\"heure\" : \"" + trajet.heure + "\",\n" +
            "\t\t\t\"horodatage\" : \"" + trajet.horodatage + "\"\n" +
            "\t\t},\n\t\t\"passagers\": ["
        );

        for (int i = 0; i < passagers.Count; i++)       //Pour chaque passager
        {
            Passager p = passagers[i];
            File.AppendAllText(fichier,         //Ajoute chaque passager
                "\n\t\t\t{\n" +
                "\t\t\t\t\"nom\" : \"" + p.nom + "\",\n" +
                "\t\t\t\t\"prenom\" : \"" + p.prenom + "\",\n" +
                "\t\t\t\t\"codeCategorie\" : \"" + p.codeCategorie + "\"\n\t\t\t}" +
                (i < passagers.Count - 1 ? "," : "")    //Quand c'est le dernier élément (passagers.Count - 1) ne pas mettre de virgule
            );
        }

        File.AppendAllText(fichier, "\n\t\t],\n\t\t\"vehicules\": [");      

        for (int i = 0; i < vehicules.Count; i++)       //Pour chaque catégorie de véhicule
        {
            Véhicule v = vehicules[i];
            File.AppendAllText(fichier,             //Ajoute chaque catégorie de véhicule dans le JSON
                "\n\t\t\t{\n" +
                "\t\t\t\t\"codeCategorie\" : \"" + v.codeCategorie + "\",\n" +
                "\t\t\t\t\"quantite\" : " + v.quantite + "\n\t\t\t}" +
                (i < vehicules.Count - 1 ? "," : "")    //Quand c'est le dernier élément (vehicules.Count - 1) ne pas mettre de virgule
            );
        }

        File.AppendAllText(fichier,
            "\n\t\t]\n\t}" + (virguleApres ? "," : "") + "\n"   // Ajouter la fin du format JSON
        );
    }
    // Trajets (Simplement la déclaration des trajets avec les horaires etc...)

    static Reservation LorientGroix(string nom)
    {
        Dictionary<int, List<string>> h = new Dictionary<int, List<string>>();
        List<string> semaine = new List<string> {"08:05","11:00","13:45","16:15","18:45"};
        List<string> jeudi13 = new List<string> {"08:05","11:00","13:45","18:45"};
        List<string> weekend = new List<string> {"09:45","12:15","17:00","19:30"};
        //ajout des horaires pour chaque jour.
        for (int i = 1; i <= 30; i++)
        {
            if (i == 1 || i == 11 || i % 7 == 2) h[i] = weekend;
            else if (i == 13) h[i] = jeudi13;
            else h[i] = semaine;
        }

        return AfficherEtValiderTrajet(h, "Lorient → Groix", "Lorient", "Groix", 1, nom);
    }

    static Reservation GroixLorient(string nom)
    {
        Dictionary<int, List<string>> h = new Dictionary<int, List<string>>();
        List<string> semaine = new List<string> {"06:50","09:30","12:30","15:00","17:30"};
        List<string> jeudi13 = new List<string> {"06:50","12:30","15:00","17:30"};
        List<string> weekend = new List<string> {"08:30","11:00","15:45","18:15"};
        //ajout des horaires.
        for (int i = 1; i <= 30; i++)
        {
            if (i == 1 || i == 11 || i % 7 == 2) h[i] = weekend;
            else if (i == 13) h[i] = jeudi13;
            else h[i] = semaine;
        }


        return AfficherEtValiderTrajet(h, "Groix → Lorient", "Groix", "Lorient", 2, nom);
    }

    static Reservation QuiberonLePalais(string nom)
    {
        Dictionary<int, List<string>> h = new Dictionary<int, List<string>>();

        List<string> QLP_1 = new List<string> {"08:15","09:30","11:15","12:45", "14:45", "17:15", "19:00", "20:00"};
        List<string> QLP_2 = new List<string> {"08:00","09:30","11:15","12:30", "14:15", "15:45", "17:15", "19:00", "20:00"};
        List<string> QLP_3_10 = new List<string> {"08:00","09:30","11:15","14:30","17:30", "19:30"};
        List<string> QLP_4 = new List<string> {"07:30", "09:30", "10:30", "13:30", "18:15", "19:30"};
        List<string> QLP_5 = new List<string> {"08:00","10:30","12:00","17:15","19:00","20:00"};
        List<string> QLP_6 = new List<string> {"07:30","08:30","11:45","14:45","18:00","19:45"};
        List<string> QLP_7 = new List<string> {"08:00","09:30","12:30","15:30","18:15","20:00"};
        List<string> QLP_8 = new List<string> {"08:15","09:45","11:00","14:15","17:15","19:30"};
        List<string> QLP_9 = new List<string> {"08:30","11:15","14:15","16:15","17:30","19:30"};
        List<string> QLP_11 = new List<string> {"08:15","11:15","14:15","16:15","17:30","19:30"};
        // M 12, J 13, L 17, M 18, M 19, J 20, L 24, M 25, M 26, J 27 :
        List<string> QLP_A = new List<string> {"08:00","09:30","11:15","14:15","16:15","17:15","20:00"};
        // V 14, V 21, V 28 :
        List<string> QLP_B = new List<string> {"08:00","09:30","11:15","14:15","17:15","19:15","20:15"};
        // L 10, S 15, S 22, S 29
        List<string> QLP_C = new List<string> {"08:00","09:30","11:15","14:15","17:30","19:30"};
        // D 16, D 23, D 30
        List<string> QLP_D = new List<string> {"08:15","11:15","14:15","16:15","17:30","19:30"};
        List<string> Verif = new List<string> {"??"};
        //ajout des horaires pour chaque jour
        for (int i = 1; i <= 30; i++)
        {
            if (i == 1 ) h[i] = QLP_1;
            else if (i == 2) h[i] = QLP_2;
            else if (i==3 || i==10) h[i] = QLP_3_10;
            else if (i==4) h[i] = QLP_4;
            else if (i == 5) h[i] = QLP_5;
            else if (i == 6) h[i] = QLP_6;
            else if (i == 7) h[i] = QLP_7;
            else if (i == 8) h[i] = QLP_8;
            else if (i == 9) h[i] = QLP_9;
            else if (i == 11) h[i] = QLP_11;
            else if (i == 12 || i == 13 || i == 17 || i == 18 || i == 19 || i == 20 || i == 24 || i == 25 || i == 26 || i == 27) h[i] = QLP_A;
            else if (i == 14 || i == 21 || i == 28) h[i] = QLP_B;
            else if (i == 10 || i == 15 || i == 22 || i == 29) h[i] = QLP_C;
            else if (i == 16 || i == 23 || i == 30) h[i] = QLP_D;
            // Vérifier que chaque jour possèdes des horaires
            else h[i] = Verif;
        }

        return AfficherEtValiderTrajet(h, "Quiberon → Le Palais", "Quiberon", "Le Palais", 3, nom);
    }

    static Reservation LePalaisQuiberon(string nom)
    {
        Dictionary<int, List<string>> h = new Dictionary<int, List<string>>();

        List<string> LPQ_1 = new List<string> { "06:45","07:45","09:45","11:00","12:45","14:45","17:30","18:45" };
        List<string> LPQ_2 = new List<string> { "06:30","07:45","09:45","11:00","12:45","14:15","15:45","17:15","18:40" };
        List<string> LPQ_3 = new List<string> { "06:30","07:45","09:45","12:45","16:00","18:00" };
        List<string> LPQ_4 = new List<string> { "06:00","07:45","09:00","12:00","16:45","18:00" };
        List<string> LPQ_5 = new List<string> { "06:30","07:45","10:30","13:30","17:30","18:30" };
        List<string> LPQ_6 = new List<string> { "06:00","07:00","09:00","13:15","16:45","18:30" };
        List<string> LPQ_7 = new List<string> { "06:30","07:45","09:45","14:00","16:45","18:45" };
        List<string> LPQ_8 = new List<string> { "07:00","08:00","09:45","12:45","15:45","17:45" };
        List<string> LPQ_9 = new List<string> { "07:15","09:45","12:45","14:45","15:45","18:00" };

        List<string> LPQ_A = new List<string> { "06:30","07:45","09:45","12:45","14:45","15:45","18:30" }; // 12,17,18,19,20
        List<string> LPQ_B = new List<string> { "06:30","07:45","09:45","12:45","15:45","18:30" };        // 24,25,26,27
        List<string> LPQ_C = new List<string> { "06:30","07:45","09:45","12:45","15:45","18:00" };        // 15,22,29
        List<string> LPQ_D = new List<string> { "07:00","09:45","12:45","14:45","15:45","18:00" };        // 11,16,23,30
        List<string> LPQ_E = new List<string> { "06:30","07:45","09:45","12:45","15:45","18:45" };        // 21,28

        List<string> Verif = new List<string> {"??"};

        for (int i = 1; i <= 30; i++)
        {
            if (i == 1)       h[i] = LPQ_1;
            else if (i == 2)  h[i] = LPQ_2;
            else if (i == 3)  h[i] = LPQ_3;
            else if (i == 4)  h[i] = LPQ_4;
            else if (i == 5)  h[i] = LPQ_5;
            else if (i == 6)  h[i] = LPQ_6;
            else if (i == 7)  h[i] = LPQ_7;
            else if (i == 8)  h[i] = LPQ_8;
            else if (i == 9)  h[i] = LPQ_9;
            else if (i == 12 || i == 13 || i == 17 || i == 18 || i == 19 || i == 20)h[i] = LPQ_A;
            else if (i == 24 || i == 25 || i == 26 || i == 27)                      h[i] = LPQ_B;
            else if (i == 10 || i == 15 || i == 22 || i == 29)                      h[i] = LPQ_C;
            else if (i == 11 || i == 16 || i == 23 || i == 30)                      h[i] = LPQ_D;
            else if (i == 14 || i == 21 || i == 28)                                 h[i] = LPQ_E;
            else h[i] = Verif;
        }
        return AfficherEtValiderTrajet(h, "Le Palais → Quiberon", "Le Palais", "Quiberon", 4, nom); 
    }
}

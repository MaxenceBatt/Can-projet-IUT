document.addEventListener("DOMContentLoaded", function () {         //Attends que toute la page HTML soit complètement chargée avant d’exécuter le JavaScript.

    var trajetSelectionne = null;       //l’id de la liaison (1, 2, 3 ou 4)
    var dateSelectionnee = null;        //la date choisie 

    var cartesTrajet = document.querySelectorAll(".carte-trajet");   //Tous les trajets cliquables
    var boutonsJour = document.querySelectorAll(".jour");           //Tous les jours cliquables
    var zoneResultats = document.getElementById("resultats");       //La zone ou on affichera les resultats

    for (var i = 0; i < cartesTrajet.length; i++) {             //On parcourt toutes les cartes de trajet

    cartesTrajet[i].addEventListener("click", function () {         //Quand on clique sur une carte on fais la fonction

        for (var j = 0; j < cartesTrajet.length; j++) {             //On enlève l’état "actif" partout
        cartesTrajet[j].classList.remove("actif");
        }

        this.classList.add("actif");        //On active le trajet cliqué (this = la carte sur laquelle on a cliqué)

        trajetSelectionne = this.getAttribute("id-liaison");      //On mémorise le trajet choisi

        afficherRemplissage();
    });
    }


    for (var k = 0; k < boutonsJour.length; k++) {              //On parcourt tous les boutons de jour

    boutonsJour[k].addEventListener("click", function () {

        for (var m = 0; m < boutonsJour.length; m++) {          //Nettoyage des jours
        boutonsJour[m].classList.remove("actif");               //(enlever les classes "actif")
        }

        this.classList.add("actif");                   //Activation du jour cliqué

        var jour = this.textContent;                   //On récupère le contenu du bouton

        if (jour.length === 1) {                       //Ajouter un '0' si le nombre est inférieur à 9
        jour = "0" + jour;
        }

        dateSelectionnee = "2025-11-" + jour;          // mettre la variable dateSelectionnee à la date au bon format pour l'API

        afficherRemplissage();
    });
    }


    function afficherRemplissage() {

    if (trajetSelectionne === null || dateSelectionnee === null) {      //Tant que l'utilisateur à rien sélectionner
        return;
    }

    zoneResultats.innerHTML = `<div class="loading">⏳ Chargement des données...</div>`; //Afficher un chargement tant que les infos sont pas chargés

    var url = "https://tomorow.eu.pythonanywhere.com/CAN/api/liaison/" + trajetSelectionne + "/remplissage/" + dateSelectionnee;     //Créer l'URL pour l'API

    fetch(url)
        .then(function (reponse) {      
        return reponse.json();
        })
        .then(function (data) {

        var html = "<h3>Remplissage du " + dateSelectionnee + "</h3>";

        html += "<table class='table-remplissage'>";
        html += "<tr>";
        html += "<th>Heure</th>";
        html += "<th>Passagers</th>";
        html += "<th>Véhicules</th>";
        html += "</tr>";

        for (var i = 0; i < data.length; i++) {     //Ajoute le bon nombres de cases dans le tableau ( le mêmenombre que cases que d'horaires)

            var tauxPassagers = 0;
            if (data[i].capacitePassagers > 0) {                      //Vérification
            tauxPassagers = Math.round(
                (data[i].nbReservationPassagers / data[i].capacitePassagers) * 100      //Calcule le pourcentage
            );
            }

            var tauxVehicules = 0;
            if (data[i].capaciteVoitures > 0) {                       //Vérification
            tauxVehicules = Math.round(
                (data[i].nbReservationVoitures / data[i].capaciteVoitures) * 100        //Calcule le pourcentage
            );
            }

            html += "<tr>";
            html += "<td>" + data[i].heure + "</td>";

            html +=
            "<td class='remplissage " + definirCouleur(tauxPassagers) + "'>" +      //Définis la classe a utiliser en fonction de la couleur
            tauxPassagers + " %" +
            "</td>";

            html +=
            "<td class='remplissage " + definirCouleur(tauxVehicules) + "'>" +      //Définis la classe a utiliser en fonction de la couleur
            tauxVehicules + " %" +
            "</td>";

            html += "</tr>";
        }

        html += "</table>";                     

        zoneResultats.innerHTML = html;             //Écrire le HTML
        })
        .catch(function () {
        zoneResultats.innerHTML = "Erreur lors du chargement des données.";     //Si il y a une erreur renvoie ce texte
        });
    }

    function definirCouleur(taux) {

    if (taux <= 50) {           //Si il y a moins de 50% de passager
        return "vert";          //Mets le fond en vert
    }

    if (taux <= 75) {           //Si il y a entre 51 et 75% de passager/véhicules
        return "orange";        //Mets le fond en orange
    }

    if (taux <= 100) {          //Si il y a entre 76 et 100% de passager/véhicules
        return "rouge";         //Mets le fond en rouge
    }

    return "jaune";             //Au-delà
    }
});
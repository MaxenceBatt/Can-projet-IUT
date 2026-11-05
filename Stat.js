document.addEventListener("DOMContentLoaded", function () { //Attend que le HTML soit entiérement chargé pour s'exécuter

    let zone = document.getElementById("stats");
    zone.innerHTML = `<div class="loading">⏳ Chargement des statistiques...</div>`;        //Afficher un chargement tant que ça a pas tout trouvé
    


    let LG = [];
    let GL = [];
    let QLP = [];
    let LPQ = [];
    let TOTAL = 0;

    function LorientGroix() {
        fetch(`https://tomorow.eu.pythonanywhere.com/CAN/api/liaison/1/chiffreAffaire`)      //Fetch le chiffre d'affaire du trajet Lorient groix
        .then(response => response.json())      //Transformer ce que ça renvoie en élément JavaScript capable de lire le JSON
        .then(r => {
            let CAtotal = r.passagers.chiffreAffaire + r.vehicules.chiffreAffaire;      //Chiffre d'affaire total 

            LG.push({
                CA: CAtotal,
                NbP: r.passagers.nombre,
                NbV: r.vehicules.quantite
            });
            TOTAL += CAtotal    //Ajouter le CAtotal de ce trajet au Total global
            AfficherStats();
        })
    }

    function GroixLorient() {
        fetch(`https://tomorow.eu.pythonanywhere.com/CAN/api/liaison/2/chiffreAffaire`)      //Fetch le chiffre d'affaire du trajet Groix Lorient
        .then(response => response.json())      //Transformer ce que ça renvoie en élément JavaScript capable de lire le JSON
        .then(r => {
            let CAtotal = r.passagers.chiffreAffaire + r.vehicules.chiffreAffaire;      //Chiffre d'affaire total

            GL.push({
                CA: CAtotal,
                NbP: r.passagers.nombre,
                NbV: r.vehicules.quantite
            });
            TOTAL += CAtotal        //Ajouter le CAtotal de ce trajet au Total global
            AfficherStats();
        })
    }

    function QuiberonLePalais() {
        fetch(`https://tomorow.eu.pythonanywhere.com/CAN/api/liaison/3/chiffreAffaire`)      //Fetch le chiffre d'affaire du trajet Quiberon Le Palais
        .then(response => response.json())      //Transformer ce que ça renvoie en élément JavaScript capable de lire le JSON
        .then(r => {
            let CAtotal = r.passagers.chiffreAffaire + r.vehicules.chiffreAffaire;      //Chiffre d'affaire total

            QLP.push({
                CA: CAtotal,
                NbP: r.passagers.nombre,
                NbV: r.vehicules.quantite
            });
            TOTAL += CAtotal        //Ajouter le CAtotal de ce trajet au Total global
            AfficherStats();
        })
    }

    function LePalaisQuiberon() {
        fetch(`https://tomorow.eu.pythonanywhere.com/CAN/api/liaison/4/chiffreAffaire`)      //Fetch le chiffre d'affaire du trajet Le Palais Quiberon
        .then(response => response.json())      //Transformer ce que ça renvoie en élément JavaScript capable de lire le JSON
        .then(r => {
            let CAtotal = r.passagers.chiffreAffaire + r.vehicules.chiffreAffaire;      //Chiffre d'affaire total

            LPQ.push({
                CA: CAtotal,
                NbP: r.passagers.nombre,
                NbV: r.vehicules.quantite
            });
            TOTAL += CAtotal        //Ajouter le CAtotal de ce trajet au Total global
            AfficherStats();
        })
    }


    function AfficherStats() {
        //Texte a rajouter dans le HTML :
        let html = `        
        <div class="stat">
            <h3>Lorient → Groix</h3>
            <p><strong>Chiffre d’affaires</strong><br>${LG[0].CA.toFixed(2)} €</p>
            <p>Passagers : ${LG[0].NbP}</p>
            <p>Véhicules : ${LG[0].NbV}</p>
        </div>

        <div class="stat">
            <h3>Groix → Lorient</h3>
            <p><strong>Chiffre d’affaires</strong><br>${GL[0].CA.toFixed(2)} €</p>
            <p>Passagers : ${GL[0].NbP}</p>
            <p>Véhicules : ${GL[0].NbV}</p>
        </div>

        <div class="stat">
            <h3>Quiberon → Le Palais</h3>
            <p><strong>Chiffre d’affaires</strong><br>${QLP[0].CA.toFixed(2)} €</p>
            <p>Passagers : ${QLP[0].NbP}</p>
            <p>Véhicules : ${QLP[0].NbV}</p>
        </div>

        <div class="stat">
            <h3>Le Palais → Quiberon</h3>
            <p><strong>Chiffre d’affaires</strong><br>${LPQ[0].CA.toFixed(2)} €</p>
            <p>Passagers : ${LPQ[0].NbP}</p>
            <p>Véhicules : ${LPQ[0].NbV}</p>
        </div>

        <div class="stat-total">
            💰 <strong>Chiffre d’affaires total – Novembre :</strong><br>
            ${TOTAL.toFixed(2)} €
        </div>
        `;


        zone.innerHTML = html;      //Ajouter le texte au bon endroit dans le HTML
    }   
    //Tout exécuter
    LorientGroix();
    GroixLorient();
    QuiberonLePalais();
    LePalaisQuiberon();
})
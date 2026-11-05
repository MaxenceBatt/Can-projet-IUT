document.addEventListener("DOMContentLoaded", function () { //Attend que le HTML soit entiérement chargé pour s'exécuter


  //Les tarifs pour chaque catégorie de véhicule
  const TARIFS_VEHICULES = {
    trot: 4.70,
    velo: 8.20,
    velelec: 11.00,
    cartand: 16.45,
    mobil: 23.10,
    moto: 66.05,
    cat1: 96.05,
    cat2: 114.80,
    cat3: 174.45,
    cat4: 210.90,
    camp: 330.20
  };


  let zone = document.getElementById("carteEmbarquement");  //Cherche l'élément avec l'id carteEmbarquement
  let btnPrev = document.getElementById("prev");            //Cherche l'élément avec l'id prev
  let btnNext = document.getElementById("next");            //Cherche l'élément avec l'id next
  let reservation = null;

  document.getElementById("charger").addEventListener("click", async () => {      //Quand on appuie sur le bouton HTML avec l'id "charger"
    const id = document.getElementById("reservationId").value;                    //On récupère le numéro écrit dans l'input
    zone.innerHTML = `<div class="loading">⏳ Chargement des cartes d'embarquement...</div>`; //Petit chargement le temps d'avoir la réponse de la fonction d'après
    await chargerReservation(id);           
  });


  async function chargerReservation(id) {         
    const r = await fetch(`https://tomorow.eu.pythonanywhere.com/CAN/api/reservation/${id}`);    //définit 'r' au contenu de l'api dans le fetch
    reservation = await r.json();       //Transforme r en élément JavaScript qui fonctionne comme un JSON

    cartes = [];
    index = 0;

    await chargerPassagers();
    await chargerVehicules();

    afficherCarte(); // affichage UNE FOIS, à la fin
  }



  async function chargerPassagers() {
    for (let i = 1; i <= reservation.nbPassagers; i++) {    //Parcourir chaque passagers
      const r = await fetch(
        `https://tomorow.eu.pythonanywhere.com/CAN/api/reservation/${reservation.id}/passager/${i}`    //Renvoie le JSON pour chaque passager
      );
      const p = await r.json();

      cartes.push({             //Ajouter dans 'cartes' le dictionnaire suivant (qui contient chaque infos nécessaires à la création d'une carte de passager)
        type: "passager",
        id: reservation.id,
        bateau: reservation.bateau,
        nom: p.nom,
        prenom: p.prenom,
        categorie: p.libelleCategorie,
        prix: p.price,
        depart: reservation.portDepart,
        arrivee: reservation.portArrivee,
        date: reservation.date,
        heure: reservation.heure,
        reservation: reservation.nom
      });
    }
  }


  async function chargerVehicules() {
    for (let i = 1; i <= reservation.nbVehicules; i++) {    //Parcourir chaque véhicules
      const r = await fetch(
        `https://tomorow.eu.pythonanywhere.com/CAN/api/reservation/${reservation.id}/vehicule/${i}`    //Renvoie le JSON pour chaque véhicule
      );
      const v = await r.json();

      cartes.push({                 //Ajouter dans 'cartes' le dictionnaire suivant (qui contient chaque infos nécessaires à la création d'une carte de véhicule)
        type: "vehicule",
        id: reservation.id,
        bateau: reservation.bateau,
        categorie: v.libelle,
        quantite: v.quantite,
        prix: v.prix,
        depart: reservation.portDepart,
        arrivee: reservation.portArrivee,
        date: reservation.date,
        heure: reservation.heure,
        reservation: reservation.nom
      });
    }
  }



  function afficherCarte() {              // Cette fonction va afficher les cartes en rajoutant le contenu HTML nécessaire
    if (cartes.length === 0) return;
    let c = cartes[index];                // On nomme la carte visible 'c' pour que ça soit plus simple 
    //On créer la variable qui contiendra le texte qu'on veut ajouter dans le HTML
    let html = `                                        
        <div class="carte-embarquement">

        <div class="carte-gauche">
            <img src="CAN.png" class="logo-carte">

            <div class="ligne">
            <span>Port de départ</span>
            <span>${c.depart}</span>
            </div>

            <div class="ligne">
            <span>Port d'arrivée</span>
            <span>${c.arrivee}</span>
            </div>

            <div class="ligne">
            <span>Date</span>
            <span>${c.date}</span>
            </div>

            <div class="ligne">
            <span>Heure départ</span>
            <span>${c.heure}</span>
            </div>

            <div class="ligne">
            <span>Bateau</span>
            <span>Breizh Nevez</span>
            </div>
        </div>

        <div class="carte-droite">

          <h3>Carte d’embarquement</h3>
          <div class="num-nom-res">
              <p><strong>Reservation :</strong> ${c.id}</p>
              <p><strong>Nom :</strong> ${c.reservation}</p>
          </div>
          <div class="infos-ligne">
            
            <div class="infos-text">
              ${c.type === "passager" ? `
                <p><strong>Nom :</strong> ${c.nom}</p>
                <p><strong>Prénom :</strong> ${c.prenom}</p>
                <p><strong>Catégorie :</strong> ${c.categorie}</p>
                <p><strong>Prix :</strong> ${c.prix} €</p>
              ` : `
                <p><strong>Catégorie :</strong> ${c.categorie}</p>
                <p><strong>Quantité :</strong> ${c.quantite}</p>
                <p><strong>Prix unitaire :</strong> ${c.prix} €</p>
                <p><strong>Total :</strong> ${(c.prix * c.quantite).toFixed(2)} €</p>
              `}
            </div>

            <div id="qrcode"></div>

          </div>
        </div>
        </div>
    `;

    zone.innerHTML = html;      // On ajoute la variable html (le texte au dessus) dans le HTML
    // Tout le long truc après c'est pour le QR Code il est 3:36 c'est pas très compliqué a comprendre normalement.
    function toAlphaQR(str) {
      return str
        .normalize("NFD")                     // enlève les accents
        .replace(/[\u0300-\u036f]/g, "")
        .toUpperCase()                        // MAJUSCULES
        .replace(/[^0-9A-Z $%*+\-./:\n]/g, ""); // caractères QR autorisés
    }

    const qrZone = document.getElementById("qrcode");
    qrZone.innerHTML = "";

    //Infos dans le QRcode
    const qrData = {
      reservation: c.reservation,
      trajet: `${c.depart} - ${c.arrivee}`,
      date: c.date,
      heure: c.heure,
      type: c.type,
    };

    if (c.type === "passager") {
      qrData.nom = c.nom;
      qrData.prenom = c.prenom;
      qrData.categorie = c.categorie;
      qrData.prix = c.prix;
    }

    if (c.type === "vehicule") {
      qrData.categorie = c.categorie;
      qrData.quantite = c.quantite;
      qrData.prix = c.prix * c.quantite;
    }

    let qrText = toAlphaQR(
      "RESERVATION: " + c.reservation + "\n" +
      "TRAJET: " + c.depart + " - " + c.arrivee + "\n" +
      "DATE: " + c.date + "\n" +
      "HEURE: " + c.heure
    );

    if (c.type === "passager") {
      qrText = toAlphaQR(
        qrText +
        "\nPASSAGER: " + c.prenom + " " + c.nom +
        "\nCATEGORIE: " + c.categorie +
        "\nPRIX: " + c.prix + " EUR"
      );
    }

    if (c.type === "vehicule") {
      qrText = toAlphaQR(
        qrText +
        "\nVEHICULE: " + c.categorie +
        "\nQUANTITE: " + c.quantite +
        "\nTOTAL: " + (c.prix * c.quantite) + " EUR"
      );
    }



    qrZone.innerHTML = "";

    try {
      new QRCode(qrZone, {
        text: qrText,
        width: 150,
        height: 150,
        correctLevel: QRCode.CorrectLevel.L
      });
    } catch (e) {
      qrZone.innerHTML = "<p>QR code indisponible</p>";
      console.error(e);
    }}



  btnNext.addEventListener("click", function () { //Passe à la carte d'après
    index++;
    if (index >= cartes.length) index = 0;        // Si on est à la dernière carte et qu'on va sur celle d'après on revient à la première
    afficherCarte();
  });

  btnPrev.addEventListener("click", function () { // Revient à la carte précédente
    index--;
    if (index < 0) index = cartes.length - 1;     // Si on est à la première carte et qu'on va sur la précédente on va à la dernière carte
    afficherCarte();
  });

});

document.addEventListener("DOMContentLoaded", function () {       //Attendre que la page soit chargé pour executer

  const zone = document.getElementById("facture");      //Chercher le div avec la classe facture dans le HTML

  let reservation = null;
  let passagers = [];
  let vehicules = [];

  document.getElementById("charger").addEventListener("click", () => {      //Quand on clique sur le bouton avec l'ID charger
    const id = document.getElementById("reservationId").value;      //Mettre la valeure de l'input dans quel id
    chargerReservation(id);
  });

  function chargerReservation(id) {
    fetch(`https://tomorow.eu.pythonanywhere.com/CAN/api/reservation/${id}`)
      .then(r => r.json())
      .then(data => {
        reservation = data;           //mettre les infos du fetch api/reservation dans reservation pour pouvoir l'utiliser plus tard
        passagers = [];
        vehicules = [];
        chargerPassagers();
        chargerVehicules();
      })
      .catch(err => {
        console.error(err);
        zone.innerHTML = "<p>Erreur lors du chargement de la réservation</p>";    //Si ça marche pas renvoyer qu'il y a une erreur
      });
  }

  function chargerPassagers() {
    for (let i = 1; i <= reservation.nbPassagers; i++) {
      fetch(`https://tomorow.eu.pythonanywhere.com/CAN/api/reservation/${reservation.id}/passager/${i}`)   //Récupérer chaque passager
        .then(r => r.json())    //Transformer ce que ça renvoie en élément JavaScript capable de lire le JSON
        .then(p => {
          passagers.push(p);    // On ajoute dans passager les infos JSON de chaque passager
          afficherFacture();
        });
    }
  }

  function chargerVehicules() {
    for (let i = 1; i <= reservation.nbVehicules; i++) {
      fetch(`https://tomorow.eu.pythonanywhere.com/CAN/api/reservation/${reservation.id}/vehicule/${i}`)
        .then(r => r.json())    //Transformer ce que ça renvoie en élément JavaScript capable de lire le JSON
        .then(v => {
          vehicules.push(v);    // On ajoute dans passager les infos JSON de chaque véhicule
          afficherFacture();
        });
    }
  }

  function afficherFacture() {
    if (!reservation) return;

    let passagersParCategorie = {};
    let totalPersonnes = 0;

    for (let p of passagers) {
      if (!passagersParCategorie[p.libelleCategorie]) {
        passagersParCategorie[p.libelleCategorie] = {
          nb: 0,
          prix: p.price
        };
      }
      passagersParCategorie[p.libelleCategorie].nb++;
      totalPersonnes += p.price;
    }

    let lignesPassagers = "";
    for (let cat in passagersParCategorie) {
      let ligne = passagersParCategorie[cat];
      lignesPassagers += `
        <tr>
          <td>${cat}</td>
          <td>${ligne.nb}</td>
          <td>${ligne.prix.toFixed(2)} €</td>
          <td>${(ligne.nb * ligne.prix).toFixed(2)} €</td>
        </tr>
      `;
    }

    let totalVehicules = 0;
    let lignesVehicules = "";

    for (let v of vehicules) {
      let totalLigne = v.prix * v.quantite;
      totalVehicules += totalLigne;

      lignesVehicules += `
        <tr>
          <td>${v.libelle}</td>
          <td>${v.quantite}</td>
          <td>${v.prix.toFixed(2)} €</td>
          <td>${totalLigne.toFixed(2)} €</td>
        </tr>
      `;
    }

    let totalFinal = totalPersonnes + totalVehicules;

    zone.innerHTML = `
      <div class="facture">

        <img src="CAN.png" class="logo-carte">

        <h2>Facture</h2>

        <p><strong>Réservation n° :</strong> ${reservation.id}</p>
        <p><strong>Nom de réservation :</strong> ${reservation.nom}</p>
        <p><strong>Traversée :</strong> ${reservation.portDepart} - ${reservation.portArrivee}</p>
        <p><strong>Date :</strong> ${reservation.date}</p>
        <p><strong>Départ :</strong> ${reservation.heure}</p>
        <p><strong>Bateau :</strong> ${reservation.bateau}</p>

        <h3>Les personnes</h3>
        <table>
          <tr>
            <th>Catégorie</th>
            <th>Nombre</th>
            <th>Prix unitaire</th>
            <th>Prix ligne</th>
          </tr>
          ${lignesPassagers}
          <tr>
            <td colspan="3"><strong>Sous-total personnes</strong></td>
            <td><strong>${totalPersonnes.toFixed(2)} €</strong></td>
          </tr>
        </table>

        <h3>Les véhicules</h3>
        <table>
          <tr>
            <th>Catégorie</th>
            <th>Quantité</th>
            <th>Prix unitaire</th>
            <th>Prix ligne</th>
          </tr>
          ${lignesVehicules}
          <tr>
            <td colspan="3"><strong>Sous-total véhicules</strong></td>
            <td><strong>${totalVehicules.toFixed(2)} €</strong></td>
          </tr>
        </table>

        <h2>Prix total : ${totalFinal.toFixed(2)} €</h2>

      </div>
    `;
  }

});

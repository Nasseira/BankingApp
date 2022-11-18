# Projet PRBD 2122 - BankApp - PRBD_2122_G01

## Notes de version itération 1 

  * Il manque quelques fonctionnalités à implémenter. (voir plus bas ou la grille d'autoévaluation)
  * Il n'y a pas de bugs connus.
 

### Liste des utilisateurs et mots de passes

  * bruno@test.com, password "bruno", manager
  * ben@test.com, password "ben", manager
  * bob@test.com, password "bob", client
  * caro@test.com, password "caro", client
  * louise@test.com, password "louise", client
  * jules@test.com, password "jules", client
  

### Liste des fonctionnalités non abouties

  1. Associer une catégorie : grâce à la combobox je peux associer une catégorie à chaque virement (ou aucune si vide)
	
	* J'avais rajouté une valeur à nulle dans mon Combobox qui rajoute une ligne vide mais celle-ci n'est pas selectionnable, j'ai donc retiré cette ligne qui ne fonctionnait pas et ne mettait pas a null dans ma DB quand je clique dessus.

  2. checkbox pour chaque catégorie : on peut cocher une ou plusieurs catégories dans le filtre pour voir les virements associés à cette ou ces catégories
	
	* Le filtre n'est pas implémenté, il y a juste l'affichage

  3. checkbox "no category" : on peut cocher ce checkbox pour voir les virements associés à aucune catégorie
	
	* Le filtre n'est pas implémenté, il y a juste l'affichage

  4. bouton "Check All" : ce bouton permet de cocher d'un coup tous les checkbox de filtre sur les catégories
   
	* les boutons selectionnent tous les filtres, mais le filtre n'est pas implémenté

  5. bouton "Check None" : ce bouton permet de décocher d'un coup tous les checkbox de filtre sur les catégories
   
	* les boutons déselectionnent tous les filtres, mais le filtre n'est pas implémenté
		
  6. filtres se combinent : tous les filtres ci-dessus se combinent
   
	* Tous les filtres que j'ai implémenté se combinent mais je n'ai pas eu le temps d'implémenté le filtre sur les catégories

  7. extraits sont paginés : la liste des extraits de compte est paginée et on peut naviguer parmi les pages (mettez une taille de page qui permet de le montrer)
   
	* fonctionnalité non implémentée
		

### Liste des fonctionnalités supplémentaires
  
  * Aucune fonctionnalité supplémentaire

### Divers
  
- Formato do player
	+ Objeto (untagged, Player): configurar a f�sica para ignorar colis�o entre Player 
		- sprite renderer
		-	controller
		-	rigidbody2d
		- BoxCollider2D (trigger)
		- simplemoverigidbody2.cs
		- player.cs

		+ HitBox (Dude|Dog, Default): ativa os triggers com outros objetos
			- BoxCollider2D (trigger)
			- CheckHitBox.cs

		+ GroundCheck (Untagged, GroundCheck): faz a verifica��o de colis�o com o ch�o. IMPORTANTE: configurar a 
																					 f�sica para fazer colis�o de GroundCheck SOMENTE com Ground.
			- CircleCollider2D

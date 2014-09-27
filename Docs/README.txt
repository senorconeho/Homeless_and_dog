- Formato do player
	+ Objeto (untagged, Player): configurar a física para ignorar colisão entre Player 
		- sprite renderer
		-	controller
		-	rigidbody2d
		- BoxCollider2D (trigger)
		- simplemoverigidbody2.cs
		- player.cs

		+ HitBox (Dude|Dog, Default): ativa os triggers com outros objetos
			- BoxCollider2D (trigger)
			- CheckHitBox.cs

		+ GroundCheck (Untagged, GroundCheck): faz a verificação de colisão com o chão. IMPORTANTE: configurar a 
																					 física para fazer colisão de GroundCheck SOMENTE com Ground.
			- CircleCollider2D
